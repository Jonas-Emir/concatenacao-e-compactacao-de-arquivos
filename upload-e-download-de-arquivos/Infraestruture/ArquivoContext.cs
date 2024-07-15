using Microsoft.EntityFrameworkCore;
using upload_e_download_de_arquivos.Models;

namespace upload_e_download_de_arquivos.Infraestruture
{
    public class ArquivoContext:DbContext
    {
        public ArquivoContext(DbContextOptions<ArquivoContext> options) : base(options)
        { }

        public DbSet<ArquivoModel> Arquivos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {    
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ArquivoModel>().HasKey(t => t.Id_Arquivo);
        }
    }
}
