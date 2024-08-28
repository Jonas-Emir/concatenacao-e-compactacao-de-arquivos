using System.Collections.Generic;
using System.Threading.Tasks;
using upload_e_download_de_arquivos.Models;

namespace upload_e_download_de_arquivos.Interfaces
{
    public interface IArquivoService
    {
        Task<IEnumerable<ArquivoModel>> ListarArquivosAsync();
        Task<bool> UploadArquivoAsync(IFormFile arquivo, string descricao);
        Task<ArquivoModel> VisualizarArquivoAsync(int id);
        Task<bool> DeletarArquivoAsync(int id);
    }
}
