using AnglerModel;
using AnglerNotes.ViewModel;
using AnglerNotes.ViewModel.Settings;
using AnglerNotes.ViewModel.WeeklySchedule;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

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

        private string GetSeparator(int count = 10)
        {
            string sep = "";
            for (int i = 0; i < 10; i++)
            {
                sep += "-";
            }
            sep += "\n";
            return sep;
        }

        private void Clipboard_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Root property = Properties.Settings.Default.Data;
                string text = "AnglerNotes " + DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss") + "\n\n";
                var tabs = property.TabList;
                foreach(var tab in tabs)
                {
                    text += GetSeparator();
                    text += tab.Name + "\n";
                    text += GetSeparator();
                    text += "\n";

                    switch (tab.NoteTabType)
                    {
                        case NoteTabType.TextBlock:
                            text += property.TextBlockTabs[tab.Index].ToString();
                        break;
                        case NoteTabType.ItemCounter:
                            {
                                text += property.ItemCounterTabs[tab.Index].ToString();
                            }
                            break;
                        case NoteTabType.CompareTime:
                            {
                                text += property.CompareTimeTabs[tab.Index].ToString();
                            }
                            break;
                        case NoteTabType.WeeklySchedule:
                            {
                                text += property.WeeklyScheduleTabs[tab.Index].ToString();
                            }
                            break;
                    }
                    text += "\n";
                }
                Clipboard.SetText(text);
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }
    }
}
