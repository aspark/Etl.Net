﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Paillave.Etl.Core
{
    public class JobPool : IDisposable
    {
        private object _lock = new object();
        private bool _isStopped = false;

        private Queue<Action> _actionQueue = new Queue<Action>();

        public JobPool()
        {
            Task.Run(() => BackgroundProcess());
        }
        private System.Threading.EventWaitHandle _mtxWaitNewProcess = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);

        private void BackgroundProcess()
        {
            while (!_isStopped)
            {
                lock (_lock)
                {
                    while (_actionQueue.Count > 0)
                        _actionQueue.Dequeue()();
                }
                _mtxWaitNewProcess.WaitOne();
            }
        }

        public void Execute(Action action)
        {
            var mtxInit = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
            lock (_lock)
            {
                _actionQueue.Enqueue(() =>
                {
                    action();
                    mtxInit.Set();
                });
            }
            _mtxWaitNewProcess.Set();
            mtxInit.WaitOne();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _isStopped = true;
                    _mtxWaitNewProcess.Set();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
