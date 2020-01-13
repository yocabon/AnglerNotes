﻿using System.Windows;

namespace AnglerNotes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string AppID = "4c0ffa3e-0280-42ee-94ec-a7095424add4";

        public static bool IsShutingDown = false;

        private void OnSettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            ViewModel.SyncManager.Instance.LoadSyncedTabsFromFiles();
        }

        public App()
        {
            //check if we are the first instance
            if (SingleInstanceManager.SingleInstance.IsFirstInstance(AppID, true))
            {
                //we are, register our event handler for receiving the new arguments
                SingleInstanceManager.SingleInstance.OnSecondInstanceStarted += NewStartupArgs;

                //place additional startup code here
                AnglerNotes.Properties.Settings.Default.SettingsLoaded += OnSettingsLoaded;
                AnglerNotes.Properties.Settings.Default.Reload();
            }
            //we are secondary instance and shutdown will happen automatically
        }

        /// <summary>
        /// Handle new startup arguments and/or do anything else for second instance launch
        /// </summary>
        private void NewStartupArgs(object sender, SingleInstanceManager.SecondInstanceStartedEventArgs e)
        {
        }

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            if (!App.IsShutingDown)
            {
                System.Windows.Application.Current.Shutdown();
                App.IsShutingDown = true;
            }
        }
    }
}
