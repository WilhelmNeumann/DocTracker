﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using MongoDB.Bson;
using WpfApp.Domain;
using WpfApp.Service;
using WpfApp.SubPages.Modals;
using Action = System.Action;

namespace WpfApp.SubPages
{
	public partial class MainContent
	{
		private List<Storage> Storages { get; set; }

		private Storage _selectedStorage;

		public MainContent()
		{
			InitializeComponent();

			Task.Factory.StartNew(GetStorages)
				.ContinueWith(result => UpdateUi());
		}

		private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

		private void GetStorages()
		{
			ConsoleWriter.Write("Загрузка складов");
			Storages = Storage.Repository.GetAll().ToList();
//			Thread.Sleep(10000);
			_resetEvent.Set();
		}

		private void UpdateUi()
		{
			_resetEvent.WaitOne();
			if (!Storages.Any()) return;

			Storages.ForEach(s => ConsoleWriter.Write(s.Name));

			Dispatcher.Invoke(() =>
			{
				SetContent(Storages.First());
				StorageMenuItems.ItemsSource = Storages;
				LoadingIndicator.IsActive = false;
				LoadingIndicator.Visibility = Visibility.Hidden;
				LoadingLabel.Visibility = Visibility.Hidden;
				AddStorageButton.IsEnabled = true;
				DeleteStorageButton.IsEnabled = true;
			});
		}

		/// <summary>
		/// Выбор хранилища из главного окна
		/// </summary>
		/// <param name="selectedStorageId">Id хранилища</param>
		public void SelectItem(string selectedStorageId)
		{
			var storage = Storages.First(s => s.Id == selectedStorageId);
			SetContent(storage);
		}

		/// <summary>
		/// Установка панели с содержимым склада
		/// </summary>
		/// <param name="storage">Склад, содержимое которого будет отображаться</param>
		private void SetContent(Storage storage)
		{
			_selectedStorage = storage;
			ContentPresenter.Content = new StorageContent(storage);
		}

		/// <summary>
		/// Поиск хранилищ в списке
		/// </summary>
		private void SearchStorages(object sender, TextChangedEventArgs e)
		{
			var filteredItems = Storages.Where(item => item.Name.ToLower().Contains(SearchInput.Text.ToLower())).ToList();
			StorageMenuItems.ItemsSource = filteredItems;
			if (!filteredItems.Any()) return;
			SetContent(filteredItems.First());
		}

		/// <summary>
		/// Выбор хранилища
		/// </summary>
		private void SelectStorage(object sender, SelectionChangedEventArgs e)
		{
			var item = (Storage) StorageMenuItems.SelectedItem;
			if (item == null) return;
			if (item.Id == _selectedStorage.Id) return;
			SetContent(item);
		}

		/// <summary>
		/// Добавление нового хранилища
		/// </summary>
		private void AddStorage(object sender, RoutedEventArgs e)
		{
			var inputDialog = new AddStorageDialog();
			if (inputDialog.ShowDialog() != true) return;

			var storageBson = new BsonDocument
			{
				{"Name", inputDialog.Name.Text},
				{"Address", inputDialog.Address.Text},
				{"Description", inputDialog.Description.Text},
			};
			Storage.Repository.AddAndSave(storageBson);
			Storages = Storage.Repository.GetAll().ToList();
			StorageMenuItems.ItemsSource = Storages;
			SetContent(Storages.First(s => s.Id == storageBson["_id"].ToString()));
		}

		/// <summary>
		/// Удаление хранилища
		/// </summary>
		private async void DeleteStorage(object sender, RoutedEventArgs e)
		{
			if (Storages.Count == 1) return;
			await Storage.Repository.DeleteById(_selectedStorage.Id);
			Storages = Storages.Where(s => s.Id != _selectedStorage.Id).ToList();
			SetContent(Storages.First());
			StorageMenuItems.ItemsSource = Storages;
		}
	}
}