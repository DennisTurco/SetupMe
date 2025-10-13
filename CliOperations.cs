using Cocona;
using Microsoft.Extensions.Logging;
using SetupMe.Commands;

namespace SetupMe
{
    public class CliOperations
    {
        public static void StartCli()
        {
            var builder = CoconaApp.CreateBuilder();
            builder.Logging.AddFilter("System.Net.Http", LogLevel.Warning);
            var app = builder.Build();
            app.AddCommands<InstallCommand>();
            app.AddCommands<UninstallCommand>();
            app.AddCommands<UpdateCommand>();
            app.AddCommands<RunCommand>();
            app.Run();
        }
    }
}
