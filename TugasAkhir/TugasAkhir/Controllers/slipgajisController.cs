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

    public class slipgajisController : Controller
    {
        private dbOpsModel db = new dbOpsModel();

        // GET: slipgajis
//punya admin
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

        //Awal Slip Gaji
        public ActionResult RequestGaji()
        {
            return View(GetRequestGaji());
        }

        IEnumerable<slipgaji> GetRequestGaji()
        {
            var slipgajis = db.slipgajis.Include(u => u.user).Where(u => u.status == "Request").OrderBy(u => u.tanggal);
            return slipgajis.ToList<slipgaji>();
        }

        public ActionResult Process()
        {
            return View(GetProcess());
        }

        IEnumerable<slipgaji> GetProcess()
        {
            var slipgajis = db.slipgajis.Include(u => u.user).Where(u => u.status == "Process").OrderBy(u => u.tanggal);
            return slipgajis.ToList<slipgaji>();
        }

        public ActionResult Riwayat()
        {
            return View(GetRiwayat());
        }

        IEnumerable<slipgaji> GetRiwayat()
        {
            var slipgajis = db.slipgajis.Include(u => u.user).Where(u => u.status == "Finish").OrderByDescending(u => u.status);
            return slipgajis.ToList<slipgaji>();
        }

        [HttpPost]
        public ActionResult EditStatus(int id)
        {
            slipgaji slipupdate = db.slipgajis.Where(u => u.id_slip == id).FirstOrDefault();
            if (slipupdate.status == "Request")
            {
                slipupdate.status = "Process";
                db.Entry(slipupdate).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestGaji", GetRequestGaji()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);            
            }
            else if (slipupdate.status == "Process")
            {
                slipupdate.status = "Finish";
                db.Entry(slipupdate).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "Process", GetProcess()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Gagal Lanjut" }, JsonRequestBehavior.AllowGet);
            }        
        }
//ajax
    public JsonResult JsonProcessGaji()
    {
        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "Process", GetProcess()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);
    }

    public JsonResult JsonRiwayatGaji()
    {
        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "Riwayat", GetRiwayat()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
    }
//Ahkir Slip Gaji Admin
//awal karyawan
            public ActionResult IndexGajiK()
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


        public ActionResult RequestGajiK()
        {
            return View(GetRequestGajiK());
        }

        IEnumerable<slipgaji> GetRequestGajiK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var slipgajis = db.slipgajis.Include(u => u.user).Where(u => u.nik == id && u.status == "Request").OrderBy(u => u.tanggal);
            return slipgajis.ToList<slipgaji>();
        }

        public ActionResult ProcessK()
        {
            return View(GetProcessK());
        }

        IEnumerable<slipgaji> GetProcessK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var slipgajis = db.slipgajis.Include(u => u.user).Where(u => u.nik == id && u.status == "Process").OrderBy(u => u.tanggal);
            return slipgajis.ToList<slipgaji>();
        }

        public ActionResult RiwayatK()
        {
            return View(GetRiwayatK());
        }

        IEnumerable<slipgaji> GetRiwayatK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var slipgajis = db.slipgajis.Include(u => u.user).Where(u => u.nik == id && u.status == "Finish").OrderByDescending(u => u.status);
            return slipgajis.ToList<slipgaji>();
        }

      

        public ActionResult DeleteGajiK(int id)
        {
            try
            {
                slipgaji slip = db.slipgajis.Where(x => x.id_slip == id).FirstOrDefault();
                db.slipgajis.Remove(slip);
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestGajiK", GetRequestGajiK()), message = "Proses Dibatalkan!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult NewRequestK()
        {         
            return View();
        }

        [HttpPost]
        public ActionResult NewRequestK(slipgaji slip)
        {
            try
            {
                int id = Convert.ToInt32(Session["nik"]);
                var cekreq = db.slipgajis.Where(x => x.nik == id && x.status == "Request").FirstOrDefault();
                if(cekreq == null)
                {
                    user cekjoin = db.users.Find(id);
                    //validasi berdasarkan join date
                    if ((slip.durasi_awal.Year == cekjoin.join_date.Year && slip.durasi_awal.Month < cekjoin.join_date.Month) || slip.durasi_awal.Year < cekjoin.join_date.Year) 
                    {                       
                       return Json(new { success = false, message = "Tanggal yang anda input tidak valid" }, JsonRequestBehavior.AllowGet);                              
                    }
                    //validasi berdasarkan input tgl
                    if (slip.durasi_akhir < slip.durasi_awal)
                    {
                        return Json(new { success = false, message = "Cek kembali tanggal yang anda inputkan!" }, JsonRequestBehavior.AllowGet);
                    }
                    //validasi berdasarkan today date
                    if (slip.durasi_awal > DateTime.Today || slip.durasi_akhir > DateTime.Today)
                    {
                        return Json(new { success = false, message = "Slip gaji yang anda request belum ada!" }, JsonRequestBehavior.AllowGet);
                    }
                slip.nik = id;
                slip.tanggal = DateTime.Today;
                slip.status = "Request";
                db.slipgajis.Add(slip);
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestGajiK", GetRequestGajiK()), message = "Request Berhasil" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Harap tunggu, Request slip gaji anda sedang diproses" }, JsonRequestBehavior.AllowGet);
                }
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
