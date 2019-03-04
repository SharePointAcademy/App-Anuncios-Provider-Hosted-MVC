using AppAnunciosMVCWeb.Models;
using AppAnunciosMVCWeb.Repository;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppAnunciosMVCWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            SharePointContext spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            List<Veiculo> lstVeiculos = Veiculo.ObterVeiculos(spContext, query);
            return View(lstVeiculos);
        }

        [SharePointContextFilter]
        public ActionResult Create()
        {
            return View();
        }

        [SharePointContextFilter]
        [HttpPost]
        public ActionResult Create(Veiculo veiculo, HttpPostedFileBase[] files)
        {
            string relativeURL = string.Empty;
            SharePointContext spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            //a primeira foto selecionada é setada como a principal
            veiculo.Foto = files[0].FileName;

            string idVeiculo = Util.ReturnString(VeiculoRepository.CriarVeiculo(spContext, Constants.Listas.Veiculos, veiculo));

            if (idVeiculo != "0")
            {
                relativeURL = VeiculoRepository.CreateFolder(Constants.Listas.VeiculosImagens, idVeiculo.ToString(), spContext);

                foreach (HttpPostedFileBase file in files)
                {

                    if (file != null)
                    {
                        var sFileUrl = string.Format("{0}/Lists/{1}/{2}/{3}", relativeURL, Constants.Listas.VeiculosImagens, idVeiculo, file.FileName);

                        FileCreationInformation newFile = new FileCreationInformation();
                        newFile.Content = Util.ReadFully(file.InputStream);
                        newFile.Url = sFileUrl;

                        VeiculoRepository.UploadFile(spContext, newFile, Constants.Listas.VeiculosImagens);
                    }
                }
            }

            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        [SharePointContextFilter]
        public ActionResult Detail(string id)
        {
            SharePointContext spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            Veiculo veiculo = Veiculo.ObterVeiculo(spContext, Util.ReturnInteger(id));

            #region obtem imagens do veiculo

            List<VeiculoImagens> lstVeiculosImagens = VeiculoRepository.ListarVeiculosImagens(spContext, Constants.Listas.VeiculosImagens, id);

            string htmlFinal = "<ul class='listaImagens'>";
            string html = string.Empty;
            string urlImagem = spContext.SPAppWebUrl + "Lists/" + Constants.Listas.VeiculosImagens + "/" + id + "/";

            foreach (var imagemVeiculo in lstVeiculosImagens)
            {
                html += "<li><img width='250' src='" + urlImagem + imagemVeiculo.Nome + "'/></li>";
            }

            htmlFinal += html;
            htmlFinal += "</ul>";

            ViewBag.ListaImagens = htmlFinal;

            #endregion

            return View(veiculo);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
