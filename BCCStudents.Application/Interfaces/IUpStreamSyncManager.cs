using System;
using BCCStudents.Application.Services.Sync;

namespace BCCStudents.Application.Interfaces
{
    /// <summary>
    /// UpStream სინქრონიზაციის მენეჯერის ინტერფეისი
    /// </summary>
    public interface IUpStreamSyncManager : IDisposable
    {
        /// <summary>
        /// Event რომელიც იძახება სინქრონიზაციის დასრულებისას
        /// </summary>
        event EventHandler<SyncStatusEventArgs> SyncCompleted;

        /// <summary>
        /// იწყებს პერიოდულ სინქრონიზაციას
        /// </summary>
        void Start();

        /// <summary>
        /// აჩერებს პერიოდულ სინქრონიზაციას
        /// </summary>
        void Stop();
    }
}

