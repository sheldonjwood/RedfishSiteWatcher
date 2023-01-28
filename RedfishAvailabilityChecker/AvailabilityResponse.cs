using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedfishAvailabilityChecker
{
    internal class AvailabilityResponse
    {
        public Dictionary<string, Campsite> campsites { get; set; } 
    }

    internal class Campsite
    {
        public Dictionary<DateTime, string> availabilities { get; set; }
        public string campsite_id { get; set; }
        public string campsite_reserve_type { get; set; }
        public string campsite_rules { get; set; }
        public string campsite_type { get; set; }
        public string capacity_rating { get; set; }
        public string loop { get; set; }
        public string max_num_people { get; set; }
        public string min_num_people { get; set; }
        //public string quantities { get; set; }
        public string site { get; set; }
        public string supplemental_camping { get; set; }
        public string type_of_use { get; set; }

        public bool IsDouble => capacity_rating == "Double";
    }
}
