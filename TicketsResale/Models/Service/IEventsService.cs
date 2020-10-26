﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketsResale.Business.Models;

namespace TicketsResale.Models.Service
{
    public interface IEventsService
    {
        Task<IEnumerable<CartItem>> GetCartItems();
        Task<IEnumerable<City>> GetCities();
        Task<Event> GetEventById(int id);
        Task<IEnumerable<Event>> GetEvents();
        Task<IEnumerable<StoreUser>> GetUsers();
        Task<IEnumerable<Venue>> GetVenues();
    }
}
