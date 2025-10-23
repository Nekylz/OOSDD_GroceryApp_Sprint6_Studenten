using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly GlobalViewModel _global;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private DateTime shelfLifeDate = DateTime.Now.AddDays(30);

        [ObservableProperty]
        private bool isAdmin;

        public NewProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            _global = global;

            // Check of gebruiker admin is
            isAdmin = _global.Client?.Role == Role.Admin;
        }

        [RelayCommand]
        private async Task SaveProduct()
        {
            // Check admin rechten (gebruik hoofdletter property)
            if (!IsAdmin)
            {
                await Shell.Current.DisplayAlert("Geen toegang",
                    "Alleen admins mogen producten aanmaken.", "OK");
                return;
            }

            // Validatie (gebruik hoofdletter properties)
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Validatie",
                    "Naam is verplicht.", "OK");
                return;
            }

            if (Stock < 0)
            {
                await Shell.Current.DisplayAlert("Validatie",
                    "Voorraad mag niet negatief zijn.", "OK");
                return;
            }

            if (Price < 0)
            {
                await Shell.Current.DisplayAlert("Validatie",
                    "Prijs mag niet negatief zijn.", "OK");
                return;
            }

            var product = new Product(
                0, // ID wordt door database gegenereerd
                Name,
                Stock,
                DateOnly.FromDateTime(ShelfLifeDate),
                Price
            );

            _productService.Add(product);

            await Shell.Current.DisplayAlert("Succes",
                "Product succesvol aangemaakt!", "OK");

            // Terug naar vorige pagina
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}