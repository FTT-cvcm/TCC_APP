using Receitando.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receitando.Data
{
	public class CanalTelegramDAO
	{
		readonly SQLiteConnection conexao;

		public List<CanalTelegram> CanalTelegram
		{
			get
			{
				return conexao.Table<CanalTelegram>().ToList();
			}
			set 
			{
				CanalTelegram = value;
			}
		}
		public CanalTelegramDAO(SQLiteConnection conexao)
		{
			this.conexao = conexao;
			this.conexao.CreateTable<CanalTelegram>();
			//Cria a tabela de acordo com o Objeto, caso já exista não faz nada...
		}

		public void Salvar(CanalTelegram canal)
		{
			conexao.DeleteAll<CanalTelegram>();
			conexao.Insert(canal);
		}

	}
}
