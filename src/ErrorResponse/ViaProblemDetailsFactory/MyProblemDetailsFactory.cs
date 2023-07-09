namespace WebApi.ErrorResponse.ViaProblemDetailsFactory;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApi.ErrorResponse.ViaProblemDetailsFactory.Exceptions;

internal sealed class MyProblemDetailsFactory : ProblemDetailsFactory
{
	public override ProblemDetails CreateProblemDetails(
		HttpContext httpContext,
		Int32? statusCode = null,
		String? title = null,
		String? type = null,
		String? detail = null,
		String? instance = null)
	{
		ProblemDetails problemDetails;
		var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
		if (exceptionHandlerFeature is not null &&
			exceptionHandlerFeature.Error is not null &&
			exceptionHandlerFeature.Error is MyExceptionBase myException)
		{   //<-- Custom exception handler
			httpContext.Response.StatusCode = myException.StatusCode;       //<-- Overrides the default 500 status code
			problemDetails = new ProblemDetails
			{
				Detail = myException.Detail,
				Instance = exceptionHandlerFeature.Path,
				Status = myException.StatusCode,
				Title = myException.Title
			};
		}
		else
		{   //<-- Default ProblemDetails instance, used for unhandled exceptions
			statusCode = statusCode ?? StatusCodes.Status500InternalServerError;
			detail = detail ?? "An error occurred, connect with your administrator.";
			title = title ?? "Internal Server Error";
			httpContext.Response.StatusCode = statusCode.Value;
			problemDetails = new ProblemDetails
			{
				Detail = detail,
				Instance = instance,
				Status = statusCode,
				Title = title,
				Type = type
			};
		}

		return problemDetails;
	}

	public override ValidationProblemDetails CreateValidationProblemDetails(
		HttpContext httpContext,
		ModelStateDictionary modelStateDictionary,
		Int32? statusCode = null,
		String? title = null,
		String? type = null,
		String? detail = null,
		String? instance = null)
	{
		ArgumentNullException.ThrowIfNull(modelStateDictionary);
		statusCode ??= StatusCodes.Status400BadRequest;

		var validationProblemDetails = new ValidationProblemDetails(modelStateDictionary)
		{
			Status = statusCode,
			Type = type,
			Detail = detail,
			Instance = instance
		};
		if (title is not null)
		{
			// For validation problem details, don't overwrite the default title with null.
			validationProblemDetails.Title = title;
		}

		var errors = new List<Error>(modelStateDictionary.Count);
		foreach (var keyModelStatePair in modelStateDictionary)
		{
			var key = keyModelStatePair.Key;
			var modelErrors = keyModelStatePair.Value.Errors;
			if (modelErrors is not null && modelErrors.Count > 0)
			{
				var item = modelErrors.Select(error => new Error(key, error.ErrorMessage));
				errors.AddRange(item);

				//if (modelErrors.Count == 1)
				//{
				//	var errorMessage = GetErrorMessage(modelErrors[0]);
				//	errorDictionary.Add(key, new[] { errorMessage });
				//}
				//else
				//{
				//	var errorMessages = new string[modelErrors.Count];
				//	for (var i = 0; i < modelErrors.Count; i++)
				//	{
				//		errorMessages[i] = GetErrorMessage(modelErrors[i]);
				//	}

				//	errorDictionary.Add(key, errorMessages);
				//}
			}
		}

		validationProblemDetails.Extensions["invalid_params"] = errors;

		return validationProblemDetails;
	}
}
