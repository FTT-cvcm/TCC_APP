using Android.Service.Voice;
using Newtonsoft.Json.Linq;
using Receitando.Data;
using Receitando.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Receitando.ViewModels
{
	public class AnaliseViewModel
	{

		private ISpeechToText _speechRecongnitionInstance;
		public bool PerfilAgressivo { get; set; }
		public string TextoCapturado { get; set; }

		private bool enabledStart = true;

		public bool EnabledStart  { get; set; }
		public string Recon { get; set; }

		public ICommand VerAnaliseAudiosCommand { get; private set; }
		public ICommand ReconhecimentoDeVozCommand { get; private set; }
		public Analise analise { get; set; }

		public AnaliseViewModel()
		{
			try
			{
				_speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();
			}
			catch (Exception ex)
			{
				Recon = ex.Message;
			}
			SendCommands();
			Subscribe();
		}

		private void Subscribe()
		{
			MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT", (sender, args) =>
			{
				SpeechToTextFinalResultRecieved(args);
			});

			MessagingCenter.Subscribe<ISpeechToText>(this, "Final", (sender) =>
			{
				EnabledStart = true;
			});

			MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
			{
				SpeechToTextFinalResultRecieved(args);
			});
		}

		public AnaliseViewModel(Analise analise)
		{
			this.analise = analise;
			SendCommands();
			try
			{
				_speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();
			}
			catch (Exception ex)
			{
				//recon = ex.Message;
			}
		}

		private void SendCommands()
		{
			VerAnaliseAudiosCommand = new Command(() =>
			{
				MessagingCenter.Send<AnaliseViewModel>(this, "VerAnaliseAudios");
			});


			ReconhecimentoDeVozCommand = new Command(() =>
			{
				ReconhecimentoVoz();
			}
			);
		}

		private void ReconhecimentoVoz()
		{
			try
			{
				_speechRecongnitionInstance.StartSpeechToText();
			}
			catch (Exception ex)
			{
				Recon = ex.Message;
			}
		}

		public void SalvarAnaliseDB()
		{

			using (var conexao = DependencyService.Get<ISQLite>().PegarConnection())
			{
				AnaliseDAO dao = new AnaliseDAO(conexao);
				dao.Salvar(analise);
			}

		}
		static async Task<String> GetCurrentLocation()
		{
			try
			{
				var request = new GeolocationRequest(GeolocationAccuracy.Best);
				var location = await Geolocation.GetLocationAsync(request);
				string LocalAtual = location.Latitude.ToString() + ";" + location.Longitude.ToString();
				return LocalAtual;
			}
			catch (Exception ex)
			{
				string LocalAtual = "Não disponível";
				return LocalAtual;
			}
		}
		static async Task<String> analiseSentimento(string texto)
		{
			HttpClient client = new HttpClient();
			string result = await client.GetStringAsync("https://analise-ldw7aff3gq-uc.a.run.app/analise?frase=" + texto);

			return result;
		}
		bool analisaResultadoAPI(string resultado)
		{
			JObject JSON = JObject.Parse(resultado);
			if ((double)JSON["valor"] > 0.6)
				return true;
			return false;
		}

		private async void SpeechToTextFinalResultRecieved(string args)
		{

			// A MÁGICA ACONTECE AQUI !!!!!


			Recon = args;
			TextoCapturado = args;
			try
			{
				PerfilAgressivo = analisaResultadoAPI(await analiseSentimento(TextoCapturado));
			}
			catch
			{
			}
			string UltimaLocalizacao = await GetCurrentLocation();
			analise = new Analise(TextoCapturado, PerfilAgressivo, UltimaLocalizacao);
			SalvarAnaliseDB();

		}

	}
}
