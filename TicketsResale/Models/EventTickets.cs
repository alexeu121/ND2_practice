﻿using System.Collections.Generic;
using TicketsResale.Context;

namespace TicketsResale.Models
{
    public class EventTickets
    {
        public Dictionary<Event, List<Ticket>> eventTickets { get; set; }

    }
}