using Microsoft.AspNetCore.Mvc;
using upload_e_download_de_arquivos.Infraestruture;
using upload_e_download_de_arquivos.Models;

namespace upload_e_download_de_arquivos.Controllers
{
    public class ArquivosController : Controller
    {
        ArquivoContext _arquivoContext;

        public ArquivosController(ArquivoContext arquivoContext)
        {
            _arquivoContext = arquivoContext;
        }

        public IActionResult Index()
        {
            var arquivos = _arquivoContext.Arquivos.ToList();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.ContentType != null && arquivo.ContentType.Contains("/"))
                    arquivo.ContentType = arquivo.ContentType.Split('/')[1];
            }
            return View(arquivos);
        }

        [HttpPost]
        public IActionResult UploadArquivo(IList<IFormFile> arquivos, string descricaoArquivo)
        {
            IFormFile imagemCarregada = arquivos.FirstOrDefault();

            if (imagemCarregada != null)
            {
                MemoryStream ms = new MemoryStream();
                imagemCarregada.OpenReadStream().CopyTo(ms);

                ArquivoModel arquivo = new ArquivoModel()
                {
                    Descricao = descricaoArquivo,
                    Dados = ms.ToArray(),
                    ContentType = imagemCarregada.ContentType,
                    DataEnvio = DateTime.Now,
                };
                _arquivoContext.Arquivos.Add(arquivo);
                _arquivoContext.SaveChanges();
            }
            else
                TempData["Mensagem"] = "Nenhum arquivo selecionado. Por favor, selecione um arquivo para continuar!";

            return RedirectToAction("Index");
        }

        public IActionResult Visualizar(int id)
        {
            var arquivosBanco = _arquivoContext.Arquivos.FirstOrDefault(a => a.Id_Arquivo == id);

            return File(arquivosBanco.Dados, arquivosBanco.ContentType);
        }

        public IActionResult DeletaArquivo(int id)
        {
            var arquivo = _arquivoContext.Arquivos.FindAsync(id);
            if (arquivo.Result == null)
                return NotFound();

            _arquivoContext.Arquivos.Remove(arquivo.Result);
            _arquivoContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
