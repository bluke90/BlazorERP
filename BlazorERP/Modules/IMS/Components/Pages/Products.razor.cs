using Microsoft.AspNetCore.Components;
using BlazorERP.Data.Entities;
using BlazorERP.Modules.IMS.Components.Dialogs;
using BlazorERP.Modules.IMS.Components.PageComponents;
using BlazorERP.Modules.Services;
using MudBlazor;

public class ProductsBase : ComponentBase
{
    
    // Injects
    [Inject]
    protected ImsService Ims { get; private set; }
    [Inject]
    protected ISnackbar Snackbar { get; set; }
    [Inject]
    protected IDialogService Dialog { get; set; }
    // List
    public List<Item>? Items { get; set; } = new List<Item>();
    
    // Selected Item 
    public Item? SelectedItem { get; set; }
    public string? SelectedItemImageUrl { get; set; }
    public Stock? SelectedStock { get; set; }
    public List<StorageLocation>? SelectedStores { get; set; }
    
    // Drop Down
    public IEnumerable<StorageLocation>? StorageLocations { get; set; }
    public StorageLocation? SelectedStorageLocation { get; set; }
    
    // Filters
    protected IEnumerable<StorageLocation> LocationFilter { get; set; } = new List<StorageLocation>();
    protected bool LocationFilterFunc(Item item) => LocFilter(item, LocationFilter.ToList());
    
    protected ItemStockTable? _itemStockTable { get; set; }
    
    // Tool Window Params
    protected string SecondaryButtonText = "Deactivate";
    protected string SecondaryButtonIcon = Icons.Material.Filled.Error;
    protected Color SecondaryButtonColor { get; set; } = Color.Error;
    protected EventCallback SecondaryButtonClick { get; set; }

    private bool LocFilter(Item item, List<StorageLocation> locations)
    {
        if (locations is null || item is null || locations.Count == 0)
            return true;
        
        if (item.Stocks
            .Any(stock => locations
                .Any(loc => loc.StorageLocationId == stock.StorageLocationId)))
            return true; // At least one stock matches the location

        return false;
    }
    
             
    // Loads storage locations and items with stock locations from the IMS service.
    protected override async Task OnInitializedAsync()
    {
        // Call the base class's initialization logic
        await base.OnInitializedAsync();
        
        // Ensure the IMS service is injected
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
        
        // Load storage locations for the dropdown menu
        StorageLocations = await Ims.GetAllStorageLocations();
        
        // Fetch items with stock locations from the IMS service
        var ItemsTask = Ims.GetItems(includeStockLocations: true);
        await Task.WhenAll(ItemsTask);
        
        // Assign the fetched items to the Items property
        Items = ItemsTask.Result;
        
        StateHasChanged();
    }


    public async Task ProductSelected(TableRowClickEventArgs<Item> args)
    {
        // Get the clicked item from the event arguments
        Item clicked_item = args.Item;
        
        // Check if the clicked item is different from the currently selected item
        if (SelectedItem != clicked_item) 
        {
            // Set Item
            SelectedItem = clicked_item;
            // Get Icon Data Url
            var itemImage = await Ims.GetItemImage(SelectedItem.ItemId);
            SelectedItemImageUrl = SetItemImageUrl(itemImage);
            // Get Location codes containing the item
            SelectedStores = await Ims.GetItemStore(SelectedItem.ItemId);

            // Set secondary button parameters based on the item's active status
            if (SelectedItem.IsActive)
            {
                SecondaryButtonText = "Deactivate";
                SecondaryButtonIcon = Icons.Material.Filled.Error;
                SecondaryButtonColor = Color.Error;
                SecondaryButtonClick = EventCallback.Factory.Create(this, async () => await DeactivateProduct());
            }
            else
            {
                SecondaryButtonText = "Activate";
                SecondaryButtonIcon = Icons.Material.Filled.Start;
                SecondaryButtonColor = Color.Info;
                SecondaryButtonClick = EventCallback.Factory.Create(this, async () => await ActivateProduct());
            }
            
            StateHasChanged();
        }
    }
    
    protected async Task ActivateProduct()
    {
        if (SelectedItem is null)
        {
            Snackbar.Add("No product selected", Severity.Error);
            return;
        }

        // Activate the selected item
        SelectedItem.IsActive = true;
        await SaveProduct();
        
        Snackbar.Add("Product activated successfully", Severity.Success);
    }
    
    protected async Task DeactivateProduct()
    {
        if (SelectedItem is null)
        {
            Snackbar.Add("No product selected", Severity.Error);
            return;
        }

        // Deactivate the selected item
        SelectedItem.IsActive = false;
        await SaveProduct();
        
        Snackbar.Add("Product deactivated successfully", Severity.Warning);
    }
    
    private static string SetItemImageUrl(ItemImage itemImage)
    {
        var ImageUrl = string.Empty;
        
        // Set the image URL for the selected item       
        if (itemImage is not null && itemImage.Content is not null)
        {
            ImageUrl = $"data:{itemImage.MimeType};base64,{Convert.ToBase64String(itemImage.Content)}";
        }
        
        return ImageUrl;
    }

    public async Task SaveStock()
    {
        // Validate that both SelectedStock and SelectedItem are not null
        if (SelectedStock is null || SelectedItem is null)
        {
            Snackbar.Add("No Stock Selected", Severity.Error);
            return;
        }
        
        // Save the stock for the selected item
        await Ims.SaveStock(SelectedStock); ;

        // Set SelectedStock to null after saving
        SelectedStock = null;
        
        Snackbar.Add("Stock saved", Severity.Success);
    }
    
    // Save product method
    protected async Task SaveProduct()
    {
        if (SelectedItem is null)
        {
            Snackbar.Add("No Item Selected", Severity.Error);
            return;
        }

        // Save the item
        await Ims.SaveItem(SelectedItem);

        // Refresh the items list
        Items = await Ims.GetItems(includeStockLocations: true);
        
        Snackbar.Add("Product saved successfully", Severity.Success);
    }
    
    public async Task OpenNewProductDialog()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            BackdropClick = true,
            Position = DialogPosition.Center
        };
        
        var dialog = await Dialog.ShowAsync<_AddProduct>("New Product", options: options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            // Refresh the items list after adding a new product
            Items = await Ims.GetItems(includeStockLocations: true);
            Snackbar.Add("Product added successfully", Severity.Success);
        }
    }

    private Item prevItem = null;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender && SelectedItem != null && prevItem != SelectedItem)
        {
            
        }
    }
    

}
