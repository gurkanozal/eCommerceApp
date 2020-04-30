using System;
using Cart.Application;
using Cart.Domain.Exception;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cart.API.Startup
{
    public static class ProblemDetailsConfiguration
    {
        public static void AddProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.ShouldLogUnhandledException = (context, exception, detail) => false;
                options.IncludeExceptionDetails = context => false;
                options.Map<RequestValidationException>(exception =>
                    new ValidationProblemDetails(exception.ValidationErrors)
                    {
                        Status = StatusCodes.Status400BadRequest
                    });
                options.Map<DomainException>(exception =>
                    new DomainProblemDetails(exception));
                options.Map<Exception>(exception =>
                    new ExceptionProblemDetails(exception, 500));
            });
        }
    }

    public class DomainProblemDetails : ProblemDetails
    {
        public DomainProblemDetails(DomainException exception)
        {
            Status = StatusCodes.Status500InternalServerError;
            Title = "Domain Error";
            Detail = exception?.Message ?? exception?.InnerException?.Message;
        }
    }
}