using AutoMapper;
using Contracts.Events;
using Contracts.Models;
using Contracts.Responses;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using Orders.Service;
using OrdersApi.Service.Clients;
using Stocks;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductStockServiceClient productStockServiceClient;
        private readonly IMapper mapper;
        private readonly Greeter.GreeterClient grpcClient;


        private readonly IPublishEndpoint publishEndpoint;//publish
        private readonly ISendEndpointProvider sendEndpointProvider;//send

        private readonly IRequestClient<VerifyOrder> requestClient;

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper,
            Stocks.Greeter.GreeterClient grpcClient,
            IPublishEndpoint publishEndpoint,
            ISendEndpointProvider sendEndpointProvider,
            IRequestClient<VerifyOrder> requestClient
            )
        {
            _orderService = orderService;
            this.productStockServiceClient = productStockServiceClient;
            this.mapper = mapper;
            this.grpcClient = grpcClient;
            this.publishEndpoint = publishEndpoint;
            this.sendEndpointProvider = sendEndpointProvider;
            this.requestClient = requestClient;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var response = await requestClient.GetResponse<OrderResult>(
                new VerifyOrder { Id = id });

            //var response = await requestClient.GetResponse<OrderResult>(
            //new { Id = id });


            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
            }
            catch
            {
                if (!await _orderService.OrderExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            var orderToAdd = mapper.Map<Order>(model);
            // var createdOrder = await _orderService.AddOrderAsync(orderToAdd);


            //send a CreateOrder Command
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-command"));
            await sendEndpoint.Send(model);

            // publish OrderCreated Event


            return Accepted();
            // return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

        private async Task<bool> VerifyStocks(List<Service.Clients.ProductStock> stocks, List<OrderItemModel> orderItems)
        {
            foreach (var item in orderItems)
            {
                var stock = stocks.FirstOrDefault(s => s.ProductId == item.ProductId);
                if (stock == null || stock.Stock < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> VerifyStocks(ProductStockList stocksList, List<OrderItemModel> orderItems)
        {
            foreach (var item in orderItems)
            {
                var stock = stocksList.Products.FirstOrDefault(s => s.ProductId == item.ProductId);
                if (stock == null || stock.Stock < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
