using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Testich.ViewModels;
using Testich.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CsvHelper;

namespace Testich.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class TestsController : Controller
    {
        private UserContext UserContext;

        private UserManager<User> _userManager;

        private List<Category> categories;

        private List<ResultScale> resultScales;
        public TestsController(UserManager<User> userManager, UserContext context)
        {
            UserContext = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(string searchString="", string searchStringCategory = "")
        {
            List<Test> tests;

            tests = await UserContext.Tests.ToListAsync();
            var cats = await UserContext.Сategories.ToListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                tests = await UserContext.Tests.Where(s => s.Name.Contains(searchString)).ToListAsync();
            }

            if (!string.IsNullOrEmpty(searchStringCategory))
            {
                var cat = await UserContext.Сategories.FirstOrDefaultAsync(s => s.Name.Contains(searchStringCategory));
                tests = await UserContext.Tests.Where(s =>  s.CategoryId == cat.Id).ToListAsync();
            }

            TestsViewModel model = new TestsViewModel
            {
                Tests = tests,
                Categories = cats,
            };

            return View(model);
        }



        public async Task<IActionResult> MyTests()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            return View(await UserContext.Tests.Where(p => p.User.Id == userId).ToListAsync());
        }

        public async Task<IActionResult> MyResults()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            return View(await UserContext.Results.Where(p => p.User.Id == userId).ToListAsync());
        }


        [HttpGet("[controller]/[action]/")]
        public async Task<IActionResult> Testing(int? id)
        {

            var t = await UserContext.Tests.FirstOrDefaultAsync(p => p.Id == id);

            var l = await UserContext.LevelsOfTest.Where(p => p.TestId == id).ToListAsync();

            var fl = await UserContext.LevelsOfTest.FirstOrDefaultAsync(p => p.TestId == id);

            var fq = await UserContext.QuestionOfTests.FirstOrDefaultAsync(p => p.LevelOfQuestionId == fl.Id);

            var fcq = await UserContext.ClosedQuestions.FirstOrDefaultAsync(p => p.QuestionOfTestId == fq.Id);

            var cqos = await UserContext.ClosedQuestionOptions.Where(p => p.ClosedQuestionId == fcq.Id).ToListAsync();

            CreateTestingViewModel model = new CreateTestingViewModel
            {
                TestId = t.Id,
                TestDescription = t.Description,
                Time = t.TimeRestricting,
                LevelId = fl.Id,
                LevelHelp = fl.Solution,
                LevelsOfTests = l,
                QuestionId = fq.Id,
                NumberClosedQuestion = fq.QuestionIndexNumber,
                ContentClosedQuestion = fcq.QuestionContent,
                ClosedQuestionOptions = cqos,
            };


            return View("./Testing/TestCheck", model);
        }

        public class Rqm
        {
            public int TestId { get; set; }
            public LevelOfTest Level { get; set; }
            public LevelOfTest LevelNext { get; set; }
            public int QuestionId { get; set; }
            public int QuestionIndexNumber { get; set; }
            public int SelectedOption { get; set; }
            public bool CorrectOption { get; set; }
            public int PersentOptions { get; set; }
            public int MaxLvls { get; set; }
            
        }

        public QuestionOfTest getRandomQuestion(List<QuestionOfTest> arraInts)
        {
            int rnd = new Random().Next(arraInts.Count);
            return arraInts[rnd];
        }

        [HttpPost("[controller]/[action]/")]
        public async Task<IActionResult> Testing([FromBody]Rqm rqm)
        {
            JsonResult jsonData = null;

            //Проверка, если меньше 2ух лвл, чтобы при некслвл прога не падала и тоже самое потестить с вопросами
            //Не отправлять тот же вопрос

            //Что пришло на проверку 
            var rqmCQ = await UserContext.ClosedQuestions.FirstOrDefaultAsync(p => p.QuestionOfTestId == rqm.QuestionId);


            if (rqmCQ.CorrectOptionNumbers == rqm.SelectedOption)
            {
                //correct

                var lastLvl = await UserContext.LevelsOfTest.LastOrDefaultAsync(p => p.TestId == rqm.TestId);

                var lqC = await UserContext.QuestionOfTests.Where(p => p.LevelOfQuestionId == rqm.LevelNext.Id).ToListAsync();
                var qofC = getRandomQuestion(lqC);
              
                var cqC = await UserContext.ClosedQuestions.FirstOrDefaultAsync(p => p.QuestionOfTestId == qofC.Id);

                var cqosC = await UserContext.ClosedQuestionOptions.Where(p => p.ClosedQuestionId == cqC.Id).ToListAsync();

                if (lastLvl.Id == rqm.Level.Id)
                {
                    jsonData = new JsonResult(new
                    {
                        LevelNext = rqm.LevelNext,
                        QuestionId = qofC.Id,
                        QuestionIndexNumber = qofC.QuestionIndexNumber,
                        QuestionContent = cqC.QuestionContent,
                        Cqos = cqosC,
                        CorrectOption = true,
                        ResultTest = true
                    });

                    if (rqm.PersentOptions == rqm.MaxLvls)
                    {
                        rqm.PersentOptions = 5;
                    }
                    else if (rqm.PersentOptions >= (rqm.MaxLvls/2))
                    {
                        rqm.PersentOptions = 4;
                    }
                    else
                    {
                        rqm.PersentOptions = 3;
                    }

                    Result result = new Result
                    {
                        User = await _userManager.GetUserAsync(HttpContext.User),
                        TestId = rqm.TestId,
                        PassingDate = System.DateTime.Now,
                        CorrectAnswersPercentage = rqm.PersentOptions, 
                        GradeBasedOnScale = 0
                    };

                    UserContext.Results.Add(result);
                    await UserContext.SaveChangesAsync();

                   
                    return jsonData;
                }

                jsonData = new JsonResult(new
                {
                    LevelNext = rqm.LevelNext,
                    QuestionId = qofC.Id,
                    QuestionIndexNumber = qofC.QuestionIndexNumber,
                    QuestionContent = cqC.QuestionContent,
                    Cqos = cqosC,
                    CorrectOption = true,
                });

            }
            else
            {
                //Wrong

                var lqW = await UserContext.QuestionOfTests.Where(p => p.LevelOfQuestionId == rqm.Level.Id).ToListAsync();
                var qofW = getRandomQuestion(lqW);
                var cqW = await UserContext.ClosedQuestions.FirstOrDefaultAsync(p => p.QuestionOfTestId == qofW.Id);
                var cqosW = await UserContext.ClosedQuestionOptions.Where(p => p.ClosedQuestionId == cqW.Id).ToListAsync();

                jsonData = new JsonResult(new
                {
                  //  LevelNext = rqm.LevelNext,
                    QuestionId = qofW.Id,
                    QuestionIndexNumber = qofW.QuestionIndexNumber,
                    QuestionContent = cqW.QuestionContent,
                    Cqos = cqosW,
                    CorrectOption = false
                });
            }

            return jsonData;
        }

        [HttpGet]
        public IActionResult UploadTest()
        {
            categories = UserContext.Сategories.ToList();
            resultScales = UserContext.ResultScales.ToList();

            UploadTestViewModel model = new UploadTestViewModel
            {

                Categories = categories,
                ResultScales = resultScales,
            };

            return View(model);
        }

        public class TestUpload
        {
            public string Name { get; set; }
            public string Des { get; set; }
            public int Time { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UploadTest(UploadTestViewModel uploadTestViewModel)
        {

            IEnumerable<TestUpload> tests;
            if(uploadTestViewModel.uploadedFile != null)
            { 
                using (var reader = new StreamReader(uploadTestViewModel.uploadedFile.OpenReadStream()))
                {
                    CsvReader csv = new CsvReader(reader);
                    csv.Configuration.HasHeaderRecord = false;
                    csv.Configuration.MissingFieldFound = null;
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.BadDataFound = null;
                    csv.Configuration.Delimiter = ";";
                    tests = csv.GetRecords<TestUpload>().ToList();
                    foreach (TestUpload item in tests)
                    {
                        Test test = new Test
                        {
                            User = await _userManager.GetUserAsync(HttpContext.User),
                            CategoryId = uploadTestViewModel.CategoryId,
                            ResultScaleId = uploadTestViewModel.ResultScaleId,
                            Name = item.Name,
                            Description = item.Des,
                            TimeRestricting = item.Time,
                            Rating = 0,
                            CreatedDate = System.DateTime.Now,
                            PublishedDate = System.DateTime.Now,
                            ReadyForPassing = false,
                            ShowAnswers = false,
                            SinglePassing = false,
                            OnlyRegisteredCanPass = false
                        };

                        UserContext.Tests.Add(test);
                    }
                }


                await UserContext.SaveChangesAsync();
            }

            return RedirectToAction("MyTests");
        }


        public IActionResult Create()
        {

            categories =  UserContext.Сategories.ToList();
            resultScales =  UserContext.ResultScales.ToList();

          
            CreateTestViewModel model = new CreateTestViewModel
            {
                Categories = categories,
                ResultScales = resultScales
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateTestViewModel createTestViewModel)
        {

            if (!string.IsNullOrEmpty(createTestViewModel.Name) && !string.IsNullOrEmpty(createTestViewModel.Description))
            {

                Test test = new Test
                {
                    User = await _userManager.GetUserAsync(HttpContext.User),
                    CategoryId = createTestViewModel.CategoryId,
                    ResultScaleId = createTestViewModel.ResultScaleId,
                    Name = createTestViewModel.Name,
                    Description = createTestViewModel.Description,
                    TimeRestricting = createTestViewModel.TimeRestricting,
                    Rating = 0,
                    CreatedDate = System.DateTime.Now,
                    PublishedDate = System.DateTime.Now,
                    ReadyForPassing = createTestViewModel.ReadyForPassing,
                    ShowAnswers = createTestViewModel.ShowAnswers,
                    SinglePassing = createTestViewModel.SinglePassing,
                    OnlyRegisteredCanPass = createTestViewModel.OnlyRegisteredCanPass
                };

                UserContext.Tests.Add(test);
                await UserContext.SaveChangesAsync();
                return RedirectToAction("MyTests");
            }

            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id != null)
            {
                var data = await UserContext.Tests.FirstOrDefaultAsync(p => p.Id == id);

                if (data == null)
                    return NotFound();

                var LOT = await UserContext.LevelsOfTest.Where(p => p.TestId == data.Id).OrderBy(p => p.LevelIndexNumber).ToListAsync();

                var QOT = await UserContext.QuestionOfTests.Where(p => p.Test.Id == data.Id).OrderBy(p => p.QuestionIndexNumber).ToListAsync();

                var CQ = await UserContext.ClosedQuestions.Where(p => QOT.Contains(p.QuestionOfTest)).ToListAsync();

                var CQO = await UserContext.ClosedQuestionOptions.Where(p => CQ.Contains(p.ClosedQuestion)).ToListAsync();

                DetailsTestViewModel model = new DetailsTestViewModel
                {
                    Test = data,
                    LevelOfTests = LOT,
                    QuestionOfTests = QOT,
                    ClosedQuestions = CQ,
                    ClosedQuestionOptions = CQO
                };

                return View(model);
            }
            return NotFound();

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var test = await UserContext.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            categories = UserContext.Сategories.ToList();
            resultScales = UserContext.ResultScales.ToList();


            ChangeTestViewModel model = new ChangeTestViewModel
            {
                TestId = test.Id.ToString(),
                CategoryId = test.CategoryId,
                Categories = categories,
                ResultScaleId = test.ResultScaleId,
                ResultScales = resultScales,
                Name = test.Name,
                Description = test.Description,
                TimeRestricting = test.TimeRestricting,
                ReadyForPassing = test.ReadyForPassing,
                ShowAnswers = test.ShowAnswers,
                SinglePassing = test.SinglePassing,
                OnlyRegisteredCanPass = test.OnlyRegisteredCanPass

            };

          
            if (user.Equals(test.User))
                return View(model);
            else
                return NotFound();
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditTest(int? id, ChangeTestViewModel changeTestViewModel)
        {
            var test = await UserContext.Tests.FindAsync(id);

            if (id != test.Id)
            {

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    test.CategoryId = changeTestViewModel.CategoryId;
                    test.ResultScaleId = changeTestViewModel.ResultScaleId;
                    test.Name = changeTestViewModel.Name;
                    test.Description = changeTestViewModel.Description;
                    test.TimeRestricting = changeTestViewModel.TimeRestricting;
                    test.ReadyForPassing = changeTestViewModel.ReadyForPassing;
                    test.ShowAnswers = changeTestViewModel.ShowAnswers;
                    test.SinglePassing = changeTestViewModel.SinglePassing;
                    test.OnlyRegisteredCanPass = changeTestViewModel.OnlyRegisteredCanPass;

                    UserContext.Update(test);
                    await UserContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.Id))
                    {

                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(test);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await UserContext.Tests.SingleOrDefaultAsync(m => m.Id == id);
            var levels = await UserContext.LevelsOfTest.Where(p => p.TestId == test.Id).ToListAsync();

            if (test != null)
            {
                  foreach (var level in levels)
                  {
                      var qots = await UserContext.QuestionOfTests.Where(x => x.LevelOfQuestionId == level.Id).ToListAsync();
                        foreach (var qot in qots)
                        {
                            var cqs = await UserContext.ClosedQuestions.Where(x => x.QuestionOfTestId == qot.Id).ToListAsync();
                            foreach (var cq in cqs)
                            {
                                var cqos = await UserContext.ClosedQuestionOptions.Where(x => x.ClosedQuestionId == cq.Id).ToListAsync();

                                UserContext.ClosedQuestionOptions.RemoveRange(cqos);
                            }
                            UserContext.ClosedQuestions.RemoveRange(cqs);
                        }
                      UserContext.QuestionOfTests.RemoveRange(qots);
                  }
                UserContext.LevelsOfTest.RemoveRange(levels);
                UserContext.Tests.Remove(test);
                await UserContext.SaveChangesAsync();
            }
            return RedirectToAction("MyTests");
        }

        //Level

        [HttpGet("[controller]/Details/[action]/{id}")]
        public async Task<IActionResult> CreateLevel(int? id)
        {
         
            var test = await UserContext.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.Equals(test.User))
                return View("./Level/CreateLevel");
            else
                return NotFound();

        }

        [HttpPost("[controller]/Details/[action]/{id}")]
        public async Task<IActionResult> CreateLevel(int? id, LevelOfTest levelOfTest)
        {
            var test = await UserContext.Tests.FirstOrDefaultAsync(p => p.Id == id);

            if (!TestExists(test.Id))
            {
                return NotFound();
            }

            var lot = new LevelOfTest
            {
                Name = levelOfTest.Name,
                TestId = test.Id,
                LevelIndexNumber = levelOfTest.LevelIndexNumber,
                Solution = levelOfTest.Solution,
            };

            UserContext.LevelsOfTest.Add(lot);
            await UserContext.SaveChangesAsync();

            return RedirectToRoute("default", new { controller = "Tests", action = "Details",  id = test.Id });

        }

        [HttpGet("[controller]/Details/{idTest}/[action]/{idLevel}/")]
        public async Task<IActionResult> EditLevel(int idTest, int idLevel)
        {

            var test = await UserContext.Tests.FindAsync(idTest);

            var level = await UserContext.LevelsOfTest.FindAsync(idLevel);

            if (test == null || level == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
          
              if (user.Equals(test.User))
                return View("./Level/EditLevel", level);
               else
                return NotFound();

        }

        [HttpPost("[controller]/Details/{idTest}/[action]/{idLevel}/")]
        public async Task<IActionResult> EditLevel(int idTest, int idLevel, LevelOfTest levelOfTest)
        {

            var level = await UserContext.LevelsOfTest.FirstOrDefaultAsync(p => p.Id == idLevel); 

            if (ModelState.IsValid)
            {
                try
                {
                    level.Name = levelOfTest.Name;
                    level.LevelIndexNumber = levelOfTest.LevelIndexNumber;
                    level.Solution = levelOfTest.Solution;
                    level.TestId = idTest;
                    UserContext.LevelsOfTest.Update(level);
                    await UserContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                    throw;
                }
                return RedirectToRoute("default", new { controller = "Tests", action = "Details", id = idTest });
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedLevel(int id)
        {
            var level = await UserContext.LevelsOfTest.SingleOrDefaultAsync(m => m.Id == id);

            if (level != null)
            {
                var qots = await UserContext.QuestionOfTests.Where(x => x.LevelOfQuestionId == level.Id).ToListAsync();
                foreach (var qot in qots)
                {
                    var cqs = await UserContext.ClosedQuestions.Where(x => x.QuestionOfTestId == qot.Id).ToListAsync();
                    foreach (var cq in cqs)
                    {
                        var cqos = await UserContext.ClosedQuestionOptions.Where(x => x.ClosedQuestionId == cq.Id).ToListAsync();

                        UserContext.ClosedQuestionOptions.RemoveRange(cqos);
                    }
                    UserContext.ClosedQuestions.RemoveRange(cqs);
                }

                UserContext.QuestionOfTests.RemoveRange(qots);

                UserContext.LevelsOfTest.Remove(level);
                await UserContext.SaveChangesAsync();
            }

            return NoContent();

        }


        //Question

        [HttpGet("[controller]/Details/{idTest}/{idLevel}/[action]/")]
        public async Task<IActionResult> CreateQOT(int idTest, int idLevel)
        {

            var test = await UserContext.Tests.FindAsync(idTest);
            var level = await UserContext.LevelsOfTest.FindAsync(idLevel);

            if (test == null || level == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var TOQ = await UserContext.TypeOfQuestions.ToListAsync();

            CreateQOTViewModel model = new CreateQOTViewModel
            {
                TestId = test.Id,
                LevelId = level.Id,
                TypeOfQuestion = TOQ,
            };

            if (user.Equals(test.User))
                return View("./Question/CreateQOT", model);
            else
                return NotFound();

        }

        [HttpPost("[controller]/Details/{idTest}/{idLevel}/[action]/")]
        public async Task<IActionResult> CreateQOT(int idTest, int idLevel, CreateQOTViewModel createQOTView)
        {

            var test = await UserContext.Tests.FindAsync(idTest);
            var level = await UserContext.LevelsOfTest.FindAsync(idLevel);

            if (test == null || level == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.Equals(test.User))
            {

                if(createQOTView.TypeOfQuestionId == 1)
                {
                    var modelCQ = new CreateCQViewModel
                    {
                        TestId = idTest,
                        LevelId = idLevel,
                        TypeOfQuestionId = createQOTView.TypeOfQuestionId,
                        QuestionIndexNumber = createQOTView.QuestionIndexNumber,
                    };

                    return View("./Question/ClosedQuestion/CreateCQ", modelCQ);

                }
               

                return NotFound();

            }
            else
                return NotFound();

        }

        [HttpGet("[controller]/Details/{idTest}/{idLevel}/{idQuestion}/[action]/")]
        public async Task<IActionResult> EditQOT(int idTest, int idLevel, int idQuestion)
        {
            var test = await UserContext.Tests.FindAsync(idTest);
            var q = await UserContext.QuestionOfTests.FindAsync(idQuestion);

            if (test == null || q == null)
            {
                return NotFound();
            }


            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.Equals(test.User))
            {
                if (q.TypeOfQuestionId == 1)
                {

                    var cq = await UserContext.ClosedQuestions.FirstOrDefaultAsync(p => p.QuestionOfTestId == q.Id);

                    var modelCQ = new ChangeCQViewModel
                    {
                        TestId = test.Id,
                        QuestionOfTestId = q.Id,
                        QuestionIndexNumber = q.QuestionIndexNumber,
                        ClosedQuestionId = cq.Id,
                        QuestionContent = cq.QuestionContent,
                        CorrectOptionNumbers = cq.CorrectOptionNumbers,
                        OnlyOneRight = cq.OnlyOneRight,
                    };

                    return View("./Question/ClosedQuestion/EditCQ", modelCQ);
                }
                return NotFound();
            }
            else
                return NotFound();

        }

        [HttpPost("[controller]/Details/{idTest}/{idLevel}/{idQuestion}/[action]/")]
        public async Task<IActionResult> EditQOT(ChangeCQViewModel changeCQViewModel)
        {
            var q = await UserContext.QuestionOfTests.FirstOrDefaultAsync(p => p.Id == changeCQViewModel.QuestionOfTestId);
            var cq = await UserContext.ClosedQuestions.FirstOrDefaultAsync(p => p.Id == changeCQViewModel.ClosedQuestionId);

            if (ModelState.IsValid)
            {
                try
                {
                    q.QuestionIndexNumber = changeCQViewModel.QuestionIndexNumber;
                    UserContext.QuestionOfTests.Update(q);
                    await UserContext.SaveChangesAsync();

                    cq.QuestionContent = changeCQViewModel.QuestionContent;
                    cq.OnlyOneRight = changeCQViewModel.OnlyOneRight;
                    cq.CorrectOptionNumbers = changeCQViewModel.CorrectOptionNumbers;
                    UserContext.ClosedQuestions.Update(cq);
                    await UserContext.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {

                    throw;
                }
                return RedirectToRoute("default", new { controller = "Tests", action = "Details", id = changeCQViewModel.TestId });
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedQOT(int id)
        {
            var q = await UserContext.QuestionOfTests.SingleOrDefaultAsync(m => m.Id == id);

            if (q != null)
            {
                try
                {
                    var cqs = await UserContext.ClosedQuestions.SingleOrDefaultAsync(x => x.QuestionOfTestId == q.Id);

                    var cqos = await UserContext.ClosedQuestionOptions.Where(x => x.ClosedQuestionId == cqs.Id).ToListAsync();
                    UserContext.ClosedQuestionOptions.RemoveRange(cqos);
  
                    UserContext.ClosedQuestions.Remove(cqs);
                    UserContext.QuestionOfTests.Remove(q);

                    await UserContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NoContent();
                    throw;
             
                }

            }
            return NoContent();
        }



        //ClosedQuestion

        [HttpPost("[controller]/Details/{idTest}/{idLevel}/[action]/")]
        public async Task<IActionResult> CreateCQ(int idTest, int idLevel, int typeOfQuestionId, int questionIndexNumber, CreateCQViewModel createCQViewModel)
        {

            var test = await UserContext.Tests.FindAsync(idTest);

            if (!TestExists(idTest))
            {
                return NotFound();
            }

            if (!LevelExists(idLevel))
            {
                return NotFound();
            }


            var qot = new QuestionOfTest
            {

                TestId = idTest,
                QuestionIndexNumber = questionIndexNumber,
                LevelOfQuestionId = idLevel,
                TypeOfQuestionId = typeOfQuestionId,

            };

            UserContext.QuestionOfTests.Add(qot);
            await UserContext.SaveChangesAsync();
        
            var cq = new ClosedQuestion
            {
                QuestionOfTestId = qot.Id,
                OnlyOneRight = createCQViewModel.OnlyOneRight,
                QuestionContent = createCQViewModel.QuestionContent,
                CorrectOptionNumbers = createCQViewModel.CorrectOptionNumbers,
            };
   
            UserContext.ClosedQuestions.Add(cq);
            await UserContext.SaveChangesAsync();
            return RedirectToRoute("default", new { controller = "Tests", action = "Details", id = test.Id });

        }

       

        //ClosedQuestionOption
        [HttpGet("[controller]/Details/{idTest}/{idLevel}/{idClosedQuestion}/[action]/")]
        public async Task<IActionResult> CreateCQO(int idTest, int idLevel, int idClosedQuestion)
        {

            var test = await UserContext.Tests.FindAsync(idTest);
            var level = await UserContext.LevelsOfTest.FindAsync(idLevel);
            var closedQuestion = await UserContext.ClosedQuestions.FindAsync(idClosedQuestion);

            if (test == null || closedQuestion == null || level==null)
            {
                return NotFound();
            }

            var modelCQO = new CreateCQOViewModel
            {
                TestId = test.Id,
                LevelId = level.Id,
                ClosedQuestionId = closedQuestion.Id,
            };

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.Equals(test.User))
                return View("./Question/ClosedQuestion/ClosedQuestionOption/CreateCQO", modelCQO);
            else
                return NotFound();

        }

        [HttpPost("[controller]/Details/{idTest}/{idLevel}/{idClosedQuestion}/[action]/")]
        public async Task<IActionResult> CreateCQO(int idTest, int idClosedQuestion, CreateCQOViewModel createCQOViewModel)
        {

            if (!TestExists(idTest))
            {
                return NotFound();
            }

            var cqo = new ClosedQuestionOption
              {

                 ClosedQuestionId = idClosedQuestion,
                 Content = createCQOViewModel.Content,
                 OptionNumber = createCQOViewModel.OptionNumber,

              };

              UserContext.ClosedQuestionOptions.Add(cqo);
              await UserContext.SaveChangesAsync();

              return RedirectToRoute("default", new { controller = "Tests", action = "Details", id = idTest });

        }


        [HttpGet("[controller]/Details/{idTest}/{idLevel}/{idClosedQuestionOption}/[action]/")]
        public async Task<IActionResult> EditCQO(int idTest, int idLevel, int idClosedQuestionOption)
        {
            var test = await UserContext.Tests.FindAsync(idTest);
            var cqo = await UserContext.ClosedQuestionOptions.FindAsync(idClosedQuestionOption);

            if (test == null || cqo == null)
            {
                return NotFound();
            }

            var modelCQO = new ChangeCQOViewModel
            {
                TestId = test.Id,
                ClosedQuestionOptionId = cqo.Id,
                Content = cqo.Content,
                OptionNumber = cqo.OptionNumber,
            };

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.Equals(test.User))
                return View("./Question/ClosedQuestion/ClosedQuestionOption/EditCQO", modelCQO);
            else
                return NotFound();

        }

        [HttpPost("[controller]/Details/{idTest}/{idLevel}/{idClosedQuestionOption}/[action]/")]
        public async Task<IActionResult> EditCQO(ChangeCQOViewModel changeCQOViewModel)
        {

            var cqo = await UserContext.ClosedQuestionOptions.FirstOrDefaultAsync(p => p.Id == changeCQOViewModel.ClosedQuestionOptionId);

            if (ModelState.IsValid)
            {
                try
                {
                    cqo.Content = changeCQOViewModel.Content;
                    cqo.OptionNumber = changeCQOViewModel.OptionNumber;
                    UserContext.ClosedQuestionOptions.Update(cqo);
                    await UserContext.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {

                    throw;
                }
                return RedirectToRoute("default", new { controller = "Tests", action = "Details", id = changeCQOViewModel.TestId });
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedCQO(int id)
        {
            var cqo = await UserContext.ClosedQuestionOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (cqo != null)
            {
                UserContext.ClosedQuestionOptions.Remove(cqo);
                await UserContext.SaveChangesAsync();
            }
            return NoContent();
        }


        private bool TestExists(int id)
        {
            return UserContext.Tests.Any(e => e.Id == id);
        }

        private bool LevelExists(int id)
        {
            return UserContext.LevelsOfTest.Any(e => e.Id == id);
        }
    }
}
