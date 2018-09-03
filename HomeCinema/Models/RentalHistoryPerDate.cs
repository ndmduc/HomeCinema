using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Models
{
    public class RentalHistoryPerDate
    {
        public int TotalRentals { get; set; }

        public DateTime Date { get; set; }
    }
}