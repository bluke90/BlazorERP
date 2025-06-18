using System.Formats.Asn1;
using BlazorERP.Data.Context;
using BlazorERP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Modules.Services;

public class ImsService
{
    private readonly proContext _context;
    public readonly IDbContextFactory<proContext> _dbContextFactory;
    private readonly ILogger<ImsService> _logger;
    
    public ImsService(proContext context, IDbContextFactory<proContext> dbContextFactory, ILogger<ImsService> logger)
    {
        _context = context;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _logger.LogInformation("ImsService initialized");
    }
    
    // ======= Get Methods =======
    // Item and stock related methods
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
    
    public async Task<List<Stock>> GetStock()
    {
        var stock = await _context.Stocks
            .Include(x => x.StorageLocation)
            .Include(x => x.Item)
            .ThenInclude(x => x.Unit)
            .ToListAsync();
        
        return stock;
    }
    
    public async Task<List<ItemCategory>> GetItemCategoriesAsync()
    {
        return 
            await _context.ItemCategories.ToListAsync();
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
    
    // Storage location related methods
    public async Task<List<StorageLocation>> GetAllStorageLocations(bool includeStocks = false, bool includeTransactions = false)
    {
        // Create a query to get all storage locations
        var query = _context.StorageLocations.AsQueryable();

        // Include related entities based on the parameters
        if (includeStocks)
        {
            query = query.Include(x => x.Stocks)
                .ThenInclude(x => x.Item);
        }

        if (includeTransactions)
        {
            query = query.Include(x => x.InventoryTransactions);
        }

        // Execute the query and get the list of storage locations
        var storageLocations = await query.ToListAsync();
        
        return storageLocations;
    }
    
    public async Task<StorageLocation> GetStorageLocation(int storageLocationId, bool includeStocks = false, bool includeTransactions = false)
    {
        // Create a query to get the storage location by ID
        var query = _context.StorageLocations.AsQueryable()
            .Where(x => x.StorageLocationId == storageLocationId);

        // Include related entities based on the parameters
        if (includeStocks)
        {
            query = query.Include(x => x.Stocks);
        }

        if (includeTransactions)
        {
            query = query.Include(x => x.InventoryTransactions);
        }

        // Execute the query and get the storage location
        var storageLocation = await query.FirstOrDefaultAsync();
        
        return storageLocation;
    }
    
    public async Task<List<StorageLocation>> GetItemStore(int ItemId)
    {
        // Create a new DbContext instance
        var db = _dbContextFactory.CreateDbContext();
        
        // Get all storage locations that have stock for the specified item
        var storageLocations = await db.Stocks
            .Where(x => x.ItemId == ItemId)
            .Select(x => x.StorageLocation)
            .Distinct()
            .ToListAsync();

        return storageLocations;
    }
    

    
    // Sales and Supply related methods
    public async Task<List<OrderStatus>> GetOrderStatuses()
    {
        return await _context.OrderStatuses
            .ToListAsync();
    }

    public async Task<List<SalesOrder>> GetSalesOrders()
    {
        var orders = await _context.SalesOrders
            .Include(x => x.OrderStatus)
            .Include(x => x.Customer)
            .Include(x => x.SalesOrderLines)
            .ThenInclude(line => line.Item)
            .ToListAsync();

        return orders;
    }
    
    public async Task<List<Customer>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
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
    
    // ======= Save Methods =======
    
    
    public async Task SaveStorageLocation(StorageLocation storageLocation)
    {
        // Check if the storage location already exists
        var storageLocationToUpdate = await _context.StorageLocations
            .FirstOrDefaultAsync(x => x.StorageLocationId == storageLocation.StorageLocationId);
        
        // If it exists, update the existing storage location
        if (storageLocationToUpdate != null)
        {
            storageLocationToUpdate.Name = storageLocation.Name;
            storageLocationToUpdate.Code = storageLocation.Code;
            storageLocationToUpdate.Address = storageLocation.Address;
            _context.StorageLocations.Update(storageLocationToUpdate);
        }
        else // If it does not exist, add the new storage location
        {
            _context.StorageLocations.Add(storageLocation);
        }
        
        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    public async Task SaveStock(Stock stock)
    {
        // Check if the stock already exists for the given item and storage location
        var stockToUpdate = await _context.Stocks
            .Include(x => x.Item)
            .Include(x => x.StorageLocation)
            .FirstOrDefaultAsync(x => x.StorageLocationId == stock.StorageLocationId && x.ItemId == stock.ItemId);
        
        // If it exists, update the existing stock
        if (stockToUpdate != null)
        {
            stockToUpdate.OnHand = stock.OnHand;
            _context.Stocks.Update(stockToUpdate);
        }
        else // If it does not exist, add the new stock
        {
            _context.Stocks.Add(stock);
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task SaveItem(Item item)
    {
        // Check if the item already exists in the database
        var itemToUpdate = await _context.Items
            .FirstOrDefaultAsync(x => x.ItemId == item.ItemId);
        
        // If it exists, update the existing item
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
        { // If it does not exist, add the new item
            _context.Items.Add(item);
        }
        
        await _context.SaveChangesAsync();
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

    public async Task UpdateSupplier(Supplier supplier)
    {
        _context.Update(supplier);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateCustomer(Customer customer)
    {
        _context.Update(customer);
        await _context.SaveChangesAsync();
    }
    
 
    // ======= Check Methods =======
    /// <summary>
    /// Check if a stock exists for a given item and storage location.
    /// </summary>
    /// <param name="stock">The stock entity in which to check for duplicates</param>
    /// <returns>True if exist, False if doesn't exist</returns>
    public bool StockExist(Stock stock)
    {
        return _context.Stocks.Any(x => x.ItemId == stock.ItemId && x.StorageLocationId == stock.StorageLocationId);
    }
    

    


 
}