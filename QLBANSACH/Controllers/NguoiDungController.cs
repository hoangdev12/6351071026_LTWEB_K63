using Newtonsoft.Json;
using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly QLBANSACHEntities1 _context;
        // GET: NguoiDung

        public NguoiDungController()
        {
            _context = new QLBANSACHEntities1();
        }
        public ActionResult Index()
        {
            return View();

        }

        [HttpGet]
        public ActionResult Dangky()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(RegisterViewModel account)
        {
            if (ModelState.IsValid)
            {

                var existingCustomer = _context.KHACHHANGs.FirstOrDefault(c => c.Taikhoan == account.TenDN.Trim().ToLower());
                if (existingCustomer != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(account);
                }
                

                KHACHHANG newAccount = new KHACHHANG
                {
                    Taikhoan = account.TenDN.Trim().ToLower(),
                    Matkhau = account.Matkhau,
                    HoTen = account.HotenKH,
                    DienthoaiKH = account.Dienthoai,
                    Ngaysinh = account.Ngaysinh,
                    Email = account.Email,
                    DiachiKH = account.Diachi
                };

                _context.KHACHHANGs.Add(newAccount);
                _context.SaveChanges();
            
                return RedirectToAction("DangNhap");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(account);
            }
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(LoginViewModel customer)
        {
            if (ModelState.IsValid)
            {
                // Find user by username (case-insensitive)
                var user = _context.KHACHHANGs.FirstOrDefault(s => s.Taikhoan.Equals(customer.TenDN, StringComparison.OrdinalIgnoreCase));

                if (user != null)
                {
                    
                    var enteredPasswordHash = customer.Matkhau; 

                    
                    if (enteredPasswordHash.Equals(user.Matkhau))
                    {
                        
                        Session["User"] = user.Taikhoan;
                        Session["AccountId"] = user.MaKH;


                        
                        return RedirectToAction("Index", "Home"); 
                    }
                    else
                    {
                       
                        ModelState.AddModelError("Matkhau", "Mật khẩu không chính xác.");
                    }
                }
                else
                {
                    
                    ModelState.AddModelError("TenDN", "Tên đăng nhập không tồn tại.");
                }
            }

            
            return View(customer);
        }




    }
}