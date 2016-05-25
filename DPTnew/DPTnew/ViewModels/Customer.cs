using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPTnew.ViewModels
{
    public class Customer
    {
        public Company Company { get; set; }
        public Contact Contact { get; set; }
    }
}