using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StudentClient.Models;

namespace StudentClient.Services;

public class StudentApiService : IStudentApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StudentApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Task<ApiResult<List<Student>>> GetAllAsync()
    {
        return SendAsync<List<Student>>(client => client.GetAsync("api/students"));
    }

    public Task<ApiResult<Student>> GetByIdAsync(int id)
    {
        return SendAsync<Student>(client => client.GetAsync($"api/students/{id}"));
    }

    public Task<ApiResult<Student>> CreateAsync(Student student)
    {
        return SendAsync<Student>(client => client.PostAsJsonAsync("api/students", student));
    }

    public async Task<ApiResult> UpdateAsync(Student student)
    {
        return await SendAsync<object>(client => client.PutAsJsonAsync($"api/students/{student.Id}", student), readData: false);
    }

    public async Task<ApiResult> DeleteAsync(int id)
    {
        return await SendAsync<object>(client => client.DeleteAsync($"api/students/{id}"), readData: false);
    }

    private async Task<ApiResult<T>> SendAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> send, bool readData = true)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("StudentApi");
            using var response = await send(client);

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResult<T>
                {
                    StatusCode = response.StatusCode,
                    ErrorMessage = await GetErrorMessageAsync(response)
                };
            }

            return new ApiResult<T>
            {
                IsSuccess = true,
                StatusCode = response.StatusCode,
                Data = readData ? await response.Content.ReadFromJsonAsync<T>() : default
            };
        }
        catch (HttpRequestException)
        {
            return new ApiResult<T> { ErrorMessage = "Не удалось подключиться к Web API. Проверьте, что сервер запущен." };
        }
        catch (TaskCanceledException)
        {
            return new ApiResult<T> { ErrorMessage = "Web API не ответил за отведённое время." };
        }
        catch (JsonException)
        {
            return new ApiResult<T> { ErrorMessage = "Web API вернул данные в неверном формате." };
        }
    }

    private static async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                var errors = await ReadValidationErrorsAsync(response);
                return errors.Count > 0
                    ? "Некорректные данные: " + string.Join(" ", errors)
                    : "Некорректные данные (400 Bad Request).";
            case HttpStatusCode.NotFound:
                return "Студент с указанным ID не найден.";
            case HttpStatusCode.InternalServerError:
                return "Внутренняя ошибка сервера (500). Попробуйте позже.";
            default:
                return $"Ошибка Web API: {(int)response.StatusCode} {response.ReasonPhrase}.";
        }
    }

    private static async Task<List<string>> ReadValidationErrorsAsync(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            return problem?.Errors.SelectMany(e => e.Value).ToList() ?? new List<string>();
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }
}
