using System;
using System.Runtime.InteropServices;

namespace MsiInstaller
{
    /// <summary>
    /// Representing possible installation results from MsiInstallProduct
    /// </summary>
    public enum InstallResult : uint
    {
        /// <summary>
        /// The installation was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// A fatal error occurred during installation.
        /// </summary>
        FatalError = 1603,

        /// <summary>
        /// Another MSI installation is currently in progress.
        /// </summary>
        InstallationInProgress = 1618,

        /// <summary>
        /// The command line provided to the installer is invalid.
        /// </summary>
        InvalidCommandLine = 1624,
    }

    /// <summary>
    /// Manages MSI product installations using the MsiInstallProduct function from msi.dll.
    /// </summary>
    public class MsiManager
    {
        // The MsiInstallProduct function installs or uninstalls an MSI package.
        // This function is part of the Windows Installer API and is used for initiating the installation of an MSI package.
        // The command line can be used to customize the installation behavior.
        // If another MSI installation is currently in progress, the function will fail with error code 1618.
        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        private static extern uint MsiInstallProduct(string packagePath, string commandLine);

        /// <summary>
        /// Executes the installation of the MSI package.
        /// </summary>
        /// <param name="msiPath">Path to the MSI package file.</param>
        /// <param name="settings">The settings for the MSI installation (e.g., installation directory, reboot options).</param>
        /// <returns>The result of the installation attempt.</returns>
        /// <exception cref="ArgumentException">Thrown if the MSI file path is null or empty.</exception>
        public static InstallResult InstallProduct(string msiPath, MsiSettings settings)
        {
            // Validate the MSI path
            if (string.IsNullOrEmpty(msiPath))
                throw new ArgumentException("Path to MSI file cannot be null or empty.", nameof(msiPath));

            // Build the command line string based on the provided settings
            string commandLine = settings.BuildCommandLine();

            // Call the MsiInstallProduct function and capture the result
            uint result = MsiInstallProduct(msiPath, commandLine);

            // Return the corresponding InstallResult enum value
            return (InstallResult)result;
        }
    }
}
