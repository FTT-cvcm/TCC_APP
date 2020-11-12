using Receitando.Data;
using Receitando.Model;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Receitando.ViewModels
{
	public class RelatorioAnalisesViewModel : BaseViewModel
	{
		ObservableCollection<Analise> listaAnalise = new ObservableCollection<Analise>();
		
		public bool Ocupado;		
		public ObservableCollection<Analise> ListaAnalise
		{
			get
			{
				return listaAnalise;
			}
			//set
			//{
			//	listaAnalise = value;
			//}

		}

		public Analise SetImage(Analise analise)
		{
			if (analise.PerfilAgressivo)
				analise.ImagemViolencia = new Image { Source = "violencia.jpg" };
			else
				analise.ImagemViolencia = new Image { Source = "naoviolencia.jpg"};

			return analise;
		}

		public RelatorioAnalisesViewModel()
		{
			Ocupado = true;
			using (var conexao = DependencyService.Get<ISQLite>().PegarConnection())
			{
				AnaliseDAO dao = new AnaliseDAO(conexao);
				var listadb = dao.Lista;
				this.listaAnalise.Clear();
				foreach (var itemDB in listadb)
				{					
					this.listaAnalise.Add(SetImage(itemDB));					
				}

			}
			Ocupado = false;
		}
	}
}
