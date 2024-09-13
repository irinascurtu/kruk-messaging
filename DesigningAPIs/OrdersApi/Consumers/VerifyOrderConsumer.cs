using Contracts.Responses;
using MassTransit;
using Orders.Domain.Entities;

namespace OrdersApi.Consumers
{
    public class VerifyOrderConsumer : IConsumer<VerifyOrder>
    {
        public async Task Consume(ConsumeContext<VerifyOrder> context)
        {
            if (!context.IsResponseAccepted<Order>())
            {
                throw new ArgumentException(nameof(context));
            }

            if (context.Message.Id == 1)
            {
                await context.RespondAsync<OrderResult>(new
                {
                    Id = context.Message.Id,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending
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
