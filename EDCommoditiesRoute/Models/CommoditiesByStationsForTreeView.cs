using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDCommoditiesRoute.Models
{
    public partial class CommoditiesByStationsForTreeView : ObservableObject
    {
        [ObservableProperty]
        private string itemName;

        [ObservableProperty]
        ObservableCollection<CommodityInfo> commodityInfos;

    }
}
