using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using Orders.Service;
using OrdersApi.Models;
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

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper,
            Stocks.Greeter.GreeterClient grpcClient
            )
        {
            _orderService = orderService;
            this.productStockServiceClient = productStockServiceClient;
            this.mapper = mapper;
            this.grpcClient = grpcClient;
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
            var stocks = await productStockServiceClient.GetStock(
                model.OrderItems.Select(p => p.ProductId).ToList());

            //var stockRequest = new StockRequest();
            //stockRequest.ProductId.AddRange(model.OrderItems.Select(p => p.ProductId));

            //var request = grpcClient.GetStock(stockRequest);

            //Verify if all products have stock
            //if (!await VerifyStocks(stocks, model.OrderItems))
            //{
            //    ModelState.AddModelError("Item in cart", "Sorry, we don't have enough stock now");
            //    //add model state error sorry, we can't process your order, we don't have enough stock for item: 
            //}

            var orderToAdd = mapper.Map<Order>(model);
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
            //diminish stock


            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
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
