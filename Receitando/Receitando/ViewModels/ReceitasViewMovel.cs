using Android.Locations;
using Receitando.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Receitando.ViewModels
{
	public class ReceitasViewMovel : BaseViewModel
	{
		List<Receita> listaReceita = new List<Receita>();

		//Controla o entre os clicks
		Stopwatch stopwatch = new Stopwatch();
		int count = 0;		

		public List<Receita> ListaReceita
		{
			get
			{
				return listaReceita;
			}			
		}
		
		
		public ICommand AnalisePageCommand { get; private set; }
		public ReceitasViewMovel()		
		{
			listaReceita = new List<Receita>
			{
				new Receita{Imagem = "paocaseiro.jpeg", Nome = "Pão Caseiro",
							Ingredientes = String.Format("500 ml de água\r\n" +
														 "1 kg de farinha de trigo\r\n"+
														 "1 unidade de ovo\r\n" +
														 "20 gr de fermento biológico fresco\r\n"+
														 "2 colheres (sopa) de manteiga\r\n"+
														 "2 colheres (sopa) de açúcar\r\n"+
														 "1 colher (sopa) de sal\r\n")

				},
				new Receita{Imagem = "iogurtecaseiro.jpeg", Nome = "Iogurte Caseiro sabor Morango",
							Ingredientes =  String.Format("2 litros de leite\r\n"+
							"1 lata de leite condensado\r\n"+
							"200 gr de iogurte grego\r\n"+
							"1 caixinha de gelatina sabor morango\r\n"+
							"1 xícara (chá) de morango picado\r\n")
				},
				new Receita{Imagem = "paodecebolacaseiro.jpeg", Nome = "Pão de Cebola Caseiro",
							Ingredientes =  String.Format("1/4 xícara (chá) de óleo de soja\r\n" +
							"1 xícara (chá) de leite\r\n" +
							"2 unidades de ovo\r\n" +
							"2 colheres (sopa) de margarina\r\n" +
							"1 colher (sopa) de sal\r\n" +
							"1 colher (sopa) de sal\r\n" +
							"1 colher (sopa) de sal\r\n"+
							"50 gr de fermento biológico fresco\r\n"+
							"1 kg de farinha de trigo\r\n"+
							"1 unidade de gema de ovo")
				}
			};

			AnalisePageCommand = new Command(() =>
			{
				if (stopwatch.ElapsedMilliseconds == 0)
					stopwatch.Start();

				if (stopwatch.ElapsedMilliseconds > 3000)
				{
					count = 0;
					stopwatch.Reset();
				}
				else
					count++;

				if (count > 4)
				{
					stopwatch.Reset();
					count = 0;
					MessagingCenter.Send<ReceitasViewMovel>(this, "EntrarAnaliseView");
				}
			});

		}

	}
}
