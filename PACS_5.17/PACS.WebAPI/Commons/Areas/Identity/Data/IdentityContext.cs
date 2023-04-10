using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PACS.Shared.Entities;
using PACS.WebAPI.Areas.Identity.Data;
using PACS.WebAPI.Commons.Areas.Identity.Data;

namespace PACS.WebAPI.Data
{
    public class IdentityContext : IdentityDbContext<AppUser>
    {
        public DbSet<FileFolderModel> FileFolders { get; set; }
        public DbSet<FileItemModel> FileItems { get; set; }
        public DbSet<FileMaskModel> FileMasks { get; set; }
        public DbSet<UserFileFolders> UserFileFolders { get; set; }
        public DbSet<FolderFiles> FolderFiles { get; set; }

        public DbSet<AutoDiagnoseItemModel> AutoDiagnoseItems { get; set; }


        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<UserFileFolders>().HasKey(e => new { e.UserId, e.FileFolderId });
            builder.Entity<FolderFiles>().HasKey(e => new { e.FileId, e.FileFolderId });


            
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {

            
        }
    }
}
