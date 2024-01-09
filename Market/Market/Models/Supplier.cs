using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
        }
        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống!")]
        [DisplayName("Mã nhà cung cấp")]
        public string SupplierId { get; set; } = null!;

        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống!")]
        [DisplayName("Tên nhà cung cấp")]
        public string? SupplierName { get; set; }

        [Required(ErrorMessage = "Địa chỉ nhà cung cấp không được để trống!")]
        [DisplayName("Địa chỉ")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "SĐT nhà cung cấp không được để trống!")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        [DisplayName("Số điện thoại")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống!")]
        [DisplayName("Trạng thái")]
        public int? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
