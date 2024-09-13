using AutoMapper;
using Contracts.Responses;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;
using OrdersApi.Services;

namespace OrdersApi.Consumers
{
    public class VerifyOrderConsumer : IConsumer<VerifyOrder>
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;
        public VerifyOrderConsumer(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<VerifyOrder> context)
        {

            var existingOrder = await orderService.GetOrderAsync(context.Message.Id);

            if (!context.IsResponseAccepted<Order>())
            {
                throw new ArgumentException(nameof(context));
            }

            if (existingOrder != null)
            {
                await context.RespondAsync<OrderResult>(new
                {
                    Id = existingOrder.Id,
                    OrderDate = existingOrder.OrderDate,
                    Status = existingOrder.Status
                });
            }
            else
            {
                await context.RespondAsync<OrderNotFoundResult>(
                    new OrderNotFoundResult()
                    {
                        ErrorMessage = "Order not found"
                    });

            }
        }
    }
}
