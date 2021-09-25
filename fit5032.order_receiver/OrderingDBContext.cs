using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fit5032.order_receiver.Models;
using Microsoft.EntityFrameworkCore;

namespace fit5032.order_receiver
{
    public class OrderingDBContext: DbContext
    {
        public OrderingDBContext(DbContextOptions<OrderingDBContext> options): base(options) { }

        public DbSet<CustomerOrder> CustomerOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CustomerOrder>().HasKey(x => x.OrderNumber);
        }
    }
}
