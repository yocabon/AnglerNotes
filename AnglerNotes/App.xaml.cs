using System;
using System.Collections.Specialized;
using System.Windows;

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
            Console.WriteLine("Loaded!");
            if (AnglerNotes.Properties.Settings.Default.Data != null)
            {
                ViewModel.SyncManager.Instance.LoadSyncedTabsFromFiles();
            }
        }

        public App()
        {
            //check if we are the first instance
            if (SingleInstanceManager.SingleInstance.IsFirstInstance(AppID, true))
            {
                //we are, register our event handler for receiving the new arguments
                SingleInstanceManager.SingleInstance.OnSecondInstanceStarted += NewStartupArgs;
            }
            //we are secondary instance and shutdown will happen automatically
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // https://social.msdn.microsoft.com/Forums/en-US/fea3d1c2-990e-4973-bb1c-03db83f58c61/wpf-app-never-appears-if-showing-a-messagebox-during-app-constructor?forum=wpf
            // load user data
            AnglerNotes.Properties.Settings.Default.SettingsLoaded += OnSettingsLoaded;
            AnglerNotes.Properties.Settings.Default.Reload();

            // handle first load with a bit of safety
            if (AnglerNotes.Properties.Settings.Default.Data == null)
            {
                string messageBoxText = "Data == null, do you want to initialize data?";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        AnglerNotes.Properties.Settings.Default.Data = new AnglerModel.Root();
                        AnglerNotes.Properties.Settings.Default.Save();
                        break;
                    case MessageBoxResult.No:
                        // User pressed No button, cancel operation
                        Environment.Exit(0);
                        break;
                }
            }

            if (AnglerNotes.Properties.Settings.Default.WindowPlacement == null)
            {
                AnglerNotes.Properties.Settings.Default.WindowPlacement = new StringCollection();
                AnglerNotes.Properties.Settings.Default.Save();
            }
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
