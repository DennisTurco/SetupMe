using YamlDotNet.Serialization;

namespace setupme.Entities
{
    public class Config
    {
        [YamlMember(Alias = "name")] public string Name { get; set; } = string.Empty;
        [YamlMember(Alias = "installs")] public IEnumerable<InstallOptions> InstallOptions { get; set; } = new List<InstallOptions>();
        [YamlMember(Alias = "actions")] public IEnumerable<CustomAction> Actions { get; set; } = new List<CustomAction>();
    }

    public class InstallOptions
    {
        [YamlMember(Alias = "flags")] public Flags Flags { get; set; } = new Flags();
        [YamlMember(Alias = "packages")] public IEnumerable<string> Packages { get; set; } = new List<string>();
    }

    public class Flags
    {
        [YamlMember(Alias = "version")] public string Version { get; set; } = string.Empty;
        [YamlMember(Alias = "force")] public bool Force { get; set; }
        [YamlMember(Alias = "source")] public string Source { get; set; } = string.Empty;
        [YamlMember(Alias = "quiet")] public bool Quiet { get; set; }
        [YamlMember(Alias = "confirm")] public bool Confirm { get; set; }
    }

    public class CustomAction
    {
        [YamlMember(Alias = "name")] public string Name { get; set; } = string.Empty;
        [YamlMember(Alias = "run")] public string Run { get; set; } = string.Empty;
    }
}
