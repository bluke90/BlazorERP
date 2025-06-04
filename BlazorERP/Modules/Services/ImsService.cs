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
    
    public async Task SaveStorageLocation(StorageLocation storageLocation)
    {
        var storageLocationToUpdate = await _context.StorageLocations
            .FirstOrDefaultAsync(x => x.StorageLocationId == storageLocation.StorageLocationId);
        
        if (storageLocationToUpdate != null)
        {
            storageLocationToUpdate.Name = storageLocation.Name;
            storageLocationToUpdate.Code = storageLocation.Code;
            storageLocationToUpdate.Address = storageLocation.Address;
            _context.StorageLocations.Update(storageLocationToUpdate);
        }
        else
        {
            // If the storage location does not exist, add it
            _context.StorageLocations.Add(storageLocation);
        }
        
        await _context.SaveChangesAsync();
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

    public async Task<List<Stock>> GetItemStock(int ItemId, int StorageLocationId = 0)
    {
        // If StorageLocationId is provided, filter by it; otherwise, get all stock(s) for the item
        var stock = await _context.Stocks.Where(x => x.ItemId == ItemId && 
                                                (StorageLocationId == 0 || x.StorageLocationId == StorageLocationId))
            .Include(x => x.StorageLocation)
            .ToListAsync();

        return stock;
    }

    public async Task<List<StorageLocation>> GetItemStore(int ItemId)
    {
        // Get all storage locations that have stock for the specified item
        var storageLocations = await _context.Stocks
            .Where(x => x.ItemId == ItemId)
            .Select(x => x.StorageLocation)
            .Distinct()
            .ToListAsync();

        return storageLocations;
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
        query.Include(x => x.Unit);

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

    public async Task<List<StorageLocation>> GetAllStorageLocations(bool includeStocks = false, bool includeTransactions = false)
    {
        var query = _context.StorageLocations.AsQueryable();

        if (includeStocks)
        {
            query = query.Include(x => x.Stocks)
                .ThenInclude(x => x.Item);
        }

        if (includeTransactions)
        {
            query = query.Include(x => x.InventoryTransactions);
        }

        var storageLocations = await query.ToListAsync();
        
        return storageLocations;
    }
    
    public async Task<StorageLocation> GetStorageLocation(int storageLocationId, bool includeStocks = false, bool includeTransactions = false)
    {
        var query = _context.StorageLocations.AsQueryable()
            .Where(x => x.StorageLocationId == storageLocationId);

        if (includeStocks)
        {
            query = query.Include(x => x.Stocks);
        }

        if (includeTransactions)
        {
            query = query.Include(x => x.InventoryTransactions);
        }

        var storageLocation = await query.FirstOrDefaultAsync();
        
        return storageLocation;
    }
    
    public List<ItemCategory> GetItemCategories()
    {
        return 
            _context.ItemCategories.ToList();
    }
    
    public async Task<List<ItemCategory>> GetItemCategoriesAsync()
    {
        return 
            await _context.ItemCategories.ToListAsync();
    }
    
    public List<Unit> GetUnits()
    {
        return _context.Units.ToList();
    }

    public async Task<List<Unit>> GetUnitsAsync()
    {
        return await _context.Units.ToListAsync();
    }
    
    public async Task<ItemImage?> GetItemImage(int itemId)
    {
        return await _context.ItemImages
            .FirstOrDefaultAsync(x => x.ItemId == itemId);
    }

    public async Task<List<Customer>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }
}