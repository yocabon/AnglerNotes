using AnglerModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AnglerNotes.ViewModel.ItemCounter
{
    public class ObservableItem: ViewModelBase
    {
        private string name;
        public string Name
        {
            get { return name;  }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }
        public ObservableItem(string name, int count)
        {
            this.Name = name;
            this.Count = count;
        }
        public ObservableItem(Item item)
        {
            Name = item.Name;
            Count = item.Count;
        }

        public Item ToItem()
        {
            return new Item(Name, Count);
        }

        public static ObservableCollection<ObservableItem>  GetObservableCollection(List<Item> items)
        {
            return new ObservableCollection<ObservableItem>(items.Select(i => new ObservableItem(i)));
        }
        public static List<Item> GetList(ObservableCollection<ObservableItem> items)
        {
            return items.Select(i => i.ToItem()).ToList();
        }
    }

    public class ItemCounterViewModel : ViewModelBase
    {
        private int Index;

        private ObservableCollection<ObservableItem> itemList;

        /// <summary>
        /// Get / Set value directly from / to db
        /// </summary>
        public ObservableCollection<ObservableItem> ItemList
        {
            get
            {
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    itemList = ObservableItem.GetObservableCollection(Properties.Settings.Default.Data.ItemCounterTabs[Index].ItemList);
                    itemList.CollectionChanged += CollectionChanged;
                    // Make sure that changing Count saves
                    foreach (ObservableItem item in itemList)
                    {
                        item.PropertyChanged += EntityViewModelPropertyChanged;
                    }
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return itemList;
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Make sure that changing Count saves
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ObservableItem item in e.OldItems)
                {
                    item.PropertyChanged -= EntityViewModelPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ObservableItem item in e.NewItems)
                {
                    item.PropertyChanged += EntityViewModelPropertyChanged;
                }
            }
            EntityViewModelPropertyChanged(sender, null);
        }

        public void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                Properties.Settings.Default.Data.ItemCounterTabs[Index].ItemList = ObservableItem.GetList(itemList);
                SaveTimer.Instance.RequestSave();
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        public ItemCounterViewModel(int index)
        {
            this.Index = index;
            this.itemList = new ObservableCollection<ObservableItem>();
            itemList.CollectionChanged += CollectionChanged;
        }

        /// <summary>
        /// Cannot dupplicate item names, return false is this case
        /// </summary>
        public bool TryAddItem(string Name)
        {
            bool success = false;
            if (ModelAccessLock.Instance.RequestAccess())
            {
                if (!itemList.Any(w => (w.Name == Name)))
                {
                    itemList.Add(new ObservableItem(Name, 0));

                    success = true;
                    OnPropertyChanged("ItemList");
                }
                ModelAccessLock.Instance.ReleaseAccess();
            }
            return success;
        }

        /// <summary>
        /// use ItemList.Remove(item);, must be the same reference
        /// </summary>
        public void Remove(ObservableItem item)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                itemList.Remove(item);

                OnPropertyChanged("ItemList");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Move movedItem to inPlaceItem's position in the list
        /// </summary>
        public void Move(ObservableItem movedItem, ObservableItem inPlaceItem)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                int indexMovedItem = itemList.IndexOf(movedItem);
                int indexInPlaceItem = itemList.IndexOf(inPlaceItem);

                if (indexMovedItem < indexInPlaceItem)
                {
                    itemList.Insert(indexInPlaceItem + 1, movedItem);
                    itemList.RemoveAt(indexMovedItem);
                }
                else
                {
                    if (itemList.Count + 1 > indexMovedItem + 1)
                    {
                        itemList.Insert(indexInPlaceItem, movedItem);
                        itemList.RemoveAt(indexMovedItem + 1);
                    }
                }

                OnPropertyChanged("ItemList");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }
    }
}
