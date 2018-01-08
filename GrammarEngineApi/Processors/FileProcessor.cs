using System;
using System.IO;
using log4net;

namespace GrammarEngineApi.Processors
{
    public class FileProcessor<T>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FileProcessor<>));

        private readonly string[] _files;
        private readonly GrammarEnginePool _enginePool;
        private readonly int _batchSize;
        private readonly Action<string, T> _action;
        private readonly int _numTasks;
        private readonly Func<T> _fileContextFactory;
        private readonly Action<string, T> _fileFinalizer;

        public FileProcessor(string path, GrammarEnginePool enginePool, int batchSize, Func<T> fileContextFactory, Action<string, T> action, Action<string, T> fileFinalizer)
            : this(path, enginePool, batchSize, fileContextFactory, action, fileFinalizer, Environment.ProcessorCount)
        {
        }

        public FileProcessor(string path, GrammarEnginePool enginePool, int batchSize, Func<T> fileContextFactory, Action<string, T> action, Action<string, T> fileFinalizer, int numTasks)
        {
            _enginePool = enginePool;
            _batchSize = batchSize;
            _action = action;
            _numTasks = numTasks;
            _fileFinalizer = fileFinalizer;
            _fileContextFactory = fileContextFactory;
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

                var context = _fileContextFactory();
                var eng = _enginePool.GetInstance();
                using (var segmenter = eng.CreateTextFileSegmenter(file, true, Languages.RUSSIAN_LANGUAGE))
                {
                    _enginePool.ReturnInstance(eng);

                    var processor = new ParallelSentenceProcessor(segmenter, _batchSize, sentence => _action(sentence, context), _numTasks);
                    processor.Process();
                }

                _fileFinalizer?.Invoke(file, context);
                _log.Info($"Finished processing {file}");
            }
        }
    }
}