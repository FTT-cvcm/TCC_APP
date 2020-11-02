﻿using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json.Linq;
using Receitando.Data;
using Receitando.Model;
using Receitando.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Receitando.ViewModels
{
	public class AnaliseViewModel : BaseViewModel
	{

		SpeechRecognizer recognizer;
		IMicrophoneService micService;
		bool isTranscribing = false;
		Color buttonColor = Color.LightBlue;
		bool analisando = false;

		public Color ButtonColor
		{
			get
			{
				return buttonColor;
			}
			set
			{
				buttonColor = value;
				OnPropertyChanged();
				OnPropertyChanged("ButtonColor");
			}
		}

		string buttonText = "Play";


		public string ButtonText
		{
			get
			{
				return buttonText;
			}
			set
			{
				buttonText = value;
				OnPropertyChanged();
				OnPropertyChanged(ButtonText);
			}
		}
		bool activeIndicator = false;
		public bool ActiveIndicator
		{
			get
			{
				return activeIndicator;
			}
			set
			{
				activeIndicator = value;
				OnPropertyChanged();
				OnPropertyChanged("ActiveIndicator");
			}
		}


		//Azure

		//private ISpeechToText _speechRecongnitionInstance;
		public bool PerfilAgressivo { get; set; }
		public List<string> TextoCapturado = new List<string>();
		public ICommand VerAnaliseAudiosCommand { get; private set; }
		public ICommand ReconhecimentoDeVozCommand { get; private set; }
		public Analise analise { get; set; }
		public AnaliseViewModel()
		{

			micService = DependencyService.Resolve<IMicrophoneService>();
			SendCommands();
			
		}
				

		public AnaliseViewModel(Analise analise)
		{
			micService = DependencyService.Resolve<IMicrophoneService>();
			this.analise = analise;
			SendCommands();		
		}

		private void SendCommands()
		{
			VerAnaliseAudiosCommand = new Command(() =>
			{
				MessagingCenter.Send<AnaliseViewModel>(this, "VerAnaliseAudios");
			});


			ReconhecimentoDeVozCommand = new Command(() =>
			{
				TranscribeClicked();
			}
			);
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
	
		async void TranscribeClicked()
		{
			bool isMicEnabled = await micService.GetPermissionAsync();

			// EARLY OUT: make sure mic is accessible
			if (!isMicEnabled)
			{
				UpdateTranscription("Acesso ao microfone não concedido.");
				return;
			}

			// initialize speech recognizer 
			if (recognizer == null)
			{
				var config = SpeechConfig.FromSubscription(Constants.CognitiveServicesApiKey, Constants.CognitiveServicesRegion);
				config.SpeechRecognitionLanguage = "pt-BR";
				recognizer = new SpeechRecognizer(config);
				recognizer.Recognized += (obj, args) =>
				{
					UpdateTranscription(args.Result.Text);
				};
			}

			// if already transcribing, stop speech recognizer
			if (isTranscribing)
			{
				try
				{
					await recognizer.StopContinuousRecognitionAsync();
				}
				catch (Exception ex)
				{
					UpdateTranscription(ex.Message);
				}
				isTranscribing = false;
			}

			// if not transcribing, start speech recognizer
			else
			{
				try
				{
					await recognizer.StartContinuousRecognitionAsync();
				}
				catch (Exception ex)
				{
					UpdateTranscription(ex.Message);
				}
				isTranscribing = true;
			}
			UpdateDisplayState();
		}
		int i = 0;
		private async void SpeechToTextFinalResultRecieved()
		{
			analisando = true;
            while(i < TextoCapturado.Count)
			{
				try
				{
					PerfilAgressivo = analisaResultadoAPI(await analiseSentimento(TextoCapturado[i]));
				}
				catch
				{
				}
				string UltimaLocalizacao = await GetCurrentLocation();
				analise = new Analise(TextoCapturado[i], PerfilAgressivo, UltimaLocalizacao);
				SalvarAnaliseDB();
				//TextoCapturado.Remove(TextoCapturado[i]); não dá para limpar a lista enquanto outro objeto adicionar nela.
				i++;
			}
			analisando = false;
		}

		void UpdateTranscription(string newText)
		{
			//Device.BeginInvokeOnMainThread(() =>
			//{
			if (!string.IsNullOrWhiteSpace(newText))
			{
				TextoCapturado.Add(newText); //pega o texto transcrito						
			}
			if (!analisando)
			{
				SpeechToTextFinalResultRecieved();
			}
			//});

		}
		void UpdateDisplayState()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				if (isTranscribing)
				{
					ButtonText = "Stop";
					ButtonColor = Color.Red;
					ActiveIndicator = true;
				}
				else
				{
					ButtonText = "Play";
					ButtonColor = Color.LightBlue;
					ActiveIndicator = false;

				}
			});
		}

	}
}