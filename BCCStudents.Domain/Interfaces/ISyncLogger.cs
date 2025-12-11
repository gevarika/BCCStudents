// Pseudocode / Plan:
// - Add an interface named ISyncLogger to the BCCStudents.Domain.Interfaces namespace.
// - The SyncLogger class implements ISyncLogger and expects the following members:
//     - void Info(string message);
//     - void Warn(string message);
//     - void Error(string message, Exception exception = null);
// - Implement the interface with matching signatures so SyncLogger can compile.
// - Keep the interface minimal and focused on the logging contract.

using System;

namespace BCCStudents.Domain.Interfaces
{
    public interface ISyncLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception exception = null);
    }
}
