using CliWrap;
using CliWrap.Builders;

namespace setupme.Services
{
    public class CliWrapperService
    {
        public static async Task<int> ExecuteCliCommand(string processName, Action<ArgumentsBuilder> builder)
        {
            var cli = await Cli.Wrap(processName)
                .WithArguments(builder)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.Error.WriteLine))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            return cli?.ExitCode ?? -1;
        }
    }
}
