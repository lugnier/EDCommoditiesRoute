namespace EDCommoditiesRoute
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzI4MDM1NkAzMjM1MmUzMDJlMzBZTll1d1VMQUZOUWtsLzFKd2dpY3ZVazhGeTE3TzNzS3dnb0lrTHozV29nPQ==");

            MainPage = new AppShell();
        }
    }
}
