using Dragablz;
using AnglerModel;
using AnglerNotes.View.CompareTime;
using AnglerNotes.View.ItemCounter;
using AnglerNotes.View.TextBlock;
using AnglerNotes.View.WeeklySchedule;
using AnglerNotes.ViewModel.Tabs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace AnglerNotes.View.Tabs
{
    /// <summary>
    /// Interaction logic for NoteTabView.xaml
    /// </summary>
    public partial class NoteTabView : UserControl
    {
        /// <summary>
        /// Provides mechanisms for providing new windows and closing windows
        /// </summary>
        public IInterTabClient InterTabClient
        {
            get
            {
                return noteTabViewModel.InterTabClient;
            }
        }

        private NoteTabViewModel noteTabViewModel;

        public NoteTabView()
        {
            InitializeComponent();

            noteTabViewModel = new NoteTabViewModel();
            DataContext = noteTabViewModel;
        }

        /// <summary>
        /// Load all open tabs for this window
        /// </summary>
        public void SetTabs(int windowIndex)
        {
            NoteTabs.Items.Clear();
            NoteTabs.ClosingItemCallback += NoteTabs_ClosingEvent;

            List<NoteTab> tabNames = noteTabViewModel.TabNames;
            for (int i = 0; i < tabNames.Count; i++)
            {
                NoteTab tab = tabNames[i];
                if (tab.WindowNumber == windowIndex)
                {
                    switch (tab.NoteTabType)
                    {
                        case NoteTabType.TextBlock:
                            NoteTabs.Items.Add(new TabItem() { Header = tab.Name, Content = new TextBlockView(tab.Index) });
                            break;
                        case NoteTabType.WeeklySchedule:
                            NoteTabs.Items.Add(new TabItem() { Header = tab.Name, Content = new WeeklyScheduleView(tab.Index) });
                            break;
                        case NoteTabType.CompareTime:
                            NoteTabs.Items.Add(new TabItem() { Header = tab.Name, Content = new CompareTimeView(tab.Index) });
                            break;
                        case NoteTabType.ItemCounter:
                            NoteTabs.Items.Add(new TabItem() { Header = tab.Name, Content = new ItemCounterView(tab.Index) });
                            break;
                        default:
                            throw new System.NotImplementedException("Tab type not implemented");
                    }
                }
            }

            if (NoteTabs.Items.Count > 0)
                NoteTabs.SelectedItem = NoteTabs.Items[NoteTabs.Items.Count - 1];
        }

        /// <summary>
        /// When quitting the app, save the current order of tabs so that the tabs appear in the same order when it is restarted
        /// </summary>
        public void ReorderTabs(int windowIndex)
        {
            var orderedHeaders = NoteTabs.GetOrderedHeaders();
            List<NoteTab> orderedTabs = orderedHeaders.Select(h => new NoteTab((string)((TabItem)h.Content).Header, ((ITabView)((TabItem)h.Content).Content).NoteTabType, ((ITabView)((TabItem)h.Content).Content).Index, windowIndex)).ToList();
            NoteTabViewModel.AddOrderedTabs(orderedTabs);
        }

        /// <summary>
        /// Creates a new QuickNote tab with default parameters
        /// </summary>
        public void QuickNote_Click(object sender, RoutedEventArgs e)
        {
            NoteTab noteTab = noteTabViewModel.Add(NoteTabType.TextBlock);
            NoteTabs.Items.Add(new TabItem() { Header = noteTab.Name, Content = new TextBlockView(noteTab.Index) });
            NoteTabs.SelectedItem = NoteTabs.Items[NoteTabs.Items.Count - 1];
        }

        /// <summary>
        /// Creates a new WeeklySchedule tab with default parameters
        /// </summary>
        public void WeeklySchedule_Click(object sender, RoutedEventArgs e)
        {
            NoteTab noteTab = noteTabViewModel.Add(NoteTabType.WeeklySchedule);
            NoteTabs.Items.Add(new TabItem() { Header = noteTab.Name, Content = new WeeklyScheduleView(noteTab.Index) });
            NoteTabs.SelectedItem = NoteTabs.Items[NoteTabs.Items.Count - 1];
        }

        /// <summary>
        /// Creates a new CompareTime tab with default parameters
        /// </summary>
        public void CompareTime_Click(object sender, RoutedEventArgs e)
        {
            NoteTab noteTab = noteTabViewModel.Add(NoteTabType.CompareTime);
            NoteTabs.Items.Add(new TabItem() { Header = noteTab.Name, Content = new CompareTimeView(noteTab.Index) });
            NoteTabs.SelectedItem = NoteTabs.Items[NoteTabs.Items.Count - 1];
        }

        /// <summary>
        /// Creates a new ItemCounter tab with default parameters
        /// </summary>
        public void ItemCounter_Click(object sender, RoutedEventArgs e)
        {
            NoteTab noteTab = noteTabViewModel.Add(NoteTabType.ItemCounter);
            NoteTabs.Items.Add(new TabItem() { Header = noteTab.Name, Content = new ItemCounterView(noteTab.Index) });
            NoteTabs.SelectedItem = NoteTabs.Items[NoteTabs.Items.Count - 1];
        }

        /// <summary>
        /// Push the saved ordering of tabs to the db
        /// </summary>
        public void SendOrderedTabs()
        {
            NoteTabViewModel.SendOrderedTabs();
        }

        /// <summary>
        /// Tab is being closed
        /// </summary>
        private void NoteTabs_ClosingEvent(ItemActionCallbackArgs<TabablzControl> args)
        {
            if (args.DragablzItem.Content == BindingOperations.DisconnectedSource)
                args.Cancel();

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                TabItem toBeRemovedTab = (TabItem)args.DragablzItem.Content;
                ITabView tabView = (ITabView)toBeRemovedTab.Content;

                int index = tabView.Index;
                NoteTabType noteTabType = tabView.NoteTabType;

                bool success = noteTabViewModel.Delete(index, noteTabType);

                if (!success) // timeout
                {
                    args.Cancel();
                    return;
                }

                // Select preceding tab if the removed tab was previously the selected tab
                if (NoteTabs.SelectedItem == toBeRemovedTab && NoteTabs.Items.Count > 1)
                {
                    var item = NoteTabs.Items[0];
                    var orderedItems = NoteTabs.GetOrderedHeaders().Select(h => (TabItem)h.Content).ToArray();

                    for (int i = 1; i < NoteTabs.Items.Count; i++)
                    {
                        if (orderedItems[i] == toBeRemovedTab)
                        {
                            item = orderedItems[i - 1];
                            break;
                        }
                    }

                    NoteTabs.SelectedItem = item;
                }
            }
            else
                args.Cancel();
        }

        private TabItem toBeEditedItem;
        private TextBox toBeEditedTextbox;

        /// <summary>
        /// Double click of tab name ==> rename tab
        /// </summary>
        private void NoteTabs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Controls.Primitives.Thumb) && toBeEditedItem == null)
            {
                System.Windows.Controls.TextBlock headerBox = ((System.Windows.Controls.Primitives.Thumb)sender).Parent.FindChild<System.Windows.Controls.TextBlock>("HeaderBox");
                TextBox editBox = ((System.Windows.Controls.Primitives.Thumb)sender).Parent.FindChild<TextBox>("EditBox");
                Thumb dragThumb = ((System.Windows.Controls.Primitives.Thumb)sender).Parent.FindChild<Thumb>("DragThumb");

                TabItem selectedItem = (TabItem)NoteTabs.SelectedItem;
                DragablzItem dragablzItem = editBox.TryFindParent<DragablzItem>();

                if ((TabItem)dragablzItem.Content != selectedItem)
                    return;

                dragThumb.IsEnabled = false;
                dragThumb.Focusable = false;
                dragThumb.IsHitTestVisible = false;
                headerBox.Visibility = Visibility.Collapsed;

                editBox.Text = headerBox.Text;
                editBox.Visibility = Visibility.Visible;
                editBox.LostFocus += TextBox_LostFocus;
                NoteTabs.SelectionChanged += NoteTabs_TabChanged;

                editBox.Focus();

                toBeEditedTextbox = editBox;
                toBeEditedItem = (TabItem)NoteTabs.SelectedItem;
            }
        }

        /// <summary>
        /// When renaming a tab, clicking outside = finished renaming
        /// </summary>
        private void NoteTabs_TabChanged(object sender, RoutedEventArgs e)
        {
            if (toBeEditedTextbox != null)
                ChangeHeader(toBeEditedTextbox);
        }

        /// <summary>
        /// When renaming a tab, clicking outside = finished renaming
        /// </summary>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeHeader(sender);
        }

        /// <summary>
        /// When renaming a tab, press enter = finished renaming
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChangeHeader(sender);
        }

        /// <summary>
        /// Core of the code for renaming a tab
        /// </summary>
        private void ChangeHeader(object sender)
        {
            if (toBeEditedItem == null)
                return;

            TextBox editBox = ((TextBox)sender);
            if (editBox != toBeEditedTextbox)
                return;

            System.Windows.Controls.TextBlock headerBox = editBox.Parent.FindChild<System.Windows.Controls.TextBlock>("HeaderBox");
            Thumb dragThumb = editBox.Parent.FindChild<Thumb>("DragThumb");

            dragThumb.IsEnabled = true;
            dragThumb.Focusable = true;
            dragThumb.IsHitTestVisible = true;

            ITabView tabView = (ITabView)toBeEditedItem.Content;

            bool success = noteTabViewModel.UpdateTabName(tabView.Index, tabView.NoteTabType, editBox.Text);

            headerBox.Visibility = Visibility.Visible;
            headerBox.Focus();

            editBox.Visibility = Visibility.Collapsed;
            editBox.LostFocus -= TextBox_LostFocus;
            NoteTabs.SelectionChanged -= NoteTabs_TabChanged;

            if (success && !String.IsNullOrEmpty(editBox.Text))
                toBeEditedItem.Header = editBox.Text;

            toBeEditedItem = null;
            toBeEditedTextbox = null;
        }
    }
}
