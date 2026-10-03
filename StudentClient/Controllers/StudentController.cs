using Microsoft.AspNetCore.Mvc;
using StudentClient.Models;
using StudentClient.Services;

namespace StudentClient.Controllers;

public class StudentController : Controller
{
    private readonly IStudentApiService _studentApi;

    public StudentController(IStudentApiService studentApi)
    {
        _studentApi = studentApi;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _studentApi.GetAllAsync();
        if (!result.IsSuccess)
            ViewData["Error"] = result.ErrorMessage;

        return View(result.Data ?? new List<Student>());
    }

    public async Task<IActionResult> Details(int? id)
    {
        ViewData["SearchId"] = id;
        if (id == null)
            return View();

        var result = await _studentApi.GetByIdAsync(id.Value);
        if (!result.IsSuccess)
        {
            ViewData["Error"] = result.ErrorMessage;
            return View();
        }

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Student());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (!ModelState.IsValid)
            return View(student);

        var result = await _studentApi.CreateAsync(student);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(student);
        }

        TempData["Message"] = $"Студент «{result.Data?.FullName}» добавлен ({result.StatusText}).";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _studentApi.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        student.Id = id;
        if (!ModelState.IsValid)
            return View(student);

        var result = await _studentApi.UpdateAsync(student);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return View(student);
        }

        TempData["Message"] = $"Данные студента обновлены ({result.StatusText}).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _studentApi.DeleteAsync(id);
        if (result.IsSuccess)
            TempData["Message"] = $"Студент удалён ({result.StatusText}).";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }
}
