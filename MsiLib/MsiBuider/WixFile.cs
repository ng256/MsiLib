using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MsiInstaller
{
    /// <summary>
    /// Represents a WiX (Windows Installer XML) file used for building an MSI installer.
    /// This class handles the serialization and deserialization of WiX files, and provides methods 
    /// for adding registry keys, files, and custom actions to the installer configuration.
    /// </summary>
    public class WixFile : IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the name of the product being installed.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the unique product ID (GUID).
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the upgrade code (GUID) for the product.
        /// </summary>
        public Guid UpgradeCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the manufacturer of the product.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the version of the product.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the output path for the installer (MSI file).
        /// </summary>
        public string InstallerOutputPath { get; set; }

        /// <summary>
        /// Gets or sets the directory where the product will be installed.
        /// </summary>
        public string InstallDirectory { get; set; }

        /// <summary>
        /// Gets or sets the list of components to be included in the installer.
        /// </summary>
        public List<WixComponentData> Components { get; set; } = new List<WixComponentData>();

        /// <summary>
        /// Gets or sets the list of custom actions to be executed during installation.
        /// </summary>
        public List<WixCustomActionData> CustomActions { get; set; } = new List<WixCustomActionData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WixFile"/> class with default values.
        /// </summary>
        public WixFile()
        {
            ProductName = "MyProduct";
            ProductId = "*";
            UpgradeCode = GenerateGuid();
            Manufacturer = "MyCompany";
            Version = "1.0.0.0";
            InstallerOutputPath = "Installer.msi";
            InstallDirectory = "INSTALLFOLDER";
        }

        /// <summary>
        /// Adds a registry key with a string value to the WiX installer.
        /// </summary>
        /// <param name="root">The root of the registry key (e.g., HKLM, HKCU).</param>
        /// <param name="key">The path of the registry key.</param>
        /// <param name="name">The name of the registry value.</param>
        /// <param name="value">The string value of the registry entry.</param>
        /// <param name="componentGuid">The GUID of the component to which the registry key belongs (optional).</param>
        public void AddRegistryKey(string root, string key, string name, string value, Guid componentGuid = default)
        {
            AddRegistryKeyInternal(root, key, name, "string", value, componentGuid);
        }

        /// <summary>
        /// Adds a registry key with an integer value to the WiX installer.
        /// </summary>
        /// <param name="root">The root of the registry key.</param>
        /// <param name="key">The path of the registry key.</param>
        /// <param name="name">The name of the registry value.</param>
        /// <param name="value">The integer value of the registry entry.</param>
        /// <param name="componentGuid">The GUID of the component to which the registry key belongs (optional).</param>
        public void AddRegistryKey(string root, string key, string name, int value, Guid componentGuid = default)
        {
            AddRegistryKeyInternal(root, key, name, "integer", value.ToString(), componentGuid);
        }

        /// <summary>
        /// Adds a registry key with a long value to the WiX installer.
        /// </summary>
        /// <param name="root">The root of the registry key.</param>
        /// <param name="key">The path of the registry key.</param>
        /// <param name="name">The name of the registry value.</param>
        /// <param name="value">The long value of the registry entry.</param>
        /// <param name="componentGuid">The GUID of the component to which the registry key belongs (optional).</param>
        public void AddRegistryKey(string root, string key, string name, long value, Guid componentGuid = default)
        {
            AddRegistryKeyInternal(root, key, name, "integer", value.ToString(), componentGuid);
        }

        /// <summary>
        /// Adds a registry key with a string array value (multi-string) to the WiX installer.
        /// </summary>
        /// <param name="root">The root of the registry key.</param>
        /// <param name="key">The path of the registry key.</param>
        /// <param name="name">The name of the registry value.</param>
        /// <param name="value">The string array value of the registry entry.</param>
        /// <param name="componentGuid">The GUID of the component to which the registry key belongs (optional).</param>
        public void AddRegistryKey(string root, string key, string name, string[] value, Guid componentGuid = default)
        {
            var multiStringValue = string.Join("\\0", value) + "\\0";
            AddRegistryKeyInternal(root, key, name, "multiString", multiStringValue, componentGuid);
        }

        /// <summary>
        /// Adds a registry key with a byte array value (binary) to the WiX installer.
        /// </summary>
        /// <param name="root">The root of the registry key.</param>
        /// <param name="key">The path of the registry key.</param>
        /// <param name="name">The name of the registry value.</param>
        /// <param name="value">The byte array value of the registry entry.</param>
        /// <param name="componentGuid">The GUID of the component to which the registry key belongs (optional).</param>
        public void AddRegistryKey(string root, string key, string name, byte[] value, Guid componentGuid = default)
        {
            var binaryValue = BitConverter.ToString(value).Replace("-", string.Empty);
            AddRegistryKeyInternal(root, key, name, "binary", binaryValue, componentGuid);
        }

        /// <summary>
        /// Internal method to add registry keys (used by public overloads and elsewhere).
        /// </summary>
        /// <param name="root">The root of the registry key.</param>
        /// <param name="key">The path of the registry key.</param>
        /// <param name="name">The name of the registry value.</param>
        /// <param name="type">The type of the registry value (e.g., string, integer, binary).</param>
        /// <param name="value">The value of the registry entry.</param>
        /// <param name="componentGuid">The GUID of the component to which the registry key belongs.</param>
        internal void AddRegistryKeyInternal(string root, string key, string name, string type, string value, Guid componentGuid)
        {
            if (componentGuid.Equals(default))
            {
                componentGuid = GenerateGuid();
            }

            var registryXml = new WixRegistryValue
            {
                Root = root,
                Key = key,
                Name = name,
                Value = value,
                Type = type,
                KeyPath = WixKeyPath.Yes
            };

            var componentXml = new WixComponentData
            {
                Guid = componentGuid,
                RegistryValues = new List<WixRegistryValue> { registryXml }
            };

            Components.Add(componentXml);
        }

        /// <summary>
        /// Adds a file to the WiX installer.
        /// </summary>
        /// <param name="sourcePath">The path of the source file to be added.</param>
        /// <param name="destinationName">The destination name of the file (optional).</param>
        /// <param name="componentGuid">The GUID of the component to which the file belongs (optional).</param>
        public void AddFile(string sourcePath, string destinationName = null, Guid componentGuid = default)
        {
            if (string.IsNullOrEmpty(destinationName))
            {
                destinationName = Path.GetFileName(sourcePath);
            }
            if (componentGuid.Equals(default))
            {
                componentGuid = GenerateGuid();
            }

            var fileXml = new WixFileData
            {
                Id = destinationName,
                Source = sourcePath,
                KeyPath = WixKeyPath.Yes
            };

            var componentXml = new WixComponentData
            {
                Guid = componentGuid,
                Files = new List<WixFileData> { fileXml }
            };

            Components.Add(componentXml);
        }

        /// <summary>
        /// Adds a custom action to the WiX installer.
        /// </summary>
        /// <param name="id">The ID of the custom action.</param>
        /// <param name="exePath">The path to the executable to be run for the custom action.</param>
        /// <param name="arguments">The arguments to pass to the executable.</param>
        public void AddCustomAction(string id, string exePath, string arguments)
        {
            var customAction = new WixCustomActionData
            {
                Id = id,
                ExeCommand = exePath,
                Arguments = arguments,
                Return = "check"
            };
            CustomActions.Add(customAction);
        }

        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        /// <returns>A new GUID.</returns>
        private static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Deserializes a WixFile object from XML.
        /// </summary>
        /// <param name="reader">The XmlReader to read the XML data.</param>
        public void ReadXml(XmlReader reader)
        {
            // Deserialization logic for WixFile
            reader.MoveToContent();

            ProductName = reader.GetAttribute("ProductName");
            ProductId = reader.GetAttribute("ProductId");
            UpgradeCode = new Guid(reader.GetAttribute("UpgradeCode") ?? throw new InvalidOperationException());
            Manufacturer = reader.GetAttribute("Manufacturer");
            Version = reader.GetAttribute("Version");
            InstallerOutputPath = reader.GetAttribute("InstallerOutputPath");
            InstallDirectory = reader.GetAttribute("InstallDirectory");

            reader.ReadStartElement();

            // Deserialize components and actions
            while (reader.IsStartElement("Component"))
            {
                var component = new WixComponentData();
                reader.ReadStartElement("Component");
                component.Guid = new Guid(reader.GetAttribute("Guid") ?? throw new InvalidOperationException());

                while (reader.IsStartElement("File"))
                {
                    var file = new WixFileData
                    {
                        Id = reader.GetAttribute("Id"),
                        Source = reader.GetAttribute("Source"),
                        KeyPath = Enum.TryParse(reader.GetAttribute("KeyPath"), true, out WixKeyPath keyPath) ? keyPath : WixKeyPath.Yes
                    };
                    component.Files.Add(file);
                    reader.Read();
                }

                while (reader.IsStartElement("RegistryValue"))
                {
                    var registry = new WixRegistryValue
                    {
                        Root = reader.GetAttribute("Root"),
                        Key = reader.GetAttribute("Key"),
                        Name = reader.GetAttribute("Name"),
                        Value = reader.GetAttribute("Value"),
                        Type = reader.GetAttribute("Type"),
                        KeyPath = Enum.TryParse(reader.GetAttribute("KeyPath"), true, out WixKeyPath keyPath) ? keyPath : WixKeyPath.Yes
                    };
                    component.RegistryValues.Add(registry);
                    reader.Read();
                }

                Components.Add(component);
                reader.ReadEndElement();
            }

            // Custom actions
            while (reader.IsStartElement("CustomAction"))
            {
                var customAction = new WixCustomActionData
                {
                    Id = reader.GetAttribute("Id"),
                    ExeCommand = reader.GetAttribute("ExeCommand"),
                    Arguments = reader.GetAttribute("ExeArguments"),
                    Return = reader.GetAttribute("Return")
                };
                CustomActions.Add(customAction);
                reader.Read();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Serializes the WixFile object to XML.
        /// </summary>
        /// <param name="writer">The XmlWriter to write the XML data.</param>
        public void WriteXml(XmlWriter writer)
        {
            // Serialization logic for WixFile
            writer.WriteStartElement("WixFile");

            writer.WriteAttributeString("ProductName", ProductName);
            writer.WriteAttributeString("ProductId", ProductId);
            writer.WriteAttributeString("UpgradeCode", UpgradeCode.ToString());
            writer.WriteAttributeString("Manufacturer", Manufacturer);
            writer.WriteAttributeString("Version", Version);
            writer.WriteAttributeString("InstallerOutputPath", InstallerOutputPath);
            writer.WriteAttributeString("InstallDirectory", InstallDirectory);

            foreach (var component in Components)
            {
                writer.WriteStartElement("Component");
                writer.WriteAttributeString("Guid", component.Guid.ToString("B"));

                foreach (var file in component.Files)
                {
                    writer.WriteStartElement("File");
                    writer.WriteAttributeString("Id", file.Id);
                    writer.WriteAttributeString("Source", file.Source);
                    writer.WriteAttributeString("KeyPath", file.KeyPath.ToString());
                    writer.WriteEndElement();
                }

                foreach (var registry in component.RegistryValues)
                {
                    writer.WriteStartElement("RegistryValue");
                    writer.WriteAttributeString("Root", registry.Root);
                    writer.WriteAttributeString("Key", registry.Key);
                    writer.WriteAttributeString("Name", registry.Name);
                    writer.WriteAttributeString("Value", registry.Value);
                    writer.WriteAttributeString("Type", registry.Type);
                    writer.WriteAttributeString("KeyPath", registry.KeyPath.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            foreach (var action in CustomActions)
            {
                writer.WriteStartElement("CustomAction");
                writer.WriteAttributeString("Id", action.Id);
                writer.WriteAttributeString("ExeCommand", action.ExeCommand);
                writer.WriteAttributeString("ExeArguments", action.Arguments);
                writer.WriteAttributeString("Return", action.Return);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Returns the schema for XML serialization (not used in this case).
        /// </summary>
        /// <returns>null.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }
    }
}



// Custom Action class


/*using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MsiInstaller
{
    public class WixBuilder
    {
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string UpgradeCode { get; set; }
        public string Manufacturer { get; set; }
        public string Version { get; set; }
        public string InstallerOutputPath { get; set; }
        public string InstallDirectory { get; set; }

        private readonly List<ComponentData> _components = new List<ComponentData>();
        private readonly List<string> _customActions = new List<string>();

        public WixBuilder()
        {
            ProductName = "MyProduct";
            ProductId = "*";
            UpgradeCode = GenerateGuid();
            Manufacturer = "MyCompany";
            Version = "1.0.0.0";
            InstallerOutputPath = "Installer.msi";
            InstallDirectory = "INSTALLFOLDER";
        }

        // Adds a registry key with string value
        public void AddRegistryKey(string root, string key, string name, string value, string componentGuid = null)
        {
            AddRegistryKeyInternal(root, key, name, "string", value, componentGuid);
        }

        // Adds a registry key with int value
        public void AddRegistryKey(string root, string key, string name, int value, string componentGuid = null)
        {
            AddRegistryKeyInternal(root, key, name, "integer", value.ToString(), componentGuid);
        }

        // Adds a registry key with long value
        public void AddRegistryKey(string root, string key, string name, long value, string componentGuid = null)
        {
            AddRegistryKeyInternal(root, key, name, "integer", value.ToString(), componentGuid);
        }

        // Adds a registry key with string array value (multi-string)
        public void AddRegistryKey(string root, string key, string name, string[] value, string componentGuid = null)
        {
            var multiStringValue = string.Join("\\0", value) + "\\0";
            AddRegistryKeyInternal(root, key, name, "multiString", multiStringValue, componentGuid);
        }

        // Adds a registry key with byte array value (binary)
        public void AddRegistryKey(string root, string key, string name, byte[] value, string componentGuid = null)
        {
            var binaryValue = BitConverter.ToString(value).Replace("-", string.Empty);
            AddRegistryKeyInternal(root, key, name, "binary", binaryValue, componentGuid);
        }

        // Internal method to add registry keys (used by public overloads and elsewhere)
        internal void AddRegistryKeyInternal(string root, string key, string name, string type, string value, string componentGuid)
        {
            if (string.IsNullOrEmpty(componentGuid))
            {
                componentGuid = GenerateGuid();
            }

            var registryXml = string.Format(
                @"<RegistryValue Root='{0}' Key='{1}' Name='{2}' Value='{3}' Type='{4}' KeyPath='yes' />",
                root,
                key,
                name,
                value,
                type);

            var componentXml = string.Format(
                @"<Component Id='{0}Component' Guid='{1}'>
                  {2}
                </Component>",
                key.Replace("\\", "_"),
                componentGuid,
                registryXml);

            _components.Add(new ComponentData { Guid = componentGuid, XmlContent = componentXml });
        }

        public void AddFile(string sourcePath, string destinationName = null, string componentGuid = null)
        {
            if (string.IsNullOrEmpty(destinationName))
            {
                destinationName = Path.GetFileName(sourcePath);
            }
            if (string.IsNullOrEmpty(componentGuid))
            {
                componentGuid = GenerateGuid();
            }

            var fileXml = string.Format(
                @"<File Id='{0}' Source='{1}' KeyPath='yes' />",
                destinationName,
                sourcePath);

            var componentXml = string.Format(
                @"<Component Id='{0}Component' Guid='{1}'>
                  {2}
                </Component>",
                destinationName,
                componentGuid,
                fileXml);

            _components.Add(new ComponentData { Guid = componentGuid, XmlContent = componentXml });
        }

        public void AddCustomAction(string id, string exePath, string arguments)
        {
            _customActions.Add(string.Format(
                @"<CustomAction Id='{0}' FileKey='{1}' ExeCommand='{2}' Return='check' />",
                id,
                Path.GetFileName(exePath),
                arguments));
        }

        private static string GenerateGuid()
        {
            return "{" + Guid.NewGuid().ToString().ToUpper() + "}";
        }

        public string GenerateWixXml()
        {
            var builder = new StringBuilder();

            builder.AppendLine(@"<?xml version='1.0' encoding='UTF-8'?>");
            builder.AppendLine(@"<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>");

            builder.AppendLine(string.Format(
                @"<Product Id='{0}' Name='{1}' Language='1033' Version='{2}' Manufacturer='{3}' UpgradeCode='{4}'>
                    <Package InstallerVersion='500' Compressed='yes' InstallScope='perMachine' />
                    <Media Id='1' Cabinet='product.cab' EmbedCab='yes' />
                    <Directory Id='TARGETDIR' Name='SourceDir'>
                      <Directory Id='ProgramFilesFolder'>
                        <Directory Id='{5}' Name='{6}'>",
                ProductId,
                ProductName,
                Version,
                Manufacturer,
                UpgradeCode,
                InstallDirectory,
                ProductName));

            foreach (var component in _components)
            {
                builder.AppendLine(component.XmlContent);
            }

            builder.AppendLine(@"          </Directory>
                      </Directory>
                    </Directory>");

            builder.AppendLine(@"<Feature Id='DefaultFeature' Level='1'>");
            foreach (var component in _components)
            {
                builder.AppendLine(string.Format("  <ComponentRef Id='{0}' />", component.Guid));
            }
            builder.AppendLine(@"</Feature>");

            if (_customActions.Count > 0)
            {
                builder.AppendLine("<CustomActionGroup>");
                builder.Append(string.Join(Environment.NewLine, _customActions));
                builder.AppendLine("</CustomActionGroup>");
            }

            builder.AppendLine("</Product>");
            builder.AppendLine("</Wix>");

            return builder.ToString();
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, GenerateWixXml(), Encoding.UTF8);
        }

        private class ComponentData
        {
            public string Guid { get; set; }
            public string XmlContent { get; set; }
        }
    }
}*/
