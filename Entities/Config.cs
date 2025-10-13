using YamlDotNet.Serialization;

namespace setupme.Entities
{
    public class Config
    {
        [YamlMember(Alias = "name")] public string? Name { get; set; }
        [YamlMember(Alias = "installs")] public IEnumerable<InstallOptions>? InstallOptions { get; set; }
    }

    public class InstallOptions
    {
        [YamlMember(Alias = "flags")] public Flags? Flags { get; set; }
        [YamlMember(Alias = "packages")] public IEnumerable<string>? Packages { get; set; }
    }

    public class Flags
    {
        [YamlMember(Alias = "force")] public bool Force { get; set; } = false;
        [YamlMember(Alias = "source")] public string? Source { get; set; } = "choco";
        [YamlMember(Alias = "quiet")] public bool Quiet { get; set; } = false;
        [YamlMember(Alias = "confirm")] public bool Confirm { get; set; } = false;
    }
}
