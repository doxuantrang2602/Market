using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Market.Models
{
    public partial class User

    {
        [Display(Name = "Mã Nhân Viên")]
        public string? EmployeeId { get; set; }
        [StringLength(10, ErrorMessage = "Tên Đăng Nhập không được vượt quá 10 ký tự.")]
        [Display(Name = "Tên Đăng Nhập")]

        public string UserName { get; set; } = null!;
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }

        [Display(Name = "Trạng Thái")]
        public int? Status { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
