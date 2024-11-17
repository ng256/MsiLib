namespace MsiInstaller
{
    /// <summary>
    /// Represents the data for a custom action in the installer, including the action's identifier, executable command,
    /// arguments, and the return type for the action.
    /// </summary>
    public class WixCustomActionData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the custom action.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the executable command that the custom action will run.
        /// </summary>
        public string ExeCommand { get; set; }

        /// <summary>
        /// Gets or sets the arguments that will be passed to the executable command.
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the return type for the custom action (e.g., check or other return behavior).
        /// </summary>
        public string Return { get; set; }
    }
}