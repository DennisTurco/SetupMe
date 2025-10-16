using YamlDotNet.Serialization;

namespace setupme.Entities
{
    public class Config
    {
        [YamlMember(Alias = "name")] public string Name { get; set; } = string.Empty;
        [YamlMember(Alias = "installs")] public IEnumerable<PackageOptions> InstallOptions { get; set; } = new List<PackageOptions>();
        [YamlMember(Alias = "uninstalls")] public IEnumerable<PackageOptions> UninstallOptions { get; set; } = new List<PackageOptions>();
        [YamlMember(Alias = "upgrades")] public IEnumerable<PackageOptions> UpgradeOptions { get; set; } = new List<PackageOptions>();
        [YamlMember(Alias = "actions")] public IEnumerable<CustomAction> Actions { get; set; } = new List<CustomAction>();
    }

    public class PackageOptions
    {
        [YamlMember(Alias = "flags")] public Flags Flags { get; set; } = new Flags();
        [YamlMember(Alias = "packages")] public IEnumerable<string> Packages { get; set; } = new List<string>();
    }

    public class Flags
    {
        [YamlMember(Alias = "version")] public string Version { get; set; } = string.Empty;
        [YamlMember(Alias = "all")] public bool AllVersions { get; set; }
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
