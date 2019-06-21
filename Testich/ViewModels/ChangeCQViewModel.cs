using System.ComponentModel.DataAnnotations;

namespace Testich.ViewModels
{
    public class ChangeCQViewModel
    {
        public int TestId { get; set; }
        public int QuestionOfTestId { get; set; }
        public int ClosedQuestionId { get; set; }
        [Required]
        [Display(Name = "Порядковый номер в уровне")]
        public int QuestionIndexNumber { get; set; }
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
