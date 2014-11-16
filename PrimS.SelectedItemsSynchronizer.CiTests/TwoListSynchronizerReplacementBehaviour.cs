namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System.Collections.ObjectModel;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TwoListSynchronizerReplacementBehaviour
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
    public void ShouldNotSynchroniseReplacement()
    {
      // Arrange
      this.sut.StopSynchronizing();

      // Act
      this.masterList[0] = "Replaced";

      // Assert
      this.targetList.Should().NotBeEquivalentTo(this.masterList);
    }

    [TestMethod]
    public void ShouldSynchroniseReplacement()
    {
      // Act
      this.masterList[0] = "Replaced";

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList, opt => opt.WithStrictOrdering());
    }

    [TestMethod]
    public void ShouldSynchroniseReplacementOnTarget()
    {
      // Act
      this.targetList[0] = "Replaced";

      // Assert
      this.targetList.ShouldBeEquivalentTo(this.masterList, opt => opt.WithStrictOrdering());
    }
  }
}