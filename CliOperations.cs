using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using setupme.Interfaces;
using setupme.Services;
using SetupMe.Commands;
using SetupMe.Installers;
using SetupMe.Interfaces;

namespace SetupMe
{
    public class CliOperations
    {
        public static void StartCli()
        {
            var builder = CoconaApp.CreateBuilder();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Logging.AddFilter("System.Net.Http", LogLevel.Warning);

            builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

            // dependency injection
            builder.Services.AddSingleton<IPackageInstaller, ChocolatelyInstaller>();
            builder.Services.AddSingleton<IPackageInstaller, WingetInstaller>();
            builder.Services.AddSingleton<IAppConfig>(sp =>
                sp.GetRequiredService<IOptions<AppConfig>>().Value);

            //builder.Services.AddTransient<RunConfigurationCommand>(); // only for testing

            try
            {
                // commands registrations
                var app = builder.Build();
                app.AddCommands<InstallCommand>();
                app.AddCommands<UninstallCommand>();
                app.AddCommands<UpdateCommand>();
                app.AddCommands<RunConfigurationCommand>();
                app.AddCommands<EditConfigurationCommand>();

                // for testing
                //var runConfigCommand = app.Services.GetRequiredService<RunConfigurationCommand>();
                //runConfigCommand.RunConfiguration("example1");

                app.Run(); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
