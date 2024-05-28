using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
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

        public IActionResult Conversor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CarregarPdfOrigem(IList<IFormFile> arquivos, string nomeArquivo, PdfSharpCore.Pdf.PdfDocument? destinoArquivo = null)
        {
            var pdfStreams = new List<(Stream stream, string fileName)>();
            var caminhoUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            foreach (var file in arquivos)
            {
                if (file.ContentType != "application/pdf")
                {
                    ViewBag.Message = "Only PDF files are allowed.";
                    return View("Index");
                }

                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                memoryStream.Position = 0;

                pdfStreams.Add((memoryStream, file.FileName));
            }

            var outputFilePath = Path.Combine(caminhoUploads, "ConcatenatedOutput.pdf");

            ConcatenatePdfs(pdfStreams, outputFilePath);

            return RedirectToAction("Index");
        }

        public static void ConcatenatePdfs(List<(Stream stream, string fileName)> pdfStreams, string outputPdfPath)
        {
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
                        Console.WriteLine($"Erro ao processar o PDF '{fileName}'. Arquivo corrompido ou inválido!");

                        //   TempData["Mensagem"] = "Nenhum arquivo selecionado. Por favor, selecione um arquivo para continuar!";
                    }
                }
                if (outputDocument.PageCount > 0)
                    outputDocument.Save(outputPdfPath);
                else
                    Console.WriteLine($"Nenhuma página identificada para criar o novo arquivo!");
            }
            else
                Console.WriteLine($"Por favor, selecione mais de um arquivo para continuar!");
        }

        public void BuscarPastaDestino()
        {

        }

        public void PdfMergeDinamico()
        {
            
        }
    }
}
