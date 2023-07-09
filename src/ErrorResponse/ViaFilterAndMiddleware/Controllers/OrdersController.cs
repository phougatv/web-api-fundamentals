namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi.ErrorResponse.ViaFilterAndMiddleware.Attributes;
using WebApi.ErrorResponse.ViaFilterAndMiddleware.Exceptions;
using WebApi.ErrorResponse.ViaFilterAndMiddleware.ViewModels;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
	private readonly List<Order> _orders;

	public OrdersController()
	{
		_orders = GetOrders();
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetAsync([GreaterThanZeroValidation][FromRoute]Int32 id)
	{
		var order = _orders.FirstOrDefault(o => o.Id == id);
		if (order is not null)
			return Ok(order);

		throw new MyNotFoundException();
	}

	[HttpPost]
	public async Task<IActionResult> CreateAsync([FromBody] OrderViewModel viewModel)
	{
		var order = new Order { Id = viewModel.Id, ProductQuantityMap = viewModel.ProductQuantityMap! };
		_orders.Add(order);

		return StatusCode(StatusCodes.Status201Created, "Order successfully placed!");
	}

	private static List<Order> GetOrders()
	{
		var orders = new List<Order>(4)
		{
			new Order
			{
				Id = 1,
				ProductQuantityMap = new Dictionary<Int32, Int32>{ {1, 2} }
			},
			new Order
			{
				Id = 2,
				ProductQuantityMap = new Dictionary<Int32, Int32>{ {1, 4} }
			},
			new Order
			{
				Id = 3,
				ProductQuantityMap = new Dictionary<Int32, Int32>{ {2, 3} }
			},
			new Order
			{
				Id = 4,
				ProductQuantityMap = new Dictionary<Int32, Int32>{ {3, 6} }
			}
		};

		return orders;
	}
}

public class Order
{
	public Int32 Id { get; set; }
	public IDictionary<Int32, Int32> ProductQuantityMap { get; set; } = null!;
}
