namespace PrimS.SelectedItemsSynchronizer
{
  using System;
  using System.Collections;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Windows;

  /// <summary>
  /// Keeps two lists synchronized. 
  /// </summary>
  public class TwoListSynchronizer : IWeakEventListener
  {
    private static readonly IListItemConverter DefaultConverter = new DoNothingListItemConverter();
    private readonly IList masterList;
    private readonly IListItemConverter masterTargetConverter;
    private readonly IList targetList;

    /// <summary>
    /// Initialises a new instance of the <see cref="TwoListSynchronizer"/> class.
    /// </summary>
    /// <param name="masterList">The master list.</param>
    /// <param name="targetList">The target list.</param>
    /// <param name="masterTargetConverter">The master-target converter.</param>
    public TwoListSynchronizer(IList masterList, IList targetList, IListItemConverter masterTargetConverter)
    {
      this.masterList = masterList;
      this.targetList = targetList;
      this.masterTargetConverter = masterTargetConverter;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="TwoListSynchronizer"/> class.
    /// </summary>
    /// <param name="masterList">The master list.</param>
    /// <param name="targetList">The target list.</param>
    public TwoListSynchronizer(IList masterList, IList targetList) : this(masterList, targetList, DefaultConverter)
    {
    }

    private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter);

    /// <summary>
    /// Starts synchronizing the lists.
    /// </summary>
    public void StartSynchronizing()
    {
      lock (this)
      {
        this.ListenForChangeEvents(this.masterList);
        this.ListenForChangeEvents(this.targetList);

        // Update the Target list from the Master list
        this.SetListValuesFromSource(this.masterList, this.targetList, this.ConvertFromMasterToTarget);

        // In some cases the target list might have its own view on which items should included:
        // so update the master list from the target list
        // (This is the case with a ListBox SelectedItems collection: only items from the ItemsSource can be included in SelectedItems)
        if (!this.TargetAndMasterCollectionsAreEqual())
        {
          this.SetListValuesFromSource(this.targetList, this.masterList, this.ConvertFromTargetToMaster);
        }
      }
    }

    /// <summary>
    /// Stop synchronizing the lists.
    /// </summary>
    public void StopSynchronizing()
    {
      lock (this)
      {
        this.StopListeningForChangeEvents(this.masterList);
        this.StopListeningForChangeEvents(this.targetList);
      }
    }

    /// <summary>
    /// Receives events from the centralized event manager.
    /// </summary>
    /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
    /// <param name="sender">Object that originated the event.</param>
    /// <param name="e">Event data.</param>
    /// <returns>
    /// True if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
    /// </returns>
    public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      this.HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);

