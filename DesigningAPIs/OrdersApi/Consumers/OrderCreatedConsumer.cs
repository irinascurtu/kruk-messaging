using Contracts.Events;
using MassTransit;

namespace OrdersApi.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await Task.Delay(1000);

            Console.WriteLine(context.ReceiveContext.InputAddress);
            Console.WriteLine($"I just consumed the message with OrderId: {context.Message.OrderId} createdAt:{context.Message.CreatedAt}");
           
        }
    }
}
