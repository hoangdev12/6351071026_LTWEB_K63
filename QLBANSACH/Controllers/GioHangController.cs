using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBANSACH.Controllers
{
    public class GioHangController : Controller
    {

        private readonly QLBANSACHEntities1 _context;

        public GioHangController()
        {
            _context = new QLBANSACHEntities1();
        }
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }

        public List<Giohang> laygiohang()
        {
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<Giohang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        //them
        public ActionResult ThemGioHang(int iMasach, string strUrl)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["User"] == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            // Lấy giỏ hàng từ session (hoặc tạo mới nếu chưa có)
            List<Giohang> lstGioHang = laygiohang();

            // Tìm sản phẩm trong giỏ hàng
            Giohang sanpham = lstGioHang.Find(n => n.iMasach == iMasach);

            // Nếu sản phẩm chưa có trong giỏ hàng
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach); // Tạo sản phẩm mới
                lstGioHang.Add(sanpham); // Thêm vào giỏ hàng
            }
            else
            {
                sanpham.iSoluong++; // Nếu sản phẩm đã có, tăng số lượng lên
            }

            // Lưu giỏ hàng lại vào session
            Session["GioHang"] = lstGioHang;

            // Sau khi thêm giỏ hàng thành công, chuyển hướng đến trang giỏ hàng
            return RedirectToAction("GioHang", "GioHang");
        }



        private int TongSoLuong()
        {
            int iTongSoluong = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if (lstGioHang != null)
            {
                iTongSoluong = lstGioHang.Sum(n => n.iSoluong);
            }
            return iTongSoluong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGioHang = Session["GioHang"] as List<Giohang>;
            if(lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            List<Giohang> lstGioHang = laygiohang();
            if(lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }


        public ActionResult SuaGioHang(int iMasach, int soLuongMoi)
        {
            List<Giohang> lstGioHang = laygiohang();

            // Tìm sản phẩm trong giỏ hàng
            Giohang sanpham = lstGioHang.Find(n => n.iMasach == iMasach);

            if (sanpham != null)
            {
                // Cập nhật số lượng sản phẩm
                sanpham.iSoluong = soLuongMoi;

               

                // Lưu lại giỏ hàng vào session
                Session["GioHang"] = lstGioHang;
            }

            // Sau khi sửa, chuyển hướng lại giỏ hàng
            return RedirectToAction("GioHang");
        }

        public ActionResult ChiTietGioHang(int iMasach)
        {
            // Tìm sản phẩm trong giỏ hàng
            List<Giohang> lstGioHang = laygiohang();
            Giohang sanpham = lstGioHang.Find(n => n.iMasach == iMasach);

            if (sanpham != null)
            {
                return View(sanpham);  // Trả về view chi tiết của sản phẩm
            }

            // Nếu sản phẩm không có trong giỏ hàng, chuyển về giỏ hàng
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang(int iMasach)
        {
            // Lấy giỏ hàng từ session
            List<Giohang> lstGioHang = laygiohang();

            // Tìm sản phẩm trong giỏ hàng
            Giohang sanpham = lstGioHang.Find(n => n.iMasach == iMasach);

            if (sanpham != null)
            {
                lstGioHang.Remove(sanpham); 
                Session["GioHang"] = lstGioHang; 
            }

            // Sau khi xóa, quay lại trang giỏ hàng
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCaGioHang()
        {
            // Xóa giỏ hàng khỏi session
            Session["GioHang"] = null;

            // Quay lại trang giỏ hàng hoặc trang chính
            return RedirectToAction("Index", "Home");
        }


        public ActionResult GioHangpartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        [HttpGet]
        public ActionResult Dathang()
        {
            KHACHHANG kh = Session["User"] as KHACHHANG;
            if (kh == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            List<Giohang> lstGiohang = laygiohang();
            ViewBag.KhachHang = kh; 
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();

            return View(lstGiohang);
        }



        [HttpPost]
        public ActionResult Dathang(FormCollection collection)
        {
            KHACHHANG kh = Session["User"] as KHACHHANG; // Sử dụng ép kiểu an toàn

            if (kh == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            DONDATHANG ddh = new DONDATHANG();
            List<Giohang> gh = laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;

            var ngaygiao = collection["Ngaygiao"];
            if (string.IsNullOrEmpty(ngaygiao))
            {
                ModelState.AddModelError("Ngaygiao", "Ngày giao không thể để trống.");
                return View();
            }

            try
            {
                ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("Ngaygiao", "Ngày giao không hợp lệ.");
                return View();
            }

            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;

            _context.DONDATHANGs.Add(ddh);
            _context.SaveChanges();

            foreach (Giohang g in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG
                {
                    MaDonHang = ddh.MaDonHang,
                    Masach = g.iMasach,
                    Soluong = g.iSoluong,
                    Dongia = (decimal)g.dDongia
                };
                _context.CHITIETDONTHANGs.Add(ctdh);
            }

            _context.SaveChanges();
            Session["Giohang"] = null;

            return RedirectToAction("Xacnhandonhang", "GioHang");
        }





        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}