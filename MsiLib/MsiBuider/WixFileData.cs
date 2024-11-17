namespace MsiInstaller
{
    /// <summary>
    /// Represents the data for a file within a component in the installer, including its identifier, source path,
    /// and key path property.
    /// </summary>
    public class WixFileData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the file.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the source path of the file to be installed.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the key path for the file, indicating the file's role as a key file
        /// in the component (e.g., if it's used to determine the installation state).
        /// </summary>
        public WixKeyPath KeyPath { get; set; }
    }
}