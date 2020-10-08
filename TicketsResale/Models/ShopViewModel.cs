﻿using TicketsResale.Context;

namespace TicketsResale.Models
{
    public class ShopViewModel
    {
        public Event[] Events { get; set; }

        public Ticket[] Tickets { get; set; }
        
        public Order[] Orders { get; set; }

        public User User { get; set; }
    }
}