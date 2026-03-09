using System.Net;
using LicenseRemake.Domain.Errors;
using LicenseRemake.DTO.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LicenseRemake.Infrastructure.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(
            context.Exception,
            "Unhandled exception in {Endpoint}",
            context.ActionDescriptor.DisplayName);

        ApiErrorResponse response;
        int httpStatus;

        if (context.Exception is ResponseException ex)
        {
            httpStatus = ex.StatusCode ?? MapHttpStatus(ex.ErrorCode);

            response = new ApiErrorResponse
            {
                err_code = ex.Code,
                err_code_string = ex.CodeString,
                err_descr = ex.Message,
                param = ex.Param
            };
        }
        else
        {
            httpStatus = (int)HttpStatusCode.InternalServerError;

            response = new ApiErrorResponse
            {
                err_code = (int)ResponseErrorCode.UndefinedError,
                err_code_string = "undefined_error",
                err_descr = context.Exception.Message
            };
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = httpStatus
        };

        context.ExceptionHandled = true;
    }

    private static int MapHttpStatus(ResponseErrorCode code)
    {
        return code switch
        {
            ResponseErrorCode.AccessForbidden => (int)HttpStatusCode.Forbidden,
            ResponseErrorCode.InvalidCredentials => (int)HttpStatusCode.Unauthorized,
            ResponseErrorCode.EntityNotFound => (int)HttpStatusCode.NotFound,
            ResponseErrorCode.UserNotFound => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.BadRequest
        };
    }
}
