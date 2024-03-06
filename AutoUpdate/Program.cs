using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Renci.SshNet;
using SecurityDll;

namespace AutoUpdate
{
    public class FileDetails
    {
        public string Name { get; set; }
        public bool UsingRoot { get; set; }
        public bool Run { get; set; }
    }

    public class ServerPathInfo
    {
        public string Root { get; set; }
        public List<FileDetails> Files { get; set; }
    }

    public class SftpSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public ServerPathInfo ServerPath { get; set; }
        public bool KeepConsoleOpen { get; set; }
    }

    internal class Program
    {
        private static void DownloadFromSftp(SftpSettings sftp)
        {
            try
            {
                Decrypt(ref sftp);

                using SftpClient client = new(sftp.Ip, sftp.User, sftp.Password);

                Log("Connecting to SFTP...");

                client.Connect();

                Log($"Connected to SFTP server {sftp.Ip}");

                foreach (var file in sftp.ServerPath.Files)
                {
                    using var outputFileStream = File.OpenWrite(file.Name);
                    var sftpFilePath = file.Name;

                    if (file.UsingRoot || file.UsingRoot.Equals(null))
                    {
                        var root = sftp.ServerPath.Root.TrimEnd('/') + "/";
                        sftpFilePath = $"{root}{file.Name}";
                    }

                    client.DownloadFile(sftpFilePath, outputFileStream);

                    Log($"File {sftpFilePath} downloaded successfully!");
                }

                client.Disconnect();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static void RunProgram(string filename)
        {
            try
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName = filename
                };

                Log($"File {filename} starting...");

                Process.Start(processStartInfo);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static SftpSettings ReadSettings()
        {
            const string settingsFilename = "autoupdate .json";

            if (!File.Exists(settingsFilename)) throw new FileNotFoundException("Settings file not found!");

            var settings = File.ReadAllText(settingsFilename);
            Log("Settings file found!");

            return JsonConvert.DeserializeObject<SftpSettings>(settings);
        }

        private static void Decrypt(ref SftpSettings sftp)
        {
            sftp.User = Security.Decode(sftp.User);
            sftp.Password = Security.Decode(sftp.Password);
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            const string logFilePath = "autoupdate.log";

            try
            {
                if (!File.Exists(logFilePath)) File.Create(logFilePath).Close();

                var existingLines = File.ReadAllLines(logFilePath);

                var newLines = new string[existingLines.Length + 1];
                newLines[0] = $"[{DateTime.Now:dd-MMM-yyyy HH:mm:ss}] - {message}";

                var index = 1;
                foreach (var line in existingLines) newLines[index++] = line;

                File.WriteAllLines(logFilePath, newLines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }


        private static void Main()
        {
            try
            {
                var sftpSettings = ReadSettings();
                DownloadFromSftp(sftpSettings);

                foreach (var file in sftpSettings.ServerPath.Files.Where(file => file.Run))
                {
                    RunProgram(file.Name);
                }

                if (!sftpSettings.KeepConsoleOpen) return;

                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Log($"Error: {e.Message}");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}