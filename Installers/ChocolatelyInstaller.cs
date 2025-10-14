using System.Diagnostics;
using setupme.Entities;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Installers
{
    public class ChocolatelyInstaller : IPackageInstaller
    {
        private readonly string _chocoPath;

        public ChocolatelyInstaller()
        {
            _chocoPath = Environment.ExpandEnvironmentVariables(@"%ProgramData%\chocolatey\bin\choco.exe");
        }
        
        public async Task InstallPackage(string packageName, Flags flags)
        {
            if (!IsChocolateyInstalled())
            {
                InstallChocolately();
            }

            Console.WriteLine($"Installing package {packageName} via chocolately");

            string command = $"choco install {packageName}";

            if (string.IsNullOrEmpty(flags.Version))
            {
                command += $" --version {flags.Version}";
            }
            if (flags.Force)
            {
                command += " --force";
            }
            if (flags.Quiet)
            {
                command += " --limitoutput";
            }
            if (flags.Confirm)
            {
                command += " --yes";
            }

            Process.Start("CMD.exe", command);

            await Task.CompletedTask;
        }

        private bool IsChocolateyInstalled()
        {
            if (File.Exists(_chocoPath))
            {
                return true;
            }

            return false;
        }

        private void InstallChocolately()
        {
            try
            {
                Console.WriteLine("Chocolatey is not already installed, installing...");
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = @"-NoProfile -ExecutionPolicy Bypass -Command ""Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))""",
                    UseShellExecute = true,
                    Verb = "runas" // Run as admin
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    throw new PackageInstallerException("Failed to start PowerShell process. Possibly canceled or not found.");
                }
                process.WaitForExit();

                if (process.ExitCode == 0 && File.Exists(_chocoPath))
                {
                    throw new PackageInstallerException("Chocolatey installed successfully.");
                }
                else
                {
                    throw new PackageInstallerException("Failed to install Chocolatey.");
                }
            }
            catch (Exception ex)
            {
                throw new PackageInstallerException($"Error installing Chocolatey: {ex.Message}");
            }
        }
    }
}