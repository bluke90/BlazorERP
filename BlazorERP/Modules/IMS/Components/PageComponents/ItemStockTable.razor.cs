﻿using BlazorERP.Data.Context;
using BlazorERP.Data.Entities;
using BlazorERP.Modules.Services;
using BlazorERP.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using MudBlazor;

namespace BlazorERP.Modules.IMS.Components.PageComponents;

public class ItemStockTableBase : ComponentBase
{
    [Parameter]
    public required List<Stock> Stocks { get; set; }
    [Parameter]
    public string TableClass { get; set; } = string.Empty;
    
    [Parameter]
    public ItemStockTableVariant ItemStockTableVariant { get; set; } = ItemStockTableVariant.ByStorageLocation;
    [Parameter]
    public bool TableOnly { get; set; } = false; // If true, the table will not have any actions or selection
    [Parameter]
    public bool Readonly { get; set; } = false; // If true, the table will not allow any actions or selection
    [Parameter]
    public Item? SelectedItem { get; set; }
    
    [Inject]
    protected ImsService Ims { get; private set; }
    [Inject]
    protected ISnackbar Snackbar { get; private set; }
    [Inject]
    private proContext _context { get; set; } // Injected for loading references
    protected ItemStockModel? SelectedStock { get; set; }
    protected List<ItemStockModel> ItemStockModels { get; set; } = new List<ItemStockModel>();
    protected List<StorageLocation>? StorageLocations { get; set; } = new List<StorageLocation>();
    protected StorageLocation? SelectedStorageLocation { get; set; }
    
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
            await _context.LoadReferenceAsync<Stock, Item>(Stock, x => x.Item);
            await _context.LoadReferenceAsync<Stock, StorageLocation>(Stock, x => x.StorageLocation);
            await _context.LoadReferenceAsync<Item, Unit>(Stock.Item, x => x.Unit);
            ItemStockModels.Add(new ItemStockModel(Stock.Item, Stock));
        }
    }
    
    private async Task RefreashStorageLocations()
    {
        StorageLocations = await Ims.GetAllStorageLocations();
    }
    
    public async Task AddStockClick()
    {
        // Check if SelectedStock is null
        if (SelectedStock is not null)
        {
            Snackbar.Add("Please save or cancel the current stock before adding a new one.", Severity.Warning);
            return;
        }

        if (SelectedItem is not null)
        {
            // Create a new stock for the selected item
            var newStock = new Stock
            {
                ItemId = SelectedItem.ItemId, // Assuming SelectedStock is set before this method is called
                Item = SelectedItem,
                StorageLocationId = 0, // Default or selected storage location ID
                StorageLocation = new StorageLocation() {StorageLocationId = 0, Code = "Default Location", Name = "Default"}, // Placeholder for storage location
                OnHand = 0 // Default initial stock quantity
            };
            
            // Create a new ItemStockModel with the selected item and the new stock
            var newItemStockModel = new ItemStockModel(SelectedItem, newStock);
            ItemStockModels.Add(newItemStockModel);
            SelectedStock = newItemStockModel;
            RefreashStorageLocations();
        }
    }

    public async Task SaveStock()
    {
        // Set the storage location for the stock based on selected code
        var newStock = SelectedStock.Stock.StorageLocationId == 0;
        if (newStock)
        {
            if (SelectedStock.Stock.StorageLocation.Code == "Default Location")
            {
                Snackbar.Add("Please select a valid storage location.", Severity.Warning);
                ItemStockModels.Remove(SelectedStock);
                SelectedStock = null;
                return;
            }
            
            SelectedStock.Stock.StorageLocation = StorageLocations.FirstOrDefault(x => x.Code == SelectedStock.Stock.StorageLocation.Code);
            SelectedStock.Stock.StorageLocationId = SelectedStock.Stock.StorageLocation.StorageLocationId;            
            
            if (Ims.StockExist(SelectedStock.Stock))
            {
                Snackbar.Add("Stock already exists for this item and storage location.", Severity.Warning);
                ItemStockModels.Remove(SelectedStock);
                return;
            }
        }
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
