using Android.Util;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json.Linq;
using Receitando.Data;
using Receitando.Model;
using Receitando.Models;
using Receitando.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Telegram.Bot;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Receitando.ViewModels
{
	public class AnaliseViewModel : BaseViewModel
	{
		#region Propriedades
		//Nome do canal do Telegram
		private CanalTelegram canalTelegram = new CanalTelegram();
		public CanalTelegram CanalTelegram
		{
			get
			{
				return canalTelegram;
			}
			set
			{
				canalTelegram = value;
				OnPropertyChanged();
				OnPropertyChanged(CanalTelegram.Nome);
			}
		}
		//Controla o envio de mensagens para o Telegram
		Stopwatch cronometro = new Stopwatch();

		//Texto do Botão
		string textoBotao = "Iniciar";

		//Indicador de atividade
		bool indicadorAtividade = false;

		//Cor do Botão
		Color corBotao = Color.LightSalmon;

		//Váriavel do reconhecimento de voz
		SpeechRecognizer reconhecedor;

		//Interface do serviço do microfone
		IMicrophoneService micServico;

		//Controla se está transcrevendo o áudio
		bool transcrevendo = false;

		//Controla se já está analisando o áudio
		bool analisando = false;
		public bool PerfilAgressivo { get; set; }
		public List<string> TextoCapturado = new List<string>();
		public Analise analise { get; set; }
		public string TextoBotao
		{
			get
			{
				return textoBotao;
			}
			set
			{
				textoBotao = value;
				OnPropertyChanged();
				OnPropertyChanged(TextoBotao);
			}
		}
		public bool IndicadorAtividade
		{
			get
			{
				return indicadorAtividade;
			}
			set
			{
				indicadorAtividade = value;
				OnPropertyChanged();
				OnPropertyChanged("IndicadorAtividade");
			}
		}
		public Color CorBotao
		{
			get
			{
				return corBotao;
			}
			set
			{
				corBotao = value;
				OnPropertyChanged();
				OnPropertyChanged("ButtonColor");
			}
		}
		#endregion
		public AnaliseViewModel()
		{
			using (var conexao = DependencyService.Get<ISQLite>().PegarConnection())
			{
				CanalTelegramDAO dao = new CanalTelegramDAO(conexao);
				var canal = dao.CanalTelegram;
				if(canal.Count > 0)
					canalTelegram = canal[0];

			}
			micServico = DependencyService.Resolve<IMicrophoneService>();
			Comandos();
			

		}

		//Comandos que vem da View
		public ICommand VerAnaliseAudiosCommand { get; private set; }
		public ICommand ReconhecimentoDeVozCommand { get; private set; }
		public ICommand SalvarCanalCommand { get; private set; }

		//Setando funções dos comandos
		private void Comandos()
		{
			VerAnaliseAudiosCommand = new Command(() =>
			{
				MessagingCenter.Send<AnaliseViewModel>(this, "VerAnaliseAudios");
			});


			ReconhecimentoDeVozCommand = new Command(() =>
			{
				TranscreverClick();
			}
			);

			SalvarCanalCommand = new Command(() =>
			{
				SalvarCanal();
			});
		}

		private void SalvarCanal()
		{
			using (var conexao = DependencyService.Get<ISQLite>().PegarConnection())
			{
				CanalTelegramDAO dao = new CanalTelegramDAO(conexao);
				dao.Salvar(CanalTelegram);
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
		async void TranscreverClick()
		{
			bool micHabilitado = await micServico.GetPermissionAsync();

			// EARLY OUT: make sure mic is accessible
			if (!micHabilitado)
			{
				UpdateTranscription("Acesso ao microfone não concedido.");
				return;
			}

			// initialize speech recognizer 
			if (reconhecedor == null)
			{
				var config = SpeechConfig.FromSubscription(Constants.CognitiveServicesApiKey, Constants.CognitiveServicesRegion);
				config.SpeechRecognitionLanguage = "pt-BR";
				config.SetProfanity(Microsoft.CognitiveServices.Speech.ProfanityOption.Raw);
				reconhecedor = new SpeechRecognizer(config);
				reconhecedor.Recognized += (obj, args) =>
				{
					UpdateTranscription(args.Result.Text);
				};
			}

			// if already transcribing, stop speech recognizer
			if (transcrevendo)
			{
				try
				{
					await reconhecedor.StopContinuousRecognitionAsync();
				}
				catch (Exception ex)
				{
					Log.Error("Receitando", ex.Message);
				}
				transcrevendo = false;
			}

			// if not transcribing, start speech recognizer
			else
			{
				try
				{
					await reconhecedor.StartContinuousRecognitionAsync();
				}
				catch (Exception ex)
				{
					Log.Error("Receitando", ex.Message);
				}
				transcrevendo = true;
			}
			UpdateDisplayState();
		}
		int i = 0;
		private async void SpeechToTextFinalResultRecieved()
		{
			analisando = true;
			while (i < TextoCapturado.Count)
			{
				try
				{
					PerfilAgressivo = analisaResultadoAPI(await analiseSentimento(TextoCapturado[i]));
					analise = new Analise(TextoCapturado[i], PerfilAgressivo);
					SalvarAnaliseDB();
				}
				catch (Exception ex)
				{
					Log.Error("Receitando", ex.Message);
				}
				i++;
			}

			cronometro.Stop();
			if (PerfilAgressivo && (cronometro.ElapsedMilliseconds > 2000 || cronometro.ElapsedMilliseconds == 0))
				sendMensagemTelegramAsync();
			analisando = false;
		}
		void UpdateTranscription(string newText)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				if (!string.IsNullOrWhiteSpace(newText))
				{
					TextoCapturado.Add(newText);
				}
				if (!analisando)
				{
					SpeechToTextFinalResultRecieved();
				}
			});

		}
		void UpdateDisplayState()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				if (transcrevendo)
				{
					TextoBotao = "Parar";
					CorBotao = Color.DarkSalmon;
					IndicadorAtividade = true;
				}
				else
				{
					TextoBotao = "Iniciar";
					CorBotao = Color.LightSalmon;
					IndicadorAtividade = false;

				}
			});
		}

		private async Task sendMensagemTelegramAsync()
		{

			cronometro.Restart();

			var bot = new TelegramBotClient(Constants.TokenAPIBotTelegram);
			await bot.SendTextMessageAsync("@" + CanalTelegram.Nome, "Possível ocorrência de violência detectada!");
		}
		
		


	}
}
