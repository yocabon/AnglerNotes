using AnglerNotes.Utility;
using AnglerNotes.View.Settings;
using AnglerNotes.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Interop;

namespace AnglerNotes.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel mainWindowViewModel;

        public MainWindow()
        {
            // Load size / position of window first
            mainWindowViewModel = new MainWindowViewModel();

            InitializeComponent();
            this.Deactivated += OnDeactivated;
            this.Activated += OnActivated;
        }

        /// <summary>
        /// Hide title bar when window is not focused (similar behaviour to sticky notes)
        /// </summary>
        private void OnDeactivated(object sender, EventArgs e)
        {
            try
            {
                this.UseNoneWindowStyle = true;
                this.ShowTitleBar = false;
                TitleBarFiller.Height = TitlebarHeight;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Show title bar when window is focused (similar behaviour to sticky notes, just a bit uglier because of tabs)
        /// </summary>
        private void OnActivated(object sender, EventArgs e)
        {
            try
            {
                this.UseNoneWindowStyle = false;
                this.ShowTitleBar = true;
                TitleBarFiller.Height = 0;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// When window is loaded, position it to its previous location, add the same tabs that it had
        /// </summary>
        private void MetroWindow_SourceInitialized(object sender, EventArgs e)
        {
            //base.OnSourceInitialized(e); // provokes a ContextSwitchDeadlock despite being in the original post at https://blogs.msdn.microsoft.com/davidrickard/2010/03/08/saving-window-size-and-location-in-wpf-and-winforms/
            int WindowIndex = MainWindowViewModel.GetNewWindowIndex();
            mainWindowViewModel.LoadPlacement(this, WindowIndex);

            WindowManager.AddWindow(new WindowInteropHelper(this).Handle, this.Name);

            mainWindowViewModel.SpawnChildWindows(this, WindowIndex, NoteTabView.InterTabClient, NoteTabView.NoteTabs);

            NoteTabView.SetTabs(WindowIndex);
        }

        /// <summary>
        /// When window is closed, if it is closed because it is empty, do nothing, else close all windows and save placement
        /// </summary>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowManager.RemoveWindow(new WindowInteropHelper(this).Handle);

            if (NoteTabView.NoteTabs.Items.Count != 0)
            {
                // arbitrary indexing using active windows
                int windowIndex = WindowManager.Count;
                NoteTabView.ReorderTabs(windowIndex);
                mainWindowViewModel.RegisterPlacement(this, windowIndex);
                System.Windows.Application.Current.Shutdown();

                // Window was the last active window, push placement and ordering to the db
                if (WindowManager.Count == 0)
                {
                    mainWindowViewModel.SavePlacement();
                    NoteTabView.SendOrderedTabs();
                }
            }
        }

        /// <summary>
        /// Creates a new QuickNote tab with default parameters
        /// </summary>
        private void QuickNote_Click(object sender, RoutedEventArgs e)
        {
            NoteTabView.QuickNote_Click(sender, e);
        }

        /// <summary>
        /// Creates a new WeeklySchedule tab with default parameters
        /// </summary>
        private void WeeklySchedule_Click(object sender, RoutedEventArgs e)
        {
            NoteTabView.WeeklySchedule_Click(sender, e);
        }

        /// <summary>
        /// Creates a new CompareTime tab with default parameters
        /// </summary>
        private void CompareTime_Click(object sender, RoutedEventArgs e)
        {
            NoteTabView.CompareTime_Click(sender, e);
        }

        /// <summary>
        /// Creates a new ItemCounter tab with default parameters
        /// </summary>
        private void ItemCounter_Click(object sender, RoutedEventArgs e)
        {
            NoteTabView.ItemCounter_Click(sender, e);
        }

        /// <summary>
        /// Opens the settings window
        /// </summary>
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.GetOrCreate();
        }

    }
}