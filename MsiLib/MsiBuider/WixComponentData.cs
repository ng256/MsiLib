using System;
using System.Collections.Generic;

namespace MsiInstaller
{
    /// <summary>
    /// Represents a component in the installer, which includes a GUID, files, and registry values associated with the component.
    /// </summary>
    public class WixComponentData
    {
        /// <summary>
        /// Gets or sets the unique identifier for this component.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets the list of files associated with this component.
        /// </summary>
        public List<WixFileData> Files { get; set; } = new List<WixFileData>();

        /// <summary>
        /// Gets the list of registry values associated with this component.
        /// </summary>
        public List<WixRegistryValue> RegistryValues { get; set; } = new List<WixRegistryValue>();
    }
}