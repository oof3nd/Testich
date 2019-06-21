using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Testich.Models
{
    public class Category
    {
        public int Id { get; set; } //Ключ Категории
        public string Name { get; set; } //Наименование категории
        public bool Confirmed { get; set; } //Категория подтверждена
        public List<Test> Tests { get; set; } //Тесты категории
    }

    public class ResultScale
    {
        public int Id { get; set; } //Ключ шкалы
        public string Name { get; set; } //Наименование шкалы
        public int ScaleDivisionsAmount { get; set; } //Количество возможных оценок
        public List<Test> Tests { get; set; } //Тесты шкалы
    }

    public class Tag
    {
        public int Id { get; set; } //Ключ тэга
        public string Name { get; set; } //Наименование тэга
        public List<TagTest> TagTests { get; set; }

        public Tag()
        {
            TagTests = new List<TagTest>();
        }
    }

    public class TagTest
    {
        public int? TestId { get; set; } //Ключ теста
        public Test Test { get; set; } //Тест 

        public int? TagId { get; set; } //Ключ тэга
        public Tag Tag { get; set; } //Тэг
    }

    public class Test
    {
        public int Id { get; set; } //Ключ теста        
        public User User { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ResultScaleId { get; set; }
        public ResultScale ResultScale { get; set; }
        public List<TagTest> TagTests { get; set; }
        public List<LevelOfTest> LevelOfTests { get; set; }
        public string Name { get; set; } //Наименование теста
        public string Description { get; set; } //Описание  теста
        public int TimeRestricting { get; set; } //Ограничение времени прохождения теста в минутах
        public int Rating { get; set; } //Рейтинг теста
        public DateTime CreatedDate { get; set; } //Дата создания
        public DateTime PublishedDate { get; set; } //Дата публикации
        public bool ReadyForPassing { get; set; } //Готовность теста для прохождения другими пользователями
        public bool ShowAnswers { get; set; } //Показывать ответы после прохождения
        public bool SinglePassing { get; set; } //Допускается пройти тест только один раз
        public bool OnlyRegisteredCanPass { get; set; } //Только зарегистрированные и авторизованные пользователи могут проходить тест

        public Test()
        {
            TagTests = new List<TagTest>();
        }
    }

    public class LevelOfTest
    {
        [JsonProperty("id")]
        public int Id { get; set; } //Ключ уровня
        public string Name { get; set; } //Наименование уровня
        [JsonIgnore]
        public int? TestId { get; set; }
        [JsonIgnore]
        public Test Test { get; set; }
        [JsonProperty("levelIndexNumber")]
        public int LevelIndexNumber { get; set; } //Порядковый номер уровня в тесте
        [JsonProperty("solution")]
        public string Solution { get; set; } //Подсказка уровня
        [JsonProperty("statusCorrect")]
        [NotMapped] public bool StatusCorrect { get; set; } = false; //Статус уровня
    }

    public class TypeOfQuestion
    {
        public int Id { get; set; } //Ключ вида вопроса
        public string Name { get; set; } //Наименование вида вопроса
    }

    public class QuestionOfTest
    {
        public int Id { get; set; } //Ключ вопроса в тесте
        public int? TestId { get; set; }
        public Test Test { get; set; } //Тест, к которому относится вопрос
        public int QuestionIndexNumber { get; set; } //Порядковый номер вопроса в тесте
        public int? LevelOfQuestionId { get; set; }
        public LevelOfTest LevelOfQuestion { get; set; } //Порядковый номер уровень в вопроса
        public int? TypeOfQuestionId { get; set; }
        public TypeOfQuestion TypeOfQuestion { get; set; } //Вид вопроса
    }

    public class ClosedQuestion
    {
        public int Id { get; set; } //Ключ закрытого вопроса
        [JsonIgnore]
        public int? QuestionOfTestId { get; set; } //Нумерованный элемент (пункт) списка вопросов
        [JsonIgnore]
        public QuestionOfTest QuestionOfTest { get; set; } //Нумерованный элемент (пункт) списка вопросов
        [JsonIgnore]
        public bool OnlyOneRight { get; set; } //Только один вариант ответа — правильный
        public string QuestionContent { get; set; } //Содержимое (контент) вопроса
        [JsonIgnore]
        public int CorrectOptionNumbers { get; set; } //Правильны(й/е) вариант/варианты (ИЗМЕНИТЬ НА ОДИН_КО_МНОГИМ)!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    public class ClosedQuestionOption
    {
        [JsonProperty("id")]
        public int Id { get; set; } //Ключ закрытого ответа на вопрос
        [JsonIgnore]
        public int? ClosedQuestionId { get; set; }
        [JsonIgnore]
        public ClosedQuestion ClosedQuestion { get; set; } //Вопрос закрытого типа, которому принадлежит данный вариант ответа
        [JsonProperty("content")]
        public string Content { get; set; } //Только один вариант ответа — правильный
        [JsonProperty("optionNumber")]
        public int OptionNumber { get; set; } //Порядковый номер варианта ответа на вопрос закр. типа
    }

    public class Result
    {
        [JsonProperty("id")]
        public int Id { get; set; } //Ключ результата на тест
        [JsonIgnore]
        public User User { get; set; } //Пользователь, к которому относится результат прохождения
        [JsonIgnore]
        public int? TestId { get; set; }
        [JsonIgnore]
        public Test Test { get; set; } //Тест, к которому относится вопрос
        [JsonProperty("gradeBasedOnScale")]
        public int GradeBasedOnScale { get; set; } //Оценка по шкале
        [JsonProperty("gassingDate")]
        public DateTime PassingDate { get; set; } //Дата прохождения теста
        [JsonProperty("correctAnswersPercentage")]
        public int CorrectAnswersPercentage { get; set; } //Процент правильных ответов в целочисленном формате
    }

}