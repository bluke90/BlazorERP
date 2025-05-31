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

    public void ProductSelected(TableRowClickEventArgs<Item> args)
    {
        Item clicked_item = args.Item;
        if (SelectedItem != clicked_item) 
        {
            SelectedItem = clicked_item;
        }
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
