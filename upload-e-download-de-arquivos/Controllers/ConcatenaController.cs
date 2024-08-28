using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using System.IO.Compression;
using upload_e_download_de_arquivos.Interfaces;

namespace upload_e_download_de_arquivos.Controllers
{
    public class ConcatenaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IArquivoManipulacaoService _arquivoManipulacaoService;

        public ConcatenaController(IWebHostEnvironment webHostEnvironment, IArquivoManipulacaoService arquivoManipulacaoService)
        {
            _webHostEnvironment = webHostEnvironment;
            _arquivoManipulacaoService = arquivoManipulacaoService;
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
        public async Task<IActionResult> ProcessaArquivo(IList<IFormFile> files, string fileName, string actionType)
        {
            bool sucesso;
            switch (actionType)
            {
                case "zip":
                    sucesso = await _arquivoManipulacaoService.ZiparArquivosAsync(files, fileName);
                    TempData["MensagemSucesso"] = sucesso ? "Arquivos compactados e salvos com sucesso!" : "Erro ao compactar arquivos.";
                    break;
                case "concat":
                    sucesso = await _arquivoManipulacaoService.ConcatenarPdfsAsync(files, fileName);
                    TempData["MensagemSucesso"] = sucesso ? "Arquivos concatenados e salvos com sucesso!" : "Erro ao concatenar arquivos.";
                    break;
                default:
                    TempData["MensagemErro"] = "Por favor selecione uma das opções para continuar!";
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
