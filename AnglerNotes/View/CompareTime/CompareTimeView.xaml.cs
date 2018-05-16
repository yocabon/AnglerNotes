using AnglerModel;
using AnglerNotes.View.Tabs;
using AnglerNotes.ViewModel.CompareTime;
using System.Windows.Controls;

namespace AnglerNotes.View.CompareTime
{
    /// <summary>
    /// Interaction logic for CompareTimeView.xaml
    /// </summary>
    public partial class CompareTimeView : UserControl, ITabView
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
                return NoteTabType.CompareTime;
            }
        }

        private CompareTimeViewModel compareTimeViewModel;

        public CompareTimeView(int index)
        {
            InitializeComponent();

            this.Index = index;
            compareTimeViewModel = new CompareTimeViewModel(index);
            this.DataContext = compareTimeViewModel;
        }
    }
}
