﻿namespace WebApi.ErrorResponse.ViaProblemDetailsFactory.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
		return Problem(
			detail: exceptionHandlerFeature.Error.StackTrace,
			title: exceptionHandlerFeature.Error.Message,
			instance: exceptionHandlerFeature.Path);
	}

	[Route("error")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[AllowAnonymous]
	public IActionResult HandleError() => Problem();
}
