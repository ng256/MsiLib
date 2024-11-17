using MsiInstaller;

/// <summary>
/// Represents a registry value to be installed or modified during the MSI installation.
/// This class encapsulates the registry root, key, value, type, and key path used for MSI installation.
/// </summary>
public class WixRegistryValue
{
    /// <summary>
    /// The root of the registry key (e.g., HKEY_LOCAL_MACHINE, HKEY_CURRENT_USER).
    /// Specifies which root key the registry entry belongs to.
    /// </summary>
    public string Root { get; set; }

    /// <summary>
    /// The registry key path where the value will be stored.
    /// Represents the full path of the registry key (e.g., "Software\\MyApp").
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The name of the registry value.
    /// This is the actual name of the value under the specified registry key.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The value to be assigned to the registry key.
    /// This can be a string, number, or binary value depending on the registry type.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// The type of the registry value (e.g., "String", "DWORD").
    /// This defines what kind of data the registry value holds (e.g., string, number, binary).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The key path indicating whether this registry value is considered part of the key path in the Wix format.
    /// "Yes" means the registry value is considered a key path, and "No" means it is not.
    /// This corresponds to the <see cref="WixKeyPath"/> enum values ("yes" or "no").
    /// </summary>
    public WixKeyPath KeyPath { get; set; }
}