//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TugasAkhir.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    public partial class user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public user()
        {
            this.cutis = new HashSet<cuti>();
            this.lemburs = new HashSet<lembur>();
            this.slipgajis = new HashSet<slipgaji>();
            this.surats = new HashSet<surat>();
            foto_profil = "~/AppFiles/Images/default.png";
        }

        public int nik { get; set; }
        public string nama { get; set; }
        public string email { get; set; }
        [StringLength(100, ErrorMessage = "Panjang password minimal 6 karakter", MinimumLength = 6)]
        public string password { get; set; }
        public string tempat_lahir { get; set; }
        public Nullable<System.DateTime> tanggal_lahir { get; set; }
        public string jenis_kelamin { get; set; }
        public string no_hp { get; set; }
        public string alamat_domisili { get; set; }
        public string no_ktp { get; set; }
        public string alamat_ktp { get; set; }
        public string pendidikan_akhir { get; set; }
        public string jurusan { get; set; }
        public string agama { get; set; }
        public string npwp { get; set; }
        public string bpjs { get; set; }
        public string status_pajak { get; set; }
        public System.DateTime join_date { get; set; }
        public string no_rekening { get; set; }
        public string jabatan { get; set; }
        public string penempatan { get; set; }
        public string posisi { get; set; }
        public string status_kerja { get; set; }
        public string file_ktp { get; set; }
        public string file_npwp { get; set; }
        public string file_ijasah { get; set; }
        public string file_rekening { get; set; }
        public string foto_profil { get; set; }
        [NotMapped]
        public HttpPostedFileBase foto { get; set; }
        [NotMapped]
        public HttpPostedFileBase filektp { get; set; }
        [NotMapped]
        public HttpPostedFileBase filenpwp { get; set; }
        [NotMapped]
        public HttpPostedFileBase filerekening { get; set; }
        [NotMapped]
        public HttpPostedFileBase fileijasah { get; set; }
        public string nip { get; set; }
        public string oleh { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cuti> cutis { get; set; }
        public virtual cuti_user cuti_user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<lembur> lemburs { get; set; }
        public virtual saldo_medical saldo_medical { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<slipgaji> slipgajis { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<surat> surats { get; set; }
    }
}
