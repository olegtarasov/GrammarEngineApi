using System.Collections.Concurrent;

namespace GrammarEngineApi
{
    /// <summary>
    /// Grammar engine pool designed to work in multithreaded scenarios.
    /// </summary>
    public class GrammarEnginePool
    {
        private readonly string _dictPath;
        private readonly ConcurrentQueue<GrammarEngine> _engines = new ConcurrentQueue<GrammarEngine>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="dictPath">Path to a dictionary.</param>
        public GrammarEnginePool(string dictPath)
        {
            _dictPath = dictPath;
        }

        /// <summary>
        /// Gets an instance of <see cref="GrammarEngine"/>. If there is
        /// no free object, creates a new one.
        /// </summary>
        /// <returns>Instance of GrammarEngine.</returns>
        public GrammarEngine GetInstance()
        {
            if (!_engines.TryDequeue(out var engine))
            {
                engine = new GrammarEngine(_dictPath);
            }

            return engine;
        }

        /// <summary>
        /// Returns an instance of GrammarEngine to the pool.
        /// </summary>
        /// <param name="engine">Engine to return.</param>
        public void ReturnInstance(GrammarEngine engine)
        {
            _engines.Enqueue(engine);
        }
    }
}