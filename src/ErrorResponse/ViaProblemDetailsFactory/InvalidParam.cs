namespace WebApi.ErrorResponse.ViaProblemDetailsFactory;

public class InvalidParam
{
	public String Name { get; internal set; }
	public String Reason { get; internal set; }

	public InvalidParam(String name, String reason)
	{
		Name = name;
		Reason = reason;
	}
}
