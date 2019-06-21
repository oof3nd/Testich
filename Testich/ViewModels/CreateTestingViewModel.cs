using System.Collections.Generic;
using Testich.Models;

namespace Testich.ViewModels
{
    public class CreateTestingViewModel
    {
        public int TestId { get; set; }
        public string TestDescription { get; set; }
        public int Time { get; set; }
        public int LevelId { get; set; }
        public string LevelHelp { get; set; }
        public List<LevelOfTest> LevelsOfTests { get; set; }
        public int QuestionId { get; set; }
        public int NumberClosedQuestion { get; set; }
        public string ContentClosedQuestion { get; set; }
        public List<ClosedQuestionOption> ClosedQuestionOptions { get; set; }

        public CreateTestingViewModel()
        {
            LevelsOfTests = new List<LevelOfTest>();
            ClosedQuestionOptions = new List<ClosedQuestionOption>();
        }
    }

}
