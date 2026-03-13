using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Jobs;

namespace AgroConnect.Mobile.Views.Jobs;

public partial class JobListPage : ContentPage
{
    private readonly JobListViewModel _vm;
    private readonly IAuthService _auth;

    public JobListPage(JobListViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!await _auth.IsAuthenticatedAsync()) return;
        if (_vm.Jobs.Count == 0)
            _vm.LoadJobsCommand.Execute(null);
    }
}
