namespace AnglerModel
{
    /// <summary>
    /// Contains all the information necessary to identify a tab, and to show it on screen
    /// </summary>
    public class NoteTab
    {
        /// <summary>
        /// Name of the tab (not unique)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the tab (tell in which array of <see cref="Root"/> you will find this tab
        /// </summary>
        public NoteTabType NoteTabType { get; set; }

        /// <summary>
        /// Index of this tab in the array of <see cref="Root"/> that corresponds to <see cref="NoteTabType"/>
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Index of the window on which this tab is located
        /// </summary>
        public int WindowNumber { get; set; }

        /// <summary>
        /// Creates a new instance of NoteTab with Name="", NoteTabType=0, Index=0, WindowNumber=0
        /// </summary>
        public NoteTab()
        {
            this.Name = "";
            this.NoteTabType = (NoteTabType)0;
            this.Index = 0;
            this.WindowNumber = 0;
        }

        /// <summary>
        /// Creates a new instance of NoteTab with Name = noteTabType.ToString(), WindowNumber=0 and given parameters
        /// </summary>
        public NoteTab(NoteTabType noteTabType, int index)
        {
            this.Name = noteTabType.ToString();
            this.NoteTabType = noteTabType;
            this.Index = index;
            this.WindowNumber = 0;
        }

        /// <summary>
        /// Creates a new instance of NoteTab with WindowNumber=0 and given parameters
        /// </summary>
        public NoteTab(string name, NoteTabType noteTabType, int index)
        {
            Name = name;
            NoteTabType = noteTabType;
            Index = index;
            this.WindowNumber = 0;
        }

        /// <summary>
        /// Creates a new instance of NoteTab with given parameters
        /// </summary>
        public NoteTab(string name, NoteTabType noteTabType, int index, int windowNumber)
        {
            Name = name;
            NoteTabType = noteTabType;
            Index = index;
            WindowNumber = windowNumber;
        }
    }
}
