﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.Resources.Exception;
using ValidationException = FluentValidation.ValidationException;

namespace SharedKernel.Resources.API_Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //context.Result = new JsonResult(
                //    ((ValidationException)context.Exception).Failures);
                context.Result = new JsonResult(new
                {
                    error = new[] { context.Exception.Message },
                    instance = context.HttpContext.Request.Path,
                    stackTrace = context.Exception.StackTrace
                });

                return;
            }

            var code = HttpStatusCode.InternalServerError;

            if (context.Exception is NotFoundException)
            {
                code = HttpStatusCode.NotFound;
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)code;
            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                instance = context.HttpContext.Request.Path,
                stackTrace = context.Exception.StackTrace
            });
        }
    }
}