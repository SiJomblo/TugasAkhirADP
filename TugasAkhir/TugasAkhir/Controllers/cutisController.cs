using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TugasAkhir.Models;

namespace TugasAkhir.Controllers
{
    public class cutisController : Controller
    {
        private dbOpsModel db = new dbOpsModel();

        // GET: cutis
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

        public ActionResult CutiMassal()
        {
            return View(GetCutiMassal());
        }

        IEnumerable<cuti_massal> GetCutiMassal()
        {
            var all = db.cuti_massal.OrderBy(u => u.tanggal);
            return all.ToList();
        }

        public ActionResult RiwayatCuti()
        {
            return View(GetRiwayatCuti());
        }

        IEnumerable<cuti> GetRiwayatCuti()
        {
            var all = db.cutis.Include(y => y.user.cuti_user).OrderBy(u => u.tanggal);
            return all.ToList();
        }

        public ActionResult NewCuti()
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
        public ActionResult NewCuti(cuti cute)
        {        
            try
            {
                cute.tanggal = DateTime.Today;
                cute.oleh = Session["nama"].ToString();
                db.cutis.Add(cute);
                cuti_user cutis = db.cuti_user.Find(cute.nik);
                int totalhari = Convert.ToInt32((cute.durasi_akhir - cute.durasi_awal).TotalDays);
                if (cutis.sisa_cuti >= totalhari)
                {
                    cutis.sisa_cuti -= totalhari;
                }
                else
                {
                    cutis.jatah_cuti = (cutis.jatah_cuti + cutis.sisa_cuti) - totalhari;
                    cutis.sisa_cuti = 0;
                }
                db.Entry(cutis).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatCuti", GetRiwayatCuti()), message = "Input Cuti Sukses!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CutiMassal(FormCollection get)
        {
            try
            {               
                int totalhari = Convert.ToInt32(get["jumlah"]);
                if (totalhari > 0)
                {
                    var emp = db.cuti_user.Include(u => u.user).Where(u => u.user.jabatan == "k" && u.user.status_kerja == "Aktif").ToList();
                    foreach (var cutiemp in emp)
                    {
                        if (cutiemp.sisa_cuti >= totalhari)
                        {
                            cutiemp.sisa_cuti -= totalhari;
                        }
                        else
                        {
                            cutiemp.jatah_cuti = (cutiemp.jatah_cuti + cutiemp.sisa_cuti) - totalhari;
                            cutiemp.sisa_cuti = 0;
                        }
                        db.Entry(cutiemp).State = EntityState.Modified;
                    }
                    cuti_massal cutimassal = new cuti_massal();
                    cutimassal.tanggal = Convert.ToDateTime(get["tanggal"]);
                    cutimassal.keterangan = get["keterangan"];
                    cutimassal.jumlah = totalhari;
                    cutimassal.oleh = Session["nama"].ToString();
                    db.cuti_massal.Add(cutimassal);
                    db.SaveChanges();
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "CutiMassal", GetCutiMassal()), message = "Input cuti massal sukses!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Input tidak valid" }, JsonRequestBehavior.AllowGet);
                }              
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //punya karyawan

        [HttpGet]
        public ActionResult IndexCutiK()
        {
            if (Session["nik"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["jabatan"].ToString() != "k")
            {
                return RedirectToAction("Admin", "Login");
            }
            int id = Convert.ToInt32(Session["nik"]);
            cuti_user cek = db.cuti_user.Find(id);
            ViewBag.jatah = cek.jatah_cuti;
            ViewBag.sisa = cek.sisa_cuti;
            ViewBag.validjatah = cek.valid_jatah;
            ViewBag.validsisa = cek.valid_sisa;
            return View();
        }

        public ActionResult RiwayatCutiK()
        {
            return View(GetRiwayatCutiK());
        }

        IEnumerable<cuti> GetRiwayatCutiK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var all = db.cutis.Include(y => y.user.cuti_user).Where(u => u.nik == id).OrderBy(u => u.tanggal);
            return all.ToList();
        }

        public ActionResult Details(int id = 0)
        {
            cuti cute = new cuti();
            if (id != 0) 
            {
                cute = db.cutis.Find(id);
            }
            return View(cute);
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
