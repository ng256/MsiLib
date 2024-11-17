namespace MsiInstaller
{
    /// <summary>
    /// Enum for ALLUSERS property. Specifies how the installation will handle user-specific vs machine-wide installations.
    /// </summary>
    public enum MsiAllUsers
    {
        /// <summary>
        /// Installation applies only to the current user.
        /// </summary>
        CurrentUser = 0,

        /// <summary>
        /// Installation applies to all users of the machine.
        /// </summary>
        AllUsers = 1,

        /// <summary>
        /// Installation applies to all users but only for machine-wide administrative privileges.
        /// </summary>
        PerMachineAdmin = 2
    }
}
