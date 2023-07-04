using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartCharging.Lib.Exceptions;
using System.Net;
using DataValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SmartCharging.Api.Filters;

public class HttpResponseExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ResourceNotFoundException notFoundException)
        {
            context.Result = new NotFoundObjectResult(new { message = notFoundException.Message, resource = notFoundException.Resource });
        }
        else if (context.Exception is BusinessRulesValidationException bussinesRulesException)
        {
            var exceptionResponse = new { message = bussinesRulesException.Message, errors = bussinesRulesException.Errors };
            context.Result = new ObjectResult(exceptionResponse)
            {
                StatusCode = (int)HttpStatusCode.PreconditionFailed
            };
        }
        else if (context.Exception is DataValidationException dataValidationException)
        {
            var exceptionResponse = new { message = dataValidationException.Message, errors = dataValidationException.ValidationResult };
            context.Result = new ObjectResult(exceptionResponse)
            {
                StatusCode = (int)HttpStatusCode.PreconditionFailed
            };
        }
        else
        {
            context.Result = new ObjectResult(context.Exception)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
