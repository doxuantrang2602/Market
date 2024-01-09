using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Market.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Orders = new HashSet<Order>();
            Users = new HashSet<User>();
        }
        [Required(ErrorMessage = "Mã nhân viên không được để trống!")]
        [Display(Name = "Mã Nhân Viên")]
        public string EmployeeId { get; set; } = null!;
        [Required(ErrorMessage = "Tên nhân viên không được để trống!")]

        [Display(Name = "Tên Nhân Viên")]


        public string? EmployeeName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống!")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        [Display(Name = "Số Điện Thoại")]

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vị trí không được để trống!")]
        [Display(Name = "Vị Trí")]

        public string? Position { get; set; }
        [Display(Name = "Trạng Thái")]

        [Required(ErrorMessage = "Trạng thái không được để trống!")]
        public int? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
