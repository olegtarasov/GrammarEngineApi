using System;

namespace GrammarEngineApi
{
    /// <summary>
    /// Syntax analysis flags.
    /// </summary>
    [Flags]
    public enum SyntaxFlags
    {
        DEFAULT = 0,

        /// <summary>
        ///     Сортировать ветки в синтаксическом дереве так, чтобы они при перечислении шли в порядке исходных
        ///     позиций слов-вершин. Флаг решает декоративную задачу, так как из-за технических особенностей процесса парсинга
        ///     ветки могут присоединяться к вершинам в любом порядке.
        /// </summary>
        SOL_GREN_REORDER_TREE = 0x00000400,

        SOL_GREN_FINDFACTS = 0x00001000
    }
}