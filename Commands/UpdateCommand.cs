using Cocona;

namespace SetupMe.Commands
{
    public class UpdateCommand
    {
        public UpdateCommand() { }

        [Command("update", Description = "Update a package")]
        public async Task UpdateAsync(string packageName)
        {
            
        }
    }
}