using System.ComponentModel.DataAnnotations;

namespace Testich.ViewModels
{
    public class CreateCQViewModel
    {
        public int TestId { get; set; }
        public int LevelId { get; set; }
        public int QuestionOfTestId { get; set; }
        public int QuestionIndexNumber { get; set; }
        public int TypeOfQuestionId { get; set; }
        [Required]
        [Display(Name = "Один правильный ответ")]
        public bool OnlyOneRight { get; set; }
        [Required]
        [Display(Name = "Описание вопроса")]
        public string QuestionContent { get; set; }
        [Required]
        [Display(Name = "Номер правильного(ых) варианта(ов)")]
        public int CorrectOptionNumbers { get; set; }
    }
}
