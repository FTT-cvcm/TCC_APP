using Receitando.Data;
using Receitando.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
					this.listaAnalise.Add(itemDB);
				}

			}
			Ocupado = false;		
		}

		public void RemoverAnalise(Analise analise)
		{
			try
			{
				using (var conexao = DependencyService.Get<ISQLite>().PegarConnection())
				{
					AnaliseDAO dao = new AnaliseDAO(conexao);
					dao.Deletar(analise);
				}
			}
			catch
			{

			}
		}
		

	}
}
