using Receitando.Model;
using Receitando.ViewModels;
using Receitando.Views;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Receitando
{
    public partial class AnaliseView : ContentPage
    {
        private ISpeechToText _speechRecongnitionInstance;

        public bool PerfilAgressivo { get; set; }
        public string TextoCapturado { get; set; }
        public Analise analise { get; set; }
        public AnaliseViewModel viewModel {get; set;}
        public AnaliseView()
        {
            InitializeComponent();
            this.viewModel = new AnaliseViewModel();
            this.BindingContext = viewModel;

            try
            {
                _speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
            }


            MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            MessagingCenter.Subscribe<ISpeechToText>(this, "Final", (sender) =>
            {
                start.IsEnabled = true;
            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });
        }

        private async void SpeechToTextFinalResultRecieved(string args)
        {

            // A MÁGICA ACONTECE AQUI !!!!!
                        

            recon.Text = args;
            TextoCapturado = args;
            PerfilAgressivo = analisaResultadoAPI(await analiseSentimento(TextoCapturado));
            string UltimaLocalizacao = await GetCurrentLocation();

            analise = new Analise(TextoCapturado, PerfilAgressivo, UltimaLocalizacao);
            viewModel.analise = analise;
            viewModel.SalvarAnaliseDB();          
        }

        private void Button_Clicked(object sender, EventArgs e)
        {            
            try
            {
                _speechRecongnitionInstance.StartSpeechToText();
            }
            catch (Exception ex)
            {
                recon.Text = ex.Message;
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
            // ... Use HttpClient.
            HttpClient client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes("1575-12+AosVZ:tVmYCvtDx1BJ2sF6GEK6Wj1GQUdY1ulynqGkqQ9zr5BF");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var data = new { T = texto, SL = "PtBr", EM = "True" };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://api.gotit.ai/NLU/v1.4/Analyze", content);
            HttpContent responseContent = response.Content;

            // ... Check Status Code                                            
            //Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);

            // ... Read the string.
            string result = await responseContent.ReadAsStringAsync();

            return result;
        }

        bool analisaResultadoAPI(string resultado)
        {
            JObject JSON = JObject.Parse(resultado);
            string raiva = (string)JSON["emotions"]["anger"];
            string medo = (string)JSON["emotions"]["fear"];
            if (Convert.ToDouble(raiva) > 0.8)
                return true;
            if (Convert.ToDouble(medo) > 0.8)
                return true;
            return false;
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
