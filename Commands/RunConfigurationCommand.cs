using Cocona;

namespace SetupMe.Commands
{
    public class RunConfigurationCommand
    {
        public RunConfigurationCommand() { }

        [Command("run", Description = "Run a configured yaml file")]
        public async Task RunConfigurationAsync()
        {
            
        }
    }
}