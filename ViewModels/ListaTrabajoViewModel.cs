using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using QuimiOSCompanion.Helpers;
using QuimiOSCompanion.Models;
using QuimiOSCompanion.Services;

namespace QuimiOSCompanion.ViewModels
{
    public class ListaTrabajoViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private ObservableCollection<Sample> _samples;
        private bool _isLoading;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private int? _clientId;

        public ListaTrabajoViewModel()
        {
            _apiService = new ApiService();
            Samples = new ObservableCollection<Sample>();
            LoadSamplesCommand = new RelayCommand(async _ => await LoadSamplesAsync());
        }

        public ObservableCollection<Sample> Samples
        {
            get => _samples;
            set => SetProperty(ref _samples, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public int? ClientId
        {
            get => _clientId;
            set => SetProperty(ref _clientId, value);
        }

        public ICommand LoadSamplesCommand { get; }

        private async Task LoadSamplesAsync()
        {
            try
            {
                IsLoading = true;
                var samples = await _apiService.GetSamplesAsync(ClientId, StartDate, EndDate);
                Samples.Clear();
                foreach (var sample in samples)
                {
                    Samples.Add(sample);
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
