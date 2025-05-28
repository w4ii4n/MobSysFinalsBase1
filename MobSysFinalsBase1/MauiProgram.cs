﻿using Microsoft.Extensions.Logging;
using MobSysFinalsBase1.Services;
using MobSysFinalsBase1.Shared;

namespace MobSysFinalsBase1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AppShellContext>();
            builder.Services.AddSingleton<DatabaseContext>();
            builder.Services.AddSingleton<BookService>();
            return builder.Build();
        }
    }
}
