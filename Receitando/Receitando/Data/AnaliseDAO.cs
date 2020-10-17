using Receitando.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receitando.Data
{
    public class AnaliseDAO
    {
        readonly SQLiteConnection conexao;

        private List<Analise> lista;

        public List<Analise> Lista
        {
            get
            {
                return conexao.Table<Analise>().ToList();
            }

            private set
            {
                lista = value;
            }
        }


        public AnaliseDAO(SQLiteConnection conexao)
        {
            this.conexao = conexao;
            this.conexao.CreateTable<Analise>();
            //Cria a tabela de acordo com o Objeto, caso já exista não faz nada...
        }

        public void Salvar(Analise analise)
        {
            conexao.Insert(analise);
        }

    }
}
