using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace modiki2

{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //logs.Text += ($"{Environment.NewLine}");

        string repoUrl = "https://github.com/lRatiol/lost-souls/raw/refs/heads/main/mods";
        string manifestUrl = "https://raw.githubusercontent.com/lRatiol/lost-souls/refs/heads/main/manifest.txt";
        string searchUrlPattern = "manifestUrl:";
        string searchRepoUrlPattern = "repoUrl:";

        string appPath;
        string manifestName;
        string manifestPath;
        string urlsName;
        string urlsPath;
        string appDataPath;
        string versionsPath;
        string packPath;

        private List<string> filesToDelete = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            // ����
            appPath = Application.StartupPath;
            manifestName = Path.GetFileName(manifestUrl);
            manifestPath = Path.Combine(appPath, manifestName);
            urlsName = "urls.txt";
            urlsPath = Path.Combine(appPath, urlsName);

            logs.Text += ($"����:{Environment.NewLine}-----------------");

            manifestSearch();
            findModPackFolder();
        }

        private void manifestSearch() // ���� url manifest � �����������, ���� ���� �� ����
        {
            try
            {
                if (File.Exists(urlsPath))
                {
                    logs.Text += ($"{Environment.NewLine}���� \"urls\" � �������� ������");

                    // ������ ��� ������ �����
                    string[] allLines = File.ReadAllLines(urlsPath);
                    string variableValue = null;
                    foreach (string line in allLines)// ���� ������ � ������ ������ searchUrlPattern
                    {
                        if (line.Contains(searchUrlPattern))
                        {
                            // ��������� �������� ����� �����
                            int index = line.IndexOf(searchUrlPattern) + searchUrlPattern.Length;
                            variableValue = line.Substring(index).Trim();
                            break; // ��������� ���� ����� ���������� ������� ����������
                        }
                    }

                    if (variableValue == null)
                    {
                        logs.Text += ($"{Environment.NewLine}����� <{searchUrlPattern}> �� �������");
                    }
                    else
                    {
                        if (variableValue == "")
                        {
                            logs.Text += ($"{Environment.NewLine}{searchUrlPattern} ������ ������");
                        }
                        else
                        {
                            manifestUrl = variableValue;
                            logs.Text += ($"{Environment.NewLine}{searchUrlPattern}{manifestUrl}");
                        }
                    }

                    // ������ ��� ������ �����
                    allLines = File.ReadAllLines(urlsPath);
                    variableValue = null;
                    foreach (string line in allLines)// ���� ������ � ������ ������ searchRepoUrlPattern
                    {
                        if (line.Contains(searchRepoUrlPattern))
                        {
                            // ��������� �������� ����� �����
                            int index = line.IndexOf(searchRepoUrlPattern) + searchRepoUrlPattern.Length;
                            variableValue = line.Substring(index).Trim();
                            break; // ��������� ���� ����� ���������� ������� ����������
                        }
                    }

                    if (variableValue == null)
                    {
                        logs.Text += ($"{Environment.NewLine}����� <{searchRepoUrlPattern}> �� �������");
                    }
                    else
                    {
                        if (variableValue == "")
                        {
                            logs.Text += ($"{Environment.NewLine}{searchRepoUrlPattern} ������ ������");
                        }
                        else
                        {
                            repoUrl = variableValue;
                            logs.Text += ($"{Environment.NewLine}{searchRepoUrlPattern}{repoUrl}");
                        }
                    }
                }
                else
                {
                    logs.Text += ($"{Environment.NewLine}���� \"urls\" � �������� �� ������");
                }
            }
            catch (Exception ex)
            {
                logs.Text += ($"{Environment.NewLine}������: {ex.Message}");
            }
            logs.Text += ($"{Environment.NewLine}-----------------");
        }

        private void findModPackFolder() // ���� lost souls
        {
            // ��������� ���� � ���������� .minecraft/versions
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            versionsPath = Path.Combine(appDataPath, ".minecraft", "versions");

            // ��������� ������������� ������� ����������
            if (!Directory.Exists(versionsPath))
            {
                logs.Text += ($"{Environment.NewLine}����� .minecraft �� �������");
                textBoxPath.Text = "����� .minecraft �� �������";
                return;
            }

            try
            {
                // ���� �����, ���������� "lost souls" � �������� (��� ����� ��������)
                logs.Text += ($"{Environment.NewLine}���� lost souls");
                var foundDirs = Directory.GetDirectories(versionsPath)
                    .Where(dir => dir.Contains("lost souls", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (foundDirs.Length > 0)
                {
                    textBoxPath.Text = foundDirs[0] + "\\mods"; // ����� ������ ���������
                    logs.Text += ($"{Environment.NewLine}������� ����� lost souls:{foundDirs[0]}");
                    packPath = textBoxPath.Text;
                }
                else
                {
                    logs.Text += ($"{Environment.NewLine}����� lost souls �� �������");
                    textBoxPath.Text = "����� lost souls �� �������";
                }
            }
            catch (Exception ex)
            {
                logs.Text += ($"{Environment.NewLine}������: {ex.Message}");
                textBoxPath.Text = $"������: {ex.Message}";
            }
            logs.Text += ($"{Environment.NewLine}-----------------");
        }

        private void lockBtn(bool State) // ����. ��������
        {
            btn1.Enabled = !State;
            btnObserver.Enabled = !State;
        }

        //llllllllllll

        private async void btn1_Click(object sender, EventArgs e)
        {
            await SyncFiles();
        }
        
        private async Task SyncFiles()
        {
            try
            {
                progressBar.Value = 0;
                logs.Text += $"{Environment.NewLine}�������� ���������...";

                // ���������� ���������
                var manifest = await DownloadManifest();
                if (manifest == null) return;

                logs.Text += $"{Environment.NewLine}������������ ��������� ������...";
                progressBar.Value = 10;

                // �������� � ���������� ������
                var localFiles = GetAllLocalFiles(packPath);
                progressBar.Value = 20;

                logs.Text += $"{Environment.NewLine}�������� � ���������� ������...";
                var extraFiles = await CheckAndUpdateFiles(manifest, localFiles);

                // ��������� ������ ������
                if (extraFiles.Any())
                {
                    logs.Text += $"{Environment.NewLine}���������� ������ �����...";
                    progressBar.Value = 90;

                    using (var cleanupForm = new CleanupForm(extraFiles))
                    {
                        if (cleanupForm.ShowDialog() == DialogResult.OK)
                        {
                            logs.Text += $"{Environment.NewLine}�������� ��������� ������...";
                            DeleteFiles(cleanupForm.FilesToDelete);
                        }
                    }
                }

                progressBar.Value = 100;
                logs.Text += $"{Environment.NewLine}������������� ���������!";
                MessageBox.Show("������������� ���������!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}");
            }
        }

        private async Task<Dictionary<string, string>> DownloadManifest()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    var response = await httpClient.GetAsync(manifestUrl, HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"�� ������� ��������� ��������. ������: {response.StatusCode}");
                        return null;
                    }

                    var manifestText = await response.Content.ReadAsStringAsync();
                    return ParseManifest(manifestText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��� �������� ���������: {ex.Message}");
                    return null;
                }
            }
        }

        private Dictionary<string, string> ParseManifest(string manifestText)
        {
            var lines = manifestText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var manifest = new Dictionary<string, string>();

            foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                var parts = line.Trim().Split(new[] { ' ' }, 2);
                if (parts.Length == 2)
                {
                    // ������� ��������� ����� � ������ ����
                    var filePath = parts[1].Trim();
                    manifest[filePath] = parts[0];
                }
            }
            return manifest;
        }

        private List<string> GetAllLocalFiles(string basePath)
        {
            return Directory.GetFiles(basePath, "*", SearchOption.AllDirectories)
                .Select(f => f.Substring(basePath.Length + 1))
                .ToList();
        }

        private async Task<List<string>> CheckAndUpdateFiles(
            Dictionary<string, string> manifest,
            List<string> localFiles)
        {
            var extraFiles = new List<string>(localFiles);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                int totalFiles = manifest.Count;
                int processedFiles = 0;

                foreach (var entry in manifest)
                {
                    var relativePath = entry.Key;
                    var filePath = Path.Combine(packPath, relativePath);
                    extraFiles.Remove(relativePath);

                    if (!File.Exists(filePath) || !VerifyFileHash(filePath, entry.Value))
                    {
                        await DownloadFile(httpClient, relativePath, filePath);
                    }

                    processedFiles++;
                    // ���������� ���������
                    int progress = 20 + (int)((double)processedFiles / totalFiles * 60);
                    progressBar.Value = Math.Min(progress, 80);
                    logs.Text += $"{Environment.NewLine}��������� ������: {processedFiles}/{totalFiles} - ";

                    // ��������� ��������� ��������, ����� �� ����������� ������
                    await Task.Delay(10);
                }
            }
            return extraFiles;
        }

        private bool VerifyFileHash(string filePath, string expectedHash)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task DownloadFile(HttpClient httpClient, string relativePath, string filePath)
        {
            try
            {
                // ��������� ���������� URL ��� raw.githubusercontent.com
                var normalizedPath = relativePath.Replace('\\', '/');
                var url = $"{repoUrl}{normalizedPath}";

                LogMessage($"������� ��������: {url}");

                var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    // ������� �������������� URL � mods/
                    var altUrl = $"{repoUrl}mods/{normalizedPath}";
                    LogMessage($"������� ��������������� URL: {altUrl}");

                    response = await httpClient.GetAsync(altUrl, HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        LogMessage($"�� ������� ��������� ����: {relativePath}. ������: {response.StatusCode}");
                        return;
                    }
                }

                var fileData = await response.Content.ReadAsByteArrayAsync();

                var directory = Path.GetDirectoryName(filePath); //lol
                if (!Directory.Exists(directory)) //lol
                { //lol
                    Directory.CreateDirectory(directory); //lol
                } //lol


                File.WriteAllBytes(filePath, fileData);

                LogMessage($"������� ��������: {relativePath}");
            }
            catch (Exception ex)
            {
                LogMessage($"������ ��� �������� ����� {relativePath}: {ex.Message}");
            }
        }

        private void DeleteFiles(IEnumerable<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
            {
                try
                {
                    File.Delete(Path.Combine(packPath, file));
                    LogMessage($"������ ����: {file}");
                }
                catch (Exception ex)
                {
                    LogMessage($"������ ��� �������� ����� {file}: {ex.Message}");
                }
            }
        }

        private void LogMessage(string message)
        {
            try
            {
                if (logs != null)
                {
                    logs.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                }
                File.AppendAllText("sync_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
            }
            catch
            {
                // ���������� ������ �����������
            }
        }

        private void btnObserver_Click(object sender, EventArgs e) // �����
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "�������� ����� lost souls";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = folderDialog.SelectedPath + "\\mods";
                    if (!Directory.Exists(textBoxPath.Text))
                    {
                        Directory.CreateDirectory(textBoxPath.Text);
                    }
                    packPath = textBoxPath.Text;
                    logs.Text += ($"{Environment.NewLine}������� ���������� lost souls ��:{folderDialog.SelectedPath}");
                }
            }
            logs.Text += ($"{Environment.NewLine}-----------------");
        }
    }

    // ����� ��� �������� ������ ������
    public class CleanupForm : Form
    {
        public List<string> FilesToDelete { get; } = new List<string>();

        public CleanupForm(List<string> extraFiles)
        {
            InitializeForm();
            PopulateFileList(extraFiles);
        }

        private void InitializeForm()
        {
            this.Text = "�������� ������ ������";
            this.Size = new System.Drawing.Size(500, 400);

            // ���� ��������� ������������ ���� ��� ���� ��������� ����������
            var listBox = new System.Windows.Forms.CheckedListBox { Dock = DockStyle.Fill };
            var deleteButton = new System.Windows.Forms.Button
            {
                Text = "������� ���������",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            var cancelButton = new System.Windows.Forms.Button
            {
                Text = "������",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            deleteButton.Click += (s, e) =>
            {
                FilesToDelete.AddRange(listBox.CheckedItems.Cast<string>());
                DialogResult = DialogResult.OK;
                Close();
            };

            cancelButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            var panel = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80
            };
            panel.Controls.Add(deleteButton);
            panel.Controls.Add(cancelButton);

            Controls.Add(listBox);
            Controls.Add(panel);
        }

        private void PopulateFileList(List<string> files)
        {
            var listBox = Controls.OfType<System.Windows.Forms.CheckedListBox>().First();
            foreach (var file in files)
            {
                listBox.Items.Add(file, true);
            }
        }
    }
}
