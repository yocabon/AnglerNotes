using System;
using System.Collections.Concurrent;

namespace AnglerNotes.Utility
{
    /// <summary>
    /// Class used to keep track of open windows
    /// </summary>
    public static class WindowManager
    {
        private static ConcurrentDictionary<IntPtr, string> windows;

        /// <summary>
        /// Active main windows
        /// </summary>
        public static int Count
        {
            get
            {
                return windows == null ? 0 : windows.Count;
            }
        }

        /// <summary>
        /// Register a MainWindow : will be used to save window placement when closing
        /// </summary>
        /// <param name="windowHandle">the window handle</param>
        /// <param name="windowName">the window name, not super important</param>
        public static void AddWindow(IntPtr windowHandle, string windowName)
        {
            if (windows == null)
                windows = new ConcurrentDictionary<IntPtr, string>();

            windows.TryAdd(windowHandle, windowName);
        }

        /// <summary>
        /// Tell WindowManager that a MainWindow was closed, and placement either has been saved or is no longer needed (no tabs on window)
        /// </summary>
        /// <param name="windowHandle">the window handle</param>
        public static void RemoveWindow(IntPtr windowHandle)
        {
            windows.TryRemove(windowHandle, out string name);
        }
    }
}
