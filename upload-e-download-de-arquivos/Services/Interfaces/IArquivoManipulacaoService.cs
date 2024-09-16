namespace upload_e_download_de_arquivos.Interfaces
{
    public interface IArquivoManipulacaoService
    {
        Task<bool> ZiparArquivosAsync(IList<IFormFile> arquivos, string fileName);
        Task<bool> ConcatenarPdfsAsync(IList<IFormFile> arquivos, string fileName);
    }
}
