using System.ComponentModel.DataAnnotations;
using Testich.Models;
using System.Collections.Generic;


namespace Testich.ViewModels
{
    public class ChangeTestViewModel
    {

        public string TestId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Категории")]
        public List<Category> Categories;

        public int ResultScaleId { get; set; }

        [Required]
        [Display(Name = "Шкала оценки")]
        public List<ResultScale> ResultScales;

        [Required(ErrorMessage = "Не указано наименование")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Длина строки должна быть от 5 до 200 символов")]
        [Display(Name = "Наименование теста")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указано описание")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Длина строки должна быть от 5 до 200 символов")]
        [Display(Name = "Описание теста")]
        public string Description { get; set; }

        [Required]
        [Range(1, 110, ErrorMessage = "Недопустое время")]
        [Display(Name = "Ограничение времени прохождения теста в минутах")]
        public int TimeRestricting { get; set; }

        [Required]
        [Display(Name = "Рейтинг теста")]
        public int Rating { get; set; }

        [Required]
        [Display(Name = "Готовность теста для прохождения другими пользователями")]
        public bool ReadyForPassing { get; set; }
        [Required]
        [Display(Name = "Показывать ответы после прохождения")]
        public bool ShowAnswers { get; set; }

        [Required]
        [Display(Name = "Допускается пройти тест только один раз")]
        public bool SinglePassing { get; set; }

        [Required]
        [Display(Name = "Только зарегистрированные и авторизованные пользователи могут проходить тест")]
        public bool OnlyRegisteredCanPass { get; set; }

        public ChangeTestViewModel()
        {
            Categories = new List<Category>();
            ResultScales = new List<ResultScale>();
        }
    }
}
