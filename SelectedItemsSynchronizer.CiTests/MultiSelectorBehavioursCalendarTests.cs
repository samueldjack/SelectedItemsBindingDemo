namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System;
  using System.Collections;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Controls;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  
  [TestClass]
  public class MultiSelectorBehavioursCalendarTests
  {
    ObservableCollection<DateTime> selectedDates;
    Calendar calendar;

    [TestInitialize]
    public void TestInitialize()
    {
      selectedDates = new ObservableCollection<DateTime>();
      calendar = new Calendar();
      calendar.SelectionMode = CalendarSelectionMode.SingleRange;
      MultiSelectorBehaviours.SetSynchronizedSelectedItems(calendar, (IList)selectedDates);
    }

    [TestMethod]
    public void InitialiseToNoSelection()
    {
      this.selectedDates.Count().Should().Be(0);
    }

    [TestMethod]
    public void SelectInCollectionShouldReflectOnCalendar()
    {
      // Arrange
      this.calendar.MonitorEvents();

      // Act
      this.selectedDates.Add(DateTime.Today);

      // Assert
      this.calendar.ShouldRaise("SelectedDatesChanged");
    }

    [TestMethod]
    public void SelectOnCalendarShouldReflectInCollection()
    {
      // Act
      this.calendar.SelectedDates.Add(DateTime.Today);

      // Assert
      this.selectedDates.Count().Should().Be(1);
    }

    [TestMethod]
    public void SelectConsecutiveOnCalendarShouldReflectInCollection()
    {
      // Act
      this.calendar.SelectedDates.AddRange(DateTime.Today, DateTime.Today.AddDays(1));

      // Assert
      this.selectedDates.Count().Should().Be(2);
    }

    [Ignore]
    [TestMethod]
    public void SelectConsecutiveInCollectionShouldReflectOnCalendar()
    {
      //This test is failing to highlight the fact that the implementation currently only works OneWayToSource due to the 
      //odd behaviour of SelectedDateCollection.Add.

      // Act
      this.selectedDates.Add(DateTime.Today);
      this.selectedDates.Add(DateTime.Today.AddDays(1));

      // Assert
      this.selectedDates.Count().Should().Be(2);
      this.calendar.SelectedDates.Count().Should().Be(2);
    }
  }
}