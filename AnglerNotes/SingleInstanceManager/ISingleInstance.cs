using System.ServiceModel;

/// <summary>
/// From https://www.codeproject.com/Articles/1186738/A-Robust-and-Elegant-Single-Instance-WPF-Applicati
/// </summary>
namespace SingleInstanceManager
{
    /// <summary> The WCF interface for passing the startup parameters </summary>
    [ServiceContract]
    public interface ISingleInstance
    {
        /// <summary>
        /// Notifies the first instance that another instance of the application attempted to start.
        /// </summary>
        /// <param name="args">The other instance's command-line arguments.</param>
        [OperationContract]
        void PassStartupArgs(string[] args);
    }
}