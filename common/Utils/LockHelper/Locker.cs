using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace common.Utils.LockHelper
{
    public class Locker : IDisposable
    {
        public long threadId { get; private set; }
        public long lockId { get; private set; }
        public string methodName { get; private set; }
        public int lineNo { get; private set; }
        public string lockName { get; private set; }

        private object lockObj = new object();

        public Locker(long _lockId, string _name)
        {
            lockName = _name;
            lockId = _lockId;
        }

        internal void SetLockData(int _tId, string _mName, int _lineNo)
        {
            threadId = _tId;
            methodName = _mName;
            lineNo = _lineNo;
        }

        internal void BeginLock()
        {
            Monitor.Enter(lockObj);
        }

        void EndLock()
        {
            Monitor.Exit(lockObj);
            methodName = "";
            lineNo = 0;
            threadId = 0;
        }

        public void Dispose()
        {
            EndLock();
        }
    }
}
