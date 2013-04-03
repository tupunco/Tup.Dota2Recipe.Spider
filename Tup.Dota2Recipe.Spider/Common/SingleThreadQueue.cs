using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tup.Dota2Recipe.Spider.Common
{
    /// <summary>
    /// The SingleThreadQueue class is a queue attached with a thread &amp; a process handler. Items added to the queue are always processed by specified process handler in a single thread.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to be added &amp; processed in the queue.</typeparam>
    /// <remarks>
    /// From:       http://code.google.com/p/nintegrate/source/browse/trunk/core/NIntegrate/Threading/SingleThreadQueue.cs
    /// license:    http://www.opensource.org/licenses/bsd-license.php
    /// </remarks>
    internal abstract class SingleThreadQueue<TItem> where TItem : class
    {
        private object _entryLock;
        private ManualResetEvent _entryWait;
        private ManualResetEvent _removeEntryWait;
        private ConcurrentQueue<TItem> _itemQueue;
        private AutoResetEvent _processWait;
        //private Thread _workerThread;
        private bool _hasWorkerThread;
        private const int _defaultSleepMilliseconds = 10000;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleThreadQueue&lt;TItem&gt;"/> class.
        /// </summary>
        /// <param name="maxQueueLength">Length of the max queue.</param>
        protected SingleThreadQueue(int maxQueueLength)
            : this(maxQueueLength, _defaultSleepMilliseconds)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleThreadQueue&lt;TItem&gt;"/> class.
        /// </summary>
        /// <param name="maxQueueLength">Length of the max queue.</param>
        /// <param name="threadSleepMilliseconds">The thread sleep milliseconds.</param>
        protected SingleThreadQueue(int maxQueueLength, int threadSleepMilliseconds)
        {
            _itemQueue = new ConcurrentQueue<TItem>();
            _processWait = new AutoResetEvent(false);
            _entryWait = new ManualResetEvent(true);
            _removeEntryWait = new ManualResetEvent(true);
            _entryLock = new object();
            if (maxQueueLength < -1)
            {
                throw new ArgumentException("Max queue length must be -1 (infinite), 0 (process inline) or a positive number.");
            }
            if (threadSleepMilliseconds <= 0)
            {
                throw new ArgumentException("Thread sleep interval must be positive.");
            }
            MaxQueueLength = maxQueueLength;
            ThreadSleepMilliseconds = threadSleepMilliseconds;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the length of the max queue.
        /// </summary>
        /// <value>The length of the max queue.</value>
        protected int MaxQueueLength { get; private set; }

        /// <summary>
        /// Gets or sets the thread sleep milliseconds.
        /// </summary>
        /// <value>The thread sleep milliseconds.</value>
        protected int ThreadSleepMilliseconds { get; private set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        protected int Count
        {
            get
            {
                return _itemQueue.Count;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueues the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Enqueue(TItem item)
        {
            if (MaxQueueLength == 0)
            {
                Process(item);
            }
            else if (Count < MaxQueueLength)
            {
                AddToQueue(item);
                InvokeThreadStart();
            }
            else
            {
                OnQueueOverflow(item);
            }
        }
        /// <summary>
        /// 当前项是否在 队列中
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TItem item)
        {
            ThrowHelper.ThrowIfNull(item, "item");

            if (_itemQueue.Count <= 0)
                return false;

            return _itemQueue.Contains(item);
        }
        #endregion

        #region Non-Public Methods
        /// <summary>
        /// Adds to queue.
        /// </summary>
        /// <param name="item">The item.</param>
        protected void AddToQueue(TItem item)
        {
            _itemQueue.Enqueue(item);
        }

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        protected TItem Dequeue()
        {
            TItem outItem = null;
            if (_itemQueue.TryDequeue(out outItem) && outItem != null)
                return outItem;
            else
                return default(TItem);
        }

        /// <summary>
        /// Invokes the thread start.
        /// </summary>
        protected void InvokeThreadStart()
        {
            if (!_hasWorkerThread)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    ProcessQueue();

                    _hasWorkerThread = false;
                });

                LogHelper.LogDebug("---------InvokeThreadStart----------");
                _hasWorkerThread = true;
            }
            //if ((_workerThread == null) && (_workerThread == null))
            //{
            //    _workerThread = new Thread(new ThreadStart(ProcessQueue));
            //    _workerThread.Name = GetType().Name;
            //    _workerThread.Start();
            //}
            //_workerThread.IsBackground = false;
            _processWait.Set();
        }

        /// <summary>
        /// Called when after process.
        /// </summary>
        protected virtual void OnAfterProcess()
        {
            if (Count < MaxQueueLength)
            {
                _entryWait.Set();
            }
        }

        /// <summary>
        /// Called when queue overflow.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void OnQueueOverflow(TItem item)
        {
            while (item != null)
            {
                _entryWait.WaitOne();

                lock (_entryLock)
                {
                    if (Count < MaxQueueLength)
                    {
                        AddToQueue(item);
                        item = default(TItem);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected abstract void Process(TItem item);

        /// <summary>
        /// Tries the peek.
        /// </summary>
        /// <param name="item_out">The item_out.</param>
        /// <returns></returns>
        protected bool TryPeek(out TItem item_out)
        {
            return _itemQueue.TryPeek(out item_out);
        }

        /// <summary>
        /// Tries the Dequeue.
        /// </summary>
        /// <param name="item_out">The item_out.</param>
        /// <returns></returns>
        protected bool TryDequeue(out TItem item_out)
        {
            return _itemQueue.TryDequeue(out item_out);
        }

        /// <summary>
        /// Processes the queue.
        /// </summary>
        private void ProcessQueue()
        {
            TItem item = null;
            while (true)
            {
                //_processWait.WaitOne(ThreadSleepMilliseconds, false);
                _processWait.WaitOne(ThreadSleepMilliseconds);
                //Thread.CurrentThread.IsBackground = false;
                while (_removeEntryWait.WaitOne()
                    && TryPeek(out item))
                {
                    Process(item);
                    OnAfterProcess();
                    Dequeue();
                }
                //lock (_itemQueue)
                //{
                //    if (Count == 0)
                //    {
                //        Thread.CurrentThread.IsBackground = true;
                //    }
                //}
            }
        }
        /// <summary>
        /// 移出队列中的某些项
        /// </summary>
        /// <param name="predicate"></param>
        /// <remarks>
        /// 被删除的项
        /// </remarks>
        public List<TItem> RemoveQueueItems(Func<TItem, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            List<TItem> deleteItems = new List<TItem>();

            lock (_entryLock)
            {
                try
                {
                    _removeEntryWait.Reset();

                    ConcurrentQueue<TItem> newItemQueue = null;
                    lock (_itemQueue)
                    {
                        if (_itemQueue.Count <= 0)
                            return deleteItems;

                        newItemQueue = new ConcurrentQueue<TItem>();
                        foreach (var item in _itemQueue)
                        {
                            if (!predicate(item))
                                newItemQueue.Enqueue(item);
                            else
                                deleteItems.Add(item);
                        }
                    }
                    Interlocked.Exchange<ConcurrentQueue<TItem>>(ref _itemQueue, newItemQueue);
                }
                finally
                {
                    _removeEntryWait.Set();
                }
            }

            return deleteItems;
        }
        #endregion
    }
}
