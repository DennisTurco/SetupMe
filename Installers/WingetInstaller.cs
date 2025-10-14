using setupme.Entities;
using setupme.Exceptions;
using SetupMe.Interfaces;
using System.Diagnostics;

namespace SetupMe.Installers
{
    public class WingetInstaller : IPackageInstaller
    {
        private readonly string _wingetPath;

        public WingetInstaller()
        {
            _wingetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winget.exe");
        }

        public async Task InstallPackage(string packageName, Flags flags)
        {
            if (!IsWingetInstalled())
            {
                InstallWinget();
            }

            Console.WriteLine($"⬇ Installing package {packageName} via winget");

            string command = $"winget install {packageName}";

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
                command += " --silent";
            }

            Process.Start("CMD.exe", command);

            await Task.CompletedTask;
        }

        private bool IsWingetInstalled()
        {
            if (File.Exists(_wingetPath))
            {
                return true;
            }

            return false;
        }

        private void InstallWinget()
        {
            try
            {
                Console.WriteLine("Winget is not installed. Installing App Installer from Microsoft Store...");
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = @"-NoProfile -ExecutionPolicy Bypass -Command ""Add-AppxPackage -Path 'https://aka.ms/Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.appxbundle'""",
                    UseShellExecute = true,
                    Verb = "runas" // Run as admin
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    throw new PackageInstallerException("Failed to start PowerShell process. Possibly canceled or not found.");
                }
                process.WaitForExit();

                if (process.ExitCode == 0 && File.Exists(_wingetPath))
                {
                    throw new PackageInstallerException("Winget installed successfully.");
                }
                else
                {
                    throw new PackageInstallerException("Failed to install Winget. Make sure your system supports App Installer.");
                }
            }
            catch (Exception ex)
            {
                throw new PackageInstallerException($"Error installing Winget: {ex.Message}");
            }
        }
    }
}