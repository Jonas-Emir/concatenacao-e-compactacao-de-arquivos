using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.IO.Compression;
using upload_e_download_de_arquivos.Interfaces;

namespace upload_e_download_de_arquivos.Services
{
    public class ArquivoManipulacaoService : IArquivoManipulacaoService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArquivoManipulacaoService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> ZiparArquivosAsync(IList<IFormFile> arquivos, string fileName)
        {
            try
            {
                var caminhoPasta = Path.Combine(_webHostEnvironment.WebRootPath, "arquivos");
                var caminhoArquivoZip = Path.Combine(caminhoPasta, $"{fileName}.zip");

                using (var zipStream = new FileStream(caminhoArquivoZip, FileMode.Create))
                {
                    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                    {
                        foreach (var arquivo in arquivos)
                        {
                            var caminhoArquivo = Path.Combine(caminhoPasta, arquivo.FileName);
                            using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
                            {
                                await arquivo.CopyToAsync(fileStream);
                            }

                            archive.CreateEntryFromFile(caminhoArquivo, arquivo.FileName);
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ConcatenarPdfsAsync(IList<IFormFile> arquivos, string fileName)
        {
            try
            {
                var caminhoPasta = Path.Combine(_webHostEnvironment.WebRootPath, "arquivos");
                var caminhoArquivoConcat = Path.Combine(caminhoPasta, $"{fileName}.pdf");

                using (var outputDocument = new PdfDocument())
                {
                    foreach (var arquivo in arquivos)
                    {
                        var caminhoArquivo = Path.Combine(caminhoPasta, arquivo.FileName);
                        using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(fileStream);
                        }

                        using (var inputDocument = PdfReader.Open(caminhoArquivo, PdfDocumentOpenMode.Import))
                        {
                            foreach (var page in inputDocument.Pages)
                            {
                                outputDocument.AddPage(page);
                            }
                        }
                    }

                    outputDocument.Save(caminhoArquivoConcat);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
