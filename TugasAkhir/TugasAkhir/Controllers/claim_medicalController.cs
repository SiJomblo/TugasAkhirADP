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
    public class claim_medicalController : Controller
    {
        private dbOpsModel db = new dbOpsModel();
//punya karyawan
        [HttpGet]
        public ActionResult IndexClaimK()
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
            saldo_medical valid = db.saldo_medical.Find(id);
            ViewBag.saldoinap = valid.saldo_rawatinap;
            ViewBag.saldojalan = valid.saldo_rawatjalan;
            ViewBag.saldokacamata = valid.saldo_kacamata;
            ViewBag.validinap = valid.valid_rawat;
            ViewBag.validjalan = valid.valid_rawat;
            ViewBag.validkacamata = valid.valid_kacamata;
            return View();
        }

        public ActionResult ListRequestClaimK()
        {
            return View(GetRequestClaimK());
        }

        IEnumerable<claim_medical> GetRequestClaimK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var allclaim = db.claim_medical.Include(y => y.saldo_medical).Where(u => u.nik == id && u.status == "Request").OrderBy(u => u.tanggal);
            return allclaim.ToList();
        }

        public ActionResult RejectClaimK()
        {
            return View(GetRejectClaimK());
        }

        IEnumerable<claim_medical> GetRejectClaimK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var allclaim = db.claim_medical.Include(y => y.saldo_medical).Where(u => u.nik == id && u.status == "Reject").OrderBy(u => u.tanggal);
            return allclaim.ToList();
        }

        public ActionResult RiwayatClaimK()
        {
            return View(GetRiwayatClaimK());
        }

        IEnumerable<claim_medical> GetRiwayatClaimK()
        {
            int id = Convert.ToInt32(Session["nik"]);
            var allclaim = db.claim_medical.Include(y => y.saldo_medical).Where(u => u.nik == id && u.status == "Finish").OrderBy(u => u.tanggal);
            return allclaim.ToList();
        }


        public ActionResult Details(int id = 0)
        {
            claim_medical detailclaim = new claim_medical();
            if (id != 0)
            {
                detailclaim = db.claim_medical.Find(id);
            }
            return View(detailclaim);
        }

        public JsonResult JsonRejectClaimK()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RejectClaimK", GetRejectClaimK()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RequestClaimK()
        {         
            return View();
        }

        [HttpPost]
        public ActionResult RequestClaimK(claim_medical claim)
        {
            int id = Convert.ToInt32(Session["nik"]);
            saldo_medical ceksaldo = db.saldo_medical.Find(id);
            saldo_awal saldoawal = db.saldo_awal.Find(1);
            if (claim.total_tagihan < 0)
            {
                return Json(new { success = false, message = "Total tagihan tidak boleh minus!" }, JsonRequestBehavior.AllowGet);
            }
            if (claim.tanggal_perawatan > DateTime.Today)
            {
                return Json(new { success = false, message = "Input tanggal tidak valid!" }, JsonRequestBehavior.AllowGet);
            }
            if (claim.tipe_perawatan == "Rawat Inap")
            {
                var cekinap = db.claim_medical.Where(x => x.nik == id && x.status == "Request" && x.tipe_perawatan == "Rawat Inap").FirstOrDefault();
                if (cekinap == null)
                {
                    if(ceksaldo.saldo_rawatinap != 0)
                    {
                        claim.nik = Convert.ToInt32(Session["nik"]);
                        claim.tanggal = DateTime.Today;
                        claim.status = "Request";
                        claim.total_claim = 0;
                        string fileName = Path.GetFileNameWithoutExtension(claim.filekwintasi.FileName);
                        string extension = Path.GetExtension(claim.filekwintasi.FileName);
                        fileName = fileName + claim.nik + DateTime.Now.ToString("yymmssffff") + extension;
                        claim.file_kwintasi = "~/AppFiles/FileUser/" + fileName;
                        claim.filekwintasi.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                        db.claim_medical.Add(claim);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaimK", GetRequestClaimK()), message = "Request claim medical rawat inap berhasil diajukan!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Saldo claim rawat inap anda sudah habis " }, JsonRequestBehavior.AllowGet);
                    }              
                }
                else
                {
                    return Json(new { success = false, message = "Harap tunggu, Request claim inap anda sebelumnya masih diproses" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (claim.tipe_perawatan == "Rawat Jalan")
            {
                var cekjalan = db.claim_medical.Where(x => x.nik == id && x.status == "Request" && x.tipe_perawatan == "Rawat Jalan").FirstOrDefault();
                if (cekjalan == null)
                {
                    if (ceksaldo.saldo_rawatjalan != 0)
                    {
                        claim.nik = Convert.ToInt32(Session["nik"]);
                        claim.tanggal = DateTime.Today;
                        claim.status = "Request";
                        claim.total_claim = 0;
                        string fileName = Path.GetFileNameWithoutExtension(claim.filekwintasi.FileName);
                        string extension = Path.GetExtension(claim.filekwintasi.FileName);
                        fileName = fileName + claim.nik + DateTime.Now.ToString("yymmssffff") + extension;
                        claim.file_kwintasi = "~/AppFiles/FileUser/" + fileName;
                        claim.filekwintasi.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                        db.claim_medical.Add(claim);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaimK", GetRequestClaimK()), message = "Request claim medical rawat jalan berhasil diajukan!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Saldo claim rawat jalan anda sudah habis " }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Harap tunggu, Request claim jalan anda sebelumnya masih diproses" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var cekkacamata = db.claim_medical.Where(x => x.nik == id && x.status == "Request" && x.tipe_perawatan == "Kacamata").FirstOrDefault();
                if (cekkacamata == null)
                {
                    saldo_medical cek = db.saldo_medical.Find(id);
                    if (cek.saldo_kacamata == saldoawal.default_kacamata)
                    {
                        claim.nik = Convert.ToInt32(Session["nik"]);
                        claim.tanggal = DateTime.Today;
                        claim.status = "Request";
                        claim.total_claim = 0;
                        string fileName = Path.GetFileNameWithoutExtension(claim.filekwintasi.FileName);
                        string extension = Path.GetExtension(claim.filekwintasi.FileName);
                        fileName = fileName + claim.nik + DateTime.Now.ToString("yymmssffff") + extension;
                        claim.file_kwintasi = "~/AppFiles/FileUser/" + fileName;
                        claim.filekwintasi.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                        db.claim_medical.Add(claim);
                        db.SaveChanges();
                        return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaimK", GetRequestClaimK()), message = "Request Claim Medical Berhasil Diajukan!" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Claim kacamata hanya bisa sekali dalam 2 tahun!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Harap tunggu, Request claim kacamata anda sedang diproses" }, JsonRequestBehavior.AllowGet);
                }
            }

           
        }

        public ActionResult DeleteClaim(int id)
        {
            try
            {                          
                claim_medical claim = db.claim_medical.Where(x => x.id_claim == id).FirstOrDefault();
                var fullpath = Request.MapPath(claim.file_kwintasi);
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                db.claim_medical.Remove(claim);
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaimK", GetRequestClaimK()), message = "Proses Claim Dibatalkan!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult Download(string filepath)
        {
            var FileVirtualPath = filepath;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        public ActionResult EditClaimK(int id = 0)
        {
            claim_medical claim = new claim_medical();
            if (id != 0)
            {
               claim = db.claim_medical.Where(x => x.id_claim == id).FirstOrDefault<claim_medical>();
            }
            return View(claim);
        }

        [HttpPost]
        public ActionResult EditClaimK(claim_medical claim)
        {
            try
            {
                if (claim.tanggal_perawatan > DateTime.Today)
                {
                    return Json(new { success = false, message = "Input tanggal tidak valid!" }, JsonRequestBehavior.AllowGet);
                }
                claim_medical claim2 = db.claim_medical.Find(claim.id_claim);
                claim2.tanggal = DateTime.Today;
                claim2.tanggal_perawatan = claim.tanggal_perawatan;
                claim2.total_tagihan = claim.total_tagihan;
                claim2.nama_dokter = claim.nama_dokter;
                claim2.nama_pasien = claim.nama_pasien;
                claim2.lokasi_RS = claim.lokasi_RS;
                claim2.no_telp = claim.no_telp;
                claim2.diagnosa = claim.diagnosa;        
                claim2.status = "Request";
                if (claim.file_kwintasi != null)
                {
                    var filelama = db.claim_medical.Where(u => u.id_claim== claim.id_claim).FirstOrDefault();
                    var fullpath = Request.MapPath(filelama.file_kwintasi);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(claim.filekwintasi.FileName);
                    string extension = Path.GetExtension(claim.filekwintasi.FileName);
                    fileName = fileName + claim.nik + DateTime.Now.ToString("yymmssffff") + extension;
                    claim.file_kwintasi = "~/AppFiles/FileUser/" + fileName;
                    claim2.file_kwintasi = claim.file_kwintasi;
                    claim.filekwintasi.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                }          
                db.Entry(claim2).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaimK", GetRequestClaimK()), message = "Edit Data Sukses" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //punya admin
        [HttpGet]
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

        public ActionResult ListRequestClaim()
        {
            return View(GetRequestClaim());
        }

        IEnumerable<claim_medical> GetRequestClaim()
        {
            var allclaim = db.claim_medical.Include(y => y.saldo_medical).Where(u => u.status == "Request").OrderBy(u => u.tanggal);
            return allclaim.ToList();
        }

        public ActionResult RejectClaim()
        {
            return View(GetRejectClaim());
        }

        IEnumerable<claim_medical> GetRejectClaim()
        {
            var allclaim = db.claim_medical.Include(y => y.saldo_medical).Where(u => u.status == "Reject").OrderBy(u => u.tanggal);
            return allclaim.ToList();
        }

        public ActionResult RiwayatClaim()
        {
            return View(GetRiwayatClaim());
        }

        IEnumerable<claim_medical> GetRiwayatClaim()
        {
            var allclaim = db.claim_medical.Include(y => y.saldo_medical).Where(u => u.status == "Finish").OrderBy(u => u.tanggal);
            return allclaim.ToList();
        }

        [HttpPost]
        public ActionResult EditClaim(claim_medical claim)
        {
            try
            {
                claim_medical claim2 = db.claim_medical.Find(claim.id_claim);
                claim2.keterangan = claim.keterangan;
                claim2.status = "Reject";
                db.Entry(claim2).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaim", GetRequestClaim()), message = "Berhasil Reject Claim" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AcceptClaim(int id)
        {
            try
            {
                claim_medical claim2 = db.claim_medical.Find(id);
                claim2.status = "Finish";
                claim2.total_claim =(claim2.total_tagihan * 80) / 100;
                saldo_medical saldo = db.saldo_medical.Find(claim2.nik);
                if (claim2.tipe_perawatan == "Rawat Inap") {
                    if (saldo.saldo_rawatinap < claim2.total_claim)
                    {
                        claim2.total_claim = saldo.saldo_rawatinap;
                        saldo.saldo_rawatinap -= saldo.saldo_rawatinap;
                    }
                    else
                    {
                        saldo.saldo_rawatinap -= claim2.total_claim;
                    }
                }
                if (claim2.tipe_perawatan == "Rawat Jalan")
                {
                    if (saldo.saldo_rawatjalan < claim2.total_claim)
                    {
                        claim2.total_claim = saldo.saldo_rawatjalan;
                        saldo.saldo_rawatjalan -= saldo.saldo_rawatjalan;
                    }
                    else
                    {
                        saldo.saldo_rawatjalan -= claim2.total_claim;
                    }
                }
                if (claim2.tipe_perawatan == "Kacamata")
                {
                    claim2.total_claim = claim2.total_tagihan;
                    if (saldo.saldo_kacamata < claim2.total_claim)
                    {

                        claim2.total_claim = saldo.saldo_kacamata;
                        saldo.saldo_kacamata -= saldo.saldo_kacamata;
                    }
                    else
                    {
                        saldo.saldo_kacamata -= claim2.total_claim;
                    }
                }
                db.Entry(claim2).State = EntityState.Modified;
                db.Entry(saldo).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaim", GetRequestClaim()), message = "Request claim berhasil diterima" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditClaim(int id = 0)
        {
            claim_medical claim = new claim_medical();
            if (id != 0)
            {
                claim = db.claim_medical.Where(x => x.id_claim == id).FirstOrDefault<claim_medical>();
            }
            return View(claim);
        }

        public ActionResult PopUpSaldo(int id = 1)
        {
            saldo_awal saldo = new saldo_awal();
            saldo = db.saldo_awal.Where(x => x.id_saldo == id).FirstOrDefault();
            return View(saldo);
        }

        [HttpPost]
        public ActionResult PopUpSaldo(saldo_awal saldo)
        {
            try
            {
                saldo_awal saldoupdate = db.saldo_awal.Find(1);
                saldoupdate.default_inap = saldo.default_inap;
                saldoupdate.default_jalan = saldo.default_jalan;
                saldoupdate.default_kacamata = saldo.default_kacamata;
                db.Entry(saldoupdate).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ListRequestClaim", GetRequestClaim()), message = "Saldo Awal berhasil diubah" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //loadajax
        public JsonResult JsonRiwayatClaim()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RiwayatClaim", GetRiwayatClaim()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonRejectClaim()
        {
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "RejectClaim", GetRejectClaim()), message = "Proses Selesai" }, JsonRequestBehavior.AllowGet);
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
