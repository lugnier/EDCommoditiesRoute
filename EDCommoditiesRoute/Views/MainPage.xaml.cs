using EDCommoditiesRoute.Models;
using EDCommoditiesRoute.ViewModels;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.DataGrid;
using System.Collections.ObjectModel;

namespace EDCommoditiesRoute.Views;

public partial class MainPage : ContentPage
{
    private ILogger<MainPage> logger;

    public MainPage(ILogger<MainPage> _logger)
    {
        logger = _logger;
        InitializeComponent();
    }

    /// <summary>
    /// Syncfusion sfComboBox bug - selectedItems can't be binded to an ViewModel 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SfButtonLauncheResearch_Clicked(object sender, EventArgs e)
    {
        if (!(this.BindingContext is MainViewModel))
        {
            throw new Exception("Wrong viewmodel type");
        }

        // Clear the list
        (this.BindingContext as MainViewModel)?.SelectedCommodityInfoList.Clear();
        // fill the ViewModel with the selectedItems of the comboBox
        foreach (CommodityInfo ci in SfComboBoxCommodities.SelectedItems)
        {
            (this.BindingContext as MainViewModel)?.SelectedCommodityInfoList.Add(ci);
        }
        // call the relay command
        (this.BindingContext as MainViewModel)?.LaunchResearch(new());

    }

    /// <summary>
    /// manual creation of the needed number of column in the datagrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SfDataGrid_ItemsSourceChanged(object sender, Syncfusion.Maui.DataGrid.DataGridItemsSourceChangedEventArgs e)
    {

        if (e.NewItems is ObservableCollection<CommoditiesByStationsForDataGrid>)
        {
            try
            {
                // get the items
                ObservableCollection<CommoditiesByStationsForDataGrid> r2 = e.NewItems as ObservableCollection<CommoditiesByStationsForDataGrid>;

                // get the number of commodities. It will be the number of columns
                Int32 numberOfCommodities = (this.BindingContext as MainViewModel).CommoditiesTitles.Count;

                DataGridCommoditiesByStations.Columns.Clear();
                // add the header line
                DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { Width = 150, HeaderText = "Type", MappingName = "StationType", ColumnWidthMode=ColumnWidthMode.Auto});
                DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { Width = 140, HeaderText = "Station", MappingName = "Station", ColumnWidthMode=ColumnWidthMode.Auto});
                DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { Width = 150, HeaderText = "System", MappingName = "System", ColumnWidthMode=ColumnWidthMode.Auto});
                DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { Width = 70, HeaderText = "Pad", MappingName = "Pad", ColumnWidthMode=ColumnWidthMode.Auto});
                DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { Width = 100, HeaderText = "Sl", MappingName = "SL", ColumnWidthMode=ColumnWidthMode.Auto});
                DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { Width = 100, HeaderText = "Ly", MappingName = "LY", ColumnWidthMode=ColumnWidthMode.Auto});

                // add header of each column and the mapping between binding and column
                int Counter = 1;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn 
                { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter-1], 
                    MappingName = "Commod01", CellTextAlignment = TextAlignment.Center
                });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod02", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod03", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod04", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod05", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod06", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod07", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod08", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod09", CellTextAlignment = TextAlignment.Center });
                Counter++;
                if (numberOfCommodities >= Counter) DataGridCommoditiesByStations.Columns.Add(new DataGridTextColumn { HeaderText = (this.BindingContext as MainViewModel).CommoditiesTitles[Counter - 1], MappingName = "Commod10", CellTextAlignment = TextAlignment.Center });

            }
            catch (Exception)
            {
                // left blank intentionnaly
            }
        }
    }
}