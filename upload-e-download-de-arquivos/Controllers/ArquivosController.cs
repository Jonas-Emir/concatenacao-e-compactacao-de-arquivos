using Microsoft.AspNetCore.Mvc;

namespace upload_e_download_de_arquivos.Controllers
{
    public class ArquivosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
