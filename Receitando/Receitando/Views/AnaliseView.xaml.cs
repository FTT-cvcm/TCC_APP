using Receitando.Services;
using Receitando.ViewModels;
using Receitando.Views;
using Xamarin.Forms;

namespace Receitando
{
	public partial class AnaliseView : ContentPage
	{

		IMicrophoneService micService;
		public AnaliseViewModel viewModel { get; set; }
		public AnaliseView()
		{
			InitializeComponent();
			this.viewModel = new AnaliseViewModel();
			this.BindingContext = viewModel;
			micService = DependencyService.Resolve<IMicrophoneService>();

		}			

		protected override void OnAppearing()
		{
			base.OnAppearing();

			MessagingCenter.Subscribe<AnaliseViewModel>(this, "VerAnaliseAudios",
				(msg) =>
				{
					Navigation.PushAsync(new RelatorioAnalisesView());
				}
				);

		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			MessagingCenter.Unsubscribe<AnaliseViewModel>(this, "VerAnaliseAudios");

		}


	}
}
