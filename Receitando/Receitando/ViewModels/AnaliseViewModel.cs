using Receitando.Data;
using Receitando.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Receitando.ViewModels
{
    public class AnaliseViewModel
    {

        public ICommand VerAnaliseAudiosCommand { get; private set; }
        public Analise analise { get; set; }

        public AnaliseViewModel()
        {
            SendCommands();
        }
       
        public AnaliseViewModel(Analise analise)
        {
            this.analise = analise;
            SendCommands();
        }

        private void SendCommands()
        {
            VerAnaliseAudiosCommand = new Command(() =>
            {
                MessagingCenter.Send<AnaliseViewModel>(this, "VerAnaliseAudios");
            });
        }



        public async void SalvarAnaliseDB()
        {

            using (var conexao = DependencyService.Get<ISQLite>().PegarConnection())
            {
                AnaliseDAO dao = new AnaliseDAO(conexao);
                dao.Salvar(analise);
            }

        }



    }
}
