/*using Microsoft.EntityFrameworkCore;
using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Commons
{
    public class DataContext : DbContext
    {
        public DbSet<FileFolderModel> FileFolders { get; set; }
        public DbSet<FileItemModel> FileItems { get; set; }
        public DbSet<FileMaskModel> FileMasks { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite("Data Source=PACS.WebAPI.Data.db");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
*/