namespace WebApi.ErrorResponse.ViaFilterAndMiddlewareTest.Controllers.ErrorsControllerTest;

public class HandleErrorDevelopmentShould
{
	[Fact]
	public void ReturnNotFound_WhenHostEnvironmentIsNotNullAndNonDevelopment()
	{
		//Arrange
		var features = GetMockedFeatures(null!);
		var httpContext = GetMockedHttpContext(features);
		var controller = GetErrorsController(httpContext);

		//Act
		var actualResult = controller.HandleErrorDevelopment(GetMockedHostEnvironment(false));

		//Assert
		actualResult.Should().NotBeNull();

		var actualNotFoundResult = actualResult.Should().BeOfType<NotFoundResult>().Subject;
		actualNotFoundResult.Should().NotBeNull();
		actualNotFoundResult.StatusCode.Should().Be(Status404NotFound);
	}

	[Fact]
	public void ReturnProblemDetailsInstanceWithInternalServerError_WhenHostEnvironmentIsNull()
	{
		//Arrange
		var features = GetMockedFeatures(null!);
		var httpContext = GetMockedHttpContext(features);
		var controller = GetErrorsController(httpContext);

		//Act
		var actualResult = controller.HandleErrorDevelopment(null!);

		//Assert
		actualResult.Should().NotBeNull();

		var actualObjectResult = actualResult.Should().BeOfType<ObjectResult>().Subject;
		actualObjectResult.Should().NotBeNull();
		actualObjectResult.Value.Should().NotBeNull();
		actualObjectResult.StatusCode.Should().Be(Status500InternalServerError);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Status.Should().Be(Status500InternalServerError);
		actualProblemDetails.Title.Should().Be("Internal Server Error");
		actualProblemDetails.Detail.Should().Be("You are on your own, start debugging.");
	}

	[Fact]
	public void ReturnProblemDetailsInstanceWithInternalServerError_WhenExceptionHandlerFeatureIsNull()
	{
		//Arrange
		var features = GetMockedFeatures(null!);
		var httpContext = GetMockedHttpContext(features);
		var controller = GetErrorsController(httpContext);

		//Act
		var actualResult = controller.HandleErrorDevelopment(GetMockedHostEnvironment(true));

		//Assert
		actualResult.Should().NotBeNull();

		var actualObjectResult = actualResult.Should().BeOfType<ObjectResult>().Subject;
		actualObjectResult.Should().NotBeNull();
		actualObjectResult.Value.Should().NotBeNull();
		actualObjectResult.StatusCode.Should().Be(Status500InternalServerError);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Status.Should().Be(Status500InternalServerError);
		actualProblemDetails.Title.Should().Be("Internal Server Error");
		actualProblemDetails.Detail.Should().Be("You are on your own, start debugging.");
	}

	[Fact]
	public void ReturnProblemDetailsInstanceWithInternalServerError_WhenErrorPropertyOfExceptionHandlerFeatureIsNull()
	{
		//Arrange
		var exceptionHandlerFeature = GetMockedExceptionHandlerFeature(null!, null!);
		var features = GetMockedFeatures(exceptionHandlerFeature);
		var httpContext = GetMockedHttpContext(features);
		var controller = GetErrorsController(httpContext);

		//Act
		var actualResult = controller.HandleErrorDevelopment(GetMockedHostEnvironment(true));

		//Assert
		actualResult.Should().NotBeNull();

		var actualObjectResult = actualResult.Should().BeOfType<ObjectResult>().Subject;
		actualObjectResult.Should().NotBeNull();
		actualObjectResult.Value.Should().NotBeNull();
		actualObjectResult.StatusCode.Should().Be(Status500InternalServerError);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Status.Should().Be(Status500InternalServerError);
		actualProblemDetails.Title.Should().Be("Internal Server Error");
		actualProblemDetails.Detail.Should().Be("You are on your own, start debugging.");
	}

