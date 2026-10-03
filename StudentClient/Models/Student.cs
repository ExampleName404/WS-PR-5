using System.ComponentModel.DataAnnotations;

namespace StudentClient.Models;

public class Student
{
    public int Id { get; set; }

    [Display(Name = "ФИО")]
    [Required(ErrorMessage = "Введите ФИО")]
    [StringLength(100, ErrorMessage = "Не более 100 символов")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Группа")]
    [Required(ErrorMessage = "Введите группу")]
    [StringLength(20, ErrorMessage = "Не более 20 символов")]
    public string Group { get; set; } = string.Empty;

    [Display(Name = "Курс")]
    [Range(1, 6, ErrorMessage = "Курс должен быть от 1 до 6")]
    public int Course { get; set; } = 1;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Введите Email")]
    [EmailAddress(ErrorMessage = "Неверный формат Email")]
    public string Email { get; set; } = string.Empty;
}
