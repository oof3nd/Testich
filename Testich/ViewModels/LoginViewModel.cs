using Microsoft.AspNetCore.Authentication;
using System;
using System.ComponentModel.DataAnnotations;

namespace Testich.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле Email обязательно для заполнения.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле пароль обязательно для заполнения.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
}
