using AnglerModel;
using System.Collections.Generic;
using System.Linq;

namespace AnglerNotes.ViewModel.ItemCounter
{
    public class ItemCounterViewModel : ViewModelBase
    {
        private int Index;

        /// <summary>
        /// Get / Set value directly from / to db
        /// </summary>
        public List<Item> ItemList
        {
            get
            {
                List<Item> itemList = new List<Item>();
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    itemList = Properties.Settings.Default.Data.ItemCounterTabs[Index].ItemList;
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                return itemList;
            }
            set
            {
                List<Item> itemList = new List<Item>();
                if (ModelAccessLock.Instance.RequestAccess())
                {
                    Properties.Settings.Default.Data.ItemCounterTabs[Index].ItemList = value;
                    Properties.Settings.Default.Save();
                    ModelAccessLock.Instance.ReleaseAccess();
                }
                OnPropertyChanged("ItemList");
            }
        }

        public ItemCounterViewModel(int index)
        {
            this.Index = index;
        }

        /// <summary>
        /// Cannot dupplicate item names, return false is this case
        /// </summary>
        public bool TryAddItem(string Name)
        {
            bool success = false;
            if (ModelAccessLock.Instance.RequestAccess())
            {
                if (!ItemList.Any(w => (w.Name == Name)))
                {
                    ItemList.Add(new Item(Name, 0));
                    ItemList = new List<Item>(ItemList);

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
        public void Remove(Item item)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                ItemList.Remove(item);
                ItemList = new List<Item>(ItemList);

                OnPropertyChanged("ItemList");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }

        /// <summary>
        /// Move movedItem to inPlaceItem's position in the list
        /// </summary>
        public void Move(Item movedItem, Item inPlaceItem)
        {
            if (ModelAccessLock.Instance.RequestAccess())
            {
                int indexMovedItem = ItemList.IndexOf(movedItem);
                int indexInPlaceItem = ItemList.IndexOf(inPlaceItem);

                if (indexMovedItem < indexInPlaceItem)
                {
                    ItemList.Insert(indexInPlaceItem + 1, movedItem);
                    ItemList.RemoveAt(indexMovedItem);
                }
                else
                {
                    if (ItemList.Count + 1 > indexMovedItem + 1)
                    {
                        ItemList.Insert(indexInPlaceItem, movedItem);
                        ItemList.RemoveAt(indexMovedItem + 1);
                    }
                }

                ItemList = new List<Item>(ItemList);

                OnPropertyChanged("ItemList");
                ModelAccessLock.Instance.ReleaseAccess();
            }
        }
    }
}
