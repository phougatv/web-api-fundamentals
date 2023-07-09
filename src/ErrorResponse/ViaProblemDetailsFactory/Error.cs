namespace WebApi.ErrorResponse.ViaProblemDetailsFactory;

public class Error
{
	public String Name { get; set; }
	public String Reason { get; set; }

	public Error(String name, String reason)
	{
		Name = name;
		Reason = reason;
	}
}
