using System;

namespace AnglerModel
{
    /// <summary>
    /// Simple tab similar to the microsoft sticky notes app
    /// </summary>
    [Serializable]
    public class NoteTextBlock
    {
        /// <summary>
        /// Content of the text tab
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Creates a new instance of NoteTextBlock with Content=""
        /// </summary>
        public NoteTextBlock()
        {
            this.Content = "";
        }
    }
}
