using Microsoft.AspNetCore.Mvc;
using upload_e_download_de_arquivos.Interfaces;

namespace upload_e_download_de_arquivos.Controllers
{
    public class ArquivosController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IArquivoService _arquivoService;
        private readonly IArquivoManipulacaoService _arquivoManipulacaoService;

        public ArquivosController(IWebHostEnvironment webHostEnvironment, IArquivoService arquivoService, IArquivoManipulacaoService arquivoManipulacaoService)
        {
            _webHostEnvironment = webHostEnvironment;
            _arquivoService = arquivoService;
            _arquivoManipulacaoService = arquivoManipulacaoService;
        }

        public async Task<IActionResult> Index()
        {
            var arquivos = await _arquivoService.ListarArquivosAsync();
            return View(arquivos);
        }

        [HttpPost]
        public async Task<IActionResult> UploadArquivo(IList<IFormFile> arquivos, string descricaoArquivo)
        {
            var imagemCarregada = arquivos.FirstOrDefault();

            if (imagemCarregada != null)
            {
                if (!string.IsNullOrWhiteSpace(descricaoArquivo))
                {
                    var sucesso = await _arquivoService.UploadArquivoAsync(imagemCarregada, descricaoArquivo);

                    TempData["MensagemSucesso"] = sucesso ? "Arquivo enviado com sucesso!" : "Falha ao enviar o arquivo!";
                }
                else
                    TempData["MensagemErro"] = "Necessário atribuir uma descrição!";
            }
            else
                TempData["MensagemErro"] = "Nenhum arquivo selecionado!";

            return RedirectToAction(nameof(Index));
        }

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

        public async Task<IActionResult> Visualizar(int id)
        {
            var arquivo = await _arquivoService.VisualizarArquivoAsync(id);
            if (arquivo == null)
                return NotFound();

            return File(arquivo.Dados, arquivo.ContentType);
        }

        public async Task<IActionResult> DeletaArquivo(int id)
        {
            var sucesso = await _arquivoService.DeletarArquivoAsync(id);

            TempData["MensagemTabelaSucesso"] = sucesso ? "Arquivo excluído com sucesso!" : "Falha ao excluir o arquivo!";

            return RedirectToAction(nameof(Index));
        }
    }
}
