namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Controls;

  [TestClass]
  public class MultiSelectorBehavioursTests
  {
    List<string> names;
    ObservableCollection<string> selectedNames;
    ListView listView;

    [TestInitialize]
    public void TestInitialize()
    {
      names = new List<string>() { "Abraham", "Lincoln", "James", "Buchanan" };
      selectedNames = new ObservableCollection<string>();
      listView = new ListView();
      listView.ItemsSource = names;
      listView.SelectionMode = SelectionMode.Extended;
      MultiSelectorBehaviours.SetSynchronizedSelectedItems(listView, (IList)selectedNames);
    }

    [TestMethod]
    public void InitialiseToNoSelection()
    {
      this.selectedNames.Count().Should().Be(0);
    }

    [TestMethod]
    public void ShouldSynchroniseListViewSelectAll()
    {
      // Act
      this.listView.SelectAll();

      // Assert
      this.selectedNames.Count().Should().Be(this.names.Count());
    }

    [TestMethod]
    public void ShouldSynchroniseListViewSetSelectedIndex()
    {
      // Act
      this.listView.SelectedIndex = 0;

      // Assert
      this.selectedNames.Count().Should().Be(1);
    }

    [TestMethod]
    public void ShouldSynchroniseListViewSetSelectedItem()
    {
      //Arrange
      Random random = new Random(DateTime.Now.Millisecond);
      object itemToSelect = this.listView.Items.GetItemAt(random.Next(this.names.Count));

      // Act
      this.listView.SelectedItem = itemToSelect;

      // Assert
      this.selectedNames.Count().Should().Be(1);
    }

    [TestMethod]
    public void ShouldSynchroniseListViewAddSingleSelectedItem()
    {
      //Arrange
      Random random = new Random(DateTime.Now.Millisecond);
      object itemToSelect = this.listView.Items.GetItemAt(random.Next(this.names.Count));

      // Act
      this.listView.SelectedItems.Add(itemToSelect);

      // Assert
      this.selectedNames.Count().Should().Be(1);
    }

    [TestMethod]
    public void ShouldSynchroniseListViewAddMultipleSelectedItems()
    {
      // Act
      this.listView.SelectedItems.Add(this.listView.Items.GetItemAt(0));
      this.listView.SelectedItems.Add(this.listView.Items.GetItemAt(1));

      // Assert
      this.selectedNames.ShouldBeEquivalentTo(this.listView.SelectedItems, opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldSynchroniseListAddMultipleSelectedItems()
    {
      // Act
      this.selectedNames.Add(this.names.First());
      this.selectedNames.Add(this.names.Last());

      // Assert
      this.listView.SelectedItems.ShouldBeEquivalentTo(this.selectedNames, opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldNotSynchroniseListAddMultipleSelectedItems()
    {
      //Arrange
      ObservableCollection<string> secondSelectedNames = new ObservableCollection<string>();

      // Act
      MultiSelectorBehaviours.SetSynchronizedSelectedItems(listView, (IList)secondSelectedNames);
      this.selectedNames.Add(this.names.First());
      this.selectedNames.Add(this.names.Last());

      // Assert
      this.listView.SelectedItems.Should().NotBeEquivalentTo(this.selectedNames);
    }

    [TestMethod]
    public void ShouldSynchroniseChangedListAddMultipleSelectedItems()
    {
      //Arrange
      ObservableCollection<string> secondSelectedNames = new ObservableCollection<string>();

      // Act
      MultiSelectorBehaviours.SetSynchronizedSelectedItems(listView, (IList)secondSelectedNames);
      secondSelectedNames.Add(this.names.First());
      secondSelectedNames.Add(this.names.Last());

      // Assert
      this.listView.SelectedItems.ShouldBeEquivalentTo(secondSelectedNames);
    }
  }
}
