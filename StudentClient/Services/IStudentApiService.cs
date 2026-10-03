using StudentClient.Models;

namespace StudentClient.Services;

public interface IStudentApiService
{
    Task<ApiResult<List<Student>>> GetAllAsync();
    Task<ApiResult<Student>> GetByIdAsync(int id);
    Task<ApiResult<Student>> CreateAsync(Student student);
    Task<ApiResult> UpdateAsync(Student student);
    Task<ApiResult> DeleteAsync(int id);
}
