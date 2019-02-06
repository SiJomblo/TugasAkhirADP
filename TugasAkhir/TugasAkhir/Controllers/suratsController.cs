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
    public class suratsController : Controller
    {
        private dbOpsModel db = new dbOpsModel();

//Awal rekening
        public ActionResult IndexRekening()
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
        public ActionResult RequestRekening()
        {
            return View(GetRequestRekening());
        }

        IEnumerable<surat> GetRequestRekening()
        {
            var rekening = db.surats.Include(u => u.detail_surat).Include(x => x.jenis_surat).Where(u => u.id_jenis == 1 && u.status == "Request").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult ProcessRekening()
        {
            return View(GetProcessRekening());
        }

        IEnumerable<surat> GetProcessRekening()
        {
            var rekening = db.surats.Include(u => u.detail_surat).Include(x => x.jenis_surat).Where(u => u.id_jenis == 1 && u.status == "Process").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult RiwayatRekening()
        {
            return View(GetRiwayatRekening());
        }

        IEnumerable<surat> GetRiwayatRekening()
        {
            var rekening = db.surats.Include(u => u.detail_surat).Include(x => x.jenis_surat).Where(u => u.id_jenis == 1 && u.status == "Finish").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult EditStatusRekening(int id)
        {
            surat rekening = db.surats.Where(u => u.id_surat == id).FirstOrDefault();
            if (rekening.status == "Request")
            {
                rekening.status = "Process";
                db.Entry(rekening).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestRekening", GetRequestRekening()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);
            }
            else if (rekening.status == "Process")
            {
                rekening.status = "Finish";
                db.Entry(rekening).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessRekening", GetProcessRekening()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Success");
        }

    //loadajax
        public JsonResult JsonProcessRekening()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessRekening", GetProcessRekening()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonRiwayatRekening()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatRekening", GetRiwayatRekening()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

 //Ahkir Rekening

 //Awal keterangankerja
        public ActionResult IndexKerja()
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
        public ActionResult RequestKerja()
        {
            return View(GetRequestKerja());
        }

        IEnumerable<surat> GetRequestKerja()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Where(u => u.id_jenis == 2 && u.status == "Request").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult ProcessKerja()
        {
            return View(GetProcessKerja());
        }

        IEnumerable<surat> GetProcessKerja()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Where(u => u.id_jenis == 2 && u.status == "Process").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult RiwayatKerja()
        {
            return View(GetRiwayatKerja());
        }

        IEnumerable<surat> GetRiwayatKerja()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Where(u => u.id_jenis == 2 && u.status == "Finish").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult EditStatusKerja(int id)
        {
            surat kerja = db.surats.Where(u => u.id_surat == id).FirstOrDefault();
            if (kerja.status == "Request")
            {
                kerja.status = "Process";
                db.Entry(kerja).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestKerja", GetRequestKerja()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);
            }
            else if (kerja.status == "Process")
            {
                kerja.status = "Finish";
                db.Entry(kerja).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessKerja", GetProcessKerja()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Success");
        }

        //loadajax
        public JsonResult JsonProcessKerja()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessKerja", GetProcessKerja()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonRiwayatKerja()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatKerja", GetRiwayatKerja()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        //Ahkir keterangankerja

        //Awal NPWP
        public ActionResult IndexNPWP()
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
        public ActionResult RequestNPWP()
        {
            return View(GetRequestNPWP());
        }

        IEnumerable<surat> GetRequestNPWP()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.id_jenis == 3 && u.status == "Request").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult ProcessNPWP()
        {
            return View(GetProcessNPWP());
        }

        IEnumerable<surat> GetProcessNPWP()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.id_jenis == 3 && u.status == "Process").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult RiwayatNPWP()
        {
            return View(GetRiwayatNPWP());
        }

        IEnumerable<surat> GetRiwayatNPWP()
        {
            var rekening = db.surats.Include(u => u.detail_surat).Include(x => x.jenis_surat).Where(u => u.id_jenis == 3 && u.status == "Finish").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult EditStatusNPWP(int id)
        {
            surat npwp = db.surats.Where(u => u.id_surat == id).FirstOrDefault();
            if (npwp.status == "Request")
            {
                npwp.status = "Process";
                db.Entry(npwp).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestNPWP", GetRequestNPWP()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);

            }
            else if (npwp.status == "Process")
            {
                npwp.status = "Finish";
                db.Entry(npwp).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessNPWP", GetProcessNPWP()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);

            }     
            return Json("Success");
        }

        //loadajax
        public JsonResult JsonProcessNPWP()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessNPWP", GetProcessNPWP()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonRiwayatNPWP()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatNPWP", GetRiwayatNPWP()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

//Ahkir NPWP

//Awal Pemberitahuan
        public ActionResult IndexPemberitahuan()
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
        public ActionResult RequestPemberitahuan()
        {
            return View(GetRequestPemberitahuan());
        }

        IEnumerable<surat> GetRequestPemberitahuan()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.id_jenis == 4 && u.status == "Request").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult ProcessPemberitahuan()
        {
            return View(GetProcessPemberitahuan());
        }

        IEnumerable<surat> GetProcessPemberitahuan()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.id_jenis == 4 && u.status == "Process").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult RiwayatPemberitahuan()
        {
            return View(GetRiwayatPemberitahuan());
        }

        IEnumerable<surat> GetRiwayatPemberitahuan()
        {
            var rekening = db.surats.Include(u => u.detail_surat).Include(x => x.jenis_surat).Where(u => u.id_jenis == 4 && u.status == "Finish").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult EditStatusPemberitahuan(int id)
        {
            surat info = db.surats.Where(u => u.id_surat == id).FirstOrDefault();
            if (info.status == "Request")
            {
                info.status = "Process";
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestPemberitahuan", GetRequestPemberitahuan()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);
            }
            else if (info.status == "Process")
            {
                info.status = "Finish";
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessPemberitahuan", GetProcessPemberitahuan()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
            }     
            return Json("Success");
        }

        //ajax
        public JsonResult JsonProcessPemberitahuan()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessPemberitahuan", GetProcessPemberitahuan()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonRiwayatPemberitahuan()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatPemberitahuan", GetRiwayatPemberitahuan()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }
//Ahkir Pemberitahuan

//Awal Visa
        public ActionResult IndexVisa()
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
        public ActionResult RequestVisa()
        {
            return View(GetRequestVisa());
        }

        IEnumerable<surat> GetRequestVisa()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.id_jenis == 5 && u.status == "Request").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult ProcessVisa()
        {
            return View(GetProcessVisa());
        }

        IEnumerable<surat> GetProcessVisa()
        {
            var rekening = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.id_jenis == 5 && u.status == "Process").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult RiwayatVisa()
        {
            return View(GetRiwayatVisa());
        }

        IEnumerable<surat> GetRiwayatVisa()
        {
            var rekening = db.surats.Include(u => u.detail_surat).Include(x => x.jenis_surat).Where(u => u.id_jenis == 5 && u.status == "Finish").OrderBy(u => u.tanggal);
            return rekening.ToList();
        }

        public ActionResult EditStatusVisa(int id)
        {
            surat visa = db.surats.Where(u => u.id_surat == id).FirstOrDefault();
            if (visa.status == "Request")
            {
                visa.status = "Process";
                db.Entry(visa).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RequestVisa", GetRequestVisa()), message = "Proses Dilanjutkan" }, JsonRequestBehavior.AllowGet);
            }
            else if (visa.status == "Process")
            {
                visa.status = "Finish";
                db.Entry(visa).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessVisa", GetProcessVisa()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Success");
        }

 //ajax
        public JsonResult JsonProcessVisa()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessVisa", GetProcessVisa()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonRiwayatVisa()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatVisa", GetRiwayatVisa()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }
