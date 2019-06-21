using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Testich.Models;
using Microsoft.AspNetCore.Http;

namespace Testich.ViewModels
{
    public class UploadTestViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Категории")]
        public List<Category> Categories;

        public int ResultScaleId { get; set; }

        [Required]
        [Display(Name = "Шкала оценки")]
        public List<ResultScale> ResultScales;

        [Required]
        public IFormFile uploadedFile { get; set; }
    }
}
