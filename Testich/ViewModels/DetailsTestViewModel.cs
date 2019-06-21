using System.ComponentModel.DataAnnotations;
using Testich.Models;
using System.Collections.Generic;

namespace Testich.ViewModels
{
    public class DetailsTestViewModel
    {

        public Test Test { get; set; }
        public List<LevelOfTest> LevelOfTests { get; set; }
        public List<QuestionOfTest> QuestionOfTests { get; set; }
        public List<ClosedQuestion> ClosedQuestions { get; set; }
        public List<ClosedQuestionOption> ClosedQuestionOptions { get; set; }

    }
}
