using CliWrap.Builders;
using Cocona;
using setupme.Interfaces;
using setupme.Services;
using SetupMe.Interfaces;

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
        public async Task EditConfigurationAsync(
            [Option('e', Description = "Specify preferred editor")] string? editor
        )
        {
            string editorCommand = editor?.ToLower() switch
            {
                "vscode" or "code" => "code",
                "notepad++" or "notepadplusplus" => "notepadplusplus.exe",
                _ => "notepad.exe"
            };

            var filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _appConfig.ConfigFilePath));

            Action<ArgumentsBuilder> arguments = args =>
            {
                args.Add("/C")
                    .Add(editorCommand)
                    .Add(filePath);
            };

            await CliWrapperService.ExecuteCliCommand("cmd.exe", arguments);
        }
    }
}
