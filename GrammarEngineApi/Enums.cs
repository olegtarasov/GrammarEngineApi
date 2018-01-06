// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace GrammarEngineApi
{
    /// <summary>
    /// Supported languages.
    /// </summary>
    public enum Languages
    {
        RUSSIAN_LANGUAGE = 2, // language Russian
        ENGLISH_LANGUAGE = 3, // language English
        FRENCH_LANGUAGE = 4, // language French
        SPANISH_LANGUAGE = 5, // language Spanish
        CHINESE_LANGUAGE = 6, // language Chinese
        JAPANESE_LANGUAGE = 7, // language Japanese
        GERMAN_LANGUAGE = 8, // language German
        THESAURUS_LANGUAGE = 9, // language ThesaurusLanguage
    }

    /// <summary>
    /// Word classes.
    /// </summary>
    public enum WordClassesRu
    {
        ///<summary>
        /// Число
        ///</summary>
        NUM_WORD_CLASS = 2,

        ///<summary>
        /// СУЩЕСТВИТЕЛЬНОЕ
        ///</summary>
        NOUN_ru = 6,

        ///<summary>
        /// МЕСТОИМ_СУЩ
        ///</summary>
        PRONOUN2_ru = 7,

        ///<summary>
        /// МЕСТОИМЕНИЕ
        ///</summary>
        PRONOUN_ru = 8,

        ///<summary>
        /// ПРИЛАГАТЕЛЬНОЕ
        ///</summary>
        ADJ_ru = 9,

        ///<summary>
        /// ЧИСЛИТЕЛЬНОЕ
        ///</summary>
        NUMBER_CLASS_ru = 10,

        ///<summary>
        /// ИНФИНИТИВ
        ///</summary>
        INFINITIVE_ru = 11,

        ///<summary>
        /// ГЛАГОЛ
        ///</summary>
        VERB_ru = 12,

        ///<summary>
        /// ДЕЕПРИЧАСТИЕ
        ///</summary>
        GERUND_2_ru = 13,

        ///<summary>
        /// ПРЕДЛОГ
        ///</summary>
        PREPOS_ru = 14,

        ///<summary>
        /// БЕЗЛИЧ_ГЛАГОЛ
        ///</summary>
        IMPERSONAL_VERB_ru = 15,

        ///<summary>
        /// ЧАСТИЦА
        ///</summary>
        PARTICLE_ru = 18,

        ///<summary>
        /// СОЮЗ
        ///</summary>
        CONJ_ru = 19,

        ///<summary>
        /// НАРЕЧИЕ
        ///</summary>
        ADVERB_ru = 20,

        ///<summary>
        /// ПУНКТУАТОР
        ///</summary>
        PUNCTUATION_class = 21,

        ///<summary>
        /// ПОСЛЕЛОГ
        ///</summary>
        POSTPOS_ru = 26,

        ///<summary>
        /// ПРИТЯЖ_ЧАСТИЦА
        ///</summary>
        POSESS_PARTICLE = 27,

        ///<summary>
        /// ЕДИНИЦА_ИЗМЕРЕНИЯ
        ///</summary>
        MEASURE_UNIT = 28,

        ///<summary>
        /// ПРЕФИКС_СОСТАВПРИЛ
        ///</summary>
        COMPOUND_ADJ_PREFIX = 29,

        ///<summary>
        /// ПРЕФИКС_СОСТАВ_СУЩ
        ///</summary>
        COMPOUND_NOUN_PREFIX = 30,

        ///<summary>
        /// UnknownEntries
        ///</summary>
        UNKNOWN_ENTRIES_CLASS = 85
    }

    public enum Enums
    {
        ///<summary>
        /// CharCasing
        ///</summary>
        CharCasing = 4,

        ///<summary>
        /// PERSON_ru
        ///</summary>
        PERSON_ru = 27,

        ///<summary>
        /// ЧИСЛО
        ///</summary>
        NUMBER_ru = 28,

        ///<summary>
        /// РОД
        ///</summary>
        GENDER_ru = 29,

        ///<summary>
        /// ПЕРЕХОДНОСТЬ
        ///</summary>
        TRANSITIVENESS_ru = 30,

        ///<summary>
        /// ПРИЧАСТИЕ
        ///</summary>
        PARTICIPLE_ru = 31,

        ///<summary>
        /// СТРАД
        ///</summary>
        PASSIVE_PARTICIPLE_ru = 32,

        ///<summary>
        /// ВИД
        ///</summary>
        ASPECT_ru = 33,

        ///<summary>
        /// НАКЛОНЕНИЕ
        ///</summary>
        VERB_FORM_ru = 35,

        ///<summary>
        /// ВРЕМЯ
        ///</summary>
        TENSE_ru = 36,

        ///<summary>
        /// КРАТКИЙ
        ///</summary>
        SHORTNESS_ru = 37,

        ///<summary>
        /// ПАДЕЖ
        ///</summary>
        CASE_ru = 39,

        ///<summary>
        /// ОДУШ
        ///</summary>
        FORM_ru = 40,

        ///<summary>
        /// ПЕРЕЧИСЛИМОСТЬ
        ///</summary>
        COUNTABILITY_ru = 41,

        ///<summary>
        /// СТЕПЕНЬ
        ///</summary>
        COMPAR_FORM_ru = 42,

        ///<summary>
        /// ПадежВал
        ///</summary>
        CASE_GERUND_ru = 43,

        ///<summary>
        /// МОДАЛЬНЫЙ
        ///</summary>
        MODAL_ru = 44,

        ///<summary>
        /// СГД_Время
        ///</summary>
        VERBMODE_TENSE = 45,

        ///<summary>
        /// СГД_Направление
        ///</summary>
        VERBMODE_DIRECTION = 46,

        ///<summary>
        /// КАТЕГОРИЯ_ЧИСЛ
        ///</summary>
        NUMERAL_CATEGORY = 47,

        ///<summary>
        /// ОБСТ_ВАЛ
        ///</summary>
        ADV_SEMANTICS = 48,

        ///<summary>
        /// ТИП_ПРИЛ
        ///</summary>
        ADJ_TYPE = 49,

        ///<summary>
        /// ТИП_МЕСТОИМЕНИЯ
        ///</summary>
        PRONOUN_TYPE_ru = 51,

        ///<summary>
        /// ТИП_ГЛАГОЛА
        ///</summary>
        VERB_TYPE_ru = 52,

        ///<summary>
        /// ТИП_ЧАСТИЦЫ
        ///</summary>
        PARTICLE_TYPE = 53,

        ///<summary>
        /// ТИП_МОДИФ
        ///</summary>
        ADV_MODIF_TYPE = 54

    }

    public enum CharCasing
    {
        ///<summary>
        /// CharCasing
        ///</summary>
        DECAPITALIZED_CASED = 0,

        ///<summary>
        /// CharCasing
        ///</summary>
        FIRST_CAPITALIZED_CASED = 1,

        ///<summary>
        /// CharCasing
        ///</summary>
        ALL_CAPITALIZED_CASED = 2,

        ///<summary>
        /// CharCasing
        ///</summary>
        EACH_LEXEM_CAPITALIZED_CASED = 3,

    }

    public enum PersonRu
    {
        ///<summary>
        /// ЛИЦО 1
        ///</summary>
        PERSON_1_ru = 0,

        ///<summary>
        /// ЛИЦО 2
        ///</summary>
        PERSON_2_ru = 1,

        ///<summary>
        /// ЛИЦО 3
        ///</summary>
        PERSON_3_ru = 2,

    }

    public enum NumberRu
    {
        ///<summary>
        /// ЧИСЛО : ЕД
        ///</summary>
        SINGULAR_NUMBER_ru = 0,

        ///<summary>
        /// ЧИСЛО : МН
        ///</summary>
        PLURAL_NUMBER_ru = 1,

    }

    public enum GenderRu
    {
        ///<summary>
        /// РОД : МУЖ
        ///</summary>
        MASCULINE_GENDER_ru = 0,

        ///<summary>
        /// РОД : ЖЕН
        ///</summary>
        FEMININE_GENDER_ru = 1,

        ///<summary>
        /// РОД : СР
        ///</summary>
        NEUTRAL_GENDER_ru = 2,

    }

    public enum TransitivenesRu
    {
        ///<summary>
        /// ПЕРЕХОДНОСТЬ : НЕПЕРЕХОДНЫЙ
        ///</summary>
        NONTRANSITIVE_VERB_ru = 0,

        ///<summary>
        /// ПЕРЕХОДНОСТЬ : ПЕРЕХОДНЫЙ
        ///</summary>
        TRANSITIVE_VERB_ru = 1,

    }

    public enum AspectRu
    {
        ///<summary>
        /// ВИД : СОВЕРШ
        ///</summary>
        PERFECT_ru = 0,

        ///<summary>
        /// ВИД : НЕСОВЕРШ
        ///</summary>
        IMPERFECT_ru = 1,

    }

    public enum VerbFormRu
    {
        ///<summary>
        /// НАКЛОНЕНИЕ : ИЗЪЯВ
        ///</summary>
        VB_INF_ru = 0,

        ///<summary>
        /// НАКЛОНЕНИЕ : ПОБУД
        ///</summary>
        VB_ORDER_ru = 1,

    }

    public enum TenseRu
    {
        ///<summary>
        /// ВРЕМЯ : ПРОШЕДШЕЕ
        ///</summary>
        PAST_ru = 0,

        ///<summary>
        /// ВРЕМЯ : НАСТОЯЩЕЕ
        ///</summary>
        PRESENT_ru = 1,

        ///<summary>
        /// ВРЕМЯ : БУДУЩЕЕ
        ///</summary>
        FUTURE_ru = 2,

    }

    public enum CaseRu
    {
        ///<summary>
        /// ПАДЕЖ : ИМ
        ///</summary>
        NOMINATIVE_CASE_ru = 0,

        ///<summary>
        /// ЗВАТ
        ///</summary>
        VOCATIVE_CASE_ru = 1,

        ///<summary>
        /// ПАДЕЖ : РОД
        ///</summary>
        GENITIVE_CASE_ru = 2,

        ///<summary>
        /// ПАРТ
        ///</summary>
        PARTITIVE_CASE_ru = 3,

        ///<summary>
        /// ПАДЕЖ : ТВОР
        ///</summary>
        INSTRUMENTAL_CASE_ru = 5,

        ///<summary>
        /// ПАДЕЖ : ВИН
        ///</summary>
        ACCUSATIVE_CASE_ru = 6,

        ///<summary>
        /// ПАДЕЖ : ДАТ
        ///</summary>
        DATIVE_CASE_ru = 7,

        ///<summary>
        /// ПАДЕЖ : ПРЕДЛ
        ///</summary>
        PREPOSITIVE_CASE_ru = 8,

        ///<summary>
        /// МЕСТ
        ///</summary>
        LOCATIVE_CASE_ru = 10,

    }

    public enum FormRu
    {
        ///<summary>
        /// ОДУШ : ОДУШ
        ///</summary>
        ANIMATIVE_FORM_ru = 0,

        ///<summary>
        /// ОДУШ : НЕОДУШ
        ///</summary>
        INANIMATIVE_FORM_ru = 1,

    }

    public enum CountabilityRu
    {
        ///<summary>
        /// ПЕРЕЧИСЛИМОСТЬ : ДА
        ///</summary>
        COUNTABLE_ru = 0,

        ///<summary>
        /// ПЕРЕЧИСЛИМОСТЬ : НЕТ
        ///</summary>
        UNCOUNTABLE_ru = 1,

    }

    public enum ComparativeRu
    {
        ///<summary>
        /// СТЕПЕНЬ : АТРИБ
        ///</summary>
        ATTRIBUTIVE_FORM_ru = 0,

        ///<summary>
        /// СТЕПЕНЬ : СРАВН
        ///</summary>
        COMPARATIVE_FORM_ru = 1,

        ///<summary>
        /// СТЕПЕНЬ : ПРЕВОСХ
        ///</summary>
        SUPERLATIVE_FORM_ru = 2,

        ///<summary>
        /// СТЕПЕНЬ : КОМПАРАТИВ2
        ///</summary>
        LIGHT_COMPAR_FORM_RU = 3,

        ///<summary>
        /// СТЕПЕНЬ : СОКРАЩ
        ///</summary>
        ABBREV_FORM_ru = 4,

    }

    public enum NumeralCategoriesRu
    {
        ///<summary>
        /// КАТЕГОРИЯ_ЧИСЛ : КОЛИЧ
        ///</summary>
        CARDINAL = 0,

        ///<summary>
        /// КАТЕГОРИЯ_ЧИСЛ : СОБИР
        ///</summary>
        COLLECTION = 1,

    }

    public enum SemanticsRu
    {
        ///<summary>
        /// ОБСТ_ВАЛ : МЕСТО
        ///</summary>
        S_LOCATION = 0,

        ///<summary>
        /// ОБСТ_ВАЛ : НАПРАВЛЕНИЕ
        ///</summary>
        S_DIRECTION = 1,

        ///<summary>
        /// ОБСТ_ВАЛ : НАПРАВЛЕНИЕ_ОТКУДА
        ///</summary>
        S_DIRECTION_FROM = 2,

        ///<summary>
        /// ОБСТ_ВАЛ : КОНЕЧНАЯ_ТОЧКА
        ///</summary>
        S_FINAL_POINT = 3,

        ///<summary>
        /// ОБСТ_ВАЛ : РАССТОЯНИЕ
        ///</summary>
        S_DISTANCE = 4,

        ///<summary>
        /// ОБСТ_ВАЛ : СКОРОСТЬ
        ///</summary>
        S_VELOCITY = 5,

        ///<summary>
        /// ОБСТ_ВАЛ : МОМЕНТ_ВРЕМЕНИ
        ///</summary>
        S_MOMENT = 6,

        ///<summary>
        /// ОБСТ_ВАЛ : ДЛИТЕЛЬНОСТЬ
        ///</summary>
        S_DURATION = 7,

        ///<summary>
        /// ОБСТ_ВАЛ : КРАТНОСТЬ
        ///</summary>
        S_TIME_DIVISIBILITY = 8,

        ///<summary>
        /// ОБСТ_ВАЛ : СОПОСТАВЛЕНИЕ
        ///</summary>
        S_ANALOG = 9,

        ///<summary>
        /// ОБСТ_ВАЛ : МНОЖИТЕЛЬ
        ///</summary>
        S_FACTOR = 10,

    }

    public enum AdjectiveTypes
    {
        ///<summary>
        /// ТИП_ПРИЛ : ПРИТЯЖ
        ///</summary>
        ADJ_POSSESSIVE = 0,

        ///<summary>
        /// ТИП_ПРИЛ : ПОРЯДК
        ///</summary>
        ADJ_ORDINAL = 1,

    }

    public enum VerbStatesRu
    {
        ///<summary>
        /// ТИП_ГЛАГОЛА : СВЯЗОЧН
        ///</summary>
        COPULA_VERB_ru = 2

    }

    public enum ParticleTypesRu
    {
        ///<summary>
        /// ТИП_ЧАСТИЦЫ : ПРЕФИКС
        ///</summary>
        PREFIX_PARTICLE = 0,

        ///<summary>
        /// ТИП_ЧАСТИЦЫ : ПОСТФИКС
        ///</summary>
        POSTFIX_PARTICLE = 1,

    }

    public enum Links
    {
        OBJECT_link = 0,
        ATTRIBUTE_link = 1,
        CONDITION_link = 2,
        CONSEQUENCE_link = 3,
        SUBJECT_link = 4,
        RHEMA_link = 5,
        COVERB_link = 6,
        NUMBER2OBJ_link = 12,
        TO_VERB_link = 16,
        TO_INF_link = 17,
        TO_PERFECT = 18,
        TO_UNPERFECT = 19,
        TO_NOUN_link = 20,
        TO_ADJ_link = 21,
        TO_ADV_link = 22,
        TO_RETVERB = 23,
        TO_GERUND_2_link = 24,
        WOUT_RETVERB = 25,
        TO_ENGLISH_link = 26,
        TO_RUSSIAN_link = 27,
        TO_FRENCH_link = 28,
        SYNONYM_link = 29,
        SEX_SYNONYM_link = 30,
        CLASS_link = 31,
        MEMBER_link = 32,
        TO_SPANISH_link = 33,
        TO_GERMAN_link = 34,
        TO_CHINESE_link = 35,
        TO_POLAND_link = 36,
        TO_ITALIAN_link = 37,
        TO_PORTUGUAL_link = 38,
        ACTION_link = 39,
        ACTOR_link = 40,
        TOOL_link = 41,
        RESULT_link = 42,
        TO_JAPANESE_link = 43,
        TO_KANA_link = 44,
        TO_KANJI_link = 45,
        ANTONYM_link = 46,
        EXPLANATION_link = 47,
        WWW_link = 48,
        ACCENT_link = 49,
        YO_link = 50,
        TO_DIMINUITIVE_link = 51,
        TO_RUDE_link = 52,
        TO_BIGGER_link = 53,
        TO_NEUTRAL_link = 54,
        TO_SCOLARLY = 55,
        TO_SAMPLE_link = 56,
        USAGE_TAG_link = 57,
        PROPERTY_link = 58,
        TO_CYRIJI_link = 59,
        HABITANT_link = 60,
        CHILD_link = 61,
        PARENT_link = 62,
        UNIT_link = 63,
        SET_link = 64,
        TO_WEAKENED_link = 65,
        VERBMODE_BASIC_link = 66,
        NEGATION_PARTICLE_link = 67,
        NEXT_COLLOCATION_ITEM_link = 68,
        SUBORDINATE_CLAUSE_link = 69,
        RIGHT_GENITIVE_OBJECT_link = 70,
        ADV_PARTICIPLE_link = 71,
        POSTFIX_PARTICLE_link = 72,
        INFINITIVE_link = 73,
        NEXT_ADJECTIVE_link = 74,
        NEXT_NOUN_link = 75,
        THEMA_link = 76,
        RIGHT_AUX2INFINITIVE_link = 77,
        RIGHT_AUX2PARTICIPLE = 78,
        RIGHT_AUX2ADJ = 79,
        RIGHT_LOGIC_ITEM_link = 80,
        RIGHT_COMPARISON_Y_link = 81,
        RIGHT_NOUN_link = 82,
        RIGHT_NAME_link = 83,
        ADJ_PARTICIPLE_link = 84,
        PUNCTUATION_link = 85,
        IMPERATIVE_SUBJECT_link = 86,
        IMPERATIVE_VERB2AUX_link = 87,
        AUX2IMPERATIVE_VERB = 88,
        PREFIX_PARTICLE_link = 89,
        PREFIX_CONJUNCTION_link = 90,
        LOGICAL_CONJUNCTION_link = 91,
        NEXT_CLAUSE_link = 92,
        LEFT_AUX_VERB_link = 93,
        BEG_INTRO_link = 94,
        RIGHT_PREPOSITION_link = 95,
        WH_SUBJECT_link = 96,
        IMPERATIVE_PARTICLE_link = 97,
        GERUND_link = 98,
        PREPOS_ADJUNCT_link = 99,
        DIRECT_OBJ_INTENTION_link = 100,
        COPULA_link = 101,
        DETAILS_link = 102,
        SENTENCE_CLOSER_link = 103,
        OPINION_link = 104,
        APPEAL_link = 105,
        TERM_link = 106,
        SPEECH_link = 107,
        QUESTION_link = 108,
        POLITENESS_link = 109,
        SEPARATE_ATTR_link = 110,
        POSSESSION_POSTFIX_link = 111,
        COMPOUND_PREFIX_link = 112,
        UNKNOWN_SLOT_link = 113,
        SECOND_VERB_link = 114,
    }
}