using System;
namespace PrimS.SelectedItemsSynchronizer
{
  public interface ISynchronizationManager
  {
    void StartSynchronizing();
    void StopSynchronizing();
  }
}