	[Fact]
	public void ReturnProblemDetailsInstance_WhenErrorPropertyOfExceptionHandlerFeature_IsNotNullAndOfType_MyExceptionBase()
	{
		//Arrange
		var message = "Title will contain the message of the exception.";
		var stackTrace = "Detail will contain the stack trace of the exception.";
		var path = "Instance will contain the relative path where exception occurred.";
		var mockedException = new Mock<ArgumentNullException>();
		mockedException.Setup(ex => ex.StackTrace).Returns(stackTrace);
		mockedException.Setup(ex => ex.Message).Returns(message);

		var exceptionHandlerFeature = GetMockedExceptionHandlerFeature(mockedException.Object, path);
		var features = GetMockedFeatures(exceptionHandlerFeature);
		var httpContext = GetMockedHttpContext(features);
		var controller = GetErrorsController(httpContext);

		//Act
		var actualResult = controller.HandleErrorDevelopment(GetMockedHostEnvironment(true));

		//Assert
		actualResult.Should().NotBeNull();

		var actualObjectResult = actualResult.Should().BeOfType<ObjectResult>().Subject;
		actualObjectResult.Should().NotBeNull();
		actualObjectResult.Value.Should().NotBeNull();
		actualObjectResult.StatusCode.Should().Be(Status500InternalServerError);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Status.Should().Be(Status500InternalServerError);
		actualProblemDetails.Title.Should().Be("Title will contain the message of the exception.");
		actualProblemDetails.Detail.Should().Be("Detail will contain the stack trace of the exception.");
		actualProblemDetails.Instance.Should().Be("Instance will contain the relative path where exception occurred.");
	}

	[Fact]
	public void ReturnProblemDetailsInstance_WhenErrorPropertyOfExceptionHandlerFeature_IsNotNullAndNotOfType_MyExceptionBase()
	{
		//Arrange
		var path = "api/orders/8";
		var myNotFoundException = new MyNotFoundException();
		var exceptionHandlerFeature = GetMockedExceptionHandlerFeature(myNotFoundException, path);
		var features = GetMockedFeatures(exceptionHandlerFeature);
		var httpContext = GetMockedHttpContext(features);
		var controller = GetErrorsController(httpContext);

		//Act
		var actualResult = controller.HandleErrorDevelopment(GetMockedHostEnvironment(true));

		//Assert
		actualResult.Should().NotBeNull();

		var actualObjectResult = actualResult.Should().BeOfType<ObjectResult>().Subject;
		actualObjectResult.Should().NotBeNull();
		actualObjectResult.Value.Should().NotBeNull();
		actualObjectResult.StatusCode.Should().Be(Status404NotFound);

		var actualProblemDetails = actualObjectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
		actualProblemDetails.Status.Should().Be(Status404NotFound);
		actualProblemDetails.Title.Should().Be("Not found");
		actualProblemDetails.Detail.Should().Be("The resource requested does not exists or was removed.");
		actualProblemDetails.Instance.Should().Be("api/orders/8");
	}

	#region Private Methods
	private static ErrorsController GetErrorsController(HttpContext httpContext)
	{
		var controller = new ErrorsController
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			}
		};

		return controller;
	}

	private static HttpContext GetMockedHttpContext(IFeatureCollection features)
	{
		var mockedHttpContext = new Mock<HttpContext>();
		mockedHttpContext.Setup(context => context.Features).Returns(features);
		return mockedHttpContext.Object;
	}

	private static IFeatureCollection GetMockedFeatures(IExceptionHandlerFeature exFeature)
	{
		var mockedFeatures = new Mock<IFeatureCollection>();
		mockedFeatures.Setup(feature => feature.Get<IExceptionHandlerFeature>()).Returns(exFeature);
		return mockedFeatures.Object;
	}

	private static IExceptionHandlerFeature GetMockedExceptionHandlerFeature(Exception exception, String path)
	{
		var mockedFeature = new Mock<IExceptionHandlerFeature>();
		mockedFeature.Setup(feature => feature.Error).Returns(exception);
		mockedFeature.Setup(feature => feature.Path).Returns(path);
		return mockedFeature.Object;
	}

	private static IHostEnvironment GetMockedHostEnvironment(Boolean isDevelopment)
	{
		var environmentName = isDevelopment ? "Development" : "Non-Development";
		var mockedHostEnvironment = new Mock<IHostEnvironment>();
		mockedHostEnvironment.Setup(env => env.EnvironmentName).Returns(environmentName);
		return mockedHostEnvironment.Object;
	}
	#endregion Private Methods
}
