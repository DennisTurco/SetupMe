using Cocona;
using setupme.Interfaces;
using System.Diagnostics;

namespace SetupMe.Commands
{
    public class EditConfigurationCommand
    {
        private readonly IAppConfig _appConfig;

        public EditConfigurationCommand(IAppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        [Command("edit", Description = "Edit the yaml configuration file")]
        public void EditConfiguration(
            [Option('e', Description = "Specify preferred editor")] string? editor
        )
        {
            string editorCommand = editor?.ToLower() switch
            {
                "vscode" or "code" => "code",
                "notepad++" => "notepad++.exe",
                _ => "notepad.exe"
            };

            var filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _appConfig.ConfigFilePath));
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {editorCommand} \"{filePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }
    }
}
