namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.ViewModels;

public class InvalidParam
{
	public String Name { get; set; }
	public String[] Reasons { get; set; }

	public InvalidParam(String name, String[] reasons)
	{
		Name = name;
		Reasons = reasons;
	}
}
