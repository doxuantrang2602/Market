using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Required(ErrorMessage = "Mã danh mục không được để trống!")]
        [DisplayName("Mã danh mục")]
        public string CategoryId { get; set; } = null!;

        [Required(ErrorMessage = "Mã danh mục không được để trống!")]
        [DisplayName("Tên danh mục")]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống!")]
        [DisplayName("Trạng thái")]
        public int? Status { get; set; }
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
        public virtual ICollection<Product> Products { get; set; }
    }
}
