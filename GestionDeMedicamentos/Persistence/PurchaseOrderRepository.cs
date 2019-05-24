﻿using GestiónDeMedicamentos.Database;
using GestiónDeMedicamentos.Domain;
using GestiónDeMedicamentos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestiónDeMedicamentos.Persistence
{
    public class PurchaseOrderRepository : BaseRepository, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(PostgreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PurchaseOrder>> ListAsync()
        {
            return await _context.PurchaseOrders.ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> ListAsync(DateTime date, string order)
        {
            var purchaseOrders = _context.PurchaseOrders.Include(p => p.MedicinePurchaseOrders).ThenInclude(mp => mp.Medicine).Where(p => (date == null || p.Date == (date)));

            bool descending = false;
            if (order != null)
            {
                order = order.Substring(0, 1).ToUpper() + order.Substring(1, order.Length - 1);
                if (order.EndsWith("_desc"))
                {
                    order = order.Substring(0, order.Length - 5);
                    descending = true;
                }

                if (descending)
                {
                    purchaseOrders = purchaseOrders.OrderByDescending(e => EF.Property<object>(e, order));
                }
                else
                {
                    purchaseOrders = purchaseOrders.OrderBy(e => EF.Property<object>(e, order));
                }
            }

            return await purchaseOrders.ToListAsync();
        }

        public async Task<PurchaseOrder> FindAsync(int id)
        {
            return await _context.PurchaseOrders.Include(p => p.MedicinePurchaseOrders).ThenInclude(mp => mp.Medicine).FirstOrDefaultAsync(p => p.Id == id);
        }


        public EntityState Update(PurchaseOrder purchaseOrder)
        {
            return _context.Entry(purchaseOrder).State = EntityState.Modified;
        }

        public async Task<EntityEntry> CreateAsync(PurchaseOrder purchaseOrder)
        {
            return await _context.PurchaseOrders.AddAsync(purchaseOrder);
        }

        public EntityEntry Delete(PurchaseOrder purchaseOrder)
        {
            return _context.PurchaseOrders.Remove(purchaseOrder);
        }

        public bool PurchaseOrderExists(int id)
        {
            return _context.Drugs.Any(e => e.Id == id);
        }
    }
}