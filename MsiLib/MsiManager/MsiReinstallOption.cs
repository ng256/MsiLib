namespace MsiInstaller
{
    /// <summary>
    /// Enum for REINSTALL property. Specifies how to handle reinstallation.
    /// </summary>
    public enum MsiReinstallOption
    {
        /// <summary>
        /// No reinstallation.
        /// </summary>
        None,

        /// <summary>
        /// Reinstall all files, even if they are already installed.
        /// </summary>
        All
    }
}