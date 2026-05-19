namespace BioInfoProject.Shared.Responses;

public class ApiResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = new();

    public static ApiResponse Ok(string message = "") => new()
    {
        Success = true,
        Message = message
    };

    public static ApiResponse Fail(string message, IEnumerable<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors?.ToList() ?? new List<string>()
    };
}
