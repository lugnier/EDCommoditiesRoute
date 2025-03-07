using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

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
