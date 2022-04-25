using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using QuimiOSCompanion.Helpers;
using QuimiOSCompanion.Models;
using QuimiOSCompanion.Services;

namespace QuimiOSCompanion.ViewModels
{
    public class InventarioViewModel : ViewModelBase
    {
        private readonly ApiService _apiService;
        private readonly PdfParserService _pdfParser;
        private readonly ConsumptionCalculator _calculator;
        private readonly QuimiOSApiClient _quimiosClient;

        private string _selectedPdfPath;
        private DateTime _consumptionDate;
        private ObservableCollection<Reagent> _availableReagents;
        private ObservableCollection<Reagent> _selectedCalibrations;
        private ObservableCollection<ConsumptionData> _parsedConsumptions;
        private ObservableCollection<string> _validationErrors;
        private bool _isProcessing;
        private string _statusMessage;

        public InventarioViewModel()
        {
            _apiService = new ApiService();
            _pdfParser = new PdfParserService();
            _calculator = new ConsumptionCalculator();
            _quimiosClient = new QuimiOSApiClient();

            AvailableReagents = new ObservableCollection<Reagent>();
            SelectedCalibrations = new ObservableCollection<Reagent>();
            ParsedConsumptions = new ObservableCollection<ConsumptionData>();
            ValidationErrors = new ObservableCollection<string>();

            ConsumptionDate = DateTime.Today;

            SelectPdfCommand = new RelayCommand(_ => SelectPdf());
            ParsePdfCommand = new RelayCommand(async _ => await ParsePdfAsync(), _ => !string.IsNullOrEmpty(SelectedPdfPath));
            LoadReagentsCommand = new RelayCommand(async _ => await LoadReagentsAsync());
            CalculateConsumptionCommand = new RelayCommand(async _ => await CalculateConsumptionsAsync(), _ => ParsedConsumptions.Any());
            SubmitToQuimiOSCommand = new RelayCommand(async _ => await SubmitToQuimiOSAsync(), _ => ParsedConsumptions.Any() && !ValidationErrors.Any());
            SubmitToHubCommand = new RelayCommand(async _ => await SubmitToHubAsync(), _ => ParsedConsumptions.Any() && !ValidationErrors.Any());

            _ = LoadReagentsAsync();
        }

        public string SelectedPdfPath
        {
            get => _selectedPdfPath;
            set => SetProperty(ref _selectedPdfPath, value);
        }

        public DateTime ConsumptionDate
        {
            get => _consumptionDate;
            set => SetProperty(ref _consumptionDate, value);
        }

        public ObservableCollection<Reagent> AvailableReagents
        {
            get => _availableReagents;
            set => SetProperty(ref _availableReagents, value);
        }

        public ObservableCollection<Reagent> SelectedCalibrations
        {
            get => _selectedCalibrations;
            set => SetProperty(ref _selectedCalibrations, value);
        }

        public ObservableCollection<ConsumptionData> ParsedConsumptions
        {
            get => _parsedConsumptions;
            set => SetProperty(ref _parsedConsumptions, value);
        }

        public ObservableCollection<string> ValidationErrors
        {
            get => _validationErrors;
            set => SetProperty(ref _validationErrors, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand SelectPdfCommand { get; }
        public ICommand ParsePdfCommand { get; }
        public ICommand LoadReagentsCommand { get; }
        public ICommand CalculateConsumptionCommand { get; }
        public ICommand SubmitToQuimiOSCommand { get; }
        public ICommand SubmitToHubCommand { get; }

        private void SelectPdf()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                Title = "Select Consumption PDF"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedPdfPath = dialog.FileName;
                StatusMessage = $"Selected: {System.IO.Path.GetFileName(SelectedPdfPath)}";
            }
        }

        private async Task ParsePdfAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Parsing PDF...";

                var (startDate, endDate, rows) = _pdfParser.ParseConsumptionPdf(SelectedPdfPath);

                ConsumptionDate = startDate;

