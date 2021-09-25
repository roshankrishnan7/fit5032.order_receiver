using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fit5032.order_receiver.Models
{
    public class CustomerOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderNumber { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string Status { get; set; }
    }
}
