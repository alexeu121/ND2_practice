﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketsResale.Business.Models;
using TicketsResale.Models;
using TicketsResale.Models.Service;

namespace TicketsResale.Controllers
{
    public class TicketsController : Controller
    {
        private readonly IStringLocalizer<TicketsController> localizer;
        private readonly ITicketsService ticketsService;
        private readonly IEventsService eventsService;
        private readonly IVenuesService venuesService;
        private readonly ICitiesService citiesService;
        private readonly IUsersAndRolesService usersAndRolesService;
        private readonly IOrdersService ordersService;

        public TicketsController(ITicketsService ticketsService, IEventsService eventsService, IVenuesService venuesService, ICitiesService citiesService, IUsersAndRolesService usersAndRolesService, IOrdersService ordersService, IStringLocalizer<TicketsController> localizer)
        {
            this.localizer = localizer;
            this.ticketsService = ticketsService;
            this.eventsService = eventsService;
            this.venuesService = venuesService;
            this.citiesService = citiesService;
            this.usersAndRolesService = usersAndRolesService;
            this.ordersService = ordersService;
        }

        public async Task<IActionResult> GetEventTickets(int eventId)
        {
            EventTicketsViewModel model;

            Dictionary<Event, List<Ticket>> dic = new Dictionary<Event, List<Ticket>>();

            ViewData["Title"] = localizer["ticketsTitle"];

            var @event = await eventsService.GetEventById(eventId);

            var eventTickets = await ticketsService.GetTicketsByEventId(eventId);
            var eventVenue = await venuesService.GetVenueById(@event.VenueId);
            var eventCity = await citiesService.GetCityById(eventVenue.CityId);

            var sellersIdsByEvent = eventTickets.AsEnumerable().Select(et => et.SellerId).ToList();

            var sellersOfTicketsByEvent = await usersAndRolesService.GetUsersByListOfId(sellersIdsByEvent);

            dic.Add(@event, eventTickets);

            model = new EventTicketsViewModel
            {
                eventTickets = dic,   
                Venue = eventVenue,
                City = eventCity,
                Sellers = sellersOfTicketsByEvent
            };


            return View("EventTickets", model);
        }


        [Authorize]
        public async Task<IActionResult> MyTickets(TicketStatuses ticketStatus, string userName)
        {
            ViewData["Title"] = localizer["myTicketsTitle"];

            var tickets = await ticketsService.GetTicketsByStatusAndUserName(ticketStatus, userName);

            var model = new MyTicketsViewModel
            {
                Tickets = tickets,
                Events = await eventsService.GetEventsByTickets(tickets),
                Users = await usersAndRolesService.GetUsers(),
                ticketStatus = ticketStatus
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Buy([FromRoute] int id)
        {
            var ticket = await ticketsService.GetTicketById(id);

            await ordersService.AddTicketToOrder(User.Identity.Name, ticket);

            ticket.Status = TicketStatuses.waiting;

            await ticketsService.UpdTicketToDb(ticket);

            return RedirectToAction("Index", "Orders", ticket);
        }


        [Authorize]
        public async Task<IActionResult> CreateTicket()
        {
            ViewData["Title"] = localizer["createTicket"];

            var events = await eventsService.GetEvents();

            var listEvents = new SelectList(events, "Id", "Name");

            ViewBag.Events = listEvents;

            return View("CreateTicket");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTicket(TicketCreateEditModel model)
        {
            var user = await usersAndRolesService.GetUserByUserName(User.Identity.Name);

            var @event = await eventsService.GetEventById(model.Event.Id);

            var venue = await venuesService.GetVenueById(@event.VenueId);

            var city = await citiesService.GetCityById(venue.CityId);

            if (venue != null)
                @event.Venue = venue;

            if (city != null)
                @event.Venue.City = city;

            if (user != null)
                model.Seller = user;

            if (@event != null)
                model.Event = @event;

            Ticket ticket = new Ticket() { EventId = model.Event.Id, Event = model.Event, SellerId = model.Seller.Id, Seller = model.Seller, Price = model.Price, Status = model.Status };

            await ticketsService.AddTicketToDb(ticket);

            return RedirectToAction("MyTickets", new { ticketStatus = 1, userName = User.Identity.Name });

        }


        [Authorize]
        public async Task<IActionResult> ConfirmTicket(int id)
        {
            var ticket = await ticketsService.GetTicketById(id);
            var @event = await eventsService.GetEventById(ticket.EventId);

            if (ticket != null)
            {
                var model = new ConfirmRejectTicketViewModel
                {
                    Ticket = ticket,
                    Event = @event
                };

                return View(model);
            }
           
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmTicket(ConfirmRejectTicketViewModel model, int ticketId)
        {
            var needOrders = await ordersService.GetOrdersByTicketId(ticketId);

            var needTicket = await ticketsService.GetTicketById(ticketId);

            foreach (Order order in needOrders)
            {
                order.Status = model.Confirmation ? OrderStatuses.confirmed : OrderStatuses.rejected;
                order.TrackNumber = model.TrackNumber;
                await ordersService.UpdOrderToDb(order);
            }

            if (model.Confirmation)
            {
                needTicket.Status = TicketStatuses.sold;
                await ticketsService.UpdTicketToDb(needTicket);
            }

            return RedirectToAction("MyTickets", new { ticketStatus = 1, orderStatus = 0, userName = User.Identity.Name });

        }

    }
}
