namespace SelectedItemsBindingDemo
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Threading;
  using System.Windows.Input;

  public class ViewModel : INotifyPropertyChanged
  {
    private readonly ObservableCollection<string> selectedNames;
    private readonly ObservableCollection<string> selectedSecondaries;
    private string summary;

    private int selectingMap;
    private readonly object selectingMapSynchLock = new object();

    public ViewModel()
    {
      this.selectedNames = new ObservableCollection<string>();
      this.selectedNames.CollectionChanged += this.selectedNames_CollectionChanged;
      this.selectedSecondaries = new ObservableCollection<string>();
    }

    private void selectedNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      lock (this.selectingMapSynchLock)
      {
        if (this.SelectedNames.Count() > 0)
        {
          System.Diagnostics.Debug.Print("SelectedNames.CollectionChanged={0}", this.SelectedNames.Aggregate((i, o) => o = string.Format("{0}, {1}", o, i)));
        }
        else
        {
          System.Diagnostics.Debug.Print("SelectedNames.CollectionChanged=empty");
        }
        Interlocked.Increment(ref this.selectingMap);
        if (this.SelectedNames.Count() == 1)
        {
          this.SelectMap(false, this.SelectedNames.Single());
        }
        else
        {
          this.SelectedSecondaries.Clear();
        }
        Interlocked.Decrement(ref this.selectingMap);
      }
    }

    public IEnumerable<string> AvailableNames
    {
      get
      {
        return new string[] 
        {
          "Abraham",
          "George",
          "James",
          "Joel",
          "John",
          "Peter",
          "Samuel",
          "Zachariah"
        };
      }
    }

    public IEnumerable<string> AvailableSecondaries
    {
      get
      {
        return new string[] 
        {
          "Abraham1",
          "George1",
          "James1",
          "Joel1",
          "John1",
          "Peter1",
          "Samuel1",
          "Zachariah1",
          "Abraham2",
          "George2",
          "James2",
          "Joel2",
          "John2",
          "Peter2",
          "Samuel2",
          "Zachariah2"
        };
      }
    }

    public string Summary
    {
      get
      {
        return this.summary;
      }
      private set
      {
        this.summary = value;
        this.OnPropertyChanged("Summary");
      }
    }

    public ICommand SelectAll
    {
      get
      {
        return new RelayCommand(
          parameter =>
          {
            this.selectedNames.Clear();
            foreach (var item in this.AvailableNames)
            {
              this.selectedNames.Add(item);
            }
          });
      }
    }

    public ObservableCollection<string> SelectedNames
    {
      get
      {
        return this.selectedNames;
      }
    }

    public ObservableCollection<string> SelectedSecondaries
    {
      get
      {
        return this.selectedSecondaries;
      }
    }

    public ICommand NamesSelectionChangedCommand
    {
      get
      {
        return new RelayCommand(
          parameter =>
          {
            System.Diagnostics.Debug.Print("{0} NamesSelectionChangedCommand", DateTime.Now);
            this.CommonNamesSelectionAction(parameter);
          });
      }
    }

    public ICommand NamesViewSelectionChangedCommand
    {
      get
      {
        return new RelayCommand(
          parameter =>
          {
            System.Diagnostics.Debug.Print("{0} NamesViewSelectionChangedCommand", DateTime.Now);
            this.CommonNamesSelectionAction(parameter);
          });
      }
    }

    private void CommonNamesSelectionAction(object parameter)
    {
      lock (this.selectingMapSynchLock)
      {
        try
        {
          var items = (IList)parameter;
          var currentSelectedItems = items.Cast<string>();

          if (currentSelectedItems == null)
          {
            return;
          }

          this.UpdateSummary(currentSelectedItems);
        }
        catch (Exception ex)
        {
          throw;
        }
      }
    }

    public ICommand SecondariesSelectionChangedCommand
    {
      get
      {
        return new RelayCommand(
          parameter =>
          {
            lock (this.selectingMapSynchLock)
            {
              try
              {
                var items = (IList)parameter;
                var currentSecondariesSelected = items.Cast<string>();

                if (this.selectingMap == 0)
                {
                  Interlocked.Increment(ref this.selectingMap);
                  if (currentSecondariesSelected.Count() == 1)
                  {
                    this.SelectMap(true, this.AvailableNames.First(o => currentSecondariesSelected.Single().Contains(o)));
                  }
                  else if (currentSecondariesSelected.Count() == 0)
                  {
                    this.SelectedNames.Clear();
                  }
                  else
                  {
                    //If this is uncommented then the ability multi-select Secondaries is removed
                    //this.SelectedNames.Clear();
                  }
                  Interlocked.Decrement(ref this.selectingMap);
                }
              }
              catch (Exception ex)
              {
                throw;
              }
            }
          });
      }
    }

    private void SelectMap(bool isDriverSecondary, string nameToSelect)
    {
      lock (this.selectingMapSynchLock)
      {
        Interlocked.Increment(ref this.selectingMap);
        if (isDriverSecondary)
        {
          if (this.SelectedNames.Count() != 1 || this.SelectedNames.Single() != nameToSelect)
          {
            this.SelectedNames.Clear();
            this.SelectedNames.Add(nameToSelect);
          }
        }
        else
        {
          List<string> secondariesToSelect = this.AvailableSecondaries.Where(o => o.Contains(nameToSelect)).ToList();
          if (this.SelectedSecondaries.Count() != secondariesToSelect.Count() || this.SelectedSecondaries.Intersect(secondariesToSelect).Count() != secondariesToSelect.Count())
          {
            this.SelectedSecondaries.Clear();
            foreach (var secondary in secondariesToSelect)
            {
              this.SelectedSecondaries.Add(secondary);
            }
          }
        }

        Interlocked.Decrement(ref this.selectingMap);
      }
    }

    protected void OnPropertyChanged(string propertyName)
    {
      var handler = this.PropertyChanged;

      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private void UpdateSummary(IEnumerable<string> selectedNames)
    {
      this.Summary = string.Format("{0} names are selected.", selectedNames.Count());
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
  }
}