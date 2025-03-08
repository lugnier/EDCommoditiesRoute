using System.ComponentModel.DataAnnotations;

namespace EDCommoditiesRoute.Models
{
    public class InaraCommodityInfo
    {
        [Display(Order = 0)]
        public String Location { get; set; }
        [Display(Order = 1)]
        public String Pad { get; set; }
        [Display(Order = 2)]
        public Double DistanceStation { get; set; }
        [Display(Order = 3)]
        public Double DistanceSystem { get; set; }
        [Display(Order = 4)]
        public Int32 Supply { get; set; }
        [Display(Order = 5)]
        public Int32 Price { get; set; }
        [Display(Order = 6)]
        public String Updated { get; set; }
        public String Station { get; set; }
        public String System { get; set; }
        public String StationType { get; set; }
    }
}
