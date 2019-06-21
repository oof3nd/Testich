using System.ComponentModel.DataAnnotations;

namespace Testich.ViewModels
{
    public class CreateCQOViewModel
    {
        public int TestId { get; set; }
        public int LevelId { get; set; }
        public int ClosedQuestionId { get; set; }
        [Required]
        [Display(Name = "Описание ответа")]
        public string Content { get; set; }
        [Required]
        [Display(Name = "Номер ответа в вопросе")]
        public int OptionNumber { get; set; }
    }
}
