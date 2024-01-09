using System;
using System.Collections.Generic;

namespace Market.Models
{
    public partial class ShoppingCart
    {
        public string ShoppingCartId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? Unit { get; set; }
        public decimal? Price { get; set; }
        public int? Amount { get; set; }
        public string? Img { get; set; }
        public int? Status { get; set; }
        public decimal? Total { get; set; }
    }
}
