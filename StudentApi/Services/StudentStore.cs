using StudentApi.Models;

namespace StudentApi.Services;

public class StudentStore
{
    private readonly object _lock = new();
    private int _nextId = 4;

    private readonly List<Student> _students = new()
    {
        new Student { Id = 1, FullName = "Иван Иванов", Group = "SE-301", Course = 3, Email = "ivan@example.com" },
        new Student { Id = 2, FullName = "Анна Петрова", Group = "SE-302", Course = 3, Email = "anna@example.com" },
        new Student { Id = 3, FullName = "Максим Ким", Group = "SE-301", Course = 3, Email = "maxim@example.com" }
    };

    public List<Student> GetAll()
    {
        lock (_lock)
        {
            return _students.ToList();
        }
    }

    public Student? Get(int id)
    {
        lock (_lock)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }
    }

    public bool EmailExists(string email, int? exceptId = null)
    {
        lock (_lock)
        {
            return _students.Any(s => s.Id != exceptId &&
                                      s.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Student Add(Student student)
    {
        lock (_lock)
        {
            student.Id = _nextId++;
            _students.Add(student);
            return student;
        }
    }

    public bool Update(int id, Student student)
    {
        lock (_lock)
        {
            var existing = _students.FirstOrDefault(s => s.Id == id);
            if (existing == null)
                return false;

            existing.FullName = student.FullName;
            existing.Group = student.Group;
            existing.Course = student.Course;
            existing.Email = student.Email;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            return _students.RemoveAll(s => s.Id == id) > 0;
        }
    }
}
