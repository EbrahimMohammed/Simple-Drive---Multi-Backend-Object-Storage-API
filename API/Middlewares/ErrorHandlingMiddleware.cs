using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog; // Ensure you're using Serilog or another logging library
using System;
using System.Net;
using System.Threading.Tasks;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An exception occurred."); 

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create a JSON response for the error
            var result = JsonConvert.SerializeObject(new
            {
                Message = "An exception occurred, please review the logs."
            });

            await context.Response.WriteAsync(result); // Write the response body
        }
    }
}
