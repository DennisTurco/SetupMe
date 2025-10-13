using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            builder.Logging.AddFilter("System.Net.Http", LogLevel.Warning);

            // dependency injection
            builder.Services.AddSingleton<IPackageInstaller, ChocolatelyInstaller>();
            builder.Services.AddSingleton<IPackageInstaller, WingetInstaller>();
            
            var app = builder.Build();
            
            // commands registrations
            app.AddCommands<InstallCommand>();
            app.AddCommands<UninstallCommand>();
            app.AddCommands<UpdateCommand>();
            app.AddCommands<RunConfigurationCommand>();
            app.AddCommands<EditConfigurationCommand>();

            app.Run();
        }
    }
}
