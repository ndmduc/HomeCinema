using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Models
{
    public class TotalRentalHistoryViewModel
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public List<RentalHistoryPerDate> Rentals { get; set; }

        public int TotalRentals
        {
            get { return Rentals.Count; }
            set { }
        }
    }


}