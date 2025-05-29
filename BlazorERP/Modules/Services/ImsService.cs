using BlazorERP.Data.Context;
using BlazorERP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Modules.Services;

public class ImsService
{
    public readonly proContext _context;
    
    public ImsService(proContext context)
    {
        _context = context;
    }

    public async Task SaveStock(Stock stock)
    {
        var stockToUpdate = await _context.Stocks
            .Include(x => x.Item)
            .Include(x => x.StorageLocation)
            .FirstOrDefaultAsync(x => x.StorageLocationId == stock.StorageLocationId && x.ItemId == stock.ItemId);
        
        if (stockToUpdate != null)
        {
            stockToUpdate.OnHand = stock.OnHand;
            _context.Stocks.Update(stockToUpdate);
        }
        else
        {
            // If the stock does not exist, add it
            _context.Stocks.Add(stock);
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Stock>> GetStock()
    {
        var stock = await _context.Stocks
            .Include(x => x.StorageLocation)
            .Include(x => x.Item)
            .ThenInclude(x => x.Unit)
            .ToListAsync();
        
        return stock;
    }
    
    public async Task<List<PurchaseOrder>> GetPurchases()
    {
        var purchases = await _context.PurchaseOrders
            .Include(x => x.Supplier)
            .ToListAsync();
        
        return purchases;
    }

    public async Task<List<SalesOrderLine>> GetSales()
    {
        var sales = await _context.SalesOrderLines
            .Include(x => x.Item)
            .ToListAsync();
        
        return sales;
        
    }

    public async Task<List<Item>> GetItems(bool includeCategory = true, bool includeStocks = true, bool includeStockLocations = false)
    {
        var query = _context.Items.AsQueryable();

        if (includeCategory)
        {
            query = query.Include(x => x.Category);
        }

        if (includeStocks)
        {
            
            if (includeStockLocations)
                query = query.Include(x => x.Stocks).ThenInclude(s => s.StorageLocation);
            else
                query = query.Include(x => x.Stocks);
        }

        var items = await query.ToListAsync();
        
        return items;
    }

    public async Task<List<StorageLocation>> GetStorageLocations(bool includeStocks = false, bool includeTransactions = false)
    {
        var query = _context.StorageLocations.AsQueryable();

        if (includeStocks)
        {
            query = query.Include(x => x.Stocks);
        }

        if (includeTransactions)
        {
            query = query.Include(x => x.InventoryTransactions);
        }

        var storageLocations = await query.ToListAsync();
        
        return storageLocations;
    }
    
}