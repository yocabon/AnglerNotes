using Dragablz;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace AnglerNotes.Utility
{
    /// <summary>
    /// Implements tab behaviour as required by the Dragablz module
    /// </summary>
    public class AnglerInterTabClient : IInterTabClient
    {
        /// <summary>
        /// Controls the creation of new windows
        /// </summary>
        public virtual INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            if (source == null) throw new ArgumentNullException("source");
            var sourceWindow = Window.GetWindow(source);
            if (sourceWindow == null) throw new ApplicationException("Unable to ascertain source window.");
            var newWindow = (Window)Activator.CreateInstance(sourceWindow.GetType());

            newWindow.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.DataBind);

            var newTabablzControl = newWindow.LogicalTreeDepthFirstTraversal().OfType<TabablzControl>().FirstOrDefault();
            if (newTabablzControl == null) throw new ApplicationException("Unable to ascertain tab control.");

            newTabablzControl.Items.Clear();

            return new NewTabHost<Window>(newWindow, newTabablzControl);
        }

        /// <summary>
        /// Controls the removal of empty windows
        /// When closing a window, close the app only if it's not the last window
        /// </summary>
        public virtual TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            if (WindowManager.Count > 1)
                return TabEmptiedResponse.CloseWindowOrLayoutBranch;
            else
                return TabEmptiedResponse.DoNothing;
        }

    }
}
