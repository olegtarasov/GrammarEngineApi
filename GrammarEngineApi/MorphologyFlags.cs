using System;

namespace GrammarEngineApi
{
    /// <summary>
    ///     Control flags for sol_MorphologyAnalysis and sol_SyntaxAnalysis.
    ///     See http://www.solarix.ru/api/ru/sol_MorphologyAnalysis.shtml for details
    /// </summary>
    [Flags]
    public enum MorphologyFlags
    {
        /// <summary>
        ///     Режим анализа по умолчанию.
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        ///     Разрешить нечеткий поиск в лексиконе для несловарных токенов
        /// </summary>
        SOL_GREN_ALLOW_FUZZY = 0x00000002,

        /// <summary>
        ///     Разрешать только полный анализ предложения, а при невозможности включить все токены
        ///     в синтаксическое дерево - отказываться от анализа.
        /// </summary>
        SOL_GREN_COMPLETE_ONLY = 0x00000004,

        /// <summary>
        ///     Использовать символы с кодом \x1F для определения границ слов.
        ///     Таким образом, прикладной код может выполнять токенизацию своими силами, а
        ///     штатные правила движка полностью отключаются.
        /// </summary>
        SOL_GREN_PRETOKENIZED = 0x00000008,

        /// <summary>
        ///     Выполнять только токенизацию исходного предложения.
        /// </summary>
        SOL_GREN_TOKENIZE_ONLY = 0x00000010,

        /// <summary>
        ///     Отладочный флаг: отключать некоторые правила фильтрации вариантом морфоанализа слов
        /// </summary>
        SOL_GREN_DISABLE_FILTERS = 0x00000020,

        /// <summary>
        ///     Разрешено использовать вероятностную модель морфологии языка для снижения числа перебираемых вариантов связывания.
        ///     Использование этого флага требует подключения модели в файле конфигурации словаря (обычно dictionary.xml) и наличия
        ///     файлов данных для подключенной модели.
        /// </summary>
        SOL_GREN_MODEL = 0x00000800,

        /// <summary>
        ///     Выполнять частеречную разметку в sol_MorphologyAnalysis только с помощью вероятностной модели, а правила парсинга
        ///     не использовать.
        ///     Благодаря этому разметка выполняется существенно быстрее, хотя количество ошибок увеличивается.
        /// </summary>
        SOL_GREN_MODEL_ONLY = 0x00002000,

        SOL_GREN_REORDER_TREE = 0x00000400
    }
}