
using System.Diagnostics;
using SetupMe.Interfaces;

namespace SetupMe.Installers
{
    public class ChocolatelyInstaller : IPackageInstaller
    {
        private readonly string ChocoPath;

        public ChocolatelyInstaller()
        {
            ChocoPath = Environment.ExpandEnvironmentVariables(@"%ProgramData%\chocolatey\bin\choco.exe");
        }
        
        public async Task InstallPackage(string packageName, string? version, bool force, bool quiet, bool autoConfirm)
        {
            if (!IsChocolateyInstalled())
            {
                InstallChocolately();
            }

            Console.WriteLine($"Installing package {packageName} via chocolately");

            string command = $"choco install {packageName}";

            if (string.IsNullOrEmpty(version))
            {
                command += $" --version {version}";
            }
            if (force)
            {
                command += " --force";
            }
            if (quiet)
            {
                command += " --limitoutput";
            }
            if (autoConfirm)
            {
                command += " --yes";
            }

            Process.Start("CMD.exe", command);

            await Task.CompletedTask;
        }

        private bool IsChocolateyInstalled()
        {
            if (File.Exists(ChocoPath))
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
                    throw new Exception("Failed to start PowerShell process. Possibly canceled or not found.");
                }
                process.WaitForExit();

                if (process.ExitCode == 0 && File.Exists(ChocoPath))
                {
                    Console.WriteLine("Chocolatey installed successfully.");
                }
                else
                {
                    Console.Error.WriteLine("Failed to install Chocolatey.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error installing Chocolatey: {ex.Message}");
            }
        }
    }
}