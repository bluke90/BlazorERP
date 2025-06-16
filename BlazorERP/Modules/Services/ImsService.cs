using BlazorERP.Data.Context;
using BlazorERP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Modules.Services;

public class ImsService
{
    private readonly proContext _context;
    public readonly IDbContextFactory<proContext> _dbContextFactory;
    
    public ImsService(proContext context, IDbContextFactory<proContext> dbContextFactory)
    {
        _context = context;
        _dbContextFactory = dbContextFactory;
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
        var db = _dbContextFactory.CreateDbContext();
        
        // Get all storage locations that have stock for the specified item
        var storageLocations = await db.Stocks
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

    public async Task<List<SalesOrder>> GetSalesOrders()
    {
        var orders = await _context.SalesOrders
            .Include(x => x.Customer)
            .Include(x => x.SalesOrderLines)
                .ThenInclude(line => line.Item)
            .ToListAsync();

        return orders;

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
        var db = _dbContextFactory.CreateDbContext();
        
        return await db.ItemImages
            .FirstOrDefaultAsync(x => x.ItemId == itemId);
    }

    public async Task<List<Customer>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    public bool StockExist(Stock stock)
    {
        return _context.Stocks.Any(x => x.ItemId == stock.ItemId && x.StorageLocationId == stock.StorageLocationId);
    }
    
    public async Task<List<PurchaseOrder>> GetPurchaseOrders()
    {
        var orders = await _context.PurchaseOrders
            .Include(x => x.OrderStatus)
            .Include(x => x.Supplier)
            .Include(x => x.PurchaseOrderLines)
            .ThenInclude(line => line.Item)
            .ToListAsync();

        return orders;
    }
    
    public async Task<List<Supplier>> GetSuppliers()
    {
        return await _context.Suppliers.ToListAsync();
    }

    public async Task SaveItem(Item item)
    {
        var itemToUpdate = await _context.Items
            .FirstOrDefaultAsync(x => x.ItemId == item.ItemId);
        
        if (itemToUpdate != null)
        {
            itemToUpdate.Name = item.Name;
            itemToUpdate.CategoryId = item.CategoryId;
            itemToUpdate.UnitId = item.UnitId;
            itemToUpdate.SKU = item.SKU;
            itemToUpdate.ReorderPoint = item.ReorderPoint;
            itemToUpdate.DefaultCost = item.DefaultCost;
            itemToUpdate.DefaultPrice = item.DefaultPrice;
            _context.Items.Update(itemToUpdate);
        }
        else
        {
            // If the item does not exist, add it
            _context.Items.Add(item);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderStatus>> GetOrderStatuses()
    {
        return await _context.OrderStatuses
            .ToListAsync();
    }
    
    public async Task UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
    {
        _context.Update(purchaseOrder);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateSalesOrder(SalesOrder salesOrder)
    {
        _context.Update(salesOrder);
        await _context.SaveChangesAsync();
    }
}