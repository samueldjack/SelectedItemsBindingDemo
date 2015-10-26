namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System.Collections.ObjectModel;
  using System.Linq;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TwoListSynchronizerInsertionBehaviour
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
      this.masterList.Add("Initialised");
      this.targetList = new ObservableCollection<string>();
      this.sut = new TwoListSynchronizer(this.masterList, this.targetList);
      this.sut.StartSynchronizing();
    }

    [TestMethod]
    public void ShouldBeSynchronised()
    {
      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList);
    }

    [TestMethod]
    public void ShouldNotSynchroniseInsertion()
    {
      // Arrange
      this.sut.StopSynchronizing();

      // Act
      this.masterList.Insert(0, "Insertion");

      // Assert
      this.targetList.Count().Should().Be(this.masterList.Count() - 1);
    }

    [TestMethod]
    public void ShouldSynchroniseInsertion()
    {
      // Act
      this.masterList.Insert(0, "Insertion");

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList, opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldSynchroniseInsertionOnTarget()
    {
      // Act
      this.targetList.Insert(0, "Insertion");

      // Assert
      this.masterList.ShouldBeEquivalentTo(this.targetList, opt => opt.WithStrictOrdering());
    }
  }
}