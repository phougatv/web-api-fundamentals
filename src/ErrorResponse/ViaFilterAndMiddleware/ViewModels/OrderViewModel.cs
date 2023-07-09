namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.ViewModels;

using WebApi.ErrorResponse.ViaFilterAndMiddleware.Attributes;

public class OrderViewModel
{
	[GreaterThanZeroValidation]
	public Int32 Id { get; set; }

	public Dictionary<Int32, Int32>? ProductQuantityMap { get; set; }
}
