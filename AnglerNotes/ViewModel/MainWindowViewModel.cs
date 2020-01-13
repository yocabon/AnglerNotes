using AnglerModel;
using AnglerNotes.Utility;
using Dragablz;
using System.Collections.Specialized;
using System.Windows;

namespace AnglerNotes.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets a new index for a window
        /// </summary>
        public static int GetNewWindowIndex()
        {
            int index = int.MaxValue;
            if (ModelAccessLock.Instance.RequestAccess())
            {
                index = ModelAccessLock.Instance.WindowIndex;
                ModelAccessLock.Instance.ReleaseAccess();
            }
            return index;
        }

        private void OnSettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            SyncManager.Instance.LoadSyncedTabsFromFiles();
        }

        public MainWindowViewModel()
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                if (WindowManager.Count == 0)
                {
                    Properties.Settings.Default.SettingsLoaded += OnSettingsLoaded;
                    Properties.Settings.Default.Reload();
                }

                Root property = Properties.Settings.Default.Data;
                if (property == null)
                {
                    property = new Root();
                    Properties.Settings.Default.Data = property;
                }

                StringCollection windowSizes = Properties.Settings.Default.WindowPlacement;
                if (windowSizes == null)
                {
                    Properties.Settings.Default.WindowPlacement = new StringCollection();
                }

                Properties.Settings.Default.Save();

                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Load placement of a particular window
        /// </summary>
        public void LoadPlacement(Window source, int windowIndex)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                StringCollection windowSizes = Properties.Settings.Default.WindowPlacement;

                if (windowSizes.Count > 0)
                {
                    source.SetPlacement(windowSizes[windowIndex >= windowSizes.Count ? 0 : windowIndex]);
                }
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// The first window must spawn all other windows in order to get the same layout as the previous execution
        /// </summary>
        public void SpawnChildWindows(Window source, int windowIndex, IInterTabClient InterTabClient, TabablzControl tabablzControl)
        {
            if (windowIndex == 0)
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    StringCollection windowSizes = Properties.Settings.Default.WindowPlacement;

                    for (int i = 0; i < windowSizes.Count - 1; i++)
                    {
                        var v = InterTabClient?.GetNewHost(InterTabClient, null, tabablzControl);
                        v.Container.Show();
                    }

                    ModelAccessLock.Instance.ReleaseAccess();
                }
            }
        }

        private static StringCollection WindowPlacement = new StringCollection();

        /// <summary>
        /// Note, when registering position, the windowIndex is not necessarily equal to the one it had when loading placements. 
        /// Rather, there shouldn't be any hole in the number of windows that are closed
        /// </summary>
        public void RegisterPlacement(Window source, int windowIndex)
        {
            // use ModelAccessLock cause we don't want register and save to happen at the same time
            if (ModelAccessLock.Instance.RequestAccess())
            {
                while (WindowPlacement.Count < windowIndex + 1)
                {
                    WindowPlacement.Add("");
                }

                WindowPlacement[windowIndex] = source.GetPlacement();
                Properties.Settings.Default.Save();

                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Save all placements to the db
        /// </summary>
        public void SavePlacement()
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Properties.Settings.Default.WindowPlacement = WindowPlacement;
                Properties.Settings.Default.Save();

                ModelAccessLock.Instance.ReleaseAccess();
            }
        }
    }
}
