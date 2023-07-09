namespace WebApi.ErrorResponse.ViaFilterAndMiddlewareTest.Filters.MyModelValidationResultFilterTest;

public class OnResultExecutingShould
{
	#region Private Constants
	private const String Success = "Success";
	private const String MediaTypeApplicationJson = "application/json";
	private const String MediaTypeApplicationProblemJson = "application/problem+json";
	#endregion Private Constants

	[Fact]
	public void DoNothing_WhenModelStateIsValid()
	{
		//Arrange
		var objectResult = new ObjectResult(Success) { StatusCode = Status200OK };
		objectResult.ContentTypes.Add(MediaTypeApplicationJson);

		var context = new ResultExecutingContext(
			new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor(),
				new ModelStateDictionary()),
			new List<IFilterMetadata>(),
			objectResult,
			new OrdersController());

		//Act
		var resultFilter = new MyModelValidationResultFilter();
		resultFilter.OnResultExecuting(context);

		//Assert
		var actualObjectResult = context.Result.Should().BeOfType<ObjectResult>().Subject;
		actualObjectResult.StatusCode.Should().Be(Status200OK);
		actualObjectResult.Value.Should().Be(Success);
		actualObjectResult.ContentTypes.Should().ContainSingle(MediaTypeApplicationJson);
	}

	[Theory]
	[InlineData("$", "'n' is an invalid start of a property name. Expected a '\"'. Path: $ | LineNumber: 1 | BytePositionInLine: 1.")]
	[InlineData("", "A non-empty request body is required.")]
	public void ReturnProblemDetailsInstance_WhenModelStateIsInvalid_BecauseModelBindingFailedForTheEntireModel(
		String key,
		String errorMessage)
	{
		//Arrange
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Invalid parameters.",
			Status = Status400BadRequest,
			Detail = "Your request failed model-binding."
		};
		var objectResult = new ObjectResult(Success) { StatusCode = Status200OK };
		objectResult.ContentTypes.Add(MediaTypeApplicationJson);

		var modelState = new ModelStateDictionary();
		modelState.AddModelError(key, errorMessage);
		modelState.AddModelError("viewModel", "The viewModel field is required.");

		var context = new ResultExecutingContext(
			new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor(),
				modelState),
			new List<IFilterMetadata>(),
			objectResult,
			new OrdersController());

		//Act
		var resultFilter = new MyModelValidationResultFilter();
		resultFilter.OnResultExecuting(context);

		//Assert
		var actualObjectResult = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
		actualObjectResult.StatusCode.Should().Be(expectedProblemDetails.Status);
		actualObjectResult.ContentTypes.Should().NotContain(MediaTypeApplicationJson);
		actualObjectResult.ContentTypes.Should().OnlyContain(type => type == MediaTypeApplicationProblemJson);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Should().BeEquivalentTo(expectedProblemDetails);
	}

	[Theory]
	[InlineData("$.productQuantityMap", "The JSON value could not be converted to System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]. Path: $.productQuantityMap | LineNumber: 3 | BytePositionInLine: 22.")]
	[InlineData("$.productQuantityMap", "'n' is an invalid start of a property name. Expected a '\"'. Path: $.productQuantityMap | LineNumber: 3 | BytePositionInLine: 4.")]
	public void ReturnProblemDetailsInstance_WhenModelStateIsInvalid_BecauseModelBindingFailedForPropertyOfTheModel(
		String key,
		String errorMessage)
	{
		//Arrange
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Invalid parameters.",
			Status = Status400BadRequest,
			Detail = "Your request failed model-binding: 'productQuantityMap'."
		};
		var objectResult = new ObjectResult(Success) { StatusCode = Status200OK };
		objectResult.ContentTypes.Add(MediaTypeApplicationJson);

		var modelState = new ModelStateDictionary();
		modelState.AddModelError(key, errorMessage);
		modelState.AddModelError("viewModel", "The viewModel field is required.");

		var context = new ResultExecutingContext(
			new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor(),
				modelState),
			new List<IFilterMetadata>(),
			objectResult,
			new OrdersController());

		//Act
		var resultFilter = new MyModelValidationResultFilter();
		resultFilter.OnResultExecuting(context);

		//Assert
		var actualObjectResult = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
		actualObjectResult.StatusCode.Should().Be(expectedProblemDetails.Status);
		actualObjectResult.ContentTypes.Should().NotContain(MediaTypeApplicationJson);
		actualObjectResult.ContentTypes.Should().OnlyContain(type => type == MediaTypeApplicationProblemJson);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Should().BeEquivalentTo(expectedProblemDetails);
	}

	[Theory]
	[InlineData("id", "Must be greater than 0.")]
	public void ReturnProblemDetailsInstance_WhenModelStateIsInvalid_BecauseOneOfTheModelValidationAttributeFailed(
		String key,
		String errorMessage)
	{
		//Arrange
		var invalidParams = new List<InvalidParam> { new InvalidParam(key, new[] { errorMessage }) };
		var expectedProblemDetails = new ProblemDetails
		{
			Title = "Invalid parameters.",
			Status = Status400BadRequest,
			Detail = "Your request parameters did not validate."
		};
		expectedProblemDetails.Extensions.Add(nameof(invalidParams), invalidParams);

		var objectResult = new ObjectResult(Success) { StatusCode = Status200OK };
		objectResult.ContentTypes.Add(MediaTypeApplicationJson);

		var modelState = new ModelStateDictionary();
		modelState.AddModelError(key, errorMessage);

		var context = new ResultExecutingContext(
			new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor(),
				modelState),
			new List<IFilterMetadata>(),
			objectResult,
			new OrdersController());

		//Act
		var resultFilter = new MyModelValidationResultFilter();
		resultFilter.OnResultExecuting(context);

		//Assert
		var actualObjectResult = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
		actualObjectResult.StatusCode.Should().Be(expectedProblemDetails.Status);
		actualObjectResult.ContentTypes.Should().NotContain(MediaTypeApplicationJson);
		actualObjectResult.ContentTypes.Should().OnlyContain(type => type == MediaTypeApplicationProblemJson);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Should().BeEquivalentTo(expectedProblemDetails);
	}
}