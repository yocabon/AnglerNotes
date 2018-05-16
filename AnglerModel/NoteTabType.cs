using System;

namespace AnglerModel
{
    /// <summary>
    /// Type of a tab
    /// </summary>
    [Serializable]
    public enum NoteTabType
    {
        /// <summary>
        /// A module similar to the microsoft sticky notes app
        /// </summary>
        TextBlock,
        /// <summary>
        /// A module that stores weekly events
        /// </summary>
        WeeklySchedule,
        /// <summary>
        /// A module that helps you compare two dates
        /// </summary>
        CompareTime,
        /// <summary>
        /// A module that helps you keep track of counters
        /// </summary>
        ItemCounter,
    };
}
