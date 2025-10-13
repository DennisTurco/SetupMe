using Cocona;
using System.Diagnostics;

namespace SetupMe.Commands
{
    public class EditConfigurationCommand
    {
        private readonly string ConfigPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../Templates/config.yaml"));

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
            
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {editorCommand} \"{ConfigPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }
    }
}
