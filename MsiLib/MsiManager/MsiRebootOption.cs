namespace MsiInstaller
{
    /// <summary>
    /// Enum for REBOOT property. Specifies the reboot behavior after installation.
    /// </summary>
    public enum MsiRebootOption
    {
        /// <summary>
        /// No reboot is required.
        /// </summary>
        None,

        /// <summary>
        /// Forces a reboot after installation.
        /// </summary>
        Force,

        /// <summary>
        /// Suppresses the reboot prompt, but still allows a reboot if required.
        /// </summary>
        Suppress,

        /// <summary>
        /// Prevents any reboot prompt after installation.
        /// </summary>
        ReallySuppress
    }
}