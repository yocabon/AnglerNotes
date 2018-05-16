using System;
using System.Collections.Generic;

namespace AnglerModel
{
    /// <summary>
    /// Module that stores weekly events
    /// </summary>
    public class WeeklySchedule
    {
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
            TimeZone = 0;
            WeeklyActivity = new List<WeeklyActivity>();
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
