using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace AnglerNotes.Utility
{
    /// <summary>
    /// Button with context menu attached (for the top left + button)
    /// </summary>
    public class AddButton : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnClick;
            base.OnDetaching();
        }

        /// <summary>
        /// On click of the button, show a context menu that contains all the available tabs
        /// </summary>
        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var contextMenu = AssociatedObject.ContextMenu;
            if (contextMenu == null)
            {
                return;
            }

            contextMenu.PlacementTarget = AssociatedObject;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }
    }
}
