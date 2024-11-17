using System.Text;

namespace MsiInstaller
{
    /// <summary>
    /// Represents the settings for an MSI installation.
    /// </summary>
    public class MsiSettings
    {
        /// <summary>
        /// Gets or sets the path to the installation directory.
        /// </summary>
        public string InstallDir { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the installation mode for all users (either current user or all users).
        /// </summary>
        public MsiAllUsers AllUsers { get; set; } = MsiAllUsers.CurrentUser;

        /// <summary>
        /// Gets or sets the reboot option after installation.
        /// </summary>
        public MsiRebootOption Reboot { get; set; } = MsiRebootOption.None;

        /// <summary>
        /// Gets or sets the reinstall option for the installation.
        /// </summary>
        public MsiReinstallOption Reinstall { get; set; } = MsiReinstallOption.None;

        /// <summary>
        /// Gets or sets the reinstall mode, defining the conditions under which reinstall should happen.
        /// </summary>
        public MsiReinstallMode ReinstallMode { get; set; } = MsiReinstallMode.Default;

        /// <summary>
        /// Gets or sets the quiet mode setting for installation (e.g., silent or basic UI).
        /// </summary>
        public MsiQuietMode QuietMode { get; set; } = MsiQuietMode.Default;

        /// <summary>
        /// Gets or sets the path to the MST transform file for customizing the installation.
        /// </summary>
        public string Transforms { get; set; } = string.Empty;

        /// <summary>
        /// Builds the command line arguments for the MSI installer product.
        /// </summary>
        /// <returns>The formatted command line as a string.</returns>
        public string BuildCommandLine()
        {
            StringBuilder commandLine = new StringBuilder();

            // Adds INSTALLDIR argument if specified
            if (!string.IsNullOrEmpty(InstallDir))
                commandLine.Append($"INSTALLDIR=\"{InstallDir}\" ");

            // Adds ALLUSERS argument if the installation is for all users
            if (AllUsers != MsiAllUsers.CurrentUser)
                commandLine.Append($"ALLUSERS={(int)AllUsers} ");

            // Adds REBOOT argument if the reboot option is not 'None'
            if (Reboot != MsiRebootOption.None)
                commandLine.Append($"REBOOT={Reboot} ");

            // Adds REINSTALL argument if the reinstall option is not 'None'
            if (Reinstall != MsiReinstallOption.None)
                commandLine.Append("REINSTALL=ALL ");

            // Adds REINSTALLMODE argument if the reinstall mode is not 'Default'
            if (ReinstallMode != MsiReinstallMode.Default)
                commandLine.Append($"REINSTALLMODE={(char)ReinstallMode} ");

            // Adds quiet mode arguments based on the selected mode
            if (QuietMode == MsiQuietMode.NoUI)
                commandLine.Append("/qn ");
            else if (QuietMode == MsiQuietMode.BasicUI)
                commandLine.Append("/qb ");

            // Adds TRANSFORMS argument if the MST file path is specified
            if (!string.IsNullOrEmpty(Transforms))
                commandLine.Append($"TRANSFORMS=\"{Transforms}\" ");

            // Returns the built command line string, trimmed of excess whitespace
            return commandLine.ToString().Trim();
        }
    }
}
