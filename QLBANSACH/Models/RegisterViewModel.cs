using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBANSACH.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên khách hàng.")]
        public string HotenKH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        public string TenDN { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string Matkhau { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("Matkhau", ErrorMessage = "Mật khẩu không khớp.")]
        public string Matkhaunhaplai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Dienthoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        [DataType(DataType.Date)]
        public DateTime? Ngaysinh { get; set; }

        public string Email { get; set; }

        public string Diachi { get; set; }
    }
}