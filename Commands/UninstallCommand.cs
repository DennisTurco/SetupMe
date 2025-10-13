using Cocona;

namespace SetupMe.Commands
{
    public class UninstallCommand
    {
        public UninstallCommand() { }

        [Command("uninstall", Description = "Uninstall a package")]
        public async Task UninstallAsync(string packageName)
        {
            
        }
    }
}