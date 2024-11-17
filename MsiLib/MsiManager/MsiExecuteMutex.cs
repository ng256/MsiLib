using System;
using System.Security.AccessControl;
using System.Threading;

namespace MsiInstaller
{
    /// <summary>
    /// Represents a class that manages the acquisition and release of the Global\_MSIExecute mutex,
    /// ensuring that only one MSI operation can be executed at a time.
    /// </summary>
    public class MsiExecuteMutex : IDisposable
    {
        private const string MUTEX_NAME = @"Global\_MSIExecute"; // Mutex name
        private Mutex _mutex; // The mutex instance
        private bool _ownsMutex; // Flag indicating whether this instance owns the mutex

        /// <summary>
        /// Attempts to create or open the Global\_MSIExecute mutex.
        /// If the mutex is successfully acquired, the current instance becomes the owner.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the mutex is already acquired or another MSI operation holds it.</exception>
        public void AcquireMutex()
        {
            if (_mutex != null)
            {
                throw new InvalidOperationException("Mutex is already acquired.");
            }

            // Try to open or create the mutex
            _mutex = new Mutex(initiallyOwned: false, name: MUTEX_NAME, createdNew: out _ownsMutex);

            if (!_ownsMutex)
            {
                throw new InvalidOperationException("Another MSI operation is currently holding the mutex.");
            }
        }

        /// <summary>
        /// Checks if the Global\_MSIExecute mutex is already held by another process.
        /// </summary>
        /// <returns>True if the mutex is currently held, false otherwise.</returns>
        public static bool IsMutexHeld()
        {
            try
            {
                // Try to open the mutex without acquiring ownership
                using (var mutex = Mutex.OpenExisting(MUTEX_NAME, MutexRights.Synchronize))
                {
                    return true; // If it exists, it is being held
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // If the mutex does not exist, it is not being held
                return false;
            }
        }

        /// <summary>
        /// Waits until the Global\_MSIExecute mutex becomes available.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds to wait for the mutex. Use Timeout.Infinite for no timeout.</param>
        /// <returns>True if the mutex was acquired, false if the timeout elapsed before acquiring the mutex.</returns>
        public bool WaitForMutex(int timeout = Timeout.Infinite)
        {
            // If mutex is not already initialized, create a new one
            if (_mutex == null)
            {
                _mutex = new Mutex(initiallyOwned: false, name: MUTEX_NAME, createdNew: out _);
            }

            // Wait for the mutex and return whether it was successfully acquired
            _ownsMutex = _mutex.WaitOne(timeout);
            return _ownsMutex;
        }

        /// <summary>
        /// Releases the mutex if it is owned by the current instance.
        /// </summary>
        public void ReleaseMutex()
        {
            if (_mutex != null && _ownsMutex)
            {
                // Release the mutex if this instance owns it
                _mutex.ReleaseMutex();
                _ownsMutex = false;
            }
        }

        /// <summary>
        /// Disposes of the mutex resources and releases ownership if applicable.
        /// </summary>
        public void Dispose()
        {
            // Release the mutex if owned, then dispose the mutex resource
            ReleaseMutex();
            _mutex?.Dispose();
            _mutex = null;
        }
    }
}
