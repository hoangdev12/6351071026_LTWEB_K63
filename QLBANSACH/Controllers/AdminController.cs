using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI.WebControls;
using System.IO;
using Newtonsoft.Json;

namespace QLBANSACH.Controllers
{
    public class AdminController : Controller
    {
        private readonly QLBANSACHEntities1 _context;

        public AdminController()
        {
            _context = new QLBANSACHEntities1();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];

            // Check if username or password is empty
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
               

                // Use hashed password for comparison instead of plain text password
                Admin ad = _context.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);

                if (ad != null)
                {
                    // Store only the necessary information in the session (e.g., admin ID)
                    Session["Taikhoanadmin"] = ad;

                    // Redirect to Admin Index page
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // If login fails, show a generic error message
                    ViewData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
                }
            }

            return View();
        }

        public ActionResult Sach(int? page)
        {
            int pageNumber = page ?? 1; 
            int pageSize = 4;

            var sachList = _context.SACHes.OrderBy(s => s.Tensach).ToPagedList(pageNumber, pageSize);

            return View(sachList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.MaCD = new SelectList(_context.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SACH sach, HttpPostedFileBase fileupload)
        {
            // Lấy dữ liệu cho các dropdown list
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");


            // Kiểm tra nếu người dùng chưa chọn ảnh
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa.";
                return View(sach);  // Trả về View với model hiện tại
            }

            // Kiểm tra nếu ModelState hợp lệ
            if (ModelState.IsValid)
            {
                sach.Mota = HttpUtility.HtmlEncode(sach.Mota);
                try
                {
                    // Lấy tên file từ file upload
                    var filename = Path.GetFileName(fileupload.FileName);

                    // Tạo đường dẫn nơi lưu ảnh
                    var path = Path.Combine(Server.MapPath("~/Content/images/SACH"), filename);

                    // Kiểm tra xem file có tồn tại hay không
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại. Vui lòng chọn ảnh khác.";
                        return View(sach); // Trả về View nếu có lỗi
                    }

                    // Nếu không tồn tại, lưu file
                    fileupload.SaveAs(path);

                    // Gán tên file vào thuộc tính Anhbia của sách
                    sach.Anhbia = filename;

                    // Thêm sách vào cơ sở dữ liệu
                    _context.SACHes.Add(sach);
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    // Chuyển hướng về trang danh sách sách sau khi thêm mới thành công
                    return RedirectToAction("Sach", "Admin");
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có và thông báo người dùng
                    ViewBag.Thongbao = "Có lỗi xảy ra khi lưu ảnh: " + ex.Message;
                }
            }

            // Trả về view nếu có lỗi trong quá trình tạo sách
            return View(sach);
        }

        public ActionResult Details(int id)
        {
            SACH sach = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            SACH sach = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            SACH sach = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            _context.SACHes.Remove(sach);
            _context.SaveChanges();
            return RedirectToAction("Sach","Admin");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            SACH sach = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SACH sach, HttpPostedFileBase fileupload)
        {
            // Lấy dữ liệu cho các dropdown list
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");


            // Kiểm tra nếu người dùng chưa chọn ảnh
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa.";
                return View(sach);  // Trả về View với model hiện tại
            }

            // Kiểm tra nếu ModelState hợp lệ
            if (ModelState.IsValid)
            {
                sach.Mota = HttpUtility.HtmlEncode(sach.Mota);
                try
                {
                    // Lấy tên file từ file upload
                    var filename = Path.GetFileName(fileupload.FileName);

                    // Tạo đường dẫn nơi lưu ảnh
                    var path = Path.Combine(Server.MapPath("~/Content/images/SACH"), filename);

                    // Kiểm tra xem file có tồn tại hay không
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại. Vui lòng chọn ảnh khác.";
                        return View(sach); // Trả về View nếu có lỗi
                    }

                    // Nếu không tồn tại, lưu file
                    fileupload.SaveAs(path);

                    // Gán tên file vào thuộc tính Anhbia của sách
                    sach.Anhbia = filename;

                    // Thêm sách vào cơ sở dữ liệu
                    _context.SACHes.Add(sach);
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    // Chuyển hướng về trang danh sách sách sau khi thêm mới thành công
                    return RedirectToAction("Sach", "Admin");
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có và thông báo người dùng
                    ViewBag.Thongbao = "Có lỗi xảy ra khi lưu ảnh: " + ex.Message;
                }
            }

            // Trả về view nếu có lỗi trong quá trình tạo sách
            return View(sach);
        }

        public ActionResult ThongKeSach()
        {
            using (var context = new QLBANSACHEntities1())
            {
                // Lấy số lượng sách theo mã chủ đề (MaCD)
                var bookCountsByCategory = context.SACHes
                    .GroupBy(s => s.MaCD)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        Count = g.Count()
                    }).ToList();

                // Lấy danh sách các CategoryId từ bookCountsByCategory
                var categoryIds = bookCountsByCategory.Select(b => b.CategoryId).ToList();

                // Lấy tên chủ đề dựa trên MaCD từ bảng CHUDE
                var categoryNames = context.CHUDEs
                    .Where(c => categoryIds.Contains(c.MaCD))  // Dùng categoryIds thay cho bookCountsByCategory.Select
                    .ToDictionary(c => c.MaCD, c => c.TenChuDe);

                // Chuyển mã chủ đề thành tên chủ đề
                var categories = bookCountsByCategory
                   .Select(b => categoryNames.ContainsKey(b.CategoryId.GetValueOrDefault())
                  ? categoryNames[b.CategoryId.GetValueOrDefault()]
                 : "Không xác định")
                 .ToList();

                var counts = bookCountsByCategory.Select(b => b.Count).ToList();

                // Truyền dữ liệu sang View
                ViewBag.Categories = JsonConvert.SerializeObject(categories);
                ViewBag.Counts = JsonConvert.SerializeObject(counts);
            }

            return View();
        }









    }
}