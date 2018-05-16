using AnglerModel;

namespace AnglerNotes.View.Tabs
{
    /// <summary>
    /// All view that represents a <see cref="NoteTab"/> show implements this interface
    /// </summary>
    public interface ITabView
    {
        /// <summary>
        /// Replicate <see cref="NoteTab.Index"/> inside a view element
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Replicate <see cref="NoteTab.NoteTabType"/> inside a view element
        /// </summary>
        NoteTabType NoteTabType { get; }
    }
}
