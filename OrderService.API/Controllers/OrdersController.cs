using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrdersController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
		{
			var orders = await _orderService.GetAllOrdersAsync();
			return Ok(orders);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<OrderDto>> GetOrder(string id)
		{
			var order = await _orderService.GetOrderByIdAsync(Guid.Parse(id));
			if (order == null)
			{
				return NotFound();
			}
			return Ok(order);
		}

		[HttpPost]
		public async Task<ActionResult<OrderDto>> AddOrder([FromBody] CreateOrderDto orderDto)
		{
			var newOrder = await _orderService.AddOrderAsync(orderDto);
			return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateOrder(string id, [FromBody] OrderDto orderDto)
		{
			if (id != orderDto.Id)
			{
				return BadRequest("ID mismatch");
			}

			var existingOrder = await _orderService.GetOrderByIdAsync(Guid.Parse(id));
			if (existingOrder == null)
			{
				return NotFound();
			}

			await _orderService.UpdateOrderAsync(orderDto);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteOrder(Guid id)
		{
			var existingOrder = await _orderService.GetOrderByIdAsync(id);
			if (existingOrder == null)
			{
				return NotFound();
			}

			await _orderService.DeleteOrderAsync(id);
			return NoContent();
		}

		[HttpGet("filter")]
		public async Task<ActionResult<IEnumerable<OrderDto>>> FilterOrders(
			[FromQuery] string? userName,
			[FromQuery] string? status)
		{
			var orders = await _orderService.FilterOrdersAsync(userName, status);
			return Ok(orders);
		}
	}
}
