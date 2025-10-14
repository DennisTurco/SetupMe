using Cocona;

namespace SetupMe.Commands
{
    public class UpdateCommand
    {
        public UpdateCommand() { }

        [Command("update", Description = "Update a package")]
        public void Update(string packageName)
        {
            
        }
    }
}