using AnglerModel;
using AnglerNotes.View.Tabs;
using AnglerNotes.ViewModel.TextBlock;
using System.Windows.Controls;

namespace AnglerNotes.View.TextBlock
{
    /// <summary>
    /// Interaction logic for TextBlockView.xaml
    /// </summary>
    public partial class TextBlockView : UserControl, ITabView
    {
        /// <summary>
        /// Replicate <see cref="NoteTab.Index"/> inside a view element
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Replicate <see cref="NoteTab.NoteTabType"/> inside a view element
        /// </summary>
        public NoteTabType NoteTabType
        {
            get
            {
                return NoteTabType.TextBlock;
            }
        }

        private TextBlockViewModel textBlockViewModel;

        public TextBlockView(int index)
        {
            InitializeComponent();

            this.Index = index;
            textBlockViewModel = new TextBlockViewModel(index);
            this.DataContext = textBlockViewModel;
        }
    }
}
