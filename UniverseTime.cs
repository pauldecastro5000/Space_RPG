using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG
{
    public class UniverseTime
    {
        public event EventHandler UniverseTickPerMin;

        #region Public Properties
        public DateTime GameTime { get; set; } = new DateTime(2024, 6, 1, 12, 0, 0);
        #endregion Public Properties

        #region Private Variables
        private BackgroundWorker _bgwUniverseTime;
        private bool TimeRunning = false;
        private Stopwatch _sw = new Stopwatch();
        #endregion Private Variables

        #region Constructor
        public UniverseTime()
        {
            _bgwUniverseTime = new BackgroundWorker();
            _bgwUniverseTime.WorkerSupportsCancellation = true;
            _bgwUniverseTime.DoWork += _bgwUniverseTime_DoWork;
            _bgwUniverseTime.RunWorkerAsync();
        }
        #endregion Constructor

        #region Public Methods
        public void TimeStart()
        {
            TimeRunning = true;
        }
        public void TimeStop()
        {
            TimeRunning = false;
        }

        private void _bgwUniverseTime_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_bgwUniverseTime.CancellationPending)
            {
                if (TimeRunning)
                {
                    _sw.Reset();
                    _sw.Start();
                    while (true && TimeRunning)
                    {
                        if (_sw.Elapsed.TotalSeconds >= 1)
                        {
                            MainWindow.mainVm.dateTime = MainWindow.mainVm.dateTime.AddMinutes(1);
                            OnTickPerMin(EventArgs.Empty);
                            break;
                        }
                        System.Threading.Thread.Sleep(10);
                    }
                }
                System.Threading.Thread.Sleep(10);
            }
        }
        #endregion Public Methods

        protected void OnTickPerMin(EventArgs e)
        {
            UniverseTickPerMin?.Invoke(this, e);
        }

    }
}
