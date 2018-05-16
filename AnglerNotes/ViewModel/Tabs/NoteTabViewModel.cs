using Dragablz;
using AnglerModel;
using AnglerNotes.Utility;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AnglerNotes.ViewModel.Tabs
{
    public class NoteTabViewModel : ViewModelBase
    {
        private static ConcurrentBag<NoteTab> orderedTabs;

        /// <summary>
        /// When quitting the app, save the current order of tabs so that the tabs appear in the same order when it is restarted
        /// </summary>
        public static void AddOrderedTabs(List<NoteTab> tabs)
        {
            if (orderedTabs == null)
                orderedTabs = new ConcurrentBag<NoteTab>();

            foreach (NoteTab tab in tabs)
                orderedTabs.Add(tab);
        }

        /// <summary>
        /// Push the saved ordering of tabs to the db
        /// </summary>
        public static void SendOrderedTabs()
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Root property = Properties.Settings.Default.Data;
                property.ReorderTabs(orderedTabs.ToList());
                orderedTabs = null;

                Properties.Settings.Default.Save();
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        private IInterTabClient interTabClient;

        public IInterTabClient InterTabClient
        {
            get { return interTabClient; }
        }

        /// <summary>
        /// Get tabs directly from the db
        /// </summary>
        public List<NoteTab> TabNames
        {
            get
            {
                List<NoteTab> Tabs = new List<NoteTab>();
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    Tabs = Properties.Settings.Default.Data.TabList;
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return Tabs;
            }
        }

        public NoteTabViewModel()
        {
            interTabClient = new AnglerInterTabClient();
        }

        /// <summary>
        /// Add a new tab to the db, independant of the view completely
        /// </summary>
        public NoteTab Add(NoteTabType tabType)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Root property = Properties.Settings.Default.Data;
                NoteTab noteTab = property.AddNewTab(tabType);

                Properties.Settings.Default.Save();

                ModelAccessLock.Instance.ReleaseAccess();
                return noteTab;
            }
            else
                return null;
        }

        /// <summary>
        /// Try to delete a tab from the db, independant of the view completely. False if it failed
        /// </summary>
        internal bool Delete(int index, NoteTabType noteTabType)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Root property = Properties.Settings.Default.Data;

                int tabIndex = property.TabList.FindIndex(t => t.Index == index && t.NoteTabType == noteTabType);
                property.RemoveTab(tabIndex);

                Properties.Settings.Default.Save();
                ModelAccessLock.Instance.ReleaseAccess();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        ///  Update tab name in the db
        /// </summary>
        public bool UpdateTabName(int index, NoteTabType noteTabType, string newName)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Root property = Properties.Settings.Default.Data;
                int tabIndex = property.TabList.FindIndex(t => t.Index == index && t.NoteTabType == noteTabType);

                Properties.Settings.Default.Data.TabList[tabIndex].Name = newName;
                Properties.Settings.Default.Save();

                ModelAccessLock.Instance.ReleaseAccess();
                return true;
            }
            else
                return false;
        }
    }
}
