using Microsoft.AspNetCore.Components;
using BlazorERP.Data.Entities;
using BlazorERP.Modules.IMS.Components.Dialogs;
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
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
        
        // Load storage locations for dropdown
        StorageLocations = await Ims.GetAllStorageLocations();
        
        var ItemsTask = Ims.GetItems(includeStockLocations: true);
        await Task.WhenAll(ItemsTask);
        Items = ItemsTask.Result;
    }

    public async Task ProductSelected(TableRowClickEventArgs<Item> args)
    {
        Item clicked_item = args.Item;
        if (SelectedItem != clicked_item) 
        {
            // Set Item
            SelectedItem = clicked_item;
            // Get Icon Data Url
            var itemImage = await Ims.GetItemImage(SelectedItem.ItemId);
            SelectedItemImageUrl = SetItemImageUrl(itemImage);
            // Get Location codes containing the item
            SelectedStores = await Ims.GetItemStore(SelectedItem.ItemId);
        }
    }
    
    private static string SetItemImageUrl(ItemImage itemImage)
    {
            var ImageUrl = string.Empty;
            
            // Set the image URL for the selected item       
            if (itemImage is not null)
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
            return;
        }
        
        // Save the stock for the selected item
        await Ims.SaveStock(SelectedStock); ;

        // Set SelectedStock to null after saving
        SelectedStock = null;
        
        Snackbar.Add("Stock saved", Severity.Success);
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
    

}
