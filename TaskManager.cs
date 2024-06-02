using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class TaskManager
    {
        private BackgroundWorker worker;

        public TaskManager()
        {
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
        #region PILOT TASKS
        public void StartEngine()
        {
            MainWindow.mainVm.MyShip.StartEngine();
        }
        #endregion PILOT TASKS

    }
}
