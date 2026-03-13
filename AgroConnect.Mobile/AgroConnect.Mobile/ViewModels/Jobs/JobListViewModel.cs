using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.ViewModels.Jobs;

public partial class JobListViewModel : ObservableObject
{
    private readonly IJobService _jobs;

    public JobListViewModel(IJobService jobs) => _jobs = jobs;

    public ObservableCollection<JobPostingListDto> Jobs { get; } = [];

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isEmpty;

    [RelayCommand]
    private async Task LoadJobsAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Jobs.Clear();

            var items = await _jobs.GetAvailableJobsAsync();
            foreach (var item in items)
                Jobs.Add(item);

            IsEmpty = Jobs.Count == 0;
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ApplyToJobAsync(JobPostingListDto job)
    {
        if (job.AlreadyApplied)
        {
            await Shell.Current.DisplayAlert("Info", "Ya te postulaste a este trabajo.", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Postularse",
            $"¿Querés postularte a \"{job.Title}\"?",
            "Sí, postularme", "Cancelar");

        if (!confirm) return;

        try
        {
            var result = await _jobs.ApplyToJobAsync(job.Id, new ApplyToJobRequest());
            if (result)
            {
                await Shell.Current.DisplayAlert("Listo", "Postulación enviada.", "OK");
                await LoadJobsAsync(); // Refresh
            }
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
