using Microsoft.EntityFrameworkCore;
using upload_e_download_de_arquivos.Infraestruture;
using upload_e_download_de_arquivos.Interfaces;
using upload_e_download_de_arquivos.Models;

namespace upload_e_download_de_arquivos.Services
{
    public class ArquivoService : IArquivoService
    {
        private readonly ArquivoContext _context;

        public ArquivoService(ArquivoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArquivoModel>> ListarArquivosAsync()
        {
            return await _context.Arquivos.AsNoTracking().ToListAsync();
        }

        public async Task<bool> UploadArquivoAsync(IFormFile file, string descricao)
        {
            try
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                var arquivo = new ArquivoModel
                {
                    Descricao = descricao,
                    Dados = ms.ToArray(),
                    ContentType = file.ContentType,
                    DataEnvio = DateTime.Now,
                };

                _context.Arquivos.Add(arquivo);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ArquivoModel> VisualizarArquivoAsync(int id)
        {
            return await _context.Arquivos.AsNoTracking().FirstOrDefaultAsync(a => a.Id_Arquivo == id);
        }

        public async Task<bool> DeletarArquivoAsync(int id)
        {
            try
            {
                var arquivo = await _context.Arquivos.FindAsync(id);
                if (arquivo == null)
                    return false;

                _context.Arquivos.Remove(arquivo);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