                ParsedConsumptions.Clear();
                foreach (var row in rows)
                {
                    var reagent = AvailableReagents.FirstOrDefault(r => r.Code == row.ReagentCode);
                    if (reagent != null)
                    {
                        ParsedConsumptions.Add(new ConsumptionData
                        {
                            ReagentId = reagent.Id,
                            ReagentCode = row.ReagentCode,
                            ReagentName = reagent.Name,
                            ResearchConsumption = row.ResearchConsumption,
                            RepeatConsumption = row.RepeatConsumption,
                            QCConsumption = row.QCConsumption,
                            ManualConsumption = row.ManualConsumption,
                            CalibrationConsumption = 0,
                            TotalConsumption = row.ResearchConsumption + row.RepeatConsumption + row.QCConsumption + row.ManualConsumption,
                            FinalTotal = row.ResearchConsumption + row.RepeatConsumption + row.QCConsumption + row.ManualConsumption
                        });
                    }
                }

                StatusMessage = $"Parsed {ParsedConsumptions.Count} reagent consumptions from {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error parsing PDF: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task LoadReagentsAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Loading reagents...";

                var reagents = await _apiService.GetReagentsAsync();

                AvailableReagents.Clear();
                foreach (var reagent in reagents.Where(r => r.IsActive))
                {
                    AvailableReagents.Add(reagent);
                }

                StatusMessage = $"Loaded {AvailableReagents.Count} reagents";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading reagents: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task CalculateConsumptionsAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Calculating consumptions with calibrations...";

                foreach (var consumption in ParsedConsumptions)
                {
                    var reagent = AvailableReagents.FirstOrDefault(r => r.Id == consumption.ReagentId);
                    if (reagent != null)
                    {
                        var calibrationCount = SelectedCalibrations.Count(c => c.Code == reagent.Code);
                        consumption.CalibrationConsumption = calibrationCount * reagent.CalibrationConsumption;
                        consumption.FinalTotal = consumption.TotalConsumption + consumption.CalibrationConsumption;

                        consumption.Px = (int)(consumption.ResearchConsumption - consumption.ManualConsumption);
                        consumption.Rep = (int)consumption.RepeatConsumption;
                        consumption.QC = (int)consumption.QCConsumption;
                        consumption.Cal = (int)consumption.CalibrationConsumption;
                        consumption.Canc = (int)consumption.ManualConsumption;
                        consumption.Motivo = "[Seleccione]";
                    }
                }

                var inventory = await _apiService.GetInventoryItemsAsync();
                ValidationErrors.Clear();

                var errors = _calculator.GetValidationErrors(ParsedConsumptions.ToList(), inventory);
                foreach (var error in errors)
                {
                    ValidationErrors.Add(error);
                }

                if (ValidationErrors.Any())
                {
                    StatusMessage = $"Warning: {ValidationErrors.Count} items have insufficient stock";
                }
                else
                {
                    StatusMessage = "All consumptions calculated. Inventory validated.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error calculating: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task SubmitToQuimiOSAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Connecting to QuimiOS...";

                var sessionCookie = await _quimiosClient.GetSessionCookieAsync();
                if (string.IsNullOrEmpty(sessionCookie))
                {
                    StatusMessage = "Failed to get session cookie from QuimiOS";
                    return;
                }

                StatusMessage = "Authenticating with QuimiOS...";
                var authenticated = await _quimiosClient.AuthenticateAsync(sessionCookie);
                if (!authenticated)
                {
                    StatusMessage = "Failed to authenticate with QuimiOS";
                    return;
                }

                StatusMessage = "Loading inventory grid...";
                var gridData = await _quimiosClient.LoadInventoryGridAsync(sessionCookie, ConsumptionDate);

                if (!gridData.Rows.Any())
                {
                    StatusMessage = "Failed to load inventory grid from QuimiOS";
                    return;
                }

                StatusMessage = "Submitting consumption to QuimiOS...";
                var success = await _quimiosClient.SubmitConsumptionAsync(
                    sessionCookie,
                    ConsumptionDate,
                    ParsedConsumptions.ToList(),
                    gridData);

                if (success)
                {
                    StatusMessage = "Successfully submitted to QuimiOS";
                }
                else
                {
                    StatusMessage = "Failed to submit to QuimiOS";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting to QuimiOS: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task SubmitToHubAsync()
        {
            try
            {
                IsProcessing = true;
                StatusMessage = "Submitting to QuimiOSHub...";

                var success = await _apiService.SubmitConsumptionAsync(ConsumptionDate, ParsedConsumptions.ToList());

                if (success)
                {
                    StatusMessage = "Successfully submitted to QuimiOSHub";
                }
                else
                {
                    StatusMessage = "Failed to submit to QuimiOSHub";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error submitting to Hub: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
