using Microsoft.EntityFrameworkCore;
using PACS.Commons.Models;
using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Commons.Entities
{
    public class DataContext : DbContext
    {
        public DbSet<FileFolderModel> FileFolders { get; set; }
        public DbSet<FileItemModel> FileItems { get; set; }
        public DbSet<FileMaskModel> FileMasks { get; set; }
        public DbSet<FolderFiles> FolderFiles { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlite("Data Source=PACS.Client.Data.db");
            
            //optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FolderFiles>().HasKey(e => new { e.FileId, e.FileFolderId });
        }
    }
}
