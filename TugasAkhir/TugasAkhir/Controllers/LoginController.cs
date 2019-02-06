using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TugasAkhir.Models;
using System.Web.Security;
using System.Data.Entity;

namespace TugasAkhir.Controllers
{
    public class LoginController : Controller
    {
        [Route("login")]
        public ActionResult Index()
        {
            if (Session["nik"] != null)
            {
                if (Session["jabatan"].ToString() == "k")
                {
                    return RedirectToAction("Karyawan", "Login");
                }
                else
                {
                    return RedirectToAction("Admin", "Login");
                }         
            }
            return View();
        }

        [HttpGet]
        [Route("Admin")]
        public ActionResult Admin()
        {
            using (dbOpsModel db = new dbOpsModel())
            {
                if (Session["nik"] != null)
                {
                    if (Session["jabatan"].ToString() == "k")
                    {
                        return RedirectToAction("Karyawan", "Login");
                    }             
                }
                int id = Convert.ToInt32(Session["nik"]);
                user user = db.users.Where(x => x.nik == id).FirstOrDefault();
                if(user.foto_profil != null)
                {
                    ViewBag.Foto = Url.Content(user.foto_profil);
                }
                ViewBag.Nama = user.nama;

                int slip = db.slipgajis.Where(u => u.status == "Request").Count();
                ViewData["NSlipGaji"] = slip;

                int rekening = db.surats.Where(u => u.status == "Request" && u.id_jenis == 1).Count();
                ViewData["NRekening"] = rekening;

                int kerja = db.surats.Where(u => u.status == "Request" && u.id_jenis == 2).Count();
                ViewData["NKerja"] = kerja;

                int npwp = db.surats.Where(u => u.status == "Request" && u.id_jenis== 3).Count();
                ViewData["NNPWP"] = npwp;

                int pemberitahuan = db.surats.Where(u => u.status == "Request" && u.id_jenis== 4).Count();
                ViewData["NPemberitahuan"] = pemberitahuan;

                int visa = db.surats.Where(u => u.status == "Request" && u.id_jenis == 5).Count();
                ViewData["NVisa"] = visa;

                int total = rekening + kerja + npwp + pemberitahuan;
                ViewData["NTotal"] = total;

                int claim = db.claim_medical.Where(u => u.status == "Request" ).Count();
                ViewData["NClaim"] = claim;
            }
            return View("../Admin/Index");
        }

        [HttpGet]
        [Route("Karyawan")]
        public ActionResult Karyawan()
        {
            using (dbOpsModel db = new dbOpsModel())
            {
                int id = Convert.ToInt32(Session["nik"]);
                user user = db.users.Where(x => x.nik == id).FirstOrDefault();        
                ViewBag.Foto = Url.Content(user.foto_profil);
                ViewBag.Nama = user.nama;
                return View("../Karyawan/Index");
            }
        }

        [HttpPost]
        public ActionResult Validate(user user, string returnUrl)
        {
            using (dbOpsModel db = new dbOpsModel())
            {
                string email = Convert.ToString(user.email).ToUpper().Trim();
                string pass = md5.encryption(user.password);
                //string pass = user.password;
                var userDetails = db.users.Where(x => x.email.ToUpper() == email && x.password == pass).FirstOrDefault();
                if (userDetails == null)
                {
                    ViewBag.msg = "<script>Swal.fire({ type: 'error', title: 'Login Gagal!', text: 'E-Mail atau Password salah'});</script>";
                    return View("Index");
                }       
                else if (userDetails.status_kerja == "Tidak Aktif")
                {
                    ViewBag.msg = "<script>Swal.fire({ type: 'error', title: 'Login Gagal!', text: 'Akun anda sudah dinon-aktifkan!'});</script>";
                    return View("Index");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(user.email, false);
                    Session["nik"] = userDetails.nik;
                    Session["nama"] = userDetails.nama;
                    Session["jabatan"] = userDetails.jabatan;
                    if (userDetails.jabatan == "a" || userDetails.jabatan == "s")
                    {
                        return RedirectToAction("Admin", "Login");
                    }
                    else if (userDetails.jabatan == "k")
                    {
                        //renewal cuti
                        cuti_user cek = db.cuti_user.Find(userDetails.nik);
                        if (cek.valid_jatah <= DateTime.Today)
                        {
                            while (cek.valid_jatah <= DateTime.Today)
                            {
                                DateTime validdate2 = cek.valid_jatah;
                                cek.valid_jatah = validdate2.AddYears(1);
                            }
                            DateTime validdate = cek.valid_jatah;
                            if (cek.jatah_cuti > 0)
                            {
                                cek.sisa_cuti = cek.jatah_cuti;
                                cek.jatah_cuti += 12;
                                cek.jatah_cuti -= cek.sisa_cuti;
                                cek.valid_sisa = validdate.AddMonths(6);
                            }
                            else
                            {
                                cek.jatah_cuti += 12;
                                cek.valid_sisa = validdate.AddMonths(6);
                            }
                        }
                        if (cek.valid_sisa <= DateTime.Today)
                        {
                            cek.sisa_cuti = 0;
                        }
                        db.Entry(cek).State = EntityState.Modified;
                        db.SaveChanges();
                        //akhir renewal cuti
                        //renewal saldo medical
                        saldo_medical valid = db.saldo_medical.Find(userDetails.nik);
                        saldo_awal saldoawal = db.saldo_awal.Find(1);
                        if (valid.valid_rawat <= DateTime.Today)
                        {
                            valid.saldo_rawatinap = saldoawal.default_inap;
                            valid.saldo_rawatjalan = saldoawal.default_jalan;
                            if (valid.valid_rawat == userDetails.join_date.AddMonths(3))
                            {
                                DateTime validdate = userDetails.join_date;
                                valid.valid_rawat = validdate.AddYears(1);
                                while (valid.valid_rawat <= DateTime.Today)
                                {
                                    DateTime validdate2 = valid.valid_rawat;
                                    valid.valid_rawat = validdate2.AddYears(1);
                                }
                            }
                            else
                            {
                                DateTime validdate = valid.valid_rawat;
                                valid.valid_rawat = validdate.AddYears(1);
                            }                         
                        }
                        if (valid.valid_kacamata <= DateTime.Today)
                        {
                            valid.saldo_kacamata = saldoawal.default_kacamata;
                            if (valid.valid_kacamata == userDetails.join_date.AddMonths(3))
                            {
                                DateTime validkacamata = userDetails.join_date;
                                valid.valid_kacamata = validkacamata.AddYears(2);
                                while (valid.valid_kacamata <= DateTime.Today)
                                {
                                    DateTime validdate2 = valid.valid_kacamata;
                                    valid.valid_kacamata = validdate2.AddYears(2);
                                }
                                int tai = Convert.ToInt32(userDetails.join_date.Year);
                                if (tai%2 == 0)
                                {
                                    DateTime validdate2 = valid.valid_kacamata;
                                    valid.valid_kacamata = validdate2.AddYears(1);
                                }
                            }
                            else
                            {
                                DateTime validkacamata = valid.valid_kacamata;
                                valid.valid_kacamata = validkacamata.AddYears(2);
                            }                         
                        }
                        db.Entry(valid).State = EntityState.Modified;
                        //end renewalmedical
                        db.SaveChanges();
                        return RedirectToAction("Karyawan", "Login");
                    }
                    else
                    {
                        return View("../Home/Index");
                    }
                }
            }
        }

        [ActionName("LogOut")]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}