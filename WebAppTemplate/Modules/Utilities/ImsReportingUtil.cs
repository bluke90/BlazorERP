using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WebAppTemplate.Modules.Services;

namespace WebAppTemplate.Modules.Utilities;

public static class ImsReportingUtil
{
    public static List<(string SupplierName, decimal Percentage)> GetStockSupplierPrecent(this ImsService ims)
    {
        // create list of tuples for each supplier and its total percentage of stock
        var tupleList = new List<(string SupplierName, decimal Percentage)>();
        
        
        // Get all purchase order lines and suppliers
        var POLs = ims._context.PurchaseOrderLines
            .Include(x => x.PO)
            .Include(x => x.Item)
            .ToList();
        
        // Get all suppliers
        var suppliers = ims._context.Suppliers
            .ToList();

        // Get the percentage of product each supplier is supplying
        foreach (var supplier in suppliers)
        {
            var count = POLs.Count(x => x.PO.SupplierId == supplier.SupplierId);
            if (count > 0)
            {
                var total = POLs.Count();
                var percentage = (decimal)count / total * 100;
                tupleList.Add((supplier.Name, percentage));
            }
        }
            
        return tupleList;
    }
    
    public static (string[] XAxisLabels, List<ChartSeries> itemStock) GetItemStockByLocation(this ImsService ims)
    {
        // Get all stock items
        
        var xAxisLabels = ims._context.Items
            .Include(x => x.Stocks)
            .Where(x => x.Stocks.Count > 0)
            .OrderByDescending(x => x.Name)
            .Select(x => x.Name)
            .ToList();
        
        // Get all storage locations
        var locations = ims._context.StorageLocations
            .Include(x => x.Stocks)
            .ThenInclude(x => x.Item)
            .ToList();

        // Create a list of item stock by location
        var chartSeriesList = new List<ChartSeries>();
        foreach (var loc in locations)
        {
            var chartSeries = new ChartSeries() { Name = loc.Name  , Data = loc.Stocks.OrderByDescending(x => x.Item.Name).Select(x => (double)x.OnHand).ToArray() };
            chartSeriesList.Add(chartSeries);
        }

        return (xAxisLabels.ToArray(), chartSeriesList);
    }
}