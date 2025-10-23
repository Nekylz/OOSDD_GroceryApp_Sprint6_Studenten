using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public ObservableCollection<Product> Products { get; set; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = [];
            LoadProducts(); // Laad producten bij initialisatie
        }

        /// <summary>
        /// Laadt alle producten uit de service en vult de ObservableCollection
        /// </summary>
        private void LoadProducts()
        {
            Products.Clear(); // Eerst legen
            foreach (Product p in _productService.GetAll())
            {
                Products.Add(p);
            }
        }

        /// <summary>
        /// Command om te navigeren naar NewProductView
        /// [RelayCommand] genereert automatisch NavigateToNewProductCommand
        /// </summary>
        [RelayCommand]
        private async Task NavigateToNewProduct()
        {
            await Shell.Current.GoToAsync(nameof(Views.NewProductView));
        }

        /// <summary>
        /// Deze methode wordt aangeroepen wanneer de pagina verschijnt
        /// Overschrijft de BaseViewModel.OnAppearing() methode
        /// </summary>
        public override void OnAppearing()
        {
            base.OnAppearing(); // Roep parent methode aan
            LoadProducts(); // Herlaad producten (zodat nieuwe producten zichtbaar zijn)
        }
    }
}