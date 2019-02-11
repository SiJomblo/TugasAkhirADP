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
    [RoutePrefix("user")]
    public class usersController : Controller
    {
        private dbOpsModel db = new dbOpsModel();

// punya admin
        [HttpGet]
        [ActionName("index")]
        [Route("index")]
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
        public ActionResult ViewAll()
        {
            return View(GetAllUser());
        }

        IEnumerable<user> GetAllUser()
        {
            int id = Convert.ToInt32(Session["nik"]);
            if (Session["jabatan"].ToString() == "s")
            {
                var users = db.users.Include(u => u.cuti_user).Include(u => u.saldo_medical).Where(u => u.nik != id);
                return users.ToList<user>();
            }
            else
            {
                var users = db.users.Include(u => u.cuti_user).Include(u => u.saldo_medical).Where(u => u.nik != id && u.jabatan == "k");
                return users.ToList<user>();
            }
        
        }

        public ActionResult ResetPassword(string id)
        {
            var user = db.users.Where(u => u.nip == id).FirstOrDefault();         
            if(user != null)
            {
                user emp = db.users.Find(user.nik);
                string nampung = "adins" + DateTime.Today.Year;
                emp.password = md5.encryption(nampung);
                db.Entry(emp).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Password berhasil direset menjadi "+nampung }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }
       

        public ActionResult Details(int id = 0)
        {
            user user = new user();
            if (id != 0)
            {
                user = db.users.Find(id);
            }
            return View(user);   
        }

        public ActionResult AddorEdit(int id = 0)
        {
            user emp = new user();
            if (id != 0)
            {
                emp = db.users.Where(x => x.nik == id).FirstOrDefault<user>();
            }
            else
            {
                var allemp = db.users.Where(x => x.nip.Contains(DateTime.Today.Year.ToString())).ToList();
                int count = allemp.Count + 1;
                string abdul = "";
                if (count <= 9)
                {
                    abdul = "ACE-" + DateTime.Today.Year.ToString() +"-" + "00"+count;
                }
                else if (count <= 100)
                {
                    abdul = "ACE-" + DateTime.Today.Year.ToString() + "-" + "0" + count;
                }
                emp.nip = abdul;
            }  
            return View(emp);
        }

        [HttpPost]
        public ActionResult AddorEdit(user emp, cuti_user cuti, saldo_medical saldo)
        {
            try
            {
                user users = db.users.Find(emp.nik);           
                if (emp.nik == 0)
                {  
                    var cekemail = db.users.Where(u => u.email == emp.email).FirstOrDefault();
                    if (cekemail != null)
                    {
                        return Json(new { success = false, message = "E-Mail sudah terdaftar" }, JsonRequestBehavior.AllowGet);
                    }
                    var ceknip = db.users.Where(u => u.nip == emp.nip).FirstOrDefault();
                    if (ceknip != null)
                    {
                        return Json(new { success = false, message = "NIP sudah terdaftar" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (users.email != emp.email)
                    {
                        var cekemail = db.users.Where(u => u.email == emp.email).FirstOrDefault();
                        if (cekemail != null)
                        {
                            return Json(new { success = false, message = "E-Mail sudah terdaftar" }, JsonRequestBehavior.AllowGet);
                        }                   
                    }
                    if (users.nip != emp.nip)
                    {
                        var ceknip = db.users.Where(u => u.nip == emp.nip).FirstOrDefault();
                        if (ceknip != null)
                        {
                            return Json(new { success = false, message = "NIP sudah terdaftar" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }         
                if (emp.foto != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.foto.FileName);
                    string extension = Path.GetExtension(emp.foto.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    if (emp.nik == 0)
                    {
                      emp.foto_profil = "~/AppFiles/Images/" + fileName;
                    }
                    else {
                        var fotolama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                        var fullpath = Request.MapPath(fotolama.foto_profil);
                        if (emp.foto.ContentType.Contains("image"))
                        {
                        }
                        else
                        {
                            return Json(new { success = false, message = "Format foto profil salah" }, JsonRequestBehavior.AllowGet);
                        }
                        if (fotolama.foto_profil != "~/AppFiles/Images/default.png")
                        {
                            if (System.IO.File.Exists(fullpath))
                            {
                                System.IO.File.Delete(fullpath);
                            }
                        }
                        users.foto_profil = "~/AppFiles/Images/" + fileName;
                    }
                    emp.foto.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                if (emp.filektp != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.filektp.FileName);
                    string extension = Path.GetExtension(emp.filektp.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    if (emp.nik == 0)
                    {
                    emp.file_ktp = "~/AppFiles/FileUser/" + fileName;
                    }
                    else
                    {
                        var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                        var fullpath = Request.MapPath(filelama.file_ktp);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }           
                        users.file_ktp = "~/AppFiles/FileUser/" + fileName;
                    }
                    emp.filektp.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                }
                if (emp.filenpwp != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.filenpwp.FileName);
                    string extension = Path.GetExtension(emp.filenpwp.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    if (emp.nik == 0)
                    {
                        emp.file_npwp = "~/AppFiles/FileUser/" + fileName;
                    }
                    else
                    {
                        var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                        var fullpath = Request.MapPath(filelama.file_npwp);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                        users.file_npwp = "~/AppFiles/FileUser/" + fileName;
                    }
                    emp.filenpwp.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));                  
                    }
                if (emp.filerekening != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.filerekening.FileName);
                    string extension = Path.GetExtension(emp.filerekening.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    if (emp.nik == 0)
                    {
                        emp.file_rekening = "~/AppFiles/FileUser/" + fileName;
                    }
                    else
                    {
                        var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                        var fullpath = Request.MapPath(filelama.file_npwp);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                        users.file_rekening = "~/AppFiles/FileUser/" + fileName;
                    }
                    emp.filerekening.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));                  
                    }
                if (emp.fileijasah != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(emp.fileijasah.FileName);
                    string extension = Path.GetExtension(emp.fileijasah.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    if (emp.nik == 0)
                    {
                        emp.file_ijasah = "~/AppFiles/FileUser/" + fileName;
                    }
                    else
                    {
                        var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                        var fullpath = Request.MapPath(filelama.file_ijasah);
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                        users.file_ijasah = "~/AppFiles/FileUser/" + fileName;
                    }
                    emp.fileijasah.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));            
                    }
                if (emp.nik == 0)
                {
                    //input data jatah cuti,dll saat nik di buat   
                    emp.password = md5.encryption(emp.password);
                    cuti.nik = emp.nik;
                    cuti.jatah_cuti = 0;
                    cuti.sisa_cuti = 0;
                    DateTime joindate = emp.join_date;
                    cuti.valid_jatah = joindate.AddYears(1);
                    cuti.valid_sisa = joindate.AddMonths(18);
                    saldo.nik = emp.nik;
                    saldo.saldo_kacamata = 0;
                    saldo.saldo_rawatinap = 0;
                    saldo.saldo_rawatjalan = 0;
                    saldo.valid_rawat = joindate.AddMonths(3);
                    saldo.valid_kacamata = joindate.AddMonths(3);
                    db.cuti_user.Add(cuti);
                    db.saldo_medical.Add(saldo);
                    db.users.Add(emp);               
                    db.SaveChanges();
                }       
                else
                {
                    users.nip = emp.nip;
                    users.nama = emp.nama;
                    users.email = emp.email;
                    users.no_hp = emp.no_hp;
                    if (emp.tanggal_lahir != null)
                    {
                        users.tanggal_lahir = emp.tanggal_lahir;
                    }
                    users.tempat_lahir = emp.tempat_lahir;
                    users.jenis_kelamin = emp.jenis_kelamin;
                    users.alamat_domisili = emp.alamat_domisili;
                    users.pendidikan_akhir = emp.pendidikan_akhir;
                    users.no_ktp = emp.no_ktp;
                    users.alamat_ktp = emp.alamat_ktp;
                    users.jurusan = emp.jurusan;
                    users.agama = emp.agama;
                    users.npwp = emp.npwp;
                    users.status_pajak = emp.status_pajak;
                    users.status_kerja = emp.status_kerja;
                    users.no_rekening = emp.no_rekening;
                    users.jabatan = emp.jabatan;
                    users.penempatan = emp.penempatan;
                    users.posisi = emp.posisi;
                    users.bpjs = emp.bpjs;
                    db.Entry(users).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ViewAll", GetAllUser()), message = "Edit data sukses" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ViewAll", GetAllUser()), message = "Input Sukses" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult Delete(int id)
        {
            try
            {
                user emp = db.users.Where(x => x.nik == id).FirstOrDefault();
                claim_medical kwi = db.claim_medical.Where(x => x.nik == id).FirstOrDefault();
                if (kwi != null)
                {
                    var pathkwitansi = Request.MapPath(kwi.file_kwintasi);
                    if (System.IO.File.Exists(pathkwitansi))
                    {
                        System.IO.File.Delete(pathkwitansi);
                    }
                }           
                var pathfoto = Request.MapPath(emp.foto_profil);
                var pathktp = Request.MapPath(emp.file_ktp);
                var pathnpwp = Request.MapPath(emp.file_npwp);
                var pathijasah = Request.MapPath(emp.file_ijasah);
                var pathrekening = Request.MapPath(emp.file_rekening);
                if (emp.foto_profil != "~/AppFiles/Images/default.png")
                {
                    if (System.IO.File.Exists(pathfoto))
                    {
                        System.IO.File.Delete(pathfoto);
                    }
                }
                if (System.IO.File.Exists(pathktp))
                {
                    System.IO.File.Delete(pathktp);
                }
                if (System.IO.File.Exists(pathnpwp))
                {
                    System.IO.File.Delete(pathnpwp);
                }
                if (System.IO.File.Exists(pathijasah))
                {
                    System.IO.File.Delete(pathijasah);
                }
                if (System.IO.File.Exists(pathrekening))
                {
                    System.IO.File.Delete(pathrekening);
                }          
                db.users.Remove(emp);
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "ViewAll", GetAllUser()), message = "Delete Data Sukses" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

//punya karyawan 
        [HttpGet]
        public ActionResult IndexKaryawan()
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

        [HttpGet]
        public ActionResult IndexProfil ()
        {
            if (Session["nik"] == null)
            {
                return RedirectToAction("Index", "Login");
            }          
            return View();
        }

        public ActionResult Edit(int id)
        {
            user emp = new user();
            if (id != 0)
            {
                emp = db.users.Where(x => x.nik == id).FirstOrDefault<user>();
            }
            return View(emp);
        }

 //untuk profil
        [HttpPost]
        public ActionResult Edit(user emp, cuti_user jatahcuti, saldo_medical saldo)
        {
            try
            {
                user users = db.users.Find(emp.nik);
                if (users.email != emp.email)
                {
                    var cekemail = db.users.Where(u => u.email == emp.email).FirstOrDefault();
                    if (cekemail != null)
                    {
                        return Json(new { success = false, message = "E-Mail sudah terdaftar" }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (emp.foto != null)
                {
                    var fotolama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                    var fullpath = Request.MapPath(fotolama.foto_profil);
                    if (emp.foto.ContentType.Contains("image"))
                    {
                    }
                    else
                    {
                        return Json(new { success = false, message = "Format foto profil salah" }, JsonRequestBehavior.AllowGet);
                    }
                    if (fotolama.foto_profil != "~/AppFiles/Images/default.png")
                    {
                        if (System.IO.File.Exists(fullpath))
                        {
                            System.IO.File.Delete(fullpath);
                        }
                    }
                    string fileName = Path.GetFileNameWithoutExtension(emp.foto.FileName);
                    string extension = Path.GetExtension(emp.foto.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    users.foto_profil = "~/AppFiles/Images/" + fileName;
                    emp.foto.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                }
                if (emp.filektp != null)
                {
                    var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                    var fullpath = Request.MapPath(filelama.file_ktp);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(emp.filektp.FileName);
                    string extension = Path.GetExtension(emp.filektp.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    users.file_ktp = "~/AppFiles/FileUser/" + fileName;
                    emp.filektp.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                }
                if (emp.filenpwp != null)
                {
                    var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                    var fullpath = Request.MapPath(filelama.file_npwp);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(emp.filenpwp.FileName);
                    string extension = Path.GetExtension(emp.filenpwp.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    users.file_npwp = "~/AppFiles/FileUser/" + fileName;
                    emp.filenpwp.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                }
                if (emp.filerekening != null)
                {
                    var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                    var fullpath = Request.MapPath(filelama.file_rekening);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(emp.filerekening.FileName);
                    string extension = Path.GetExtension(emp.filerekening.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    users.file_rekening = "~/AppFiles/FileUser/" + fileName;
                    emp.filerekening.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                }
                if (emp.fileijasah != null)
                {
                    var filelama = db.users.Where(u => u.nik == emp.nik).FirstOrDefault();
                    var fullpath = Request.MapPath(filelama.file_ijasah);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(emp.fileijasah.FileName);
                    string extension = Path.GetExtension(emp.fileijasah.FileName);
                    fileName = fileName + emp.nama + DateTime.Now.ToString("yymmssffff") + extension;
                    users.file_ijasah = "~/AppFiles/FileUser/" + fileName;
                    emp.fileijasah.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/FileUser/"), fileName));
                }
                users.nama = emp.nama;
                users.email = emp.email;
                
                users.no_hp = emp.no_hp;
                users.tempat_lahir = emp.tempat_lahir;
                if (emp.tanggal_lahir != null)
                {
                    users.tanggal_lahir = emp.tanggal_lahir;
                }
                if (emp.nip != null)
                {
                    users.nip = emp.nip;
                }
                users.oleh = Session["nama"].ToString();
                users.jenis_kelamin = emp.jenis_kelamin;
                users.alamat_domisili = emp.alamat_domisili;
                users.pendidikan_akhir = emp.pendidikan_akhir;
                users.no_ktp = emp.no_ktp;
                users.alamat_ktp = emp.alamat_ktp;
                users.jurusan = emp.jurusan;
                users.agama = emp.agama;
                users.npwp = emp.npwp;
                users.bpjs = emp.bpjs;
                users.status_pajak = emp.status_pajak;
                users.status_kerja = emp.status_kerja;
                users.no_rekening = emp.no_rekening;
                users.jabatan = emp.jabatan;
                users.penempatan = emp.penempatan;
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, html = Global.RenderRazorViewToString(this, "Details", GetDetail()), message = "Edit Data Sukses" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangePassword(FormCollection get)
        {
            int id = Convert.ToInt32(Session["nik"]);
            user user = db.users.Find(id);
            string oldpass = md5.encryption(get["old"]);
            if (user.password != oldpass)
            {
                return Json(new { success = false, message = "Password lama anda salah!" }, JsonRequestBehavior.AllowGet);
            }
            if (get["new"] != get["confirmnew"])
            {
                return Json(new { success = false, message = "Password baru dan konfirmasi password baru tidak sama!" }, JsonRequestBehavior.AllowGet);
            }
            user.password = md5.encryption(get["new"]);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, html = Global.RenderRazorViewToString(this, "Details", GetDetail()), message = "Password berhasil diubah" }, JsonRequestBehavior.AllowGet);
        }

        public user GetDetail()
        {
            int id = Convert.ToInt32(Session["nik"]);
            user detail = db.users.Find(id);
            return detail;
        }

        public ActionResult Profiles()
        {
            return View("Details", GetDetail());
        }

        public JsonResult JsonGetEmailEdit(string userdata)
        {
            int id = Convert.ToInt32(Session["nik"]);
            user users = db.users.Find(id);
            if (users.email == userdata)
            {
                return Json(2);
            }
            else
            {
                var data = db.users.Where(x => x.email == userdata).SingleOrDefault();
                if (data != null)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }

        }

        public JsonResult JsonGetNIPEdit(string userdata)
        {
            int id = Convert.ToInt32(Session["nik"]);
            user users = db.users.Find(id);
            if (users.nip == userdata)
            {
                return Json(2);
            }
            else
            {
                var data = db.users.Where(x => x.nip == userdata).SingleOrDefault();
                if (data != null)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
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
