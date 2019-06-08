using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrpProject.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal Money { get; set; }
    }
}