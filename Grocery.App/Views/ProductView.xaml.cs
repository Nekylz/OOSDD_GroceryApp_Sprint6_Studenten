using Grocery.App.ViewModels;

namespace Grocery.App.Views;

public partial class ProductView : ContentPage
{
    private readonly ProductViewModel _viewModel;

    public ProductView(ProductViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <summary>
    /// Deze methode wordt automatisch aangeroepen door MAUI
    /// wanneer de pagina op het scherm verschijnt
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing(); // Trigger de ViewModel's OnAppearing
    }
}