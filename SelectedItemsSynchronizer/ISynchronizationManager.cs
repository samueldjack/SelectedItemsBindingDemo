namespace PrimS.SelectedItemsSynchronizer
{
  using System;

  /// <summary>
  /// Provides functionality to interact with the Synchronization Manager.
  /// </summary>
  public interface ISynchronizationManager
  {
    /// <summary>
    /// Starts the synchronizing.
    /// </summary>
    void StartSynchronizing();

    /// <summary>
    /// Stops the synchronizing.
    /// </summary>
    void StopSynchronizing();
  }
}