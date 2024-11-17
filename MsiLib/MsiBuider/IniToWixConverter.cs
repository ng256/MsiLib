using System;
using System.IO;
using System.Text;

namespace MsiInstaller
{
    // Converts an INI configuration file to a WixFile object, which can be used for creating an MSI installer.
    internal class IniToWixConverter
    {
        private readonly IniFile _iniFile;

        public IniToWixConverter(string iniFilePath, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(iniFilePath) || !File.Exists(iniFilePath))
                throw new FileNotFoundException("INI file not found.", iniFilePath);

            this._iniFile = new IniFile(iniFilePath, encoding);
        }

        // Converts the INI file to a WiX format, which represents the data required to create an MSI installer.
        public WixFile ConvertToWixBuilder()
        {
            var wixBuilder = new WixFile
            {
                ProductName = _iniFile.GetEntry("Product", "Name", "DefaultProduct"),
                Manufacturer = _iniFile.GetEntry("Product", "Manufacturer", "DefaultManufacturer"),
                Version = _iniFile.GetEntry("Product", "Version", "1.0.0.0"),
                UpgradeCode = new Guid(_iniFile.GetEntry("Product", "UpgradeCode", GenerateGuid()))
            };

            // Add files from INI configuration
            foreach (var fileEntry in _iniFile.GetEntries("Files"))
            {
                var parts = fileEntry.Split('=');
                if (parts.Length == 2)
                {
                    wixBuilder.AddFile(parts[1], parts[0]);
                }
            }

            // Add registry keys from INI configuration
            foreach (var regEntry in _iniFile.GetEntries("Registry"))
            {
                var parts = regEntry.Split(',');
                if (parts.Length >= 4) // Expecting Root, Key, Name, Value, Type
                {
                    var root = parts[0];
                    var key = parts[1];
                    var name = parts[2];
                    var value = parts[3];
                    var type = parts.Length > 4 ? parts[4].ToUpperInvariant() : "STRING"; // Default to STRING

                    wixBuilder.AddRegistryKeyInternal(
                        root,
                        key,
                        name,
                        ParseRegistryType(type),
                        value,
                        default);
                }
            }

            // Add custom actions from INI configuration
            foreach (var actionEntry in _iniFile.GetEntries("CustomActions"))
            {
                var parts = actionEntry.Split('=');
                if (parts.Length == 2)
                {
                    wixBuilder.AddCustomAction(parts[0], parts[1], string.Empty);
                }
            }

            return wixBuilder;
        }

        // Generates a GUID in the "B" format, which includes curly braces.
        private static string GenerateGuid()
        {
            return Guid.NewGuid().ToString("B");
        }

        // Parses the registry type from the INI file and converts it to a Wix-compatible format.
        private static string ParseRegistryType(string type)
        {
            switch (type)
            {
                case "INT32":
                    return "integer";
                case "INT64":
                    return "integer";
                case "BYTE":
                    return "binary";
                case "MULTISTRING":
                    return "multiString";
                default:
                    return "string";
            }
        }
    }
}
