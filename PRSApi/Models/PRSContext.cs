using System;
using System.Collections.Generic;
using ConsoleLibrary;
using Microsoft.EntityFrameworkCore;

namespace PRSApi.Models;

public partial class PRSContext : DbContext {

    public PRSContext(DbContextOptions<PRSContext> options)
        : base(options) {
    }

    public virtual DbSet<LineItem> LineItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public async Task RecalcTotal(int requestId) {
        var request = await Requests.FindAsync(requestId);
        if (request==null) {
            MyConsole.PrintLine("Request Not Found.");
            return;
        }
        var lineItems = await LineItems
                                       .Where(li => li.RequestId==requestId)
                                       .Include(li => li.Product)
                                       .ToListAsync();
        decimal total = lineItems.Sum(li => li.Product!=null ? li.Quantity*li.Product.Price : 0);
        decimal previousTotal = request.Total;
        request.Total=total;
        // if user changes line item quantity to be more than $50 revert status to "NEW"
        if (previousTotal<=50&&total>50) {
            request.Status="NEW";
        }
        // auto approve request if <$50
        else if (total<=50) {
            request.Status="APPROVED";
        }

        int changes = await SaveChangesAsync();
    }
}