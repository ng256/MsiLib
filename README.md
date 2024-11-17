# MsiLib

MsiLib is a library that simplifies the process of managing MSI (Microsoft Installer) product installations using the `msi.dll` API and providing utility classes for manipulating MSI settings, registry values, and more. It provides an easy-to-use wrapper around MSI installation functions, including the ability to control installation options, registry values, and key paths.

## Features

- **MSI Installation Management**: Simplified methods to install MSI packages with custom settings like installation directory, reboot behavior, quiet mode, and more.
- **Registry Management**: Support for managing registry values that are part of the MSI installation process.
- **Mutex Handling**: A class to acquire and release mutexes used by the MSI installer to prevent concurrent installations.
- **Wix Support**: Support for setting key paths and registry values in a Wix-style configuration.

## Installation

To use MsiLib in your project, you can download the repository and include it as a submodule or copy the relevant source files into your project.

### NuGet Package (Coming Soon)

We are planning to release the library as a NuGet package. Stay tuned for updates!

## Usage

### Installing a Product

To install an MSI package, you can use the `MsiManager` class to install it with custom settings. Here’s a basic example of how to use it:

```csharp
using MsiInstaller;

public class MsiExample
{
    public static void Main()
    {
        // Define installation settings
        MsiSettings settings = new MsiSettings
        {
            InstallDir = @"C:\MyApp",
            AllUsers = AllUsers.CurrentUser,
            Reboot = RebootOption.NoReboot,
            QuietMode = QuietMode.BasicUI
        };

        // Install the MSI package
        MsiManager.InstallProduct(@"C:\path\to\your.msi", settings);
    }
}
```
### MsiManager Class Usage
`MsiManager` is a class that simplifies the process of installing MSI packages using the `MsiInstallProduct` function from the Windows Installer API (`msi.dll`). This function allows you to initiate the installation or uninstallation of an MSI package with custom settings, such as installation directory, reboot options, and more.

```csharp
using MsiInstaller;

public class Program
{
    public static void Main()
    {
        // Define installation settings
        MsiSettings settings = new MsiSettings
        {
            InstallDir = @"C:\Program Files\MyApp", // Installation directory
            AllUsers = AllUsers.CurrentUser,        // Installation for current user only
            Reboot = RebootOption.NoReboot,         // No reboot after installation
            QuietMode = QuietMode.NoUI             // No user interface during installation
        };

        // Path to the MSI file
        string msiFilePath = @"C:\path\to\your.msi";

        // Execute the installation
        InstallResult result = MsiManager.InstallProduct(msiFilePath, settings);

        // Check the installation result
        switch (result)
        {
            case InstallResult.Success:
                Console.WriteLine("Installation successful!");
                break;
            case InstallResult.FatalError:
                Console.WriteLine("Fatal error during installation.");
                break;
            case InstallResult.InstallationInProgress:
                Console.WriteLine("Another installation is currently in progress.");
                break;
            case InstallResult.InvalidCommandLine:
                Console.WriteLine("Invalid command line provided.");
                break;
        }
    }
}
```

### Enum: InstallResult
The InstallResult enum represents the possible outcomes of the MSI installation process.

Values:
- Success (0): The installation was successful.
- FatalError (1603): A fatal error occurred during the installation.
- InstallationInProgress (1618): Another MSI installation is currently in progress.
- InvalidCommandLine (1624): The command line provided to the installer is invalid.

### Method: InstallProduct
The InstallProduct method in the MsiManager class installs or uninstalls an MSI package using the msi.dll API. You can customize the installation process by providing an MsiSettings object.

Parameters:
- msiPath (string): The full path to the MSI file that you want to install.
- settings (MsiSettings): An instance of the MsiSettings class that defines custom installation settings (e.g., installation directory, reboot options, and more).

Return Value:
The method returns an InstallResult enum that indicates the result of the installation process.

Exceptions:
ArgumentException: Thrown if the msiPath parameter is null or empty.

### Mutex Handling
MsiLib includes a mutex manager to ensure that only one MSI installation process runs at a time:

```csharp
using MsiInstaller;

public class MutexExample
{
    public static void Main()
    {
        MsiExecuteMutex mutex = new MsiExecuteMutex();
        
        if (mutex.WaitForMutex())
        {
            // Proceed with the installation process
            mutex.ReleaseMutex();
        }
        else
        {
            Console.WriteLine("MSI installation is already in progress.");
        }
    }
}
```
