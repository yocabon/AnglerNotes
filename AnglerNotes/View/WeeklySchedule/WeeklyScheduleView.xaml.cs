using AnglerModel;
using AnglerModel.Utility;
using AnglerNotes.View.Tabs;
using AnglerNotes.ViewModel.WeeklySchedule;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnglerNotes.View.WeeklySchedule
{
    /// <summary>
    /// Interaction logic for WeeklyScheduleView.xaml
    /// </summary>
    public partial class WeeklyScheduleView : UserControl, ITabView
    {
        /// <summary>
        /// Replicate <see cref="NoteTab.Index"/> inside a view element
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Replicate <see cref="NoteTab.NoteTabType"/> inside a view element
        /// </summary>
        public NoteTabType NoteTabType
        {
            get
            {
                return NoteTabType.WeeklySchedule;
            }
        }

        private WeeklyScheduleViewModel weeklyScheduleViewModel;

        public WeeklyScheduleView(int index)
        {
            InitializeComponent();

            this.Index = index;
            weeklyScheduleViewModel = new WeeklyScheduleViewModel(index);
            this.DataContext = weeklyScheduleViewModel;
            NewActivityTimeZone.SelectedIndex = weeklyScheduleViewModel.TimeZone;
        }


        private void SetDefaultVisibility()
        {
            NewActivity.Visibility = System.Windows.Visibility.Visible;
            SyncItemShow.Visibility = System.Windows.Visibility.Visible;
            NewActivityLine.Visibility = System.Windows.Visibility.Collapsed;
            SyncItemLine.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetNewActivityVisibility()
        {
            NewActivity.Visibility = System.Windows.Visibility.Visible;
            SyncItemShow.Visibility = System.Windows.Visibility.Collapsed;
            NewActivityLine.Visibility = System.Windows.Visibility.Visible;
            SyncItemLine.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetSyncSettingsVisibility()
        {
            NewActivity.Visibility = System.Windows.Visibility.Collapsed;
            SyncItemShow.Visibility = System.Windows.Visibility.Visible;
            NewActivityLine.Visibility = System.Windows.Visibility.Collapsed;
            // reset field to db value
            SyncItemName.Text = weeklyScheduleViewModel.Filename;
            SyncItemLine.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// When the little + button is clicked, unveil hidden content
        /// </summary>
        private void NewActivity_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NewActivityLine.Visibility == System.Windows.Visibility.Visible)
                SetDefaultVisibility();
            else
            {
                SetNewActivityVisibility();
                NewActivityName.Focus();
            }
        }

        /// <summary>
        /// When the ok button is clicked
        /// </summary>
        private void NewActivityValidate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddNewActivity();
        }

        /// <summary>
        /// When the trash bin is clicked
        /// </summary>
        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Button button = ((Button)sender);
                WeeklyActivityWrapper wrapper = ((WeeklyActivityWrapper)button.DataContext);

                weeklyScheduleViewModel.Remove(wrapper);
            }
        }

        /// <summary>
        /// When enter is pressed from the add activity text box
        /// </summary>
        private void NewActivityName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AddNewActivity();
        }

        /// <summary>
        /// Can have duplicate activities, even if it is useless
        /// </summary>
        private void AddNewActivity()
        {
            if (!String.IsNullOrEmpty(NewActivityName.Text) && NewActivityDate.SelectedTime.HasValue)
            {
                DateTime dateTime = NewActivityDate.SelectedTime.Value.ToDateTime()
                    .SameTimeDifferentDay(DateTimeExtension.DaysOfWeek[NewActivityDayOfWeek.SelectedIndex])
                    .ConvertToUtc(DateTimeExtension.TimeZones[NewActivityTimeZone.SelectedIndex])
                    .IgnoreDST(DateTimeExtension.TimeZones[NewActivityTimeZone.SelectedIndex]);

                weeklyScheduleViewModel.AddActivity(NewActivityName.Text, dateTime, NewActivityTimeZone.SelectedIndex);

                NewActivityName.Text = "";
                SetDefaultVisibility();
            }
        }

        /// <summary>
        /// Timezone combobox in the add section is assigned the value of the big timezone combobox
        /// </summary>
        private void TimeZoneCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewActivityTimeZone.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        private void SyncItemShow_Click(object sender, RoutedEventArgs e)
        {
            if (SyncItemLine.Visibility == System.Windows.Visibility.Visible)
                SetDefaultVisibility();
            else
            {
                SetSyncSettingsVisibility();
                SyncItemName.Focus();
            }
        }
        private void SyncItemName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                TrySettingUpSync();
        }

        private void SyncItemValidate_Click(object sender, RoutedEventArgs e)
        {
            TrySettingUpSync();
        }


        private void TrySettingUpSync()
        {
            if (!string.IsNullOrWhiteSpace(SyncItemName.Text) && weeklyScheduleViewModel.Filename != SyncItemName.Text)
            {
                string messageBoxText = "Do you want to sync tab with " + SyncItemName.Text + "?";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        bool success = weeklyScheduleViewModel.TrySync(SyncItemName.Text);
                        if (success)
                        {
                            this.Focus();
                            SetDefaultVisibility();
                        }
                        break;
                    case MessageBoxResult.No:
                        // User pressed No button, cancel operation
                        this.Focus();
                        SetDefaultVisibility();
                        break;
                }
            }
            else if (string.IsNullOrWhiteSpace(SyncItemName.Text) && weeklyScheduleViewModel.Filename != "")
            {
                // Unsync
                string messageBoxText = "Do you want to unsync tab?";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        bool success = weeklyScheduleViewModel.TryUnsync();
                        if (success)
                        {
                            this.Focus();
                            SetDefaultVisibility();
                        }
                        break;
                    case MessageBoxResult.No:
                        // User pressed No button, cancel operation
                        this.Focus();
                        SetDefaultVisibility();
                        break;
                }
            }
        }
    }
}
