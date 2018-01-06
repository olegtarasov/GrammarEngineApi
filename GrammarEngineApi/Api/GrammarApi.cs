// .NET обертка для вызова функций процедурного API грамматического словаря
// Для более удобной работы в ООП следует использовать сборку gren_fx2 и дополнительные классы в ней.
//
// Информация по API:
// http://www.solarix.ru/api/ru/list.shtml
// http://www.solarix.ru/for_developers/api/grammar-engine-api.shtml
// http://www.solarix.ru/for_developers/api/dotnet_assembly.shtml
//
// CHANGELOG:
// 17.07.2008 - перевод вызовов WinDLL API на stdcall и соответствующий переход здесь на CallingConvention.StdCall
// 25.07.2008 - добавлены sol_FindClass, sol_FindEnum, sol_FindEnumState, sol_EnumInClass
// 27.07.2008 - добавлена sol_SeekNGrams
// 01.08.2008 - все возвращаемые процедурами DLL значения с bool заменены на int
// 04.04.2008 - изменен набор управляющих констант для API генератора текста
// 15.11.2008 - добавлена sol_LoadKnowledgeBase, добавлен флаг FG_PEDANTIC_ANALYSIS
// 15.11.2008 - обертка переделана на работу с полной версией поискового движка
// 16.11.2008 - добавлены обертки для API поисковика
// 13.02.2009 - добавлены обертки API SentenceBroker'а
// 08.04.2009 - из API удалена sol_PreloadCaches
// 21.06.2009 - переделки в алгоритме и константах синонимизатора
// 20.07.2009 - переделан API доступа к базе N-грамм
// 22.12.2009 - из сборки враппера убран инферфейс поискового движка и синонимизатора
// 02.08.2010 - добавлена sol_SaveDictionary
// 06.08.2010 - добавлены обертки sol_GetPhraseLanguage, sol_GetPhraseClass и sol_AddWord
// 19.08.2010 - добавлены обертки sol_LinksInfoFlagsTxt, sol_SetLinkFlags
// 04.09.2010 - добавлены обертки sol_GetError, sol_ClearError
// 19.06.2011 - добавлены sol_GetTranslationLog и sol_GetTranslationLogFx для вывода отладочной
//              трассировки в переводчике.
// 02.10.2011 - изменения в сигнатуре sol_MorphologyAnalysis и sol_SyntaxAnalysis
// 22.10.2011 - переделки в API sol_Tokenize -> sol_TokenizeW
// 13.11.2011 - добавлены обертки sol_GetCoordNameFX и sol_GetCoordStateNameFX
// 09.01.2012 - изменения в сигнатуре функции sol_Syllabs
// 07.01.2013 - добавлен флаг SOL_GREN_MODEL для включения вероятностной модели
// 12.09.2017 - портирование на .NET Core под Linux
//
// CD->30.01.2008
// LC->12.09.2017
// --------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using GrammarEngineApi.Properties;
using log4net;

namespace GrammarEngineApi.Api
{
    /// <summary>
    /// Low-level API for Grammar Engine and Lemmatizer.
    /// </summary>
    public sealed partial class GrammarApi
    {
        private const string GrenDllName = "solarix_grammar_engine.dll";
        private const string LemDllName = "lemmatizator.dll";

        private static readonly ILog _log = LogManager.GetLogger(typeof(GrammarEngine));
    }
}