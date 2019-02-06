using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TugasAkhir.Models;

namespace TugasAkhir.Controllers
{
    public class lembursController : Controller
    {
        private dbOpsModel db = new dbOpsModel();

        public ActionResult IndexLemburK()
        {
            if (Session["nik"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["jabatan"].ToString() != "k")
            {
                return RedirectToAction("Admin", "Login");
            }
            return View();
        }

        // Lembur Karyawan
        public ActionResult ListLemburK()
        {
            return View(GetListLemburK());
        }

        IEnumerable<lembur> GetListLemburK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var lembur = db.lemburs.Where(u => u.nik == id && u.status == "Pending").OrderBy(u => u.tanggal);
            return lembur.ToList();
        }

        public ActionResult RiwayatLemburK()
        {
            return View(GetRiwayatLemburK());
        }

        IEnumerable<lembur> GetRiwayatLemburK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var lembur = db.lemburs.Where(u => u.nik == id && u.status != "Pending").OrderBy(u => u.tanggal);
            return lembur.ToList();
        }


        //Akhir Lembur karyawan  
        // Lembur admin

        public ActionResult Index()
        {
            if (Session["nik"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["jabatan"].ToString() == "k")
            {
                return RedirectToAction("Karyawan", "Login");
            }
            return View();
        }

        public ActionResult ListLembur()
        {
            return View(GetListLembur());
        }

        IEnumerable<lembur> GetListLembur()
        {
            var lembur = db.lemburs.Include(u => u.user).Where(u => u.status == "Pending").OrderBy(u => u.tanggal);
            return lembur.ToList();
        }

        public ActionResult RiwayatLembur()
        {
            return View(GetRiwayatLembur());
        }

        IEnumerable<lembur> GetRiwayatLembur()
        {
            var lembur = db.lemburs.Include(u => u.user).Where(u => u.status != "Pending").OrderBy(u => u.tanggal);
            return lembur.ToList();
        }


        public ActionResult Create()
        {
            ViewBag.ddnama = new SelectList(db.users.Where(u => u.jabatan == "k" && u.status_kerja == "Aktif"), "nik", "nama");
            return View();
        }

        public JsonResult JsonGetNIP(int id)
        {
            var nip = db.users.Where(u => u.nik == id).FirstOrDefault();
            return Json(nip.nip);
        }

        [HttpPost]
        public ActionResult Create(lembur lembur)
        {
            try
            {             
                var lemburs = db.lemburs.Where(u => u.nik == lembur.nik && u.tanggal == lembur.tanggal).FirstOrDefault();
                if (lemburs != null)
                {
                    return Json(new { success = false, message = "Lembur sudah ada" }, JsonRequestBehavior.AllowGet);
                }
                TimeSpan totallembur = lembur.total_jam;   
                decimal? upahjam = totallembur.Hours * lembur.reward;
                decimal? upahmenit = ((totallembur.Minutes * 100 / 60) * lembur.reward) / 100;
                lembur.totalreward = upahjam + upahmenit; 
                lembur.status = "Pending";
                lembur.oleh = Session["nama"].ToString();
                db.lemburs.Add(lembur);
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListLembur", GetListLembur()), message = "Input Data Sukses" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditStatus(int[] check)
        {
            try
            {
                if (check == null)
                {
                    return Json(new { success = false, message = "Check Minimal 1 Data" }, JsonRequestBehavior.AllowGet);
                }
                foreach (int item in check)
                {
                    lembur lembur = db.lemburs.Where(e => e.id_lembur == item && e.status == "Pending").FirstOrDefault();
                    lembur.status = "Finish";
                    db.Entry(lembur).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListLembur", GetListLembur()), message = check.Length + " Proses Selesai" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult JsonRiwayatLembur()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatLembur", GetRiwayatLembur()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteLembur(int[] check)
        {
            try
            {
                if (check == null)
                {
                    return Json(new { success = false, message = "Check Minimal 1 Data" }, JsonRequestBehavior.AllowGet);
                }
                foreach (int item in check)
                {
                    var lembur = db.lemburs.Where(e => e.id_lembur == item).FirstOrDefault();
                    db.lemburs.Remove(lembur);
                }
                db.SaveChanges();


                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListLembur", GetListLembur()), message = "Delete " + check.Length + " Data Sukses" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
