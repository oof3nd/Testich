using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Testich.Models;

namespace Testich.ViewModels
{
    public class TestsViewModel
    {
        [Required]
        public List<Test> Tests;
        [Required]
        public List<Category> Categories;
    }
}
