using System.ComponentModel.DataAnnotations;
using Testich.Models;
using System.Collections.Generic;


namespace Testich.ViewModels
{
    public class CreateQOTViewModel
    {
        public int TestId { get; set; }
        public int LevelId { get; set; }
        public int TypeOfQuestionId { get; set; }
        [Required]
        [Display(Name = "Тип вопроса")]
        public List<TypeOfQuestion> TypeOfQuestion { get; set; }
        [Required]
        [Display(Name = "Порядковый номер вопроса в уровне")]
        public int QuestionIndexNumber { get; set; }
    }
}
