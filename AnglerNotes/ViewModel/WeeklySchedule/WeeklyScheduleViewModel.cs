using AnglerModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnglerModel.Utility;
using System.IO;
using AnglerNotes.ViewModel.Settings;

namespace AnglerNotes.ViewModel.WeeklySchedule
{
    public class WeeklyScheduleViewModel : ViewModelBase
    {
        private int Index;

        // private Dictionary<DayOfWeek, List<WeeklyActivityWrapper>> weeklyActivites;

        private ObservableCollection<CellWrapper> weeklyActivites;

        /// <summary>
        /// Lists of Lists are not updated as expected with the binding, so every operation rebuild the list entirely
        /// </summary>
        public ObservableCollection<CellWrapper> WeeklyActivites
        {
            get
            {
                return weeklyActivites;
            }
            set
            {
                weeklyActivites = value;
                OnPropertyChanged("WeeklyActivites");
            }
        }

        /// <summary>
        /// Formatted time zone list
        /// </summary>
        public string[] InstanceTimeZones
        {
            get
            {
                string[] formattedTimeZones = new string[DateTimeExtension.TimeZones.Count];
                for (int i = 0; i < formattedTimeZones.Length; i++)
                {
                    formattedTimeZones[i] = DateTimeExtension.FormatTimezone(i);
                }
                return formattedTimeZones;
            }
        }

        public DayOfWeek[] InstanceDaysOfWeek { get { return DateTimeExtension.DaysOfWeek; } }

        /// <summary>
        /// Selected time zone (combo box at the top) directly from / to db
        /// </summary>
        public int TimeZone
        {
            get
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    int index = Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].TimeZone;
                    ModelAccessLock.Instance.ReleaseAccess();
                    return index;
                }
                return 0;
            }
            set
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].TimeZone = value;
                    SaveTimer.Instance.RequestSave();

                    ModelAccessLock.Instance.ReleaseAccess();
                }

                Build(value);

                OnPropertyChanged("TimeZone");
                OnPropertyChanged("OffsetDST");

            }
        }

        public string Filename
        {
            get
            {
                string content = "Timeout Error";
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    content = Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].SyncFilename;
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return content;
            }
        }

        public WeeklyScheduleViewModel(int index)
        {
            this.Index = index;
            Build(TimeZone);
        }

        public bool TrySync(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return false;

            if (Filename == newName)
                return true;

            string newFullPath = Path.Combine(SettingsViewModel.GetFolderPath(), newName + ".txt");
            if (File.Exists(newFullPath))
                return false;

            bool success = false;
            if (ModelAccessLock.Instance.RequestAccess())
            {
                try
                {
                    string fullpath = Path.Combine(SettingsViewModel.GetFolderPath(), Filename + ".txt");
                    if (!string.IsNullOrWhiteSpace(Filename) && File.Exists(fullpath))
                        File.Move(fullpath, newFullPath);
                    Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].SyncFilename = newName;
                    SaveTimer.Instance.RequestSave();
                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                }
                ModelAccessLock.Instance.ReleaseAccess();
            }
            return success;
        }

        public bool TryUnsync()
        {
            if (string.IsNullOrWhiteSpace(Filename))
                return true;

            bool success = false;
            if (ModelAccessLock.Instance.RequestAccess())
            {
                try
                {
                    string fullpath = Path.Combine(SettingsViewModel.GetFolderPath(), Filename + ".txt");
                    if (File.Exists(fullpath))
                        File.Delete(fullpath);
                    Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].SyncFilename = "";
                    SaveTimer.Instance.RequestSave();
                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                }
                ModelAccessLock.Instance.ReleaseAccess();
            }
            return success;
        }

        /// <summary>
        /// Rebuild list after adding an activity to the db
        /// </summary>
        public void AddActivity(string Name, DateTime dateTime, int selectedTimeZoneIndex)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].WeeklyActivity.Add(new WeeklyActivity(Name, dateTime, selectedTimeZoneIndex));
                SaveTimer.Instance.RequestSave();

                Build(TimeZone);
                OnPropertyChanged("WeeklyActivites");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Rebuild list after removing an activity from the db
        /// </summary>
        public void Remove(WeeklyActivityWrapper weeklyActivityWrapper)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                List<WeeklyActivity> list = Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].WeeklyActivity;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Name == weeklyActivityWrapper.Name && list[i].Time == weeklyActivityWrapper.UniversalTime)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
                Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].WeeklyActivity = list;
                SaveTimer.Instance.RequestSave();

                Build(TimeZone);
                OnPropertyChanged("WeeklyActivites");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Rebuild list
        /// </summary>
        public void Build(int selectedTimeZoneIndex)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                if (WeeklyActivites == null)
                    WeeklyActivites = new ObservableCollection<CellWrapper>();
                else
                    WeeklyActivites.Clear();

                List<WeeklyActivity> list = Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].WeeklyActivity;
                foreach (WeeklyActivity activity in list)
                {
                    WeeklyActivityWrapper NewActivityWrapper = new WeeklyActivityWrapper(activity.Name, activity.Time, DateTimeExtension.TimeZones[activity.AttachedTimeZone], selectedTimeZoneIndex);

                    if (!WeeklyActivites.Any(w => w.DayOfWeek == NewActivityWrapper.DayOfWeek))
                    {
                        CellWrapper cellWrapper = new CellWrapper(NewActivityWrapper.DayOfWeek);
                        cellWrapper.Activities.Add(NewActivityWrapper);
                        WeeklyActivites.Add(cellWrapper);
                    }
                    else
                        WeeklyActivites.ToList().Find(w => w.DayOfWeek == NewActivityWrapper.DayOfWeek).Activities.Add(NewActivityWrapper);
                }

                // Sort activities within a day
                foreach (CellWrapper cellWrapper in WeeklyActivites)
                    cellWrapper.Activities.Sort((a, b) => a.ConvertedTime.TimeOfDay.CompareTo(b.ConvertedTime.TimeOfDay));

                WeeklyActivites = new ObservableCollection<CellWrapper>(WeeklyActivites.OrderBy(w => w.DayOfWeek));
                OnPropertyChanged("WeeklyActivites");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

    }
}
