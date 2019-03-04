using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppAnunciosMVCWeb.Models
{
    public class Veiculo
    {
        public int ID { get; set; }
        public string Titulo { get; set; }
        public string Marca { get; set; }
        public decimal Preco { get; set; }
        public string Foto { get; set; }

        public Veiculo()
        { }


        public Veiculo(int id, string titulo, string marca, decimal preco, string foto)
        {
            ID = id;
            Titulo = titulo;
            Marca = marca;
            Preco = preco;
            Foto = foto;
        }

        public static List<Veiculo> ObterVeiculos(SharePointContext spContext, CamlQuery camlQuery)
        {
            List<Veiculo> lstVeiculos = new List<Veiculo>();
            string urlImagem = string.Empty;

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web.Lists);
                    clientContext.ExecuteQuery();

                    List listVeiculos = web.Lists.GetByTitle(Constants.Listas.Veiculos);
                    ListItemCollection listItemsVeiculos = listVeiculos.GetItems(camlQuery);
                    clientContext.Load(listItemsVeiculos);
                    clientContext.ExecuteQuery();
                    if (listItemsVeiculos != null)
                    {

                        foreach (var itemVeiculo in listItemsVeiculos)
                        {
                            urlImagem = spContext.SPAppWebUrl + "Lists/" + Constants.Listas.VeiculosImagens + "/" + itemVeiculo.Id + "/" + itemVeiculo[VeiculoFields.Foto];

                            lstVeiculos.Add(
                            new Veiculo
                            {
                                ID = itemVeiculo.Id,
                                Titulo = itemVeiculo[VeiculoFields.Titulo].ToString(),
                                Marca = Util.ReturnString(itemVeiculo[VeiculoFields.Marca]),
                                Preco = Util.ReturnDecimal(itemVeiculo[VeiculoFields.Preco]),
                                Foto = (itemVeiculo[VeiculoFields.Foto] != null ? urlImagem : "")
                                //Foto = (itemVeiculo[VeiculoFields.Foto] != null ? ((FieldUrlValue)itemVeiculo[VeiculoFields.Foto]).Url : "")
                            });
                        }
                    }
                }
            }
            return lstVeiculos;
        }

        public static Veiculo ObterVeiculo(SharePointContext spContext, int idVeiculo)
        {
            Veiculo veiculo = null;
            string urlImagem = string.Empty;

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                if (clientContext != null)
                {
                    Web web = clientContext.Web;
                    clientContext.Load(web.Lists);
                    clientContext.ExecuteQuery();

                    List listVeiculos = web.Lists.GetByTitle(Constants.Listas.Veiculos);
                    ListItem itemVeiculo = listVeiculos.GetItemById(idVeiculo);
                    clientContext.Load(itemVeiculo);
                    clientContext.ExecuteQuery();
                    if (itemVeiculo != null)
                    {
                        urlImagem = spContext.SPAppWebUrl + "Lists/" + Constants.Listas.VeiculosImagens + "/" + itemVeiculo.Id + "/" + itemVeiculo[VeiculoFields.Foto];

                        veiculo = new Veiculo
                        (
                            itemVeiculo.Id,
                            itemVeiculo[VeiculoFields.Titulo].ToString(),
                            Util.ReturnString(itemVeiculo[VeiculoFields.Marca]),
                            Util.ReturnDecimal(itemVeiculo[VeiculoFields.Preco]),
                            (itemVeiculo[VeiculoFields.Foto] != null ? urlImagem : "")
                        //(itemVeiculo[VeiculoFields.Foto] != null ? ((FieldUrlValue)itemVeiculo[VeiculoFields.Foto]).Url : "")
                        );
                    }
                }
            }

            return veiculo;
        }

        public class VeiculoFields
        {
            public static string Titulo = "Title";
            public static string Marca = "Marca";
            public static string Preco = "Preco";
            public static string Foto = "Foto";
        }
    }
}