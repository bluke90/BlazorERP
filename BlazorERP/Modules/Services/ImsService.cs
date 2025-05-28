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
    
    public async Task<List<Stock>> GetStock()
    {
        var stock = await _context.Stocks
            .Include(x => x.StorageLocation)
            .Include(x => x.Item)
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

    public async Task<List<Item>> GetItems()
    {
        var items = await _context.Items
            .Include(x => x.Category)
            .Include(x => x.Stocks)
            .ToListAsync();
        
        return items;
    }
}