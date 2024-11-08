using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBANSACH.Models
{
    public class Giohang
    {
       
            private readonly QLBANSACHEntities1 _context = new QLBANSACHEntities1();

            public int iMasach { get; set; }
            public string sTensach { get; set; }
            public string sAnhbia { get; set; }
            public double dDongia { get; set; }
            public int iSoluong { get; set; }

            public double dThanhtien
            {
                get => iSoluong * dDongia;
            }

            public Giohang(int Masach)
            {
                iMasach = Masach;
                SACH sach = _context.SACHes.Single(n => n.Masach == iMasach);
                sTensach = sach.Tensach;
                sAnhbia = sach.Anhbia;
                dDongia = (double)sach.Giaban;
                iSoluong = 1;
            }
        


    }
}