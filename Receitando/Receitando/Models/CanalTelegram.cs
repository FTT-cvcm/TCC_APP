using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receitando.Models
{
	public class CanalTelegram
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Nome { get; set; }

		public CanalTelegram()
		{

		}
		public CanalTelegram(string NomeCanal)
		{
			Nome = NomeCanal;
		}
	}
}
