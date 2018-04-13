namespace Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Results;
    using FluentValidation;
    using FluentValidation.Results;

    public class CustomExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            switch (context.Exception)
            {
                case ValidationException exception:
                    string message = "Validation failed.";
                    string errorCode;

                    List<ValidationFailure> errors = exception.Errors.ToList();

                    if (errors.Count > 1)
                    {
                        errorCode = errors.Select(e => e.ErrorCode)
                            .GroupBy(e => e)
                            .OrderByDescending(g => g.Count())
                            .First()
                            .Key;
                    }
                    else
                    {
                        message = errors.First().ErrorMessage;
                        errorCode = errors.First().ErrorCode;
                    }

                    if (!Enum.TryParse(errorCode, out HttpStatusCode httpStatusCode))
                    {
                        httpStatusCode = (HttpStatusCode)422;
                    }

                    context.CreateResponse(httpStatusCode, new FailureResult
                    {
                        Message = message,
                        Errors = errors.Select(e => new { e.PropertyName, e.AttemptedValue, e.ErrorMessage })
                    });
                    break;

                default:
                    context.CreateResponse(HttpStatusCode.InternalServerError, new FailureResult
                    {
                        Message = context.Exception.Message,
                        Errors = context.Exception.GetInnerExceptions().Select(ex => ex.Message)
                    });
                    break;
            }
        }
    }

    public class FailureResult
    {
        public string Message { get; set; }
        public object Errors { get; set; }
    }

    public static class ExceptionHandlerExtensions
    {
        public static void CreateResponse<T>(
            this ExceptionHandlerContext context,
            HttpStatusCode httpStatusCode,
            T value)
        {
            context.Result = new ResponseMessageResult(
                context.Request.CreateResponse(
                    httpStatusCode,
                    value));
        }
    }

    public static class ExceptionExtensions
    {
        /// <summary>
        ///    Recursively unpacks an <see cref="Exception" />, yielding the
        ///    exception itself as well as every nested
        ///    <see cref="Exception.InnerException" />.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IEnumerable<Exception> GetExceptions(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var ex = exception;

            do
            {
                yield return ex;
                ex = ex.InnerException;
            }
            while (ex != null);
        }

        /// <summary>
        ///    Recursively unpacks an <see cref="Exception" />, yielding
        ///    every nested <see cref="Exception.InnerException" />.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IEnumerable<Exception> GetInnerExceptions(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Exception ex = exception.InnerException;

            while (ex != null)
            {
                yield return ex;
                ex = ex.InnerException;
            }
        }
    }
}