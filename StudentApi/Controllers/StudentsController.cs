using Microsoft.AspNetCore.Mvc;
using StudentApi.Models;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly StudentStore _store;

    public StudentsController(StudentStore store)
    {
        _store = store;
    }

    [HttpGet]
    public ActionResult<List<Student>> GetAll()
    {
        return Ok(_store.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Student> Get(int id)
    {
        var student = _store.Get(id);
        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpPost]
    public ActionResult<Student> Create(Student student)
    {
        if (_store.EmailExists(student.Email))
        {
            ModelState.AddModelError(nameof(Student.Email), "Студент с таким Email уже существует.");
            return ValidationProblem(ModelState);
        }

        var created = _store.Add(student);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Student student)
    {
        if (_store.Get(id) == null)
            return NotFound();

        if (_store.EmailExists(student.Email, id))
        {
            ModelState.AddModelError(nameof(Student.Email), "Студент с таким Email уже существует.");
            return ValidationProblem(ModelState);
        }

        _store.Update(id, student);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_store.Delete(id))
            return NotFound();

        return NoContent();
    }
}
