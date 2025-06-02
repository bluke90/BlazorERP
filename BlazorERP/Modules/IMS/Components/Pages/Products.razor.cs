using Microsoft.AspNetCore.Components;
using BlazorERP.Data.Entities;
using BlazorERP.Modules.IMS.Components.Dialogs;
using BlazorERP.Modules.Services;
using MudBlazor;

public class ProductsBase : ComponentBase
{
    
    [Inject]
    protected ImsService Ims { get; private set; }
    [Inject]
    protected ISnackbar Snackbar { get; set; }
    [Inject]
    protected IDialogService Dialog { get; set; }
    
    public List<Item>? Items { get; set; } = new List<Item>();
    
    public Item? SelectedItem { get; set; }
    public string? SelectedItemImageUrl { get; set; }
    public Stock? SelectedStock { get; set; }
    
    // Dropdown List
    public List<StorageLocation> StorageLocations { get; set; } = new List<StorageLocation>();
    
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
        
        var ItemsTask = Ims.GetItems(includeStockLocations: true);
        await Task.WhenAll(ItemsTask);
        Items = ItemsTask.Result;
        
    }

    public async Task ProductSelected(TableRowClickEventArgs<Item> args)
    {
        Item clicked_item = args.Item;
        if (SelectedItem != clicked_item) 
        {
            SelectedItem = clicked_item;
            await SetItemImageUrl();
        }
    }
    
    private async Task SetItemImageUrl()
    {
        if (SelectedItem is not null)
        {
            // Set the image URL for the selected item
            var itemImage = await Ims.GetItemImage(SelectedItem.ItemId);
            if (itemImage is not null)
            {
                SelectedItemImageUrl = $"data:{itemImage.MimeType};base64,{Convert.ToBase64String(itemImage.Content)}";
            }
            else
            {
                SelectedItemImageUrl = null; // Reset if no image found
            }
            
            return;
        }

        // Reset the image URL if no item is selected
        SelectedItemImageUrl = null;
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
