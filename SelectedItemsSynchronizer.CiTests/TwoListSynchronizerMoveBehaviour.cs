namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System.Collections.ObjectModel;
  using System.Linq;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TwoListSynchronizerMoveBehaviour
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
      this.targetList.ShouldBeEquivalentTo(this.masterList);
    }

    [TestMethod]
    public void ShouldNotSynchroniseMove()
    {
      // Arrange
      this.sut.StopSynchronizing();

      // Act
      this.masterList.Move(0, 1);

      // Assert
      this.targetList.First().Should().Be(this.masterList.Last());
      this.targetList.ShouldBeEquivalentTo(this.masterList.Reverse(), opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldSynchroniseMove()
    {
      // Act
      this.masterList.Move(0, 1);

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList, opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldSynchroniseMoveOnTarget()
    {
      // Act
      this.targetList.Move(0, 1);

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList, opt => opt.WithStrictOrdering());
    }
  }
}