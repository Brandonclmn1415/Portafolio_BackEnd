using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Portafolio.Models
{
    public class ContactFormModel
    {
        public required string Email { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
    }
}