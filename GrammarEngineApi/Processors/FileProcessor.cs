using System;
using System.IO;
using log4net;

namespace GrammarEngineApi.Processors
{
    public class FileProcessor<TFileContext, TSentenceContext>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FileProcessor<,>));

        private readonly string[] _files;
        private readonly GrammarEnginePool _enginePool;
        private readonly int _batchSize;
        private readonly Action<SentenceJob<TSentenceContext>, TFileContext> _action;
        private readonly int _numTasks;
        private readonly Func<string, TFileContext> _fileContextFactory;
        private readonly Func<TFileContext, TSentenceContext> _sentenceContextFactory;
        private readonly Action<string, TFileContext> _fileFinalizer;

        public FileProcessor(string path, 
                             GrammarEnginePool enginePool, 
                             int batchSize, 
                             Func<string, TFileContext> fileContextFactory, 
                             Func<TFileContext, TSentenceContext> sentenceContextFactory,
                             Action<SentenceJob<TSentenceContext>, TFileContext> action, 
                             Action<string, TFileContext> fileFinalizer)
            : this(path, 
                enginePool, 
                batchSize, 
                fileContextFactory, 
                sentenceContextFactory, 
                action, 
                fileFinalizer, 
                Environment.ProcessorCount)
        {
        }

        public FileProcessor(string path, 
                             GrammarEnginePool enginePool, 
                             int batchSize, 
                             Func<string, TFileContext> fileContextFactory, 
                             Func<TFileContext, TSentenceContext> sentenceContextFactory,
                             Action<SentenceJob<TSentenceContext>, TFileContext> action, 
                             Action<string, TFileContext> fileFinalizer, 
                             int numTasks)
        {
            _enginePool = enginePool;
            _batchSize = batchSize;
            _action = action;
            _numTasks = numTasks;
            _fileFinalizer = fileFinalizer;
            _fileContextFactory = fileContextFactory;
            _sentenceContextFactory = sentenceContextFactory;
            _log.Info($"Path to process is: {path}");
            if (File.Exists(path))
            {
                _log.Info("This is a single file.");
                _files = new[] {path};
                return;
            }

            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                throw new InvalidOperationException("Specified path is not a file or directory!");
            }

            string pattern = Path.GetFileName(path);
            _files = Directory.GetFiles(dir, string.IsNullOrEmpty(pattern) ? "*" : pattern);

            _log.Info($"Discovered {_files.Length} files.");
        }

        public void Process()
        {
            foreach (var file in _files)
            {
                _log.Info($"Starting to process {file}");

                var context = _fileContextFactory(file);
                var eng = _enginePool.GetInstance();
                using (var segmenter = eng.CreateTextFileSegmenter(file, true, Languages.RUSSIAN_LANGUAGE))
                {
                    _enginePool.ReturnInstance(eng);

                    var processor = new ParallelSentenceProcessor<TSentenceContext>(segmenter, _batchSize, job => _action(job, context), () => _sentenceContextFactory(context), _numTasks);
                    processor.Process();
                }

                _fileFinalizer?.Invoke(file, context);
                _log.Info($"Finished processing {file}");
            }
        }
    }
}