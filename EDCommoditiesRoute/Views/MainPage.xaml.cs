using EDCommoditiesRoute.Models;
using EDCommoditiesRoute.ViewModels;
using Syncfusion.Maui.DataSource.Extensions;
using System.Diagnostics;

namespace EDCommoditiesRoute.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private void SfComboBox_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
		Debugger.Break();
    }

    private void SfButton_Clicked(object sender, EventArgs e)
    {

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
}