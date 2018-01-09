﻿using System;
using System.Collections.Concurrent;

namespace GrammarEngineApi.Processors
{
    /// <summary>
    /// Processes in parallel sentences which are read with
    /// <see cref="TextFileSegmenter"/>.
    /// </summary>
    public class ParallelSentenceProcessor<TContext> : ParallelProcessor<SentenceJob<TContext>>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="segmenter">Text file segmenter.</param>
        /// <param name="batchSize">Batch size for one thread.</param>
        /// <param name="action">Action to perform on a sentence. This can be null!</param>
        public ParallelSentenceProcessor(TextFileSegmenter segmenter, int batchSize, Action<SentenceJob<TContext>> action, Func<TContext> contextFunc) 
            : this(segmenter, batchSize, action, contextFunc, Environment.ProcessorCount)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="segmenter">Text file segmenter.</param>
        /// <param name="batchSize">Batch size for one thread.</param>
        /// <param name="action">Action to perform on a sentence. This can be null!</param>
        /// <param name="numTasks">Number of worker tasks to spawn.</param>
        public ParallelSentenceProcessor(TextFileSegmenter segmenter, int batchSize, Action<SentenceJob<TContext>> action, Func<TContext> contextFunc, int numTasks) 
            : base(
                new ConcurrentQueue<SentenceJob<TContext>>(), batchSize, action, 
                GetProducer(segmenter, contextFunc), GetCanProduce(segmenter), 
                numTasks)
        {
        }

        private static Func<SentenceJob<TContext>> GetProducer(TextFileSegmenter segmenter, Func<TContext> contextFunc)
        {
            return () =>
            {
                lock (segmenter)
                {
                    return new SentenceJob<TContext>(segmenter.ReadSentence(), contextFunc());
                }
            };
        }

        private static Func<bool> GetCanProduce(TextFileSegmenter segmenter) => () => segmenter.CanRead;
    }

    /// <summary>
    /// A job to process a sentence.
    /// </summary>
    /// <typeparam name="T">Job context type.</typeparam>
    public struct SentenceJob<T>
    {
        /// <summary>
        /// Sentence to process.
        /// </summary>
        public readonly string Sentence;

        /// <summary>
        /// Sentence context.
        /// </summary>
        public readonly T Context;

        /// <summary>
        /// Ctor.
        /// </summary>
        public SentenceJob(string sentence, T context)
        {
            Sentence = sentence;
            Context = context;
        }
    }
}