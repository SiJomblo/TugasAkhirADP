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
    
    public partial class cuti
    {
        public int id_cuti { get; set; }
        public int nik { get; set; }
        public System.DateTime tanggal { get; set; }
        public System.DateTime durasi_awal { get; set; }
        public System.DateTime durasi_akhir { get; set; }
        public string alamat { get; set; }
        public string no_telp { get; set; }
        public string keterangan { get; set; }
        public string oleh { get; set; }
    
        public virtual user user { get; set; }
    }
}
