using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace TextUtil
{
    public class ParallelProcessor<T>
    {
        private readonly int _batchSize;
        private readonly ConcurrentDictionary<Task, object> _tasks = new ConcurrentDictionary<Task, object>();
        private readonly IProducerConsumerCollection<T> _collection;
        private readonly Action<T> _action;
        private readonly Func<T> _producer;
        private readonly Func<bool> _canProduce;
        private readonly int _numTasks;

        private readonly Action _taskBody = null;

        public ParallelProcessor(IProducerConsumerCollection<T> collection, int batchSize, Action<T> action, Func<T> producer, Func<bool> canProduce)
            : this(collection, batchSize, action, producer, canProduce, Environment.ProcessorCount)
        {
        }

        public ParallelProcessor(IProducerConsumerCollection<T> collection, int batchSize, Action<T> action, Func<T> producer, Func<bool> canProduce, int numTasks)
        {
            _collection = collection;
            _action = action;
            _batchSize = batchSize;
            _producer = producer;
            _canProduce = canProduce;
            _numTasks = numTasks;

            _taskBody = () =>
            {
                while (_collection.TryTake(out var item))
                {
                    _action(item);

                    int full = _numTasks * _batchSize;
                    if (_collection.Count < full / 2 && _canProduce())
                    {
                        while (_canProduce() && _collection.Count < full)
                        {
                            _collection.TryAdd(_producer());
                        }
                    }

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

        private void CreateTask()
        {
            var task = new Task(_taskBody);
            task.ContinueWith(t =>
            {
                return _tasks.TryRemove(t, out _);
            });
            _tasks.TryAdd(task, null);
            task.Start();
        }

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
    }
}