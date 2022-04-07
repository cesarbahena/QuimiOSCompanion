using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using QuimiOSCompanion.Helpers;
using QuimiOSCompanion.Models;
using QuimiOSCompanion.Services;

namespace QuimiOSCompanion.ViewModels
{
    public class InventarioViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private ObservableCollection<InventoryItem> _inventoryItems;
        private bool _isLoading;
        private InventoryItem _selectedItem;

        public InventarioViewModel()
        {
            _apiService = new ApiService();
            InventoryItems = new ObservableCollection<InventoryItem>();
            LoadInventoryCommand = new RelayCommand(async _ => await LoadInventoryAsync());
        }

        public ObservableCollection<InventoryItem> InventoryItems
        {
            get => _inventoryItems;
            set => SetProperty(ref _inventoryItems, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public InventoryItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ICommand LoadInventoryCommand { get; }

        private async Task LoadInventoryAsync()
        {
            try
            {
                IsLoading = true;
                var items = await _apiService.GetInventoryItemsAsync();
                InventoryItems.Clear();
                foreach (var item in items)
                {
                    InventoryItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                // TODO: Add error handling
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
