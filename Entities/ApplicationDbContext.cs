using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<FileDetails> FileDetails { get; set; }
        public DbSet<VirtualFolder> VirtualFolders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FileDetails>().ToTable("FileDetails");
            modelBuilder.Entity<VirtualFolder>().ToTable("VirtualFolders");
        }
    }
}
