﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbOpsModel : DbContext
    {
        public dbOpsModel()
            : base("name=dbOpsModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<claim_medical> claim_medical { get; set; }
        public virtual DbSet<cuti> cutis { get; set; }
        public virtual DbSet<cuti_massal> cuti_massal { get; set; }
        public virtual DbSet<cuti_user> cuti_user { get; set; }
        public virtual DbSet<detail_surat> detail_surat { get; set; }
        public virtual DbSet<jenis_surat> jenis_surat { get; set; }
        public virtual DbSet<lembur> lemburs { get; set; }
        public virtual DbSet<saldo_awal> saldo_awal { get; set; }
        public virtual DbSet<saldo_medical> saldo_medical { get; set; }
        public virtual DbSet<slipgaji> slipgajis { get; set; }
        public virtual DbSet<surat> surats { get; set; }
        public virtual DbSet<user> users { get; set; }
    }
}
