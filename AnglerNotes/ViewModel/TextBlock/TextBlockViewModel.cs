namespace AnglerNotes.ViewModel.TextBlock
{
    public class TextBlockViewModel : ViewModelBase
    {
        private int Index;

        public TextBlockViewModel(int index)
        {
            this.Index = index;
        }

        /// <summary>
        /// Get / Set value directly from / to db
        /// </summary>
        public string Content
        {
            get
            {
                string content = "Timeout Error";
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    content = Properties.Settings.Default.Data.TextBlockTabs[Index].Content;
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return content;
            }
            set
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    Properties.Settings.Default.Data.TextBlockTabs[Index].Content = value;
                    Properties.Settings.Default.Save();
                    ModelAccessLock.Instance.ReleaseAccess();
                    OnPropertyChanged("Content");
                }
            }
        }
    }
}
