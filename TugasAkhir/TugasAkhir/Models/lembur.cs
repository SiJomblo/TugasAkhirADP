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

    public partial class lembur
    {
        public int id_lembur { get; set; }
        public int nik { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan jam_mulai { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan jam_selesai { get; set; }
        public Nullable<System.DateTime> tanggal { get; set; }
        public Nullable<decimal> reward { get; set; }
        public Nullable<decimal> totalreward { get; set; }
        public string status { get; set; }
        public string oleh { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public System.TimeSpan total_jam { get; set; }
    
        public virtual user user { get; set; }
    }
}