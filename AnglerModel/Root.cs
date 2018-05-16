using System;
using System.Collections.Generic;

namespace AnglerModel
{
    /// <summary>
    /// AnglerNotes database data
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Maximum number of tab per type allowed
        /// </summary>
        public const int MaxTabCount = 50;

        /// <summary>
        /// List of all tabs
        /// </summary>
        public List<NoteTab> TabList { get; set; }

        /// <summary>
        /// Content of NoteTextBlock tabs
        /// </summary>
        public NoteTextBlock[] TextBlockTabs { get; set; }

        /// <summary>
        /// Content of WeeklySchedule tabs
        /// </summary>
        public WeeklySchedule[] WeeklyScheduleTabs { get; set; }

        /// <summary>
        /// Content of CompareTime tabs
        /// </summary>
        public CompareTime[] CompareTimeTabs { get; set; }

        /// <summary>
        /// Content of ItemCounter tabs
        /// </summary>
        public ItemCounter[] ItemCounterTabs { get; set; }

        /// <summary>
        /// Creates a new instance of Root with no tab open
        /// </summary>
        public Root()
        {
            this.TabList = new List<NoteTab>();
            this.TextBlockTabs = new NoteTextBlock[MaxTabCount];
            this.WeeklyScheduleTabs = new WeeklySchedule[MaxTabCount];
            this.CompareTimeTabs = new CompareTime[MaxTabCount];
            this.ItemCounterTabs = new ItemCounter[MaxTabCount];
        }

        /// <summary>
        /// Remove the index'th tab of TabList
        /// </summary>
        public void RemoveTab(int index)
        {
            NoteTab noteTab = TabList[index];
            TabList.RemoveAt(index);
            Array array = GetTypeArray(noteTab.NoteTabType);
            array.SetValue(null, noteTab.Index);
        }

        /// <summary>
        /// Add a new tab of noteTabType with default parameters
        /// </summary>
        public NoteTab AddNewTab(NoteTabType noteTabType)
        {
            int index = GetNextIndex(noteTabType);
            NoteTab newNoteTab = new NoteTab(noteTabType, index);

            switch (noteTabType)
            {
                case NoteTabType.TextBlock:
                    {
                        TextBlockTabs[index] = new NoteTextBlock();
                    }
                    break;
                case NoteTabType.WeeklySchedule:
                    {
                        WeeklyScheduleTabs[index] = new WeeklySchedule();
                    }
                    break;
                case NoteTabType.CompareTime:
                    {
                        CompareTimeTabs[index] = new CompareTime();
                    }
                    break;
                case NoteTabType.ItemCounter:
                    {
                        ItemCounterTabs[index] = new ItemCounter();
                    }
                    break;
                default:
                    throw new System.NotImplementedException("Tab type not implemented");
            }

            TabList.Add(newNoteTab);

            return newNoteTab;
        }

        /// <summary>
        /// Reorder TabList so that tabs appear in the same order when restarting the application
        /// </summary>
        /// <param name="NewTabList"></param>
        public void ReorderTabs(List<NoteTab> NewTabList)
        {
            if (NewTabList.Count != TabList.Count)
                throw new System.ArgumentOutOfRangeException("list must have same size as TabList");

            TabList = NewTabList;
        }

        /// <summary>
        /// Get the array that corresponds to tabs of type noteTabType
        /// </summary>
        private Array GetTypeArray(NoteTabType noteTabType)
        {
            Array array;

            switch (noteTabType)
            {
                case NoteTabType.TextBlock:
                    {
                        array = TextBlockTabs;
                    }
                    break;
                case NoteTabType.WeeklySchedule:
                    {
                        array = WeeklyScheduleTabs;
                    }
                    break;
                case NoteTabType.CompareTime:
                    {
                        array = CompareTimeTabs;
                    }
                    break;
                case NoteTabType.ItemCounter:
                    {
                        array = ItemCounterTabs;
                    }
                    break;
                default:
                    throw new System.NotImplementedException("Tab type not implemented");
            }

            return array;
        }

        /// <summary>
        /// Get an available index in the array that corresponds to tabs of type noteTabType
        /// </summary>
        private int GetNextIndex(NoteTabType noteTabType)
        {
            Array array = GetTypeArray(noteTabType);
            for (int i = 0; i < array.Length; i++)
            {
                if (array.GetValue(i) == null)
                    return i;
            }

            throw new System.Exception("Cannot create more than " + MaxTabCount + " per category");
        }
    }
}
