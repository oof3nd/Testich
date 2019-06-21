using System.ComponentModel.DataAnnotations;

namespace Testich.ViewModels
{
    public class ChangeCQOViewModel
    {
        public int TestId { get; set; }
        public int ClosedQuestionOptionId { get; set; }
        [Required]
        [Display(Name = "Описание ответа")]
        public string Content { get; set; }
        [Required]
        [Display(Name = "Номер ответа в вопросе")]
        public int OptionNumber { get; set; }
    }
}
