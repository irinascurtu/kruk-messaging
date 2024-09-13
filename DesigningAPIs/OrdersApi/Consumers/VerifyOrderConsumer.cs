using Contracts.Responses;
using MassTransit;
using Orders.Domain.Entities;

namespace OrdersApi.Consumers
{
    public class VerifyOrderConsumer : IConsumer<VerifyOrder>
    {
        public async Task Consume(ConsumeContext<VerifyOrder> context)
        {
            await context.RespondAsync<OrderResult>(new
            {
                Id = context.Message.Id,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending
            });
        }
    }
}
