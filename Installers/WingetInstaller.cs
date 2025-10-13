using SetupMe.Interfaces;
using System.Diagnostics;

namespace SetupMe.Installers
{
    public class WingetInstaller : IPackageInstaller
    {
        private readonly string WingetPath;

        public WingetInstaller()
        {
            WingetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winget.exe");
        }

        public async Task InstallPackage(string packageName, string? version, bool force, bool quiet, bool autoConfirm)
        {
            if (!IsWingetInstalled())
            {
                InstallWinget();
            }

            Console.WriteLine($"Installing package {packageName} via winget");

            string command = $"winget install {packageName}";

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
                command += " --silent";
            }

            Process.Start("CMD.exe", command);

            await Task.CompletedTask;
        }

        private bool IsWingetInstalled()
        {
            if (File.Exists(WingetPath))
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
                    throw new Exception("Failed to start PowerShell process. Possibly canceled or not found.");
                }
                process.WaitForExit();

                if (process.ExitCode == 0 && File.Exists(WingetPath))
                {
                    Console.WriteLine("Winget installed successfully.");
                }
                else
                {
                    Console.Error.WriteLine("Failed to install Winget. Make sure your system supports App Installer.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error installing Winget: {ex.Message}");
            }
        }
    }
}