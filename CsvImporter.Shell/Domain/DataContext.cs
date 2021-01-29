namespace CsvImporter.Shell.Domain
{
    using CsvImporter.Shell.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Configuration;
    public class DataContext : DbContext
    {
        #region dbsets
        public DbSet<StockItem> Stoks { get; set; }
        #endregion
        public DataContext()
        {
        }
        public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, b => b.MigrationsAssembly("pruebaCsv"))
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            base.OnConfiguring(optionsBuilder);
        }


    }
}
