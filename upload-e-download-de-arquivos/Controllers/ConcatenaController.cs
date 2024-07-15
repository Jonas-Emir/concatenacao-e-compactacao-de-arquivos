using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.IO.Compression;
using upload_e_download_de_arquivos.Infraestruture;

namespace upload_e_download_de_arquivos.Controllers
{
    public class ConcatenaController : Controller
    {
        ArquivoContext _arquivoContext;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConcatenaController(ArquivoContext arquivoContext, IWebHostEnvironment webHostEnvironment)
        {
            _arquivoContext = arquivoContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Concatena()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessaArquivo(IList<IFormFile> files, string fileName, string actionType, PdfSharpCore.Pdf.PdfDocument? destinationFile = null)
        {
            switch (actionType)
            {
                case "zip":
                    return await ZiparArquivos((List<IFormFile>)files, fileName);
                case "concat":
                    return await ConcatenarPdfs((List<IFormFile>)files, fileName);
                default:
                    TempData["MensagemErro"] = "Por favor selecione uma das opções para continuar!";
                    return View("Index");
            }
        }

        public async Task<IActionResult> ConcatenarPdfs(IList<IFormFile> files, string fileName)
        {
            var pdfStreams = new List<(Stream stream, string fileName)>();
            string destinationUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string compactadosFolder = Path.Combine(destinationUploads, "ArquivosConcatenados");

            if (!Directory.Exists(compactadosFolder))
            {
                Directory.CreateDirectory(compactadosFolder);
            }

            var outputPdfPath = Path.Combine(compactadosFolder, fileName + ".pdf");

            foreach (var file in files)
            {
                if (file.ContentType != "application/pdf")
                {
                    TempData["MensagemErro"] = "Selecione apenas arquivos .PDF";
                    return View("Index");
                }
                else
                {
                    var memoryStream = new MemoryStream();
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    pdfStreams.Add((memoryStream, file.FileName));
                }
            }

            using var outputDocument = new PdfDocument();

            if (pdfStreams.Count > 1)
            {
                foreach (var (pdfStream, file) in pdfStreams)
                {
                    try
                    {
                        using (var inputDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import))
                        {
                            int pageCount = inputDocument.PageCount;
                            for (int pageIndex = 0; pageIndex < pageCount; pageIndex++)
                            {
                                PdfPage page = inputDocument.Pages[pageIndex];
                                outputDocument.AddPage(page);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["MensagemErro"] = ($"Erro ao processar o arquivo '{fileName}'. Arquivo corrompido ou inválido! Os demais arquivos foram concatenados.");
                    }
                }
                if (outputDocument.PageCount > 0)
                {
                    outputDocument.Save(outputPdfPath);
                    TempData["MensagemSucesso"] = "Arquivos concatenados e salvos com sucesso!";
                }
                else
                    TempData["MensagemErro"] = ($"Nenhuma página identificada para criar o novo arquivo!");
            }
            else
                TempData["MensagemErro"] = ($"Por favor, selecione mais de um arquivo para concatenar!");

            return View("Index");
        }

        public async Task<IActionResult> ZiparArquivos(List<IFormFile> files, string fileName)
        {
            string zipFileName = fileName != null ? $"{fileName}.zip" : $"_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string destinationUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string compactadosFolder = Path.Combine(destinationUploads, "ArquivosCompactados");

            if (!Directory.Exists(compactadosFolder))
                Directory.CreateDirectory(compactadosFolder);

            var outputZipPath = Path.Combine(compactadosFolder, zipFileName);

            if (files != null && files.Count > 0)
            {
                try
                {
                    using (var zipToOpen = new FileStream(outputZipPath, FileMode.Create, FileAccess.Write))
                    {
                        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                        {
                            foreach (var file in files)
                            {
                                var fileEntry = archive.CreateEntry(file.FileName);
                                using (var entryStream = fileEntry.Open())
                                using (var fileStream = file.OpenReadStream())
                                {
                                    await fileStream.CopyToAsync(entryStream);
                                }
                            }
                        }

                        File(System.IO.File.ReadAllBytes(outputZipPath), "application/zip", zipFileName);
                        TempData["MensagemSucesso"] = "Arquivos compactados e salvos com sucesso!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = $"Ocorreu um erro ao compactar arquivos: {ex.Message}";
                    return View("Index");
                }
            }
            else
                TempData["MensagemErro"] = "Nenhuma arquivo selecionado!";

            return View("Index");
        }
    }
}