      return true;
    }

    /// <summary>
    /// Listens for change events on a list.
    /// </summary>
    /// <param name="list">The list to listen to.</param>
    protected void ListenForChangeEvents(IList list)
    {
      if (list is INotifyCollectionChanged)
      {
        CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
      }
    }

    /// <summary>
    /// Stops listening for change events.
    /// </summary>
    /// <param name="list">The list to stop listening to.</param>
    protected void StopListeningForChangeEvents(IList list)
    {
      if (list is INotifyCollectionChanged)
      {
        CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
      }
    }

    private void AddItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
    {
      int itemCount = e.NewItems.Count;

      for (int i = 0; i < itemCount; i++)
      {
        int insertionPoint = e.NewStartingIndex + i;

        if (insertionPoint > list.Count)
        {
          list.Add(converter(e.NewItems[i]));
        }
        else
        {
          list.Insert(insertionPoint, converter(e.NewItems[i]));
        }
      }
    }

    private object ConvertFromMasterToTarget(object masterListItem)
    {
      return this.masterTargetConverter == null ? masterListItem : this.masterTargetConverter.Convert(masterListItem);
    }

    private object ConvertFromTargetToMaster(object targetListItem)
    {
      return this.masterTargetConverter == null ? targetListItem : this.masterTargetConverter.ConvertBack(targetListItem);
    }

    private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      lock (this)
      {
        IList sourceList = sender as IList;

        switch (e.Action)
        {
          case NotifyCollectionChangedAction.Add:
            this.PerformActionOnAllLists(this.AddItems, sourceList, e);
            break;
          case NotifyCollectionChangedAction.Move:
            this.PerformActionOnAllLists(this.MoveItems, sourceList, e);
            break;
          case NotifyCollectionChangedAction.Remove:
            this.PerformActionOnAllLists(this.RemoveItems, sourceList, e);
            break;
          case NotifyCollectionChangedAction.Replace:
            this.PerformActionOnAllLists(this.ReplaceItems, sourceList, e);
            break;
          case NotifyCollectionChangedAction.Reset:
            this.UpdateListsFromSource(sender as IList);
            break;
          default:
            throw new NotImplementedException(string.Format("Unhandled enum member {0}", e.Action.ToString("f")));
        }
      }
    }

    private void MoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
    {
      this.RemoveItems(list, e, converter);
      this.AddItems(list, e, converter);
    }

    private void PerformActionOnAllLists(ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
    {
      if (sourceList == this.masterList)
      {
        this.PerformActionOnList(this.targetList, action, collectionChangedArgs, this.ConvertFromMasterToTarget);
      }
      else
      {
        this.PerformActionOnList(this.masterList, action, collectionChangedArgs, this.ConvertFromTargetToMaster);
      }
    }

    private void PerformActionOnList(IList list, ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs, Converter<object, object> converter)
    {
      this.StopListeningForChangeEvents(list);
      action(list, collectionChangedArgs, converter);
      this.ListenForChangeEvents(list);
    }

    private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
    {
      int itemCount = e.OldItems.Count;

      // for the number of items being removed, remove the item from the Old Starting Index
      // (this will cause following items to be shifted down to fill the hole).
      for (int i = 0; i < itemCount; i++)
      {
        list.RemoveAt(e.OldStartingIndex);
      }
    }

    private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e, Converter<object, object> converter)
    {
      this.RemoveItems(list, e, converter);
      this.AddItems(list, e, converter);
    }

    private void SetListValuesFromSource(IList sourceList, IList targetList, Converter<object, object> converter)
    {
      this.StopListeningForChangeEvents(targetList);

      targetList.Clear();

      foreach (object o in sourceList)
      {
        targetList.Add(converter(o));
      }

      this.ListenForChangeEvents(targetList);
    }

    private bool TargetAndMasterCollectionsAreEqual()
    {
      return this.masterList.Cast<object>().SequenceEqual(this.targetList.Cast<object>().Select(item => this.ConvertFromTargetToMaster(item)));
    }

    /// <summary>
    /// Makes sure that all synchronized lists have the same values as the source list.
    /// </summary>
    /// <param name="sourceList">The source list.</param>
    private void UpdateListsFromSource(IList sourceList)
    {
      if (sourceList == this.masterList)
      {
        this.SetListValuesFromSource(this.masterList, this.targetList, this.ConvertFromMasterToTarget);
      }
      else
      {
        this.SetListValuesFromSource(this.targetList, this.masterList, this.ConvertFromTargetToMaster);
      }
    }

    /// <summary>
    /// An implementation that does nothing in the conversions.
    /// </summary>
    internal class DoNothingListItemConverter : IListItemConverter
    {
      /// <summary>
      /// Converts the specified master list item.
      /// </summary>
      /// <param name="masterListItem">The master list item.</param>
      /// <returns>The result of the conversion.</returns>
      public object Convert(object masterListItem)
      {
        return masterListItem;
      }

      /// <summary>
      /// Converts the specified target list item.
      /// </summary>
      /// <param name="targetListItem">The target list item.</param>
      /// <returns>The result of the conversion.</returns>
      public object ConvertBack(object targetListItem)
      {
        return targetListItem;
      }
    }
  }
}