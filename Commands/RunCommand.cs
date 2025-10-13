using Cocona;

namespace SetupMe.Commands
{
    public class RunCommand
    {
        public RunCommand() { }

        [Command("run", Description = "Run a configured yaml file")]
        public async Task RunAsync()
        {
            
        }
    }
}