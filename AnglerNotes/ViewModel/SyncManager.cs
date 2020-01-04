using AnglerModel;
using AnglerNotes.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AnglerNotes.ViewModel
{
    class SyncManager: IDisposable
    {
        private static volatile SyncManager instance;
        private static object instanceLock = new System.Object();

        /// <summary>
        /// Get the singleton instance (supposedly thread safe)
        /// </summary>
        public static SyncManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new SyncManager();
                    }
                }

                return instance;
            }
        }

        private FileSystemWatcher FileSystemWatcher;
        private Dictionary<string, bool> IgnoreRead;
        /// <summary>
        /// Work around for double triggering of OnChanged
        /// </summary>
        private Dictionary<string, DateTime> LastRead;

        public delegate void TabResynced(NoteTabType tabType, int tabIndex);
        public event TabResynced ResyncEvent;

        private SyncManager()
        {
            IgnoreRead = new Dictionary<string, bool>();
            LastRead = new Dictionary<string, DateTime>();
            FileSystemWatcher = new FileSystemWatcher(SettingsViewModel.GetFolderPath(), "*.txt")
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            // Add event handlers.
            FileSystemWatcher.Changed += OnChanged;

            // Begin watching.
            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string filename = e.Name;

            // this process made the change, ignore
            if ((IgnoreRead.ContainsKey(filename) && IgnoreRead[filename]) 
                || (LastRead.ContainsKey(filename) && LastRead[filename] == File.GetLastWriteTime(e.FullPath)))
            {
                IgnoreRead[filename] = false;
                LastRead[filename] = File.GetLastWriteTime(e.FullPath);
                return;
            }

            //find corresponding filename in properties
            if (ModelAccessLock.Instance.RequestAccess())
            {
                var tabList = Properties.Settings.Default.Data.TabList;

                for (int i = 0; i < tabList.Count; i++)
                {
                    NoteTab tab = tabList[i];
                    if (tab.NoteTabType == NoteTabType.ItemCounter)
                    {
                        var itemCounter = Properties.Settings.Default.Data.ItemCounterTabs[tab.Index];
                        if (itemCounter.SyncFilename + ".txt" == filename)
                        {
                            LoadItemCounterFromFile(itemCounter);
                            ResyncEvent?.Invoke(NoteTabType.ItemCounter, tab.Index);
                            break;
                        }
                    }
                }
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        private void LoadItemCounterFromFile(AnglerModel.ItemCounter itemCounter)
        {
            if (!string.IsNullOrWhiteSpace(itemCounter.SyncFilename))
            {
                string fullpath = Path.Combine(SettingsViewModel.GetFolderPath(), itemCounter.SyncFilename + ".txt");

                if (!File.Exists(fullpath))
                {
                    itemCounter.SyncFilename = "";
                    SaveTimer.Instance.RequestSave();
                }
                else
                {
                    bool success = false;
                    int tries = 10;
                    int i = 0;
                    while (!success && i < tries)
                    {
                        try
                        {
                            string content = File.ReadAllText(fullpath);
                            itemCounter.LoadFromString(content);
                            LastRead[itemCounter.SyncFilename + ".txt"] = File.GetLastWriteTime(fullpath);
                            success = true;
                        }
                        catch (Exception)
                        {
                            i++;
                            Thread.Sleep(TimeSpan.FromMilliseconds(250));
                        }
                    }
                }
            }
        }

        private void SaveWeeklyScheduleToFile(AnglerModel.WeeklySchedule weeklySchedule)
        {
            if (!string.IsNullOrWhiteSpace(weeklySchedule.SyncFilename))
            {
                string fullpath = Path.Combine(SettingsViewModel.GetFolderPath(), weeklySchedule.SyncFilename + ".txt");

                string content = weeklySchedule.ToString();
                bool success = false;
                int tries = 10;
                int i = 0;
                while (!success && i < tries)
                {
                    try
                    {
                        IgnoreRead[weeklySchedule.SyncFilename + ".txt"] = true;
                        File.WriteAllText(fullpath, content);
                        success = true;
                    }
                    catch (Exception)
                    {
                        IgnoreRead[weeklySchedule.SyncFilename + ".txt"] = false;
                        i++;
                        Thread.Sleep(TimeSpan.FromMilliseconds(250));
                    }
                }
            }
        }

        private void SaveItemCounterToFile(AnglerModel.ItemCounter itemCounter)
        {
            if (!string.IsNullOrWhiteSpace(itemCounter.SyncFilename))
            {
                string fullpath = Path.Combine(SettingsViewModel.GetFolderPath(), itemCounter.SyncFilename + ".txt");

                string content = itemCounter.ToString();
                bool success = false;
                int tries = 10;
                int i = 0;
                while (!success && i < tries)
                {
                    try
                    {
                        IgnoreRead[itemCounter.SyncFilename + ".txt"] = true;
                        File.WriteAllText(fullpath, content);
                        success = true;
                    }
                    catch(Exception)
                    {
                        IgnoreRead[itemCounter.SyncFilename + ".txt"] = false;
                        i++;
                        Thread.Sleep(TimeSpan.FromMilliseconds(250));
                    }
                }
            }
        }


        public void SaveSyncedTabToFiles()
        {
            var tabList = Properties.Settings.Default.Data.TabList;
            for (int i = 0; i < tabList.Count; i++)
            {
                NoteTab tab = tabList[i];
                switch (tab.NoteTabType)
                {
                    case NoteTabType.WeeklySchedule:
                        var weeklySchedule = Properties.Settings.Default.Data.WeeklyScheduleTabs[tab.Index];
                        SaveWeeklyScheduleToFile(weeklySchedule);
                        break;
                    case NoteTabType.ItemCounter:
                        var itemCounter = Properties.Settings.Default.Data.ItemCounterTabs[tab.Index];
                        SaveItemCounterToFile(itemCounter);
                        break;
                    default:
                        break;
                }
            }
        }


        public void LoadSyncedTabsFromFiles()
        {
            var tabList = Properties.Settings.Default.Data.TabList;
            for (int i = 0; i < tabList.Count; i++)
            {
                NoteTab tab = tabList[i];
                switch (tab.NoteTabType)
                {
                    case NoteTabType.WeeklySchedule:
                        break;
                    case NoteTabType.ItemCounter:
                        var itemCounter = Properties.Settings.Default.Data.ItemCounterTabs[tab.Index];
                        LoadItemCounterFromFile(itemCounter);
                        break;
                    default:
                        break;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (FileSystemWatcher != null)
                    {
                        FileSystemWatcher.Dispose();
                        FileSystemWatcher = null;
                    }
                }

                disposedValue = true;
            }
        }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
        }
        #endregion
    }
}
