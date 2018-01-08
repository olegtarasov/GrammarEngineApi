using System;
using System.Collections.Concurrent;

namespace GrammarEngineApi
{
    /// <summary>
    /// Processes in parallel sentences which are read with
    /// <see cref="TextFileSegmenter"/>.
    /// </summary>
    public class ParallelSentenceProcessor : ParallelProcessor<string>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="segmenter">Text file segmenter.</param>
        /// <param name="batchSize">Batch size for one thread.</param>
        /// <param name="action">Action to perform on a sentence. This can be null!</param>
        public ParallelSentenceProcessor(TextFileSegmenter segmenter, int batchSize, Action<string> action) : this(segmenter, batchSize, action, Environment.ProcessorCount)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="segmenter">Text file segmenter.</param>
        /// <param name="batchSize">Batch size for one thread.</param>
        /// <param name="action">Action to perform on a sentence. This can be null!</param>
        /// <param name="numTasks">Number of worker tasks to spawn.</param>
        public ParallelSentenceProcessor(TextFileSegmenter segmenter, int batchSize, Action<string> action, int numTasks) 
            : base(
                new ConcurrentQueue<string>(), batchSize, action, 
                GetProducer(segmenter), GetCanProduce(segmenter), 
                numTasks)
        {
        }

        private static Func<string> GetProducer(TextFileSegmenter segmenter)
        {
            return () =>
            {
                lock (segmenter)
                {
                    return segmenter.ReadSentence();
                }
            };
        }

        private static Func<bool> GetCanProduce(TextFileSegmenter segmenter) => () => segmenter.CanRead;
    }
}