using AnglerModel;
using AnglerNotes.View.Tabs;
using AnglerNotes.ViewModel.ItemCounter;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AnglerNotes.View.ItemCounter
{
    /// <summary>
    /// Interaction logic for ItemCounterView.xaml
    /// </summary>
    public partial class ItemCounterView : UserControl, ITabView
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
                return NoteTabType.ItemCounter;
            }
        }

        private ItemCounterViewModel itemCounterViewModel;

        public ItemCounterView(int index)
        {
            InitializeComponent();

            this.Index = index;
            itemCounterViewModel = new ItemCounterViewModel(index);
            this.DataContext = itemCounterViewModel;
        }

        /// <summary>
        /// Do not dupplicate item names
        /// </summary>
        private void TryAddNewItem()
        {
            if (!String.IsNullOrEmpty(NewItemName.Text))
            {
                bool success = itemCounterViewModel.TryAddItem(NewItemName.Text);

                if (success)
                {
                    NewItemName.Text = "";
                    this.Focus();
                    SetDefaultVisibility();
                }
            }
        }

        private void TrySettingUpSync()
        {
            if (!string.IsNullOrWhiteSpace(SyncItemName.Text) && itemCounterViewModel.Filename != SyncItemName.Text)
            {
                string messageBoxText = "Do you want to sync tab with " + SyncItemName.Text  + "?";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        bool success = itemCounterViewModel.TrySync(SyncItemName.Text);
                        if (success)
                        {
                            this.Focus();
                            SetDefaultVisibility();
                        }
                        break;
                    case MessageBoxResult.No:
                        // User pressed No button, cancel operation
                        this.Focus();
                        SetDefaultVisibility();
                        break;
                }
            }
            else if(string.IsNullOrWhiteSpace(SyncItemName.Text) && itemCounterViewModel.Filename != "") 
            {
                // Unsync
                string messageBoxText = "Do you want to unsync tab?";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // User pressed Yes button
                        bool success = itemCounterViewModel.TryUnsync();
                        if (success)
                        {
                            this.Focus();
                            SetDefaultVisibility();
                        }
                        break;
                    case MessageBoxResult.No:
                        // User pressed No button, cancel operation
                        this.Focus();
                        SetDefaultVisibility();
                        break;
                }
            }
        }

        private void SetDefaultVisibility()
        {
            NewItemShow.Visibility = System.Windows.Visibility.Visible;
            SyncItemShow.Visibility = System.Windows.Visibility.Visible;
            NewItemLine.Visibility = System.Windows.Visibility.Collapsed;
            SyncItemLine.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetNewItemVisibility()
        {
            NewItemShow.Visibility = System.Windows.Visibility.Visible;
            SyncItemShow.Visibility = System.Windows.Visibility.Collapsed;
            NewItemLine.Visibility = System.Windows.Visibility.Visible;
            SyncItemLine.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetSyncSettingsVisibility()
        {
            NewItemShow.Visibility = System.Windows.Visibility.Collapsed;
            SyncItemShow.Visibility = System.Windows.Visibility.Visible;
            NewItemLine.Visibility = System.Windows.Visibility.Collapsed;
            // reset field to db value
            SyncItemName.Text = itemCounterViewModel.Filename;
            SyncItemLine.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// When the little + button is clicked, unveil hidden content
        /// </summary>
        private void NewItemShow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NewItemLine.Visibility == System.Windows.Visibility.Visible)
                SetDefaultVisibility();
            else
            {
                SetNewItemVisibility();
                NewItemName.Focus();
            }
        }

        private void SyncItemShow_Click(object sender, RoutedEventArgs e)
        {
            if (SyncItemLine.Visibility == System.Windows.Visibility.Visible)
                SetDefaultVisibility();
            else
            {
                SetSyncSettingsVisibility();
                SyncItemName.Focus();
            }
        }

        /// <summary>
        /// When the trash bin is clicked
        /// </summary>
        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Button button = ((Button)sender);
                ObservableItem item = ((ObservableItem)button.DataContext);
                itemCounterViewModel.Remove(item);
            }
        }

        /// <summary>
        /// When a item from the list is dropped into the list, reposition it
        /// </summary>
        private void MainList_Drop(object sender, DragEventArgs e)
        {
            ObservableItem droppedData = e.Data.GetData(typeof(ObservableItem)) as ObservableItem;
            ObservableItem targetItem = null;
            try
            {
                targetItem = ((System.Windows.Controls.TextBlock)e.OriginalSource).DataContext as ObservableItem;
            }
            catch (Exception) { }

            if (targetItem == null)
            {
                try
                {
                    targetItem = ((System.Windows.Controls.Border)e.OriginalSource).DataContext as ObservableItem;
                }
                catch (Exception) { }
            }

            if (targetItem != null)
                itemCounterViewModel.Move(droppedData, targetItem);
        }

        /// <summary>
        /// Item is being moved
        /// </summary>
        private bool dragInProgress;

        /// <summary>
        /// When left clicking an item, and moving the mouse, can reposition the item in the list
        /// </summary>
        private void MainList_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && sender is ListView && !dragInProgress && !(e.OriginalSource is Button))
            {
                ListView sourceList = ((ListView)sender);
                object draggedItem = sourceList.SelectedItem;
                if (draggedItem != null)
                    try
                    {
                        dragInProgress = true;
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                                new System.Threading.ParameterizedThreadStart(DoDragDrop), draggedItem);
                    }
                    catch (Exception) { }
                    finally
                    {
                        dragInProgress = false;
                    }
            }
        }

        private void DoDragDrop(object draggedItem)
        {
            DragDrop.DoDragDrop(MainList, draggedItem, DragDropEffects.Move);
        }

        /// <summary>
        /// When enter is pressed from the add item text box
        /// </summary>
        private void NewItemName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                TryAddNewItem();
        }

        /// <summary>
        /// When the ok button is clicked
        /// </summary>
        private void NewItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TryAddNewItem();
        }

        private void SyncItemName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                TrySettingUpSync();
        }

        private void SyncItemValidate_Click(object sender, RoutedEventArgs e)
        {
            TrySettingUpSync();
        }
    }
}
