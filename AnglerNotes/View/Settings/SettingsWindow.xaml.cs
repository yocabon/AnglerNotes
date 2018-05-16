using AnglerNotes.ViewModel.Settings;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;

namespace AnglerNotes.View.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        /// <summary>
        /// Ensures there is never more than one instance of Setting window
        /// </summary>
        private static readonly Object windowLock = new Object();

        /// <summary>
        /// Singleton SettingsWindow
        /// </summary>
        private static SettingsWindow Instance;

        /// <summary>
        /// Returns the instance of the singleton SettingsWindow
        /// </summary>
        public static SettingsWindow GetOrCreate()
        {
            lock (windowLock)
            {
                if (Instance == null)
                    Instance = new SettingsWindow();

                Instance.Show();
                Instance.Focus();
                return Instance;
            }
        }

        private SettingsViewModel settingsViewModel;

        public SettingsWindow()
        {
            InitializeComponent();

            settingsViewModel = new SettingsViewModel();
            this.DataContext = settingsViewModel;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (windowLock)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Goto the author's github page
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
