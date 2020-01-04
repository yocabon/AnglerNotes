using System;
using System.Collections.Generic;

namespace AnglerModel.Utility
{
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

            TimeZoneInfo timeZoneInfo = DateTimeExtension.TimeZones[selectedTimeZoneIndex];
            this.UniversalTime = universalTime;

            DateTime ProjectedTime = UniversalTime.SameTimeNextWeek();
            this.ConvertedTime = TimeZoneInfo.ConvertTime(ProjectedTime.CorrectTimeForDST(attachedTimeZone), timeZoneInfo);

            this.DayOfWeek = ConvertedTime.DayOfWeek;
            this.Time = ConvertedTime.ToString("HH:mm");
        }
    }
}

