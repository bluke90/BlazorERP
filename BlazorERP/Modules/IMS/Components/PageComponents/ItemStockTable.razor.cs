using BlazorERP.Data.Entities;
using BlazorERP.Modules.Services;
using BlazorERP.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.PageComponents;

public class ItemStockTableBase : ComponentBase
{
    [Parameter]
    public required List<Stock> Stocks { get; set; }
    
    protected ItemStockModel? SelectedStock { get; set; }
    
    [Parameter]
    public ItemStockTableVariant ItemStockTableVariant { get; set; } = ItemStockTableVariant.ByStorageLocation;
    [Parameter]
    public bool TableOnly { get; set; } = false; // If true, the table will not have any actions or selection
    
    [Inject]
    protected ImsService Ims { get; private set; }
    
    [Inject]
    protected ISnackbar Snackbar { get; private set; }
    
    protected List<ItemStockModel> ItemStockModels { get; set; } = new List<ItemStockModel>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (Ims is null)
            throw new InvalidOperationException("ImsService was not injected.");
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        
        ItemStockModels.Clear();
        
        foreach (var Stock in Stocks)
        {
            await Ims._context.LoadReferenceAsync<Stock, Item>(Stock, x => x.Item);
            await Ims._context.LoadReferenceAsync<Stock, StorageLocation>(Stock, x => x.StorageLocation);
            await Ims._context.LoadReferenceAsync<Item, Unit>(Stock.Item, x => x.Unit);
            ItemStockModels.Add(new ItemStockModel(Stock.Item, Stock));
        }
    }

    public async Task SaveStock()
    {
        // Save the stock for the selected item
        await Ims.SaveStock(SelectedStock.Stock);

        // Set SelectedStock to null after saving
        SelectedStock = null;
        
        Snackbar.Add("Stock saved", Severity.Success);
    }
}


public class ItemStockModel
{
    public Item Item { get; set; }
    public Stock Stock { get; set; }
    
    public ItemStockModel(Item item, Stock stock)
    {
        Item = item;
        Stock = stock;
    }
}

public enum ItemStockTableVariant
{
    ByStorageLocation,
    ByItem
}
