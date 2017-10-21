// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace GrammarEngineApi
{
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

    public enum WordClassesRu
    {
        NUM_WORD_CLASS = 2, // class num_word
        NOUN_ru = 6, // class СУЩЕСТВИТЕЛЬНОЕ
        PRONOUN2_ru = 7, // class МЕСТОИМ_СУЩ
        PRONOUN_ru = 8, // class МЕСТОИМЕНИЕ
        ADJ_ru = 9, // class ПРИЛАГАТЕЛЬНОЕ
        NUMBER_CLASS_ru = 10, // class ЧИСЛИТЕЛЬНОЕ
        INFINITIVE_ru = 11, // class ИНФИНИТИВ
        VERB_ru = 12, // class ГЛАГОЛ
        GERUND_2_ru = 13, // class ДЕЕПРИЧАСТИЕ
        PREPOS_ru = 14, // class ПРЕДЛОГ
        IMPERSONAL_VERB_ru = 15, // class БЕЗЛИЧ_ГЛАГОЛ
        PARTICLE_ru = 18, // class ЧАСТИЦА
        CONJ_ru = 19, // class СОЮЗ
        ADVERB_ru = 20, // class НАРЕЧИЕ
        PUNCTUATION_class = 21, // class ПУНКТУАТОР
        POSTPOS_ru = 26, // class ПОСЛЕЛОГ
        POSESS_PARTICLE = 27, // class ПРИТЯЖ_ЧАСТИЦА
        MEASURE_UNIT = 28, // class ЕДИНИЦА_ИЗМЕРЕНИЯ
        COMPOUND_ADJ_PREFIX = 29, // class ПРЕФИКС_СОСТАВ_ПРИЛ
        COMPOUND_NOUN_PREFIX = 30, // class ПРЕФИКС_СОСТАВ_СУЩ
        UNKNOWN_ENTRIES_CLASS = 85 // class UnknownEntries
    }

    public enum Enums
    {
        CharCasing = 4, // enum CharCasing
        PERSON_ru = 27, // enum PERSON_ru
        NUMBER_ru = 28, // enum ЧИСЛО
        GENDER_ru = 29, // enum РОД
        TRANSITIVENESS_ru = 30, // enum ПЕРЕХОДНОСТЬ
        PARTICIPLE_ru = 31, // enum ПРИЧАСТИЕ
        PASSIVE_PARTICIPLE_ru = 32, // enum СТРАД
        ASPECT_ru = 33, // enum ВИД
        VERB_FORM_ru = 35, // enum НАКЛОНЕНИЕ
        TENSE_ru = 36, // enum ВРЕМЯ
        SHORTNESS_ru = 37, // enum КРАТКИЙ
        CASE_ru = 39, // enum ПАДЕЖ
        FORM_ru = 40, // enum ОДУШ
        COUNTABILITY_ru = 41, // enum ПЕРЕЧИСЛИМОСТЬ
        COMPAR_FORM_ru = 42, // enum СТЕПЕНЬ
        CASE_GERUND_ru = 43, // enum ПадежВал
        MODAL_ru = 44, // enum МОДАЛЬНЫЙ
        VERBMODE_TENSE = 45, // enum СГД_Время
        VERBMODE_DIRECTION = 46, // enum СГД_Направление
        NUMERAL_CATEGORY = 47, // enum КАТЕГОРИЯ_ЧИСЛ
        ADV_SEMANTICS = 48, // enum ОБСТ_ВАЛ
        ADJ_TYPE = 49, // enum ТИП_ПРИЛ
        PRONOUN_TYPE_ru = 51, // enum ТИП_МЕСТОИМЕНИЯ
        VERB_TYPE_ru = 52, // enum ТИП_ГЛАГОЛА
        PARTICLE_TYPE = 53, // enum ТИП_ЧАСТИЦЫ
        ADV_MODIF_TYPE = 54 // enum ТИП_МОДИФ
    }

    public enum CharCasing
    {
        DECAPITALIZED_CASED = 0, // CharCasing : Lower
        FIRST_CAPITALIZED_CASED = 1, // CharCasing : FirstCapitalized
        ALL_CAPITALIZED_CASED = 2, // CharCasing : Upper
        EACH_LEXEM_CAPITALIZED_CASED = 3, // CharCasing : EachLexemCapitalized
    }

    public enum PersonRu
    {
        PERSON_1_ru = 0, // ЛИЦО : 1
        PERSON_2_ru = 1, // ЛИЦО : 2
        PERSON_3_ru = 2, // ЛИЦО : 3
    }

    public enum NumberRu
    {
        SINGULAR_NUMBER_ru = 0, // ЧИСЛО : ЕД
        PLURAL_NUMBER_ru = 1, // ЧИСЛО : МН
    }

    public enum GenderRu
    {
        MASCULINE_GENDER_ru = 0, // РОД : МУЖ
        FEMININE_GENDER_ru = 1, // РОД : ЖЕН
        NEUTRAL_GENDER_ru = 2, // РОД : СР
    }

    public enum TransitivenesRu
    {
        NONTRANSITIVE_VERB_ru = 0, // ПЕРЕХОДНОСТЬ : НЕПЕРЕХОДНЫЙ
        TRANSITIVE_VERB_ru = 1, // ПЕРЕХОДНОСТЬ : ПЕРЕХОДНЫЙ
    }

    public enum AspectRu
    {
        PERFECT_ru = 0, // ВИД : СОВЕРШ
        IMPERFECT_ru = 1, // ВИД : НЕСОВЕРШ
    }

    public enum VerbFormRu
    {
        VB_INF_ru = 0, // НАКЛОНЕНИЕ : ИЗЪЯВ
        VB_ORDER_ru = 1, // НАКЛОНЕНИЕ : ПОБУД
    }

    public enum TenseRu
    {
        PAST_ru = 0, // ВРЕМЯ : ПРОШЕДШЕЕ
        PRESENT_ru = 1, // ВРЕМЯ : НАСТОЯЩЕЕ
        FUTURE_ru = 2, // ВРЕМЯ : БУДУЩЕЕ
    }

    public enum CaseRu
    {
        NOMINATIVE_CASE_ru = 0, // ПАДЕЖ : ИМ
        VOCATIVE_CASE_ru = 1, // ЗВАТ
        GENITIVE_CASE_ru = 2, // ПАДЕЖ : РОД
        PARTITIVE_CASE_ru = 3, // ПАРТ
        INSTRUMENTAL_CASE_ru = 5, // ПАДЕЖ : ТВОР
        ACCUSATIVE_CASE_ru = 6, // ПАДЕЖ : ВИН
        DATIVE_CASE_ru = 7, // ПАДЕЖ : ДАТ
        PREPOSITIVE_CASE_ru = 8, // ПАДЕЖ : ПРЕДЛ
        LOCATIVE_CASE_ru = 10, // МЕСТ
    }

    public enum FormRu
    {
        ANIMATIVE_FORM_ru = 0, // ОДУШ : ОДУШ
        INANIMATIVE_FORM_ru = 1, // ОДУШ : НЕОДУШ
    }

    public enum CountabilityRu
    {
        COUNTABLE_ru = 0, // ПЕРЕЧИСЛИМОСТЬ : ДА
        UNCOUNTABLE_ru = 1, // ПЕРЕЧИСЛИМОСТЬ : НЕТ
    }

    public enum ComparativeRu
    {
        ATTRIBUTIVE_FORM_ru = 0, // СТЕПЕНЬ : АТРИБ
        COMPARATIVE_FORM_ru = 1, // СТЕПЕНЬ : СРАВН
        SUPERLATIVE_FORM_ru = 2, // СТЕПЕНЬ : ПРЕВОСХ
        LIGHT_COMPAR_FORM_RU = 3, // СТЕПЕНЬ : КОМПАРАТИВ2
        ABBREV_FORM_ru = 4, // СТЕПЕНЬ : СОКРАЩ
    }

    public enum NumeralCategoriesRu
    {
        CARDINAL = 0, // КАТЕГОРИЯ_ЧИСЛ : КОЛИЧ
        COLLECTION = 1, // КАТЕГОРИЯ_ЧИСЛ : СОБИР
    }

    public enum SemanticsRu
    {
        S_LOCATION = 0, // ОБСТ_ВАЛ : МЕСТО
        S_DIRECTION = 1, // ОБСТ_ВАЛ : НАПРАВЛЕНИЕ
        S_DIRECTION_FROM = 2, // ОБСТ_ВАЛ : НАПРАВЛЕНИЕ_ОТКУДА
        S_FINAL_POINT = 3, // ОБСТ_ВАЛ : КОНЕЧНАЯ_ТОЧКА
        S_DISTANCE = 4, // ОБСТ_ВАЛ : РАССТОЯНИЕ
        S_VELOCITY = 5, // ОБСТ_ВАЛ : СКОРОСТЬ
        S_MOMENT = 6, // ОБСТ_ВАЛ : МОМЕНТ_ВРЕМЕНИ
        S_DURATION = 7, // ОБСТ_ВАЛ : ДЛИТЕЛЬНОСТЬ
        S_TIME_DIVISIBILITY = 8, // ОБСТ_ВАЛ : КРАТНОСТЬ
        S_ANALOG = 9, // ОБСТ_ВАЛ : СОПОСТАВЛЕНИЕ
        S_FACTOR = 10, // ОБСТ_ВАЛ : МНОЖИТЕЛЬ
    }

    public enum AdjectiveTypes
    {
        ADJ_POSSESSIVE = 0, // ТИП_ПРИЛ : ПРИТЯЖ
        ADJ_ORDINAL = 1, // ТИП_ПРИЛ : ПОРЯДК
    }

    public enum VerbStatesRu
    {
        COPULA_VERB_ru = 2 // ТИП_ГЛАГОЛА : СВЯЗОЧН
    }

    public enum ParticleTypesRu
    {
        PREFIX_PARTICLE = 0, // ТИП_ЧАСТИЦЫ : ПРЕФИКС
        POSTFIX_PARTICLE = 1, // ТИП_ЧАСТИЦЫ : ПОСТФИКС
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