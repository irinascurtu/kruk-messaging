using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    [EntityName("Order-Created")]
    public class OrderCreated
    {
        public int OrderId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
