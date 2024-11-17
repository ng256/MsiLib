using System.Xml.Serialization;

namespace MsiInstaller
{
    /// <summary>
    /// Enum to represent KeyPath values for components in the Wix file format.
    /// </summary>
    public enum WixKeyPath
    {
        /// <summary>
        /// Represents "yes" value in XML. The component is considered a key path.
        /// </summary>
        [XmlEnum("yes")]
        Yes,

        /// <summary>
        /// Represents "no" value in XML. The component is not considered a key path.
        /// </summary>
        [XmlEnum("no")]
        No
    }
}