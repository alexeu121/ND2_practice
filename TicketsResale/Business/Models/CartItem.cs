﻿namespace TicketsResale.Business.Models
{
    public class CartItem
    {
        public int TicketId { get; set; }
        public int TicketsCartId { get; set; }
        public int Count { get; set; }

        public Ticket Ticket { get; set; }
        public TicketsCart TicketsCart { get; set; }
    }
}