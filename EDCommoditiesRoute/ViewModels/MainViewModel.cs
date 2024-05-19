using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDCommoditiesRoute.Models;
using InaraHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDCommoditiesRoute.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        #region OBBSERVABLE PROPERTIES
        [ObservableProperty]
        public String titre;

        [ObservableProperty]
        public String startingSystemHint;

        [ObservableProperty]
        public String startingSystemName = String.Empty;

        [ObservableProperty]
        public String startingSystemHelperText = String.Empty;

        [ObservableProperty]
        public String commoditiesListHint = String.Empty;

        [ObservableProperty]
        public String commoditiesListHelperText = String.Empty;

        [ObservableProperty]
        public ObservableCollection<CommodityInfo> commodityInfoList;

        [ObservableProperty]
        public CommodityInfo selectedCommodityInfo;

        [ObservableProperty]
        public ObservableCollection<object> selectedCommodityInfoList;

        [ObservableProperty]
        public String launchButtonLibelle;

        #endregion

        #region PROPERTIES

        Dictionary<String, List<CommodityInfo>> commoditiesByStations = new();

        #endregion

        #region CONSTRUCTOR

        public MainViewModel()
        {
            // valeur de debug
#if DEBUG
            startingSystemName = "volkhabe";
#endif


            Titre = "Ed Commodities Route";
            StartingSystemHint = "Système de départ";
            StartingSystemHelperText = "Saisissez votre système de départ";

            CommoditiesListHint = "Composants";
            CommoditiesListHelperText = "Choisissez les composants que vous recherchez";

            LaunchButtonLibelle = "Lancer la recherche";

            CommodityInfoList = new ObservableCollection<CommodityInfo>();
            selectedCommodityInfoList = new();

            LoadCommodities();

        }

        #endregion

        #region RELAY COMMANDS
        [RelayCommand]
        public async void LaunchResearch(object p)
        {
            // récupération des sélections
            foreach (CommodityInfo item in SelectedCommodityInfoList)
            {
                await InaraHelper.InaraHelper.GetCommodities(StartingSystemName, item);
            }

            LaunchCalculus();
        }

        private void LaunchCalculus()
        {
            // list all commodities
            // String : commodity
            // List<InaraCommodityInfo> : list of stations rhat offer this commodity
            foreach (KeyValuePair<string, List<InaraCommodityInfo>> stationsCommodity in InaraHelper.InaraHelper.StationsCommodities)
            {
                // for the current commodity, list the station that can afford it
                // stationsCommodity.Value : List<InaraCommodityInfo>
                foreach (InaraCommodityInfo inaraCommodityInfo in stationsCommodity.Value)
                {
                    // if the station is not in the list, we add it
                    // stationsCommodity.Key : commodity
                    if (CommodityInfoList.Any(x => x.Libelle == stationsCommodity.Key))
                    {

                        if (!commoditiesByStations.ContainsKey(inaraCommodityInfo.Location))
                        {
                            commoditiesByStations.Add(inaraCommodityInfo.Location, new() { CommodityInfoList.First(x => x.Libelle == stationsCommodity.Key) });
                        }
                        else
                        {
                            commoditiesByStations[inaraCommodityInfo.Location].Add(CommodityInfoList.First(x => x.Libelle == stationsCommodity.Key));
                        }
                    }
                }
            }

            Debugger.Break();
        }
        #endregion


        #region METHODS
        private async Task<Boolean> LoadCommodities()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("ED Commodities nettes EN.txt");
            using var reader = new StreamReader(stream);

            //using var reader = new StreamReader("../Resources/Raw/ED Commodities nettes EN.txt");

            String ligne;
            String[] infos;
            while (!reader.EndOfStream)
            {
                ligne = await reader.ReadLineAsync();
                infos = ligne.Split(';');
                if (infos.Length != 3)
                {
                    continue;
                }
                CommodityInfoList.Add(new CommodityInfo() { Numero = int.Parse(infos[0]), Libelle = infos[1], Famille = infos[2] });
            }
            return true;
        }
        #endregion
    }
}
