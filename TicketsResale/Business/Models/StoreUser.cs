﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketsResale.Business.Models
{
    public class StoreUser : IdentityUser
    { 

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public Localizations Localization { get; set; }

    }
}
