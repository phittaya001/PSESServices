﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PesWeb.Service.Security
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class SecurityEntities : DbContext
    {
        public SecurityEntities()
            : base("name=SecurityEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public DbSet<tbs_Group> tbs_Group { get; set; }
        public DbSet<tbs_PermissionGroupMap> tbs_PermissionGroupMap { get; set; }
        public DbSet<tbs_PermissionUserMap> tbs_PermissionUserMap { get; set; }
        public DbSet<tbs_RestrictControlItem> tbs_RestrictControlItem { get; set; }
        public DbSet<tbs_ScreenPermission> tbs_ScreenPermission { get; set; }
        public DbSet<tbs_User> tbs_User { get; set; }
        public DbSet<tbs_UserGroup> tbs_UserGroup { get; set; }
        public DbSet<tbs_Permission> tbs_Permission { get; set; }
        public DbSet<tbs_ScreenItem> tbs_ScreenItem { get; set; }
    }
}