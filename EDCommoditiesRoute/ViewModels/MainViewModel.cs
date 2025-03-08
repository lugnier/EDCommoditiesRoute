using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EDCommoditiesRoute.Models;
using Syncfusion.Maui.Data;
using Syncfusion.Maui.Sliders;
using System.Collections.ObjectModel;

namespace EDCommoditiesRoute.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        #region OBBSERVABLE PROPERTIES

        /// <summary>
        /// Title of the software
        /// </summary>
        [ObservableProperty]
        public String title;

        /// <summary>
        /// Title of the entry to choose the starting system
        /// </summary>
        [ObservableProperty]
        public String startingSystemHint;

        /// <summary>
        /// name of the starting system
        /// </summary>
        //[ObservableProperty]
        private String startingSystemName = String.Empty;

        /// <summary>
        /// not used (syncfusion bug)
        /// </summary>
        [ObservableProperty]
        public String startingSystemHelperText = String.Empty;

        /// <summary>
        /// name of the list of commodities
        /// </summary>
        [ObservableProperty]
        public String commoditiesListHint = String.Empty;

        /// <summary>
        /// not used (syncfusion bug)
        /// </summary>
        [ObservableProperty]
        public String commoditiesListHelperText = String.Empty;

        /// <summary>
        /// list of commodities
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<CommodityInfo> commodityInfoList;

        /// <summary>
        /// list of commodities choosen by user
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<object> selectedCommodityInfoList;

        /// <summary>
        /// caption of the launch button
        /// </summary>
        [ObservableProperty]
        public String launchButtonLibelle;

        /// <summary>
        /// title of the slider to choose the minimum amount of commodity in stations
        /// </summary>
        [ObservableProperty]
        public String minCommoditiesHint;

        /// <summary>
        /// value of the minimum amount of commodity in stations
        /// </summary>
        private Int32 minCommoditiesValue;
        public Int32 MinCommoditiesValue
        {
            get => minCommoditiesValue;
            set
            {
                SetProperty(ref minCommoditiesValue, value);
                Preferences.Default.Set("MinCommoditiesValue", value);
            }
        }

        [ObservableProperty]
        public String padSizeHint;

        private Boolean padS;
        public Boolean PadS
        {
            get => padS;
            set
            {
                SetProperty(ref padS, value);
                if (value)
                {
                    Preferences.Default.Set("PadSize", "S");
                }
            }
        }

        private Boolean padM;
        public Boolean PadM
        {
            get => padM;
            set
            {
                SetProperty(ref padM, value);
                if (value)
                {
                    Preferences.Default.Set("PadSize", "M");
                }
            }
        }

        private Boolean padL;
        public Boolean PadL
        {
            get => padL;
            set
            {
                SetProperty(ref padL, value);
                if (value)
                {
                    Preferences.Default.Set("PadSize", "L");
                }
            }
        }

        /// <summary>
        /// data to fill the datagrid
        /// </summary>
        [ObservableProperty]
        ObservableCollection<CommoditiesByStationsForDataGrid> commoditiesByStationsForDataGrids = new();

        /// <summary>
        /// title of the choice between X or amount of the commodity in the station
        /// </summary>
        [ObservableProperty]
        public String gridCellXOrSupplyHint;

        /// <summary>
        /// Caption of X
        /// </summary>
        [ObservableProperty]
        public String gridCellXOrSupplyTextX;

        /// <summary>
        /// caption of Supply
        /// </summary>
        [ObservableProperty]
        public String gridCellXOrSupplyTextSupply;

        /// <summary>
        /// choice of the user
        /// </summary>
        [ObservableProperty]
        public Boolean gridCellXOrSupplyIsSupply;

        #endregion


        #region PROPERTIES
        Dictionary<String, List<CommodityInfoForDataGrid>> commoditiesByStations = new();

        //public Int32 NumberOfCommodities = 3;
        public List<String> CommoditiesTitles = new();

        string minCommoditiesHint_part01 = String.Empty;
        string noneText = String.Empty;

        public string StartingSystemName
        {
            get => startingSystemName;
            set
            {
                SetProperty(ref startingSystemName, value);
                Preferences.Default.Set("StartingSystemName", value);
            }
        }

        #endregion

        #region CONSTRUCTOR

        public MainViewModel()
        {
            startingSystemName = Preferences.Default.Get("StartingSystemName", "Kalak");
            //String s = Preferences.Default.Get("PadSize", "");
            PadS = Preferences.Default.Get("PadSize", "") == "S";
            PadM = Preferences.Default.Get("PadSize", "") == "M";
            PadL = Preferences.Default.Get("PadSize", "") == "L";
            if (!PadS && !PadM && !PadL)
            {
                PadM = true;
            }
            MinCommoditiesValue = Preferences.Default.Get("MinCommoditiesValue", 0);

            Title = "Ed Commodities Route";
            StartingSystemHint = "Système de départ";
            StartingSystemHelperText = "Saisissez votre système de départ";

            GridCellXOrSupplyHint = "Affichage dans la grille de";
            GridCellXOrSupplyTextX = "X";
            GridCellXOrSupplyTextSupply = "Stock";
            GridCellXOrSupplyIsSupply = true;


            CommoditiesListHint = "Composants";
            CommoditiesListHelperText = "Choisissez les composants que vous recherchez";

            LaunchButtonLibelle = "Lancer la recherche";

            CommodityInfoList = new ObservableCollection<CommodityInfo>();
            selectedCommodityInfoList = new();

            minCommoditiesHint_part01 = "Minimum en station";
            MinCommoditiesHint = $"{minCommoditiesHint_part01} : {MinCommoditiesValue}";

            PadSizeHint = "Taille minimum de Pad";
            noneText = "Aucun";

            LoadCommodities();

        }

        #endregion

        #region RELAY COMMANDS


        /// <summary>
        /// launch the research
        /// </summary>
        /// <param name="p">Not used</param>
        [RelayCommand]
        public async void LaunchResearch(object p)
        {
            InaraHelper.InaraHelper.StationsCommodities.Clear();

            // Get all the user selections
            foreach (CommodityInfo item in SelectedCommodityInfoList)
            {
                await InaraHelper.InaraHelper.GetCommodities(StartingSystemName, item, PadS ? "S" : PadM ? "M" : "L", MinCommoditiesValue);
            }

            // launch the creation of the grid
            LaunchCalculus();
        }

        /// <summary>
        /// Set the caption of the minimum amount of commodities in the stations
        /// </summary>
        /// <param name="p">The slider value</param>
        [RelayCommand]
        public void SliderMinCommodities(object p)
        {
            if (p is SliderValueChangedEventArgs)
            {
                var svcea = p as SliderValueChangedEventArgs;
                if (svcea != null)
                {
                    if (svcea.NewValue == 0)
                    {
                        MinCommoditiesHint = $"{minCommoditiesHint_part01} : {noneText}";
                    }
                    else
                    {
                        MinCommoditiesHint = $"{minCommoditiesHint_part01} : {svcea.NewValue}";
                    }
                }

            }
        }

        [RelayCommand]
        public void CommoditiesByStationsCellTapped(object p)
        {
            try
            {
                EDCommoditiesRoute.Models.CommoditiesByStationsForDataGrid a = (p as Syncfusion.Maui.DataGrid.DataGridCellTappedEventArgs).RowData as EDCommoditiesRoute.Models.CommoditiesByStationsForDataGrid;
                String[] s = a?.Station.Split("|");
                Clipboard.SetTextAsync(s?[1]?.Trim());
            }
            catch (Exception)
            {
                Clipboard.SetTextAsync("???");
            }
        }
        #endregion
        #region METHODS

        /// <summary>
        /// Launch the creation and the filling of the datagrid
        /// </summary>
        /// <remarks>
        /// to deal with dynamic number of column with syncfusion maui sfdatagrid, I choose to hard coding 10 columns max. I know it's hugly but it works
        /// </remarks>
        private void LaunchCalculus()
        {
            String dictionnaryKey;

            CommoditiesTitles.Clear();
            commoditiesByStations.Clear();

            // list all commodities
            // String : commodity
            // List<InaraCommodityInfo> : list of stations that offer this commodity
            foreach (KeyValuePair<string, List<InaraCommodityInfo>> stationsCommodity in InaraHelper.InaraHelper.StationsCommodities)
            {
                // add the commodity
                CommoditiesTitles.Add(stationsCommodity.Key);
                // for the current commodity, list the station that can afford it
                // stationsCommodity.Value : List<InaraCommodityInfo>
                foreach (InaraCommodityInfo inaraCommodityInfo in stationsCommodity.Value)
                {
                    // if the station is not in the list, we add it
                    // we keep only if the supply is enougth
                    // stationsCommodity.Key : commodity
                    if (CommodityInfoList.Any(x => x.Libelle == stationsCommodity.Key) && inaraCommodityInfo.Supply >= MinCommoditiesValue)
                    {
                        dictionnaryKey = $"{inaraCommodityInfo.Location} | {inaraCommodityInfo.DistanceStation} Ls | {inaraCommodityInfo.DistanceSystem} Ly";
                        if (!commoditiesByStations.ContainsKey(dictionnaryKey))
                        {
                            commoditiesByStations.Add(dictionnaryKey, new()
                            {
                                new()
                                {
                                    CommodityInfo =  CommodityInfoList.First(x => x.Libelle == stationsCommodity.Key),
                                    InaraCommodityInfo = inaraCommodityInfo
                                }
                            });
                        }
                        else
                        {
                            commoditiesByStations[dictionnaryKey].Add(
                                new()
                                {
                                    CommodityInfo = CommodityInfoList.First(x => x.Libelle == stationsCommodity.Key),
                                    InaraCommodityInfo = inaraCommodityInfo
                                });
                        }
                    }
                }
            }

            // here is the hugly part
            // we fill only values in the number of commodities choose by the user (max 10 in Commod01 to Commod10)
            // the xaml.cs will detect databinding change and create only the columns needed

            // local temp collection to avoid an always update the UI
            ObservableCollection<CommoditiesByStationsForDataGrid> d2 = new ObservableCollection<CommoditiesByStationsForDataGrid>();
            CommoditiesByStationsForDataGrid cbsfdg;
            // for each commodity - order by descending number of commodities found in this station
            foreach (KeyValuePair<string, List<CommodityInfoForDataGrid>> item in commoditiesByStations.OrderByDescending(x => x.Value.Count))
            {
                cbsfdg = new CommoditiesByStationsForDataGrid();
                // add the station in the first column
                //cbsfdg.Station = item.Key;
                cbsfdg.StationType = item.Value?[0]?.InaraCommodityInfo?.StationType ?? "???";
                cbsfdg.Station = item.Value?[0]?.InaraCommodityInfo?.Station ?? "???";
                cbsfdg.System = item.Value?[0]?.InaraCommodityInfo?.System ?? "???";
                cbsfdg.Pad = item.Value?[0]?.InaraCommodityInfo?.Pad ?? "???";
                cbsfdg.SL = item.Value?[0]?.InaraCommodityInfo?.DistanceStation.ToString() ?? "???";
                cbsfdg.LY = item.Value?[0]?.InaraCommodityInfo?.DistanceSystem.ToString() ?? "???";

                // if the user wants the supply
                if (GridCellXOrSupplyIsSupply)
                {

                    if (CommoditiesTitles.Count >= 1 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[0])) cbsfdg.Commod01 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[0])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 2 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[1])) cbsfdg.Commod02 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[1])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 3 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[2])) cbsfdg.Commod03 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[2])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 4 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[3])) cbsfdg.Commod04 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[3])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 5 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[4])) cbsfdg.Commod05 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[4])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 6 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[5])) cbsfdg.Commod06 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[5])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 7 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[6])) cbsfdg.Commod07 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[6])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 8 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[7])) cbsfdg.Commod08 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[7])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 9 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[8])) cbsfdg.Commod09 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[8])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";

                    if (CommoditiesTitles.Count >= 10 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[9])) cbsfdg.Commod10 = item.Value?.FirstOrDefault(x => x.CommodityInfo.Libelle == CommoditiesTitles[9])?.InaraCommodityInfo.Supply.ToString("N0") ?? "";
                }
                else // if the user wants an X instead the supply
                {
                    if (CommoditiesTitles.Count >= 1 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[0])) cbsfdg.Commod01 = "X";
                    if (CommoditiesTitles.Count >= 2 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[1])) cbsfdg.Commod02 = "X";
                    if (CommoditiesTitles.Count >= 3 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[2])) cbsfdg.Commod03 = "X";
                    if (CommoditiesTitles.Count >= 4 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[3])) cbsfdg.Commod04 = "X";
                    if (CommoditiesTitles.Count >= 5 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[4])) cbsfdg.Commod05 = "X";
                    if (CommoditiesTitles.Count >= 6 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[5])) cbsfdg.Commod06 = "X";
                    if (CommoditiesTitles.Count >= 7 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[6])) cbsfdg.Commod07 = "X";
                    if (CommoditiesTitles.Count >= 8 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[7])) cbsfdg.Commod08 = "X";
                    if (CommoditiesTitles.Count >= 9 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[8])) cbsfdg.Commod09 = "X";
                    if (CommoditiesTitles.Count >= 10 && item.Value.Any(x => x.CommodityInfo.Libelle == CommoditiesTitles[9])) cbsfdg.Commod10 = "X";
                }
                d2.Add(cbsfdg);
            }

            // create the binding collection to update the UI
            CommoditiesByStationsForDataGrids = new ObservableCollection<CommoditiesByStationsForDataGrid>(d2);
        }

        /// <summary>
        /// Load the commodities to get the internal number.
        /// To avoid to many acces to inara web site, it's a text file FR or EN
        /// </summary>
        /// <returns>true if the commodities have been read</returns>
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
            CommodityInfoList = CommodityInfoList.OrderBy(x => x.Libelle).ToObservableCollection();
            return true;
        }
        #endregion
    }
}
