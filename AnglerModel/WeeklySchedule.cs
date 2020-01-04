using AnglerModel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnglerModel
{
    /// <summary>
    /// Module that stores weekly events
    /// </summary>
    public class WeeklySchedule
    {
        /// <summary>
        /// Sync to human readble text file. This is the filename. For now readonly.
        /// </summary>
        public string SyncFilename { get; set; }

        /// <summary>
        /// Time zone in which all weekly activities are shown
        /// </summary>
        public int TimeZone { get; set; }

        /// <summary>
        /// List of <see cref="WeeklyActivity"/>
        /// </summary>
        public List<WeeklyActivity> WeeklyActivity { get; set; }

        /// <summary>
        /// Creates a new instance of WeeklySchedule with TimeZone=0 and WeeklyActivity empty
        /// </summary>
        public WeeklySchedule()
        {
            SyncFilename = "";
            TimeZone = 0;
            WeeklyActivity = new List<WeeklyActivity>();
        }

        public override string ToString()
        {
            string text = "TimeZone " + DateTimeExtension.FormatTimezone(TimeZone) + "\n";

            var WeeklyActivites = new List<CellWrapper>();

            foreach (WeeklyActivity activity in WeeklyActivity)
            {
                WeeklyActivityWrapper NewActivityWrapper = new WeeklyActivityWrapper(activity.Name, activity.Time, DateTimeExtension.TimeZones[activity.AttachedTimeZone], TimeZone);

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

            WeeklyActivites = WeeklyActivites.OrderBy(w => w.DayOfWeek).ToList();

            foreach (var cellWrapper in WeeklyActivites)
            {
                text += "\n" + cellWrapper.DayOfWeek + "\n\n";

                foreach (var activity in cellWrapper.Activities)
                    text += activity.Name + " : " + activity.Time + " --- AttachedTimeZone: " + activity.OriginalTimeZone + "\n";
            }
            return text;
        }
    }

    /// <summary>
    /// An activity is a triplet (Name, Time, Timezone)
    /// </summary>
    public class WeeklyActivity
    {
        /// <summary>
        /// Creates a new intance of WeeklyActivity with Name="", Time=now, AttachedTimeZone=0
        /// </summary>
        public WeeklyActivity()
        {
            Name = "";
            Time = DateTime.Now;
            AttachedTimeZone = 0;
        }

        /// <summary>
        /// Creates a new intance of WeeklyActivity with given parameters
        /// </summary>
        public WeeklyActivity(string name, DateTime time, int attachedTimeZone)
        {
            Name = name;
            Time = time;
            AttachedTimeZone = attachedTimeZone;
        }

        /// <summary>
        /// Name of the activity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Time in UniversalTime at which the activity occurs
        /// </summary>
        /// <remarks>
        /// The date part of DateTime is only used to find at which day of the week the activity occurs
        /// </remarks>
        public DateTime Time { get; set; }

        /// <summary>
        /// A weekly activity is attached to a timezone, its time will stay constant in that timezone whether DST is on or off 
        /// </summary>
        public int AttachedTimeZone { get; set; }
    }
}
