using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public partial class OrderDetail
    {
        [DisplayName("Mã hóa đơn")]
        [Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
        public string OrderId { get; set; } = null!;

        [DisplayName("Mã sản phẩm")]
        [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
        public string ProductId { get; set; } = null!;

        [DisplayName("Đơn giá")]
        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        public int? Price { get; set; }

        [DisplayName("Số lượng bán")]
        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        public int? Amount { get; set; }

        [DisplayName("Giảm giá")]
        [Required(ErrorMessage = "Vui lòng nhập giảm giá")]
        public int? Sale { get; set; }

        [DisplayName("Thành tiền")]
        public decimal? Total { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
