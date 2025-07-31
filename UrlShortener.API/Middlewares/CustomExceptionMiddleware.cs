using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.API.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Recommended: to separate exceptions between controllers?

        // The Result Pattern/"generic Result pattern" is recommended? 
        // https://medium.com/@aseem2372005/the-result-pattern-in-c-a-smarter-way-to-handle-errors-c6dee28a0ef0
        // https://www.linkedin.com/pulse/result-pattern-c-comprehensive-guide-andre-baltieri-wieuf/
        
        // we may not to want to throw exceptions in services, but to return Generic Result, and we would want
        // to have general try-catch in Middleware, where in catch we will return Result.Fail(ex.message)

        // exceptions are kinda expensive        

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UserCreationException ex)
            {
                _logger.LogError(ex, "User creation failed");
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (InvalidCredentialsException ex)
            {
                _logger.LogError(ex, "Invalid login attempt");
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Invalid email or password." });
            }
            catch (UserNotFoundException ex)
            {
                _logger.LogError(ex, "User not found");
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "User not found." });
            }
            catch (AdminNotFoundException ex)
            {
                _logger.LogError(ex, "Admin not found");
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Admin not found." });
            }
            catch (ShortUrlNotFoundException ex)
            {
                _logger.LogError(ex, "Short URL not found");
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Short URL not found." });
            }
            catch (ShortUrlForbiddenDeletionException ex)
            {
                _logger.LogError(ex, "Forbidden deletion attempt");
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "You can only delete your own short URLs." });
            }
            catch (ShortUrlCreationConflictException ex)
            {
                _logger.LogError(ex, "Short URL creation conflict");
                context.Response.StatusCode = 409;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "This URL already exists." });
            }
            catch (ShortCodeGenerationException ex)
            {
                _logger.LogError(ex, "Short code generation failed");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Unable to generate unique short code after multiple attempts." });
            }
            catch (ShortCodeEmptyException ex)
            {
                _logger.LogError(ex, "Empty short code passed");
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (ShortCodeInvalidLengthException ex)
            {
                _logger.LogError(ex, "Invalid short code length requested");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Invalid short code length was provided." });
            }
            catch (AboutInfoNotFoundException ex)
            {
                _logger.LogError(ex, "About info not found");
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
            }
        }
    }
}
