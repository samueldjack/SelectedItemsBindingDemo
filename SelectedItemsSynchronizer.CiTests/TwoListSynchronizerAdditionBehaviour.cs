namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TwoListSynchronizerAdditionBehaviour
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
      this.targetList = new ObservableCollection<string>();
      this.sut = new TwoListSynchronizer(this.masterList, this.targetList);
    }

    [TestMethod]
    public void ShouldNotSynchroniseAddition()
    {
      // Act
      this.masterList.Add("testString");

      // Assert
      this.targetList.Count().Should().Be(0);
    }

    [TestMethod]
    public void ShouldSynchroniseAddition()
    {
      // Arrange
      this.sut.StartSynchronizing();

      // Act
      this.masterList.Add("testString");

      // Assert
      this.targetList.Count().Should().Be(1);
    }

    [TestMethod]
    public void ShouldSynchroniseAdditionOnTarget()
    {
      // Arrange
      this.sut.StartSynchronizing();

      // Act
      this.targetList.Add("testString");

      // Assert
      this.masterList.Count().Should().Be(1);
    }
  }
}