namespace AnglerNotes.ViewModel
{
    public class ModelAccessLock
    {
        private static volatile ModelAccessLock instance;
        private static object instanceLock = new System.Object();

        /// <summary>
        /// Get the singleton instance (supposedly thread safe)
        /// </summary>
        public static ModelAccessLock Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new ModelAccessLock();
                    }
                }

                return instance;
            }
        }

        private object modelLock;
        private const int modelLockmillisecondsTimeout = 1000;

        private int windowIndex;

        /// <summary>
        /// Gets a new index for a window
        /// </summary>
        public int WindowIndex
        {
            get
            {
                int index = windowIndex;
                windowIndex++;
                return index;
            }
        }

        public ModelAccessLock()
        {
            modelLock = new System.Object();
            windowIndex = 0;
        }

        public bool RequestAccess()
        {
            return System.Threading.Monitor.TryEnter(modelLock, modelLockmillisecondsTimeout);
        }

        public void ReleaseAccess()
        {
            System.Threading.Monitor.Exit(modelLock);
        }
    }
}
