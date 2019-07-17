using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnglerNotes.ViewModel
{
    public class SaveTimer
    {
        private static volatile SaveTimer instance;
        private static object instanceLock = new System.Object();

        /// <summary>
        /// Get the singleton instance (supposedly thread safe)
        /// </summary>
        public static SaveTimer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new SaveTimer();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Autosave 5 seconds after a change
        /// </summary>
        private const int DefaultTime = 5;

        private bool SavePlanned = false;

        private void SaveAllSettings()
        {
            if (ModelAccessLock.Instance.RequestAccess() && !App.IsShutingDown)
            {
                Properties.Settings.Default.Save();
                SavePlanned = false;
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        public void RequestSave()
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                if (!SavePlanned)
                {
                    SavePlanned = true;
                    Task.Factory.StartNew(() => Thread.Sleep(DefaultTime * 1000))
                                .ContinueWith(task =>SaveAllSettings(), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }
}
