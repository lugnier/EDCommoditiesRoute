using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDCommoditiesRoute.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        public String titre;

        #region CONSTRUCTOR
        public MainViewModel()
        {
            Titre = "Ed Commodities Route";
        }
        #endregion
    }
}
