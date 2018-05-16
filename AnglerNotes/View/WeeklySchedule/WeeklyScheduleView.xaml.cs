using AnglerModel;
using AnglerNotes.Utility;
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

        /// <summary>
        /// When the little + button is clicked, unveil hidden content
        /// </summary>
        private void NewActivity_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NewActivityLine.Visibility == System.Windows.Visibility.Visible)
                NewActivityLine.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                NewActivityLine.Visibility = System.Windows.Visibility.Visible;
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
                WeeklyScheduleViewModel.WeeklyActivityWrapper wrapper = ((WeeklyScheduleViewModel.WeeklyActivityWrapper)button.DataContext);

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
                    .SameTimeDifferentDay(WeeklyScheduleViewModel.DaysOfWeek[NewActivityDayOfWeek.SelectedIndex])
                    .ConvertToUtc(WeeklyScheduleViewModel.TimeZones[NewActivityTimeZone.SelectedIndex])
                    .IgnoreDST(WeeklyScheduleViewModel.TimeZones[NewActivityTimeZone.SelectedIndex]);

                weeklyScheduleViewModel.AddActivity(NewActivityName.Text, dateTime, NewActivityTimeZone.SelectedIndex);

                NewActivityName.Text = "";
                NewActivityLine.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Timezone combobox in the add section is assigned the value of the big timezone combobox
        /// </summary>
        private void TimeZoneCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewActivityTimeZone.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }
    }
}
