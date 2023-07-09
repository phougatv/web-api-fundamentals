namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.ErrorResponse.ViaFilterAndMiddleware.ViewModels;
using static Microsoft.AspNetCore.Http.StatusCodes;

public class MyModelValidationResultFilter : IResultFilter
{
	#region Private Constants
	private const Char Dot = '.';
	private const String DollarSign = "$";
	private const String InvalidParameters = "Invalid parameters.";
	private const String RequestFailedModelBinding = "Your request failed model-binding.";
	private const String RequestPropertyFailedModelBinding = "Your request failed model-binding: '{0}'.";
	private const String RequestParametersDidNotValidate = "Your request parameters did not validate.";
	private const String MediaTypeApplicationProblemJson = "application/problem+json";
	#endregion Private Constants

	/// <summary>
	/// 
	/// </summary>
	/// <param name="context">The result executed context.</param>
	public void OnResultExecuted(ResultExecutedContext context)
	{

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="context">The result executing context.</param>
	public void OnResultExecuting(ResultExecutingContext context)
	{
		if (context.ModelState.IsValid)
			return;

		var modelStateDictionary = context.ModelState;
		var problemDetails = new ProblemDetails
		{
			Title = InvalidParameters,
			Status = Status400BadRequest
		};

		//When model-binding fails because input is an invalid JSON
		if (modelStateDictionary.Any(pair => pair.Key == DollarSign || String.IsNullOrEmpty(pair.Key)))
		{
			problemDetails.Detail = RequestFailedModelBinding;
			context.Result = GetBadRequestObjectResult(problemDetails);
			return;
		}

		//When a specific property-binding fails
		var keyValuePair = modelStateDictionary.FirstOrDefault(pair => pair.Key.Contains("$."));
		if (keyValuePair.Key is not null)
		{
			var propertyName = keyValuePair.Key.Split(Dot)[1];
			problemDetails.Detail =
				String.IsNullOrEmpty(propertyName) ? RequestFailedModelBinding : String.Format(RequestPropertyFailedModelBinding, propertyName);
			context.Result = GetBadRequestObjectResult(problemDetails);
			return;
		}

		//When one of the input parameters failed model-validation
		var invalidParams = new List<InvalidParam>(modelStateDictionary.Count);
		foreach (var keyModelStatePair in modelStateDictionary)
		{
			var key = keyModelStatePair.Key;
			var modelErrors = keyModelStatePair.Value.Errors;
			if (modelErrors is not null && modelErrors.Count > 0)
			{
				IEnumerable<InvalidParam> invalidParam;
				if (modelErrors.Count == 1)
				{
					invalidParam = modelErrors.Select(error => new InvalidParam(keyModelStatePair.Key, new[] { error.ErrorMessage }));
				}
				else
				{
					var errorMessages = new String[modelErrors.Count];
					for (var i = 0; i < modelErrors.Count; i++)
					{
						errorMessages[i] = modelErrors[i].ErrorMessage;
					}

					invalidParam = modelErrors.Select(error => new InvalidParam(keyModelStatePair.Key, errorMessages));
				}

				invalidParams.AddRange(invalidParam);
			}
		}

		problemDetails.Detail = RequestParametersDidNotValidate;
		problemDetails.Extensions[nameof(invalidParams)] = invalidParams;
		context.Result = GetBadRequestObjectResult(problemDetails);
	}

	/// <summary>
	/// Creates <see cref="BadRequestObjectResult"/> instance.
	/// The content-types are first cleared and then set to: 'application/problem+json'
	/// </summary>
	/// <param name="problemDetails">The problem details instance.</param>
	/// <returns>The bad request object result instance.</returns>
	private static BadRequestObjectResult GetBadRequestObjectResult(ProblemDetails problemDetails)
	{
		var result = new BadRequestObjectResult(problemDetails);
		result.ContentTypes.Clear();
		result.ContentTypes.Add(MediaTypeApplicationProblemJson);
		return result;
	}
}
