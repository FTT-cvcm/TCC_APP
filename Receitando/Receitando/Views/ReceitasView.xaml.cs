﻿using Receitando.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Receitando.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReceitasView : ContentPage
	{
		public ReceitasView()
		{
			InitializeComponent();
			this.BindingContext = new ReceitasViewMovel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			MessagingCenter.Subscribe<ReceitasViewMovel>(this, "EntrarAnaliseView",
				(msg) =>
				{
					Navigation.PushAsync(new AnaliseView());
				}
				);
			
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			MessagingCenter.Unsubscribe<ReceitasViewMovel>(this, "EntrarAnaliseView");

		}

	}
}