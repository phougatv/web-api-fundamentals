namespace WebApi.ErrorResponse.ViaProblemDetailsFactory.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi.ErrorResponse.ViaProblemDetailsFactory.Exceptions;
using WebApi.ErrorResponse.ViaProblemDetailsFactory.ValidationAttributes;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
	private readonly List<Order> _orders;

	public OrdersController()
	{
		_orders = new List<Order>();
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetAsync([GreaterThanZeroValidation] Int32 id)
	{
		var order = _orders.FirstOrDefault(o => o.Id == id);
		if (order is null)
			throw new MyNotFoundException();
		//throw new ArgumentException();

		return Ok(order);
	}

	[HttpGet]
	public async Task<IActionResult> GetAllAsync()
	{
		if (!_orders.Any())
			return NotFound("No orders found");
		return Ok(_orders);
	}

	[HttpPost]
	public async Task<IActionResult> CreateAsync(OrderViewModel viewModel)
	{
		var order = new Order { Id = viewModel.Id, ProductQuantityMap = viewModel.ProductQuantityMap! };
		_orders.Add(order);

		return StatusCode(StatusCodes.Status201Created, "Order successfully placed!");
	}

	#region Private Methods
	private static List<Order> GetOrders()
		=> new List<Order>(4)
		{
			new Order { Id = 1, ProductQuantityMap = new Dictionary<Int32, Int32>{ {1, 2} } },
			new Order { Id = 2, ProductQuantityMap = new Dictionary<Int32, Int32>{ {1, 4} } },
			new Order { Id = 3, ProductQuantityMap = new Dictionary<Int32, Int32>{ {2, 3} } },
			new Order { Id = 4, ProductQuantityMap = new Dictionary<Int32, Int32>{ {3, 6} } }
		};
	#endregion Private Methods
}

public class Order
{
	public Int32 Id { get; set; }

	public IDictionary<Int32, Int32> ProductQuantityMap { get; set; } = null!;
}

public class OrderViewModel
{
	[GreaterThanZeroValidation]
	public Int32 Id { get; set; }

	public Dictionary<Int32, Int32>? ProductQuantityMap { get; set; }
}
