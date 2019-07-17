using System;
using AnglerNotes.Utility;

namespace AnglerNotes.ViewModel.CompareTime
{
    public class CompareTimeViewModel : ViewModelBase
    {
        private int Index;

        /// <summary>
        /// Get / Set value directly from / to db
        /// </summary>
        public DateTime Start
        {
            get
            {
                DateTime time = DateTime.MinValue;
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    time = Properties.Settings.Default.Data.CompareTimeTabs[Index].Start;
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return time;
            }
            set
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    Properties.Settings.Default.Data.CompareTimeTabs[Index].Start = value;
                    SaveTimer.Instance.RequestSave();
                    ModelAccessLock.Instance.ReleaseAccess();
                    OnPropertyChanged("Start");
                    OnPropertyChanged("TimeDiff");
                }
            }
        }

        /// <summary>
        /// Get / Set value directly from / to db
        /// </summary>
        public DateTime End
        {
            get
            {
                DateTime time = DateTime.MinValue;
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    time = Properties.Settings.Default.Data.CompareTimeTabs[Index].End;
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return time;
            }
            set
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    Properties.Settings.Default.Data.CompareTimeTabs[Index].End = value;
                    SaveTimer.Instance.RequestSave();
                    ModelAccessLock.Instance.ReleaseAccess();
                    OnPropertyChanged("End");
                    OnPropertyChanged("TimeDiff");
                }
            }
        }

        /// <summary>
        /// Format is [-][d’.’]hh’:’mm’:’ss[‘.’fffffff]
        /// </summary>
        public string TimeDiff
        {
            get
            {
                TimeSpan timeDiff = End.Subtract(Start);

                // round to the nearest second (else, we can get a one second error on the computed difference)
                TimeSpan roundedDiff = timeDiff.RoundSeconds(0);
                return roundedDiff.ToString("c");
            }
        }

        public CompareTimeViewModel(int index)
        {
            this.Index = index;
        }
    }
}
