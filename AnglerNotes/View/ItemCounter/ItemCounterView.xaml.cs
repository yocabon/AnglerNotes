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
        /// When the ok button is clicked
        /// </summary>
        private void NewItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TryAddNewItem();
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
                    NewItemLine.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// When the little + button is clicked, unveil hidden content
        /// </summary>
        private void NewItemShow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NewItemLine.Visibility == System.Windows.Visibility.Visible)
                NewItemLine.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                NewItemLine.Visibility = System.Windows.Visibility.Visible;
                NewItemName.Focus();
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
    }
}
