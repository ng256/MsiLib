namespace MsiInstaller
{
    /// <summary>
    /// Enum for QUIET property. Specifies the level of user interface during installation.
    /// </summary>
    public enum MsiQuietMode
    {
        /// <summary>
        /// Default installation mode with standard UI.
        /// </summary>
        Default,

        /// <summary>
        /// Completely silent installation without any UI.
        /// </summary>
        NoUI = 1,

        /// <summary>
        /// Minimal user interface, with very basic prompts.
        /// </summary>
        BasicUI = 2
    }
}