using Microsoft.AspNetCore.Identity;

namespace Worksy.Web.Core;

public class Response<T>
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
    public IEnumerable<IdentityError>? IErrors { get; set; }
    public List<string>? Errors { get; set; }
    public T? Result { get; set; }

    public static Response<T> Failure(Exception e, string message = "Ha ocurrido un error al general la solicitud")
    {
        return new Response<T>
        {
            isSuccess = false,
            Message = message,
            Errors = new List<string>
            {
                e.Message
            }
        };
    }

    public static Response<T> Failure(string message, List<string> errors = null)
    {
        return new Response<T>
        {
            isSuccess = false,
            Message = message,
            Errors = errors
        };
    }

    public static Response<T> Success(T result, string message = "Tarea realizada con éxito")
    {
        return new Response<T>
        {
            isSuccess = true,
            Message = message,
            Result = result
        };
    }
    
    public static Response<T> Success(string message = "Tarea realizada con éxito")
    {
        return new Response<T>
        {
            isSuccess = true,
            Message = message
        };
    }

}