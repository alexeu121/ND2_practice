﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketsResale.Business.Models;
using TicketsResale.Context;

namespace TicketsResale.Models.Service
{
    public class EventsService : IEventsService
    {
        private readonly StoreContext context;

        public EventsService(StoreContext storeContext)
        {
            this.context = storeContext;
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            return await context.Events.ToListAsync();
        }

        public async Task<Event> GetEventById(int id)
        {
            return await context.Events.FindAsync(id);
        }

    }
}