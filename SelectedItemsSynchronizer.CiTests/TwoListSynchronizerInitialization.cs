namespace PrimS.SelectedItemsSynchronizer.CiTests
{
  using System.Collections.ObjectModel;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TwoListSynchronizerInitialization
  {
    [TestMethod]
    public void ShouldCopyInitialMembersToTarget()
    {
      ObservableCollection<string> masterList = new ObservableCollection<string>();
      masterList.Add("Initial");
      ObservableCollection<string> targetList = new ObservableCollection<string>();

      TwoListSynchronizer sut = new TwoListSynchronizer(masterList, targetList);
      sut.StartSynchronizing();

      targetList.ShouldBeEquivalentTo(masterList, "ctor should initialise targetList.");
    }

    [TestMethod]
    public void ShouldCopyInitialMembersFromTarget()
    {
      ObservableCollection<string> masterList = new ObservableCollection<string>();
      ObservableCollection<string> targetList = new ObservableCollection<string>();
      targetList.Add("Initial");
      
      TwoListSynchronizer sut = new TwoListSynchronizer(masterList, targetList);
      sut.StartSynchronizing();

      targetList.ShouldBeEquivalentTo(masterList, "ctor should initialise targetList.");
      targetList.Should().BeEmpty("ctor should overwrite targetList with masterList content.");
    }
  }
}