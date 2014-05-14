using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tup.Dota2Recipe.Spider.Common
{
    /// <summary>
    /// 并行消费者队列
    /// </summary>
    public abstract class ConcurrentConsumerQueue<TData> : IDisposable
        where TData : class
    {
        private BlockingCollection<TData> _itemQueue = new BlockingCollection<TData>();
        /// <summary>
        /// 
        /// </summary>
        public ConcurrentConsumerQueue()
        {
            Task.Factory.StartNew(ProcessQueue, TaskCreationOptions.LongRunning);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose() { _itemQueue.CompleteAdding(); }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        public void Enqueue(TData dataItem)
        {
            _itemQueue.Add(dataItem);
        }
        /// <summary>
        /// 
        /// </summary>
        private void ProcessQueue()
        {
            foreach (var dataItem in _itemQueue.GetConsumingEnumerable())
            {
                try
                {
                    Process(dataItem);

                    Thread.Sleep(0);
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("{0}-ConcurrentConsumerQueue-DataItem:{1}-ex:{2}",
                                            this.GetType(), dataItem, ex);
                    ex = null;

                    Thread.Sleep(5);
                }
            }
        }
        /// <summary>
        /// Processes the queue.
        /// </summary>
        /// <param name="dataItem"></param>
        protected abstract void Process(TData dataItem);
    }
}
