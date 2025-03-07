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

        /// <summary>
        /// surcharge poure définir une taille de départ pour l'application
        /// </summary>
        /// <param name="activationState"></param>
        /// <returns></returns>
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Set the initial size of the window
#if WINDOWS
            window.Width = 1200;
            window.Height = 800;
#endif

            return window;
        }
    }
}
