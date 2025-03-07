using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Maui.Core.Hosting;

namespace EDCommoditiesRoute
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

        //    builder.Services.AddSerilog(
        //new LoggerConfiguration()
        //    .WriteTo.File(Path.Combine(FileSystem.Current.AppDataDirectory, "log.txt"))
        //    .CreateLogger();

            return builder.Build();
        }
    }
}
