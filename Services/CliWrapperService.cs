using CliWrap;
using CliWrap.Builders;
using CliWrap.EventStream;

namespace setupme.Services
{
    public class CliWrapperService
    {
        public static async Task<int> ExecuteCliCommand(string processName, Action<ArgumentsBuilder> builder)
        {
            var cli = Cli.Wrap(processName)
                .WithArguments(builder);

            int exitCode = await PrintColoredOutputAndGetExitCode(cli);

            await cli
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            return exitCode;
        }

        private static async Task<int> PrintColoredOutputAndGetExitCode(Command cli)
        {
            await foreach (var command in cli.ListenAsync())
            {
                switch (command)
                {
                    case StandardOutputCommandEvent stdOut:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"OUT> {stdOut.Text}");
                        Console.ResetColor();
                        break;
                    case StandardErrorCommandEvent stdErr:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"ERR> {stdErr.Text}");
                        Console.ResetColor();
                        break;
                    case ExitedCommandEvent exited:
                        return exited.ExitCode;
                }
            }
            return -1;
        }
    }
}
