namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.ErrorResponse.ViaFilterAndMiddleware.Exceptions;
using static Microsoft.AspNetCore.Http.StatusCodes;

[ApiController]
public class ErrorsController : ControllerBase
{
	[Route("error-development")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
	{
		if (hostEnvironment is not null && !hostEnvironment.IsDevelopment())
			return NotFound();

		var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
		if (exceptionHandlerFeature is null ||
			exceptionHandlerFeature.Error is null)
		{
			return Problem(
			title: "Internal Server Error",
			detail: "You are on your own, start debugging.",
			statusCode: Status500InternalServerError);
		}

		if (exceptionHandlerFeature.Error is MyExceptionBase myException)
		{
			return Problem(
				title: myException.Title,
				detail: myException.Detail,
				statusCode: myException.StatusCode,
				instance: exceptionHandlerFeature.Path);
		}

		return Problem(
			title: exceptionHandlerFeature.Error.Message,
			detail: exceptionHandlerFeature.Error.StackTrace,
			statusCode: Status500InternalServerError,
			instance: exceptionHandlerFeature.Path);
	}

	[Route("error")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[AllowAnonymous]
	public IActionResult HandleError()
	{
		var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
		if (exceptionHandlerFeature is not null &&
			exceptionHandlerFeature.Error is not null &&
			exceptionHandlerFeature.Error is MyExceptionBase myException)
		{
			return Problem(
				detail: myException.Detail,
				title: myException.Title,
				statusCode: myException.StatusCode,
				instance: exceptionHandlerFeature.Path);
		}

		return Problem(
			title: "Internal Server Error",
			detail: "Internal server error occurred, contact your admin.",
			statusCode: Status500InternalServerError);
	}
}
