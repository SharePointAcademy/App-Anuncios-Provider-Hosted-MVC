using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppAnunciosMVCWeb.Models
{
    public class VeiculoImagens
    {
        public string Nome { get; set; }
        public string Url { get; set; }


        public VeiculoImagens(string nome, string url)
        {
            Nome = nome;
            Url = url;
        }
    }
}