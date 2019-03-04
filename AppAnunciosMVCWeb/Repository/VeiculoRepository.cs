using AppAnunciosMVCWeb.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppAnunciosMVCWeb.Repository
{
    public class VeiculoRepository
    {
        public static List<VeiculoImagens> ListarVeiculosImagens(SharePointContext spContext, string listName, string idVeiculo)
        {
            List<VeiculoImagens> lstVeiculos = new List<VeiculoImagens>();

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                var list = clientContext.Web.Lists.GetByTitle(listName);
                clientContext.Load(clientContext.Web, w => w.ServerRelativeUrl);
                clientContext.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.FolderServerRelativeUrl = string.Format("{0}/Lists/{1}/{2}", clientContext.Web.ServerRelativeUrl, listName, idVeiculo);
                ListItemCollection listCol = list.GetItems(camlQuery);
                clientContext.Load(listCol);
                clientContext.ExecuteQuery();

                foreach (ListItem item in listCol)
                {
                    lstVeiculos.Add(new VeiculoImagens(Util.ReturnString(item["FileLeafRef"]), Util.ReturnString(item["FileDirRef"])));
                }
            }

            return lstVeiculos;
        }

        public static int CriarVeiculo(SharePointContext spContext, string listName, Veiculo veiculo)
        {
            int idVeiculo = 0;

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    var list = clientContext.Web.Lists.GetByTitle(listName);

                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem newItem = list.AddItem(itemCreateInfo);
                    newItem[Veiculo.VeiculoFields.Titulo] = veiculo.Titulo;
                    newItem[Veiculo.VeiculoFields.Marca] = veiculo.Marca;
                    newItem[Veiculo.VeiculoFields.Preco] = veiculo.Preco;
                    newItem[Veiculo.VeiculoFields.Foto] = veiculo.Foto;

                    newItem.Update();
                    //carrega o objeto para que possamos recuperar o id gerado
                    clientContext.Load(newItem);

                    clientContext.ExecuteQuery();
                    idVeiculo = newItem.Id;
                }
            }

            return idVeiculo;
        }

        public static void AddItemInFolder(SharePointContext spContext, Veiculo veiculo, string listName, string folderName, string relativeUrl)
        {

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    var list = clientContext.Web.Lists.GetByTitle(listName);

                    ListItemCreationInformation listItemCreationInformation = null;
                    if (!string.IsNullOrEmpty(folderName))
                    {
                        listItemCreationInformation = new ListItemCreationInformation();
                        listItemCreationInformation.FolderUrl = string.Format("{0}/Lists/{1}/{2}", relativeUrl, listName, folderName);
                    }

                    var listItem = list.AddItem(listItemCreationInformation);
                    listItem[Veiculo.VeiculoFields.Titulo] = veiculo.Titulo;
                    listItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }

        public static void UploadFile(SharePointContext spContext, FileCreationInformation newFile, string listName)
        {
            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    //SecureString passWord = new SecureString();
                    //foreach (char c in "SuaSenhaAqui".ToCharArray()) passWord.AppendChar(c);
                    //clientContext.Credentials = new SharePointOnlineCredentials("seunome@seusite.onmicrosoft.com", passWord);

                    List docs = clientContext.Web.Lists.GetByTitle(listName);
                    Microsoft.SharePoint.Client.File uploadFile = docs.RootFolder.Files.Add(newFile);

                    clientContext.Load(uploadFile);
                    clientContext.ExecuteQuery();

                }
            }
        }

        public static string CreateFolder(string listName, string folderName, SharePointContext spContext)
        {
            string relativeURL = string.Empty;
            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    List list = clientContext.Web.Lists.GetByTitle(listName);
                    clientContext.Load(clientContext.Web, w => w.ServerRelativeUrl);

                    ListItemCreationInformation info = new ListItemCreationInformation();
                    info.UnderlyingObjectType = FileSystemObjectType.Folder;
                    info.LeafName = folderName.Trim();
                    ListItem newItem = list.AddItem(info);
                    newItem["Title"] = folderName;
                    newItem.Update();
                    clientContext.ExecuteQuery();

                    relativeURL = clientContext.Web.ServerRelativeUrl;
                }
            }

            return relativeURL;
        }
    }
}