using System.Net;

namespace StudentClient.Services;

public class ApiResult
{
    public bool IsSuccess { get; init; }
    public HttpStatusCode? StatusCode { get; init; }
    public string? ErrorMessage { get; init; }

    public string StatusText => StatusCode is { } code ? $"{(int)code} {code}" : "нет ответа";
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }
}
