using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderListVM
    {
        public List<OrderDetailsVM> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
