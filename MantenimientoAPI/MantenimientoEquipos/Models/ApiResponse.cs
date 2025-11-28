namespace MantenimientoEquipos.Models;

/// <summary>
/// Respuesta paginada
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// Respuesta estándar de la API
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public ApiResponse(bool success, T? data, string? message = null, List<string>? errors = null)
    {
        Success = success;
        Data = data;
        Message = message;
        Errors = errors;
    }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new(true, data, message);

    public static ApiResponse<T> Error(string message, List<string>? errors = null)
        => new(false, default, message, errors);
}
