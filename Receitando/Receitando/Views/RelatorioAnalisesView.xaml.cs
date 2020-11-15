using Receitando.Model;
using Receitando.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Receitando.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RelatorioAnalisesView : ContentPage
	{
		public RelatorioAnalisesView()
		{
			InitializeComponent();
			this.BindingContext = new RelatorioAnalisesViewModel();

			
		}		

		private async void OnRemoverAnaliseClicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
			var analise = mi.CommandParameter as Analise;
			if (analise != null)
			{
				var opcao = await DisplayAlert("Excluir", "Tem certeza que deseja excluir a análise?", "Sim", "Não");
				if (opcao)
				{
					try
					{
						var analises = new RelatorioAnalisesViewModel();
						analises.RemoverAnalise(analise);
						AtualizarTela();
					}
					catch
					{

					}

				}
			}
		}
		private void AtualizarTela()
		{
			InitializeComponent();

			this.BindingContext = new RelatorioAnalisesViewModel();

		}
	}
}