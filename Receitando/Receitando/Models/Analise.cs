using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receitando.Model
{
    public class Analise
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get;  set; }

        public DateTime data = DateTime.Now;
        public  DateTime Data 
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
            
         }

        public string TextoCapturado { get; set; }

        public bool PerfilAgressivo { get; set; }

        public Analise()
        {

        }
        public Analise(string textoCapturado,bool analiseTexto)
        {
            this.TextoCapturado = textoCapturado;
            this.PerfilAgressivo = analiseTexto;
        }

    }
}
