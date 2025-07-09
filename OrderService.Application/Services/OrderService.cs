using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto> AddOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                UserName = createOrderDto.UserName,
                TotalPrice = createOrderDto.TotalPrice,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = createOrderDto.OrderItems.Select(itemDto => new OrderItem
                {
                    ProductId = Guid.Parse(itemDto.ProductId),
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    Price = itemDto.Price
                }).ToList()
            };
            await _orderRepository.AddAsync(order);
            return MapToDto(order);
        }

        public async Task UpdateOrderAsync(OrderDto orderDto)
        {
            var order = new Order
            {
                Id = Guid.Parse(orderDto.Id),
                UserName = orderDto.UserName,
                TotalPrice = orderDto.TotalPrice,
                OrderDate = orderDto.OrderDate,
                Status = orderDto.Status,
                OrderItems = orderDto.OrderItems.Select(itemDto => new OrderItem
                {
                    Id = Guid.Parse(itemDto.Id),
                    ProductId = Guid.Parse(itemDto.ProductId),
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    Price = itemDto.Price
                }).ToList()
            };
            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderDto>> FilterOrdersAsync(string userName, string status)
        {
            var orders = await _orderRepository.FilterAsync(userName, status);
            return orders.Select(MapToDto);
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id.ToString(),
                UserName = order.UserName,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id.ToString(),
                    ProductId = item.ProductId.ToString(),
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };
        }
    }


}
