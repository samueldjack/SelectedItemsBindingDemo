namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System.Collections.ObjectModel;
  using System.Linq;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TwoListSynchronizerDeletionBehaviour
  {
    private ObservableCollection<string> masterList;
    private ObservableCollection<string> targetList;
    private TwoListSynchronizer sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
    }

    [TestInitialize]
    public void TestInitialize()
    {
      this.masterList = new ObservableCollection<string>();
      this.masterList.Add("InitialisedFirst");
      this.masterList.Add("InitialisedSecond");
      this.targetList = new ObservableCollection<string>();
      this.sut = new TwoListSynchronizer(this.masterList, this.targetList);
      this.sut.StartSynchronizing();
    }

    [TestMethod]
    public void ShouldBeSynchronised()
    {
      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList, opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldNotSynchroniseDeletion()
    {
      // Arrange
      this.sut.StopSynchronizing();

      // Act
      this.masterList.Remove("InitialisedFirst");

      // Assert
      this.targetList.Count().Should().Be(this.masterList.Count() + 1);
    }

    [TestMethod]
    public void ShouldNotSynchroniseDeletionByIndex()
    {
      // Arrange
      this.sut.StopSynchronizing();

      // Act
      this.masterList.RemoveAt(0);

      // Assert
      this.targetList.Count().Should().Be(this.masterList.Count() + 1);
    }

    [TestMethod]
    public void ShouldSynchroniseDeletion()
    {
      // Act
      this.masterList.Remove("InitialisedFirst");

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList);
    }

    [TestMethod]
    public void ShouldSynchroniseDeletionByIndex()
    {
      // Act
      this.masterList.RemoveAt(0);

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList);
    }

    [TestMethod]
    public void ShouldNotSynchroniseClearing()
    {
      // Arrange
      this.sut.StopSynchronizing();
      int priorCount = this.masterList.Count();

      // Act
      this.masterList.Clear();
      
      // Assert
      this.targetList.Count().Should().Be(priorCount);
    }

    [TestMethod]
    public void ShouldSynchroniseClearing()
    {
      // Act
      this.masterList.Clear();

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList);
    }

    [TestMethod]
    public void ShouldSynchroniseClearingOfTarget()
    {
      // Act
      this.targetList.Clear();

      // Assert
      this.masterList.ShouldBeEquivalentTo(this.targetList);
    }
  }
}