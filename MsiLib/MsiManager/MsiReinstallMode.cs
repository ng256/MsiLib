namespace MsiInstaller
{
    /// <summary>
    /// Enum for REINSTALLMODE property. Specifies the mode for reinstalling files.
    /// </summary>
    public enum MsiReinstallMode
    {
        /// <summary>
        /// Default reinstall mode; no override.
        /// </summary>
        Default,

        /// <summary>
        /// Reinstall files if they are missing.
        /// </summary>
        FileAbsent = 'e',

        /// <summary>
        /// Reinstall if an older version of the file exists.
        /// </summary>
        FileOlderVersion = 'm',

        /// <summary>
        /// Overwrite all files during reinstall.
        /// </summary>
        FileDifferentVersion = 'a',

        /// <summary>
        /// Validate the version of the files and repair if necessary.
        /// </summary>
        VerifyAndRepair = 'v'
    }
}