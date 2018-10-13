using AnglerModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using AnglerNotes.Utility;

namespace AnglerNotes.ViewModel.WeeklySchedule
{
    public class WeeklyScheduleViewModel : ViewModelBase
    {
        private int Index;

        public static readonly ReadOnlyCollection<TimeZoneInfo> TimeZones = TimeZoneInfo.GetSystemTimeZones();
        public static readonly DayOfWeek[] DaysOfWeek = typeof(DayOfWeek).GetEnumValues().Cast<DayOfWeek>().ToArray();

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
                string[] formattedTimeZones = new string[TimeZones.Count];
                for (int i = 0; i < formattedTimeZones.Length; i++)
                {
                    var dt = DateTime.UtcNow;
                    TimeSpan utcOffsetSpan = TimeZones[i].GetUtcOffset(new DateTimeOffset(dt, TimeSpan.Zero));
                    string utcOffset = ((utcOffsetSpan < TimeSpan.Zero) ? "-" : "+") + utcOffsetSpan.ToString(@"hh\:mm");

                    string isDST = (TimeZones[i].BaseUtcOffset != utcOffsetSpan) ? " DST ON" : "";

                    string TimeZoneId = TimeZones[i].Id;
                    Regex regex = new Regex(@"^\(UTC(?<offset>[\+\-]\d\d:\d\d)?\)(?<cities>(\s*[^,]+[\s,]*)*)$");
                    Match match = regex.Match(TimeZones[i].DisplayName);
                    GroupCollection groups = match.Groups;
                    string offset = groups["offset"].Value;
                    string cities = groups["cities"].Value;

                    formattedTimeZones[i] = "(UTC" + utcOffset + isDST + ")" + " " + (String.IsNullOrEmpty(TimeZoneId) ? "" : (TimeZoneId + " :")) + cities;
                }
                return formattedTimeZones;
            }
        }

        public DayOfWeek[] InstanceDaysOfWeek { get { return DaysOfWeek; } }

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
                    Properties.Settings.Default.Save();

                    ModelAccessLock.Instance.ReleaseAccess();
                }

                Build(value);

                OnPropertyChanged("TimeZone");
                OnPropertyChanged("OffsetDST");

            }
        }

        public WeeklyScheduleViewModel(int index)
        {
            this.Index = index;
            Build(TimeZone);
        }

        /// <summary>
        /// Rebuild list after adding an activity to the db
        /// </summary>
        public void AddActivity(string Name, DateTime dateTime, int selectedTimeZoneIndex)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Properties.Settings.Default.Data.WeeklyScheduleTabs[Index].WeeklyActivity.Add(new WeeklyActivity(Name, dateTime, selectedTimeZoneIndex));
                Properties.Settings.Default.Save();

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
                Properties.Settings.Default.Save();

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
                    WeeklyActivityWrapper NewActivityWrapper = new WeeklyActivityWrapper(activity.Name, activity.Time, TimeZones[activity.AttachedTimeZone], selectedTimeZoneIndex);

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
                    cellWrapper.Activities.Sort((a, b) => a.ConvertedTime.CompareTo(b.ConvertedTime));

                WeeklyActivites = new ObservableCollection<CellWrapper>(WeeklyActivites.OrderBy(w => w.DayOfWeek));
                OnPropertyChanged("WeeklyActivites");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Wrapper that contains all activities that occur for a day of week
        /// </summary>
        public class CellWrapper
        {
            public CellWrapper(DayOfWeek dayOfWeek)
            {
                DayOfWeek = dayOfWeek;
                Activities = new List<WeeklyActivityWrapper>();
            }

            public DayOfWeek DayOfWeek { get; set; }
            public List<WeeklyActivityWrapper> Activities { get; set; }

        }

        /// <summary>
        /// Wrapper for binding activities
        /// </summary>
        public class WeeklyActivityWrapper
        {
            public string Name { get; set; }
            public string OriginalTimeZone { get; set; }
            public DateTime UniversalTime { get; set; }
            public DateTime ConvertedTime { get; set; }
            public DayOfWeek DayOfWeek { get; set; }
            public string Time { get; set; }

            public WeeklyActivityWrapper(string Name, DateTime universalTime, TimeZoneInfo attachedTimeZone, int selectedTimeZoneIndex)
            {
                this.Name = Name;
                this.OriginalTimeZone = attachedTimeZone.Id;

                TimeZoneInfo timeZoneInfo = TimeZones[selectedTimeZoneIndex];
                this.UniversalTime = universalTime;

                DateTime ProjectedTime = UniversalTime.SameTimeNextWeek();
                this.ConvertedTime = TimeZoneInfo.ConvertTime(ProjectedTime.CorrectTimeForDST(attachedTimeZone), timeZoneInfo);

                this.DayOfWeek = ConvertedTime.DayOfWeek;
                this.Time = ConvertedTime.ToString("HH:mm");
            }
        }
    }
}
