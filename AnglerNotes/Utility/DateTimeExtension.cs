using System;

namespace AnglerNotes.Utility
{
    /// <summary>
    /// <see cref="DateTime"/> and <see cref="TimeSpan"/> qol extensions
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Get timeInDay, but anchored to the first day after DateTime.Now.Date that is a timeInDay.DayOfWeek
        /// </summary>
        /// <param name="timeInDay">any DateTime</param>
        /// <remarks>
        /// Example :
        /// SameTimeNextWeek (Wednesday 08/02 02:07) called on Monday 23/07 would be Wednesday 25/02 02:07
        /// </remarks>
        public static DateTime SameTimeNextWeek(this DateTime timeInDay)
        {
            DateTime now = DateTime.Now.Date;
            now = now.Add(timeInDay.TimeOfDay);

            int today = (int)now.DayOfWeek;
            int targetDay = (int)timeInDay.DayOfWeek;

            int dayOffset = (targetDay - today);
            if (dayOffset < 0) // always look forward (make more sense for dst edge cases)
                dayOffset += 7;

            DateTime alignedDateTime = DateTime.SpecifyKind(now.AddDays(dayOffset), DateTimeKind.Utc);
            return alignedDateTime;
        }

        /// <summary>
        /// get the time that correspond to timeInDay, but in the next dayOfWeek
        /// </summary>
        /// <param name="timeInDay">any DateTime</param>
        /// <param name="dayOfWeek">dayOfWeek of the result</param>
        /// <remarks>
        /// Example :
        /// SameTimeDifferentDay (Wednesday 08/02 02:07, Monday) would be Monday 13/02 02:07
        /// </remarks>
        public static DateTime SameTimeDifferentDay(this DateTime timeInDay, DayOfWeek dayOfWeek)
        {
            int selectedDay = (int)dayOfWeek;
            int currentDay = (int)timeInDay.DayOfWeek;

            int dayOffset = (selectedDay - currentDay);
            if (dayOffset < 0) // always look forward (make more sense for dst edge cases)
                dayOffset += 7;

            DateTime alignedDateTime = DateTime.SpecifyKind(timeInDay.AddDays(dayOffset), DateTimeKind.Unspecified);

            return alignedDateTime;
        }

        /// <summary>
        /// Convert time attached to timeZoneInfo to utc (note : DST settings are applied)
        /// </summary>
        /// <param name="timeInDay">any DateTime</param>
        /// <param name="timeZoneInfo">TimeZoneInfo attached to timeInDay</param>
        /// <returns></returns>
        public static DateTime ConvertToUtc(this DateTime timeInDay, TimeZoneInfo timeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeToUtc(timeInDay, timeZoneInfo);
        }

        /// <summary>
        /// Remove the offset due to DST from dateTime
        /// </summary>
        /// <param name="dateTime">any datetime</param>
        /// <param name="timeZoneInfo">the timezone attached to dateTime</param>
        public static DateTime IgnoreDST(this DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            TimeSpan utcOffsetSpan = timeZoneInfo.GetUtcOffset(new DateTimeOffset(dateTime, TimeSpan.Zero));
            bool isDST = (timeZoneInfo.BaseUtcOffset != utcOffsetSpan);
            if (isDST)
                return dateTime.Add(timeZoneInfo.BaseUtcOffset - utcOffsetSpan);
            else return dateTime;
        }

        /// <summary>
        /// Add the offset due to DST to dateTime
        /// </summary>
        /// <param name="dateTime">any datetime</param>
        /// <param name="timeZoneInfo">the timezone attached to dateTime</param>
        public static DateTime CorrectTimeForDST(this DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            TimeSpan utcOffsetSpan = timeZoneInfo.GetUtcOffset(new DateTimeOffset(dateTime, TimeSpan.Zero));
            bool isDST = (timeZoneInfo.BaseUtcOffset != utcOffsetSpan);
            if (isDST)
                return dateTime.Add(utcOffsetSpan - timeZoneInfo.BaseUtcOffset);
            else return dateTime;
        }

        /// <summary>
        /// Round a timespan to the nearest fraction of second
        /// </summary>
        /// <param name="span">any timespan</param>
        /// <param name="nDigits">number of digits kept before rounding, 0 => nearest integer, 1 => x.x, 2 => x.xx etc</param>
        public static TimeSpan RoundSeconds(this TimeSpan span, int nDigits)
        {
            double totalSeconds = span.TotalSeconds;
            double roundedSeconds = Math.Round(totalSeconds, nDigits);
            return TimeSpan.FromSeconds(roundedSeconds);
        }

        /// <summary>
        /// Creates a datetime with today's date and timespan time of day
        /// </summary>
        /// <param name="timeSpan">any timespan</param>
        public static DateTime ToDateTime(this TimeSpan timeSpan)
        {
            DateTime now = DateTime.Now.Date;
            return now.Add(timeSpan);
        }
    }
}
