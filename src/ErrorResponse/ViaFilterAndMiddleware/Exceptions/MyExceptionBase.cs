namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.Exceptions;

public abstract class MyExceptionBase : Exception
{
	public Int32 StatusCode { get; protected set; }
	public String Detail { get; protected set; }
	public String Title { get; protected set; }

	protected MyExceptionBase(
		String message,
		Int32 statusCode,
		String detail,
		String title) : base(message)
	{
		StatusCode = statusCode;
		Detail = detail;
		Title = title;
	}

	protected MyExceptionBase(
		String message,
		Exception innerException,
		Int32 statusCode,
		String detail,
		String title) : base(message, innerException)
	{
		StatusCode = statusCode;
		Detail = detail;
		Title = title;
	}
}
