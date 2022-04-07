using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using QuimiOSCompanion.Helpers;
using QuimiOSCompanion.Models;
using QuimiOSCompanion.Services;

namespace QuimiOSCompanion.ViewModels
{
    public class EntregaTurnoViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private ObservableCollection<Sample> _pendingSamples;
        private ObservableCollection<Sample> _selectedSamples;
        private bool _isLoading;
        private int _selectedShiftId;
        private int _selectedUserId;
        private DateTime _handoverDate;
        private string _notes;

        public EntregaTurnoViewModel()
        {
            _apiService = new ApiService();
            PendingSamples = new ObservableCollection<Sample>();
            SelectedSamples = new ObservableCollection<Sample>();
            HandoverDate = DateTime.Now;

            LoadPendingSamplesCommand = new RelayCommand(async _ => await LoadPendingSamplesAsync());
            CreateHandoverCommand = new RelayCommand(async _ => await CreateHandoverAsync(), _ => CanCreateHandover());
        }

        public ObservableCollection<Sample> PendingSamples
        {
            get => _pendingSamples;
            set => SetProperty(ref _pendingSamples, value);
        }

        public ObservableCollection<Sample> SelectedSamples
        {
            get => _selectedSamples;
            set => SetProperty(ref _selectedSamples, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public int SelectedShiftId
        {
            get => _selectedShiftId;
            set => SetProperty(ref _selectedShiftId, value);
        }

        public int SelectedUserId
        {
            get => _selectedUserId;
            set => SetProperty(ref _selectedUserId, value);
        }

        public DateTime HandoverDate
        {
            get => _handoverDate;
            set => SetProperty(ref _handoverDate, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public ICommand LoadPendingSamplesCommand { get; }
        public ICommand CreateHandoverCommand { get; }

        private async Task LoadPendingSamplesAsync()
        {
            try
            {
                IsLoading = true;
                var samples = await _apiService.GetPendingSamplesAsync();
                PendingSamples.Clear();
                foreach (var sample in samples)
                {
                    PendingSamples.Add(sample);
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

        private bool CanCreateHandover()
        {
            return SelectedSamples.Any() && SelectedShiftId > 0 && SelectedUserId > 0;
        }

        private async Task CreateHandoverAsync()
        {
            try
            {
                IsLoading = true;
                var pendingSamples = SelectedSamples.Select(s => new PendingSample
                {
                    SampleId = s.Id,
                    Folio = s.Folio,
                    Reason = "Pendiente de validación"
                }).ToList();

                await _apiService.CreateShiftHandoverAsync(SelectedShiftId, SelectedUserId, HandoverDate, Notes, pendingSamples);

                SelectedSamples.Clear();
                Notes = string.Empty;
                await LoadPendingSamplesAsync();
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