//Ahkir Visa


//Awal RequestKaryawan

        [HttpGet]
        public ActionResult IndexSurat()
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

        public ActionResult ListAllSurat()
        {
            return View(GetAllSurat());
        }

        IEnumerable<surat> GetAllSurat()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var allsurat = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.nik == id && u.status == "Request").OrderBy(u => u.tanggal);
            return allsurat.ToList();
        }

        public ActionResult ProcessSurat()
        {
            return View(GetProcessSurat());
        }

        IEnumerable<surat> GetProcessSurat()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var allsurat = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.nik == id && u.status == "Process").OrderBy(u => u.tanggal);
            return allsurat.ToList();
        }

        public ActionResult RiwayatAllSurat()
        {
            return View(GetRiwayatAllSurat());
        }

        IEnumerable<surat> GetRiwayatAllSurat()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var allsurat = db.surats.Include(x => x.jenis_surat).Include(u => u.detail_surat).Where(u => u.nik == id && u.status == "Finish").OrderBy(u => u.tanggal);
            return allsurat.ToList();
        }


        public ActionResult Details(int id = 0)
        {
            surat detailsurat = new surat();
            if (id != 0)
            {
            detailsurat = db.surats.Find(id);
            } 
            return View(detailsurat);
        }

        public ActionResult EditSurat(int id = 0)
        {
            surat surat = new surat();
            if (id != 0) { 
            surat = db.surats.Where(x => x.id_surat == id).FirstOrDefault();
            }
            return View(surat);
        }

        [HttpPost]
        public ActionResult EditSurat(surat surat)
        {
            try
            {
                surat surat2 = db.surats.Find(surat.id_surat);
                surat2.detail_surat.nama_bank = surat.detail_surat.nama_bank;
                surat2.detail_surat.keperluan = surat.detail_surat.keperluan;
                surat2.detail_surat.no_passport = surat.detail_surat.no_passport;
                surat2.detail_surat.tgl_pergi = surat.detail_surat.tgl_pergi;
                surat2.detail_surat.tgl_pulang = surat.detail_surat.tgl_pulang;
                surat2.detail_surat.tujuan = surat.detail_surat.tujuan;
                surat2.detail_surat.tujuan_instansi = surat.detail_surat.tujuan_instansi;
                surat2.detail_surat.kegiatan = surat.detail_surat.kegiatan;
                surat2.detail_surat.lokasi = surat.detail_surat.lokasi;
                db.Entry(surat2).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "Details", surat2), message = "Edit Data Sukses" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditSuratA(int id = 0)
        {
            surat surat = new surat();
            if (id != 0)
            {
                surat = db.surats.Where(x => x.id_surat == id).FirstOrDefault();
            }
            return View(surat);
        }

        [HttpPost]
        public ActionResult EditSuratA(surat surat)
        {
            try
            {
                surat surat2 = db.surats.Find(surat.id_surat);
                surat2.keterangan = surat.keterangan;
                surat2.status = "Finish";
                surat2.oleh = Session["nama"].ToString();
                db.Entry(surat2).State = EntityState.Modified;
                db.SaveChanges();
                if(surat2.id_jenis == 1)
                {
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessRekening", GetProcessRekening()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
                }
                else if (surat2.id_jenis == 2)
                {
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessKerja", GetProcessKerja()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
                }
                else if (surat2.id_jenis == 3)
                {
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessNPWP", GetProcessNPWP()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
                }
                else if (surat2.id_jenis == 4)
                {
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessPemberitahuan", GetProcessPemberitahuan()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
                }
                else if (surat2.id_jenis == 5)
                {
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ProcessVisa", GetProcessVisa()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Surat Hilang" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RequestSurat()
        {
            ViewBag.id_jenis = new SelectList(db.jenis_surat, "id_jenis", "jenis_surat1");
            return View();
        }

        [HttpPost]
        public ActionResult RequestSurat(surat surat)
        {
            try
            {
                int id = Convert.ToInt32(Session["nik"]);
                if (surat.id_jenis == 1)
                {
                    var cek = db.surats.Where(x => x.nik == id && x.status != "Finish" && x.id_jenis == 1).FirstOrDefault();
                    if (cek == null)
                    {
                    surat.nik = Convert.ToInt32(Session["nik"]);
                    surat.tanggal = DateTime.Today;
                    surat.status = "Request";
                    db.surats.Add(surat);
                    db.SaveChanges();
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListAllSurat", GetAllSurat()), message = "Request surat buka rekening berhasil" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Harap tunggu, Request pembukaan rekening anda masih diproses" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (surat.id_jenis == 2)
                {
                    var cek = db.surats.Where(x => x.nik == id && x.status != "Finish" && x.id_jenis == 2).FirstOrDefault();
                    if (cek == null)
                    {
                        surat.nik = Convert.ToInt32(Session["nik"]);
                        surat.tanggal = DateTime.Today;
                        surat.status = "Request";
                        db.surats.Add(surat);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListAllSurat", GetAllSurat()), message = "Request keterangan kerja berhasil" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Harap tunggu, Request surat keterangan kerja anda masih diproses" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (surat.id_jenis == 3)
                {
                    var cek = db.surats.Where(x => x.nik == id && x.status != "Finish" && x.id_jenis == 3).FirstOrDefault();
                    if (cek == null)
                    {
                        surat.nik = Convert.ToInt32(Session["nik"]);
                        surat.tanggal = DateTime.Today;
                        surat.status = "Request";
                        db.surats.Add(surat);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListAllSurat", GetAllSurat()), message = "Request surat NPWP berhasil" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Harap tunggu, Request surat NPWP anda masih diproses" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (surat.id_jenis == 4)
                {
                    var cek = db.surats.Where(x => x.nik == id && x.status != "Finish" && x.id_jenis == 4).FirstOrDefault();
                    if (cek == null)
                    {
                        surat.nik = Convert.ToInt32(Session["nik"]);
                        surat.tanggal = DateTime.Today;
                        surat.status = "Request";
                        db.surats.Add(surat);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListAllSurat", GetAllSurat()), message = "Request surat pemberitahuan berhasil" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Harap tunggu, Request surat pemberitahuan anda masih diproses" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (surat.id_jenis == 5)
                {
                    var cek = db.surats.Where(x => x.nik == id && x.status != "Finish" && x.id_jenis == 5).FirstOrDefault();
                    if (cek == null)
                    {
                        if (surat.detail_surat.tgl_pulang < surat.detail_surat.tgl_pergi)
                        {
                            return Json(new { success = false, message = "Tanggal yang anda input tidak valid" }, JsonRequestBehavior.AllowGet);
                        }
                        surat.nik = Convert.ToInt32(Session["nik"]);
                        surat.tanggal = DateTime.Today;
                        surat.status = "Request";
                        db.surats.Add(surat);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListAllSurat", GetAllSurat()), message = "Request surat Visa berhasil" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Harap tunggu, Request surat visa anda masih diproses" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "error!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteSurat(int id)
        {
            try
            {
                surat surat = db.surats.Where(x => x.id_surat == id).FirstOrDefault();
                db.surats.Remove(surat);
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListAllSurat", GetAllSurat()), message = "Proses Dibatalkan!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Akhir RequestKaryawan


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
