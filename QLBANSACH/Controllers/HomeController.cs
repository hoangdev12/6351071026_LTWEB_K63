﻿using PagedList;
using QLBANSACH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QLBANSACH.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLBANSACHEntities1 _context;

        public HomeController()
        {
            _context = new QLBANSACHEntities1();
        }

        public ActionResult Index(int? page)
        {
            // Fetch all books from the database
            var books = _context.SACHes.ToList();

            // Determine the current page number, defaulting to 1 if not specified
            var pageNumber = page ?? 1;

            // Set the number of items to display per page
            var pageSize = 4;

            // Order the books by the specified property (Masach) and paginate the results
            var pagedBooks = books.OrderBy(b => b.Masach).ToPagedList(pageNumber, pageSize);

            // Return the paginated list to the view
            return View(pagedBooks);
        }

        public ActionResult DetailsSP(int id)
        {
            // Lấy sách theo ID, nếu không tìm thấy sẽ trả về null
            var sach = _context.SACHes.SingleOrDefault(s => s.Masach == id);

            // Kiểm tra nếu sách không tồn tại
            if (sach == null)
            {
                return HttpNotFound(); // Trả về trang 404 nếu không tìm thấy
            }

            // Trả về view với mô hình sách
            return View(sach);
        }

        public ActionResult detailNXB(int id)
        {
            var sach = from s in _context.SACHes where s.Masach == id select s;

            return View(sach);
        }

        public ActionResult subpage()
        {
            return View();
        }

        public ActionResult chude()
        {
            var chude = from cd in _context.CHUDEs select cd;
            return PartialView(chude);
        }

        public ActionResult NhaXuatBan()
        {
            var nhaxuatban = from cd in _context.NHAXUATBANs select cd;
            return PartialView(nhaxuatban);
        }
    }
}