using AutoMapper;
using Contracts.Events;
using Contracts.Models;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;
using System;
using System.Threading.Tasks;

namespace OrderCreation
{
    public class CreateOrderConsumer : IConsumer<OrderModel>
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public CreateOrderConsumer(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }


        public async Task Consume(ConsumeContext<OrderModel> context)
        {
            Console.WriteLine($"I got a command to create and order: {context.Message}");

            var orderToAdd = mapper.Map<Order>(context.Message);

            //logic to create order
            var savedOrder = await orderService.AddOrderAsync(orderToAdd);

            //send notification to admin
            var notifyOrderCreated = context.Publish(new OrderCreated()
            {
                CreatedAt = savedOrder.OrderDate,
                OrderId = savedOrder.Id
            });

            Console.WriteLine($"Order created notification sent to admin for OrderId: {savedOrder.Id}");

            await Task.CompletedTask;
        }
    }
}
