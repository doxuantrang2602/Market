using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [DisplayName("Mã hóa đơn")]
        [Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
        public string OrderId { get; set; } = null!;

        [DisplayName("Mã nhân viên")]
        public string? EmployeeId { get; set; }

        [DisplayName("Trạng thái đơn hàng")]
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        public int? Status { get; set; }

        [DisplayName("Ngày tạo đơn")]
        [Required(ErrorMessage = "Vui lòng chọn ngày tạo")]
        public DateTime? OrderDate { get; set; }

        [DisplayName("Tổng tiền hóa đơn")]
        public decimal? TotalOrders { get; set; }

        [DisplayName("Họ tên khách hàng")]
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string? CustomerName { get; set; }

        [DisplayName("Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng chọn địa chỉ")]
        public string? Address { get; set; }

        [DisplayName("Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string? Phone { get; set; }

        [DisplayName("Ghi chú")]
        public string? Note { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
