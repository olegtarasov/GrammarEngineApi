using System;

namespace GrammarEngineApi
{
    /// <summary>
    ///     Битовые флаги для выбора различных режимой работы движка.
    ///     Указываются при создании экземпляра движка в
    ///     <see cref="sol_CreateGrammarEngineExW">sol_CreateGrammarEngineExW</see>.
    /// </summary>
    [Flags]
    public enum EngineInstanceFlags
    {
        /// <summary>
        ///     Режим инициализации экземпляра по умолчанию - весь словарь грузится в память сразу.
        /// </summary>
        SOL_GREN_DEFAULT = 0,

        /// <summary>
        ///     Флаг указывается при создании экземпляра движка в памяти и меняет
        ///     стандартную тактику загрузки данных: вместо полной загрузки всего словаря
        ///     в память при подключении словаря движок будет подгружать словарные
        ///     статьи с диска по мере необходимости.
        ///     Флаг оказывает влияние только при использовании двоичного формата словаря.
        ///     Для SQL словаря информация всегда подгружается по мере необходимости.
        ///     Dictionary instance control flag.
        ///     See http://www.solarix.ru/api/ru/sol_LoadDictionary.shtml for details
        /// </summary>
        SOL_GREN_LAZY_LEXICON = 0x00000001
    }
}