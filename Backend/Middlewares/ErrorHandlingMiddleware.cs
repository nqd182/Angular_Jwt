using System.Net;
using System.Text.Json;

namespace Jwt.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next; //để chuyển request đi tiếp.
        private readonly ILogger<ErrorHandlingMiddleware> _logger; //dùng để ghi log lỗi
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context) //phương thức bắt buộc trong middleware
        {
            try
            {
                await _next(context); //chuyển request đi tiếp
            }   
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra: {Message}", ex.Message);
                var response = context.Response;
                response.ContentType = "application/json"; // Headers
                response.StatusCode = (int)HttpStatusCode.InternalServerError; // Status Line
                var result = JsonSerializer.Serialize(new
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.",
                    Data = new { Error = ex.Message }
                });
                await response.WriteAsync(result); // ghi json vào body của response
            }
        }
    }
}
