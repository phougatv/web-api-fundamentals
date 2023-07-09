namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.Exceptions;

public class MyNotFoundException : MyExceptionBase
{
	public MyNotFoundException() : this("Not found")
	{ }

	public MyNotFoundException(String message)
		: base(message, StatusCodes.Status404NotFound, "The resource requested does not exists or was removed.", "Not found")
	{ }

	public MyNotFoundException(String message, Exception innerException)
		: base(message, innerException, StatusCodes.Status404NotFound, "The resource requested does not exists or was removed.", "Not found")
	{ }
}
