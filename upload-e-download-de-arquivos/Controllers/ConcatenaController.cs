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
        public async Task<IActionResult> ProcessaArquivo(IList<IFormFile> arquivos, string fileName, string actionType, PdfSharpCore.Pdf.PdfDocument? destinoArquivo = null)
        {
            switch (actionType)
            {
                case "zip":
                    return await ZiparArquivos((List<IFormFile>)arquivos, fileName);
                case "concat":
                    return await ConcatenarPdfs((List<IFormFile>)arquivos);
                default:
                    ViewBag.Message = "Por favor selecione uma das opções para continuar!";
                    return View("Index");
            }
        }

        public async Task<IActionResult> ConcatenarPdfs(IList<IFormFile> arquivos)
        {
            var caminhoUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var outputPdfPath = Path.Combine(caminhoUploads, "ConcatenatedOutput.pdf");

            var pdfStreams = new List<(Stream stream, string fileName)>();

            foreach (var file in arquivos)
            {
                if (file.ContentType != "application/pdf")
                {
                    ViewBag.Message = "Selecione apenas arquivos .PDF";
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
                foreach (var (pdfStream, fileName) in pdfStreams)
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
                        ViewBag.Message = ($"Erro ao processar o arquivo '{fileName}'. Arquivo corrompido ou inválido! Os demais arquivos foram concatenados.");
                    }
                }
                if (outputDocument.PageCount > 0)
                    outputDocument.Save(outputPdfPath);
                else
                    ViewBag.Message = ($"Nenhuma página identificada para criar o novo arquivo!");
            }
            else
                ViewBag.Message = ($"Por favor, selecione mais de um arquivo para concatenar!");

            return View("Index");
        }

        public async Task<IActionResult> ZiparArquivos(List<IFormFile> files, string fileName)
        {
            string zipFileName = fileName != null ? $"{fileName}.zip" : $"_{DateTime.Now:yyyyMMddHHmmss}.zip";
            string zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);

            if (files != null && files.Count > 0)
            {
                try
                {
                    using (var zipToOpen = new FileStream(zipFilePath, FileMode.Create))
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
                    }

                    return File(System.IO.File.ReadAllBytes(zipFilePath), "application/zip", zipFileName);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"An error occurred: {ex.Message}";
                    return View("Index");
                }
            }
            else
                ViewBag.Message = "Nenhuma arquivo selecionado!";

            return View("Index");
        }
    }
}
