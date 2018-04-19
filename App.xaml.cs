﻿using System.Linq;
using System.Windows;
using WpfApp.Domain;
using WpfApp.ViewModel;

namespace WpfApp
{
	public partial class App
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			var mainWindow = new MainWindow();
			var storages = Storage.Repository.GetAll();
			var modules = storages.Select(storage => new SeparateDemo.SeparateDemo(storage)).ToList();

			var vm = new MainWindowViewModel(modules);

			mainWindow.DataContext = vm;
			mainWindow.Closing += (s, args) => vm.SelectedModule.Deactivate();
			mainWindow.Show();
		}
	}
}