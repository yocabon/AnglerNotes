using System;

/// <summary>
/// From https://www.codeproject.com/Articles/1186738/A-Robust-and-Elegant-Single-Instance-WPF-Applicati
/// </summary>
namespace SingleInstanceManager
{
    /// <summary>
    /// Event declaration for the startup of a second instance
    /// </summary>
    public class SecondInstanceStartedEventArgs : EventArgs
    {
        /// <summary>
        /// The event method declaration for the startup of a second instance
        /// </summary>
        /// <param name="args">The other instance's command-line arguments.</param>
        public SecondInstanceStartedEventArgs(string[] args)
        { Args = args; }

        /// <summary>
        /// Property containing the second instance's command-line arguments
        /// </summary>
        public string[] Args { get; set; }
    }
}