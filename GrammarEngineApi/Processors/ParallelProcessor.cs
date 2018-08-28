using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace GrammarEngineApi.Processors
{
    /// <summary>
    ///     Processes jobs in parallel trying to utilize all available
    ///     resources.
    /// </summary>
    /// <typeparam name="TJob">Job type.</typeparam>
    public class ParallelProcessor<TJob>
    {
        private readonly Action<TJob> _action;
        private readonly int _batchSize;
        private readonly Func<bool> _canProduce;
        private readonly IProducerConsumerCollection<TJob> _collection;
        private readonly int _numTasks;
        private readonly Func<int, TJob[]> _producer;

        private readonly Action _taskBody;
        private readonly ConcurrentDictionary<Task, object> _tasks = new ConcurrentDictionary<Task, object>();

        /// <summary>
        ///     Creates a processor with number of worker tasks equal to
        ///     <see cref="Environment.ProcessorCount" /> and no ability to load
        ///     new jobs.
        /// </summary>
        /// <param name="collection">Jobs collection.</param>
        /// <param name="batchSize">Maximum job batch size for one thread.</param>
        /// <param name="action">Job action.</param>
        public ParallelProcessor(IProducerConsumerCollection<TJob> collection, int batchSize, Action<TJob> action)
            : this(collection, batchSize, action, sz => Array.Empty<TJob>(), () => false, Environment.ProcessorCount)
        {
        }

        /// <summary>
        ///     Creates a processor with number of worker tasks equal to
        ///     <see cref="Environment.ProcessorCount" />.
        /// </summary>
        /// <param name="collection">Jobs collection.</param>
        /// <param name="batchSize">Maximum job batch size for one thread.</param>
        /// <param name="action">Job action.</param>
        /// <param name="producer">New jobs producer.</param>
        /// <param name="canProduce">Function that indicates whether new jobs can be produced.</param>
        public ParallelProcessor(IProducerConsumerCollection<TJob> collection, int batchSize, Action<TJob> action, Func<int, TJob[]> producer, Func<bool> canProduce)
            : this(collection, batchSize, action, producer, canProduce, Environment.ProcessorCount)
        {
        }

        /// <summary>
        ///     Creates a processor with manually specified number of tasks.
        /// </summary>
        /// <param name="collection">Jobs collection.</param>
        /// <param name="batchSize">Maximum job batch size for one thread.</param>
        /// <param name="action">Job action.</param>
        /// <param name="producer">New jobs producer.</param>
        /// <param name="canProduce">Function that indicates whether new jobs can be produced.</param>
        /// <param name="numTasks">Number of worker tasks.</param>
        public ParallelProcessor(IProducerConsumerCollection<TJob> collection, int batchSize, Action<TJob> action, Func<int, TJob[]> producer, Func<bool> canProduce, int numTasks)
        {
            _collection = collection;
            _action = action;
            _batchSize = batchSize;
            _producer = producer;
            _canProduce = canProduce;
            _numTasks = numTasks;

            _taskBody = () =>
            {
                ProduceJobs();

                while (_collection.TryTake(out var item))
                {
                    _action(item);

                    ProduceJobs();

                    if (_collection.Count > 0 && _tasks.Count < _numTasks)
                    {
                        for (int i = 0; i < _numTasks - _tasks.Count; i++)
                        {
                            CreateTask();
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Starts processing jobs and blocks calling thread
        /// until all jobs are processed.
        /// </summary>
        public void Process()
        {
            for (int i = 0; i < _numTasks; i++)
            {
                CreateTask();
            }

            while (_tasks.Count > 0)
            {
                Task.WaitAll(_tasks.Keys.ToArray());
            }
        }

        private void CreateTask()
        {
            var task = new Task(_taskBody);
            task.ContinueWith(t => _tasks.TryRemove(t, out _));
            _tasks.TryAdd(task, null);
            task.Start();
        }

        private void ProduceJobs()
        {
            int full = _numTasks * _batchSize;
            if (_collection.Count < full / 2 && _canProduce())
            {
                while (_canProduce() && _collection.Count < full)
                {
                    var batch = _producer(_batchSize);
                    for (int i = 0; i < batch.Length; i++)
                    {
                        _collection.TryAdd(batch[i]);
                    }
                }
            }
        }
    }
}