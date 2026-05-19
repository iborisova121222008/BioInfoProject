namespace BioInfoProject.Shared.Responses;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "") => new()
    {
        Success = true,
        Message = message,
        Data = data
    };

    public new static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors?.ToList() ?? new List<string>()
    };
}
