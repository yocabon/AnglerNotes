using System;

namespace AnglerModel
{
    /// <summary>
    /// Compute the time difference between two dates
    /// </summary>
    public class CompareTime
    {
        /// <summary>
        /// Start date in the comparison
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// End date in the comparison
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Creates a new instance of CompareTime with both dates set as <see cref="DateTime.Now"/>
        /// </summary>
        public CompareTime()
        {
            DateTime now = DateTime.Now;

            Start = now.Date;
            End = now.Date;
        }
    }
}
