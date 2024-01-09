using Market.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }
        [Required(ErrorMessage = "Mã sản phẩm không được để trống!")]
        [DisplayName("Mã sản phẩm")]
        public string ProductId { get; set; } = null!;

        [Required(ErrorMessage = "Tên sản phẩm không được để trống!")]
        [DisplayName("Tên sản phẩm")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = " Đơn vị tính không được để trống!")]
        [StringLength(10, ErrorMessage = "Đơn vị tính tối đa 10 ký tự")]
        [DisplayName("Đơn vị tính")]
        public string? Unit { get; set; }

        [Required(ErrorMessage = "Giá tiền không được để trống!")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá tiền phải lớn hơn 0.")]
        [DisplayName("Giá tiền")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống!")]
        [Range(typeof(int), "1", "1000", ErrorMessage = "Số lượng phải là số nguyên và lớn hơn 0")]
        [DisplayName("Số lượng")]
        public int? Amount { get; set; }

        [Required(ErrorMessage = "Nhà cung cấp không được để trống!")]
        [DisplayName("Nhà cung cấp")]
        public string? SupplierId { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống!")]
        [DisplayName("Danh mục")]
        public string? CategoryId { get; set; }


        [DisplayName("Hình ảnh")]
        public string? Img { get; set; }

        [NotMapped]
        [ValidateNever]
        public IFormFile picture { get; set; }

        [DisplayName("Chi tiết sản phẩm")]
        public string? Detail { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống!")]
        [DisplayName("Trạng thái")]
        public int? Status { get; set; }

        [DisplayName("Trạng thái")]
        public string StatusString
        {
            get
            {
                if (Status == 1)
                {
                    return "Đang kinh doanh";
                }
                else if (Status == 2)
                {
                    return "Ngừng kinh doanh";
                }
                return "";
            }
        }

        public virtual Category? Category { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}