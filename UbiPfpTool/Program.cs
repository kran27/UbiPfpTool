using System.Drawing.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UbiPfpTool;
using System.Windows.Forms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        UpdateImages(true);

        using (NotifyIcon icon = new NotifyIcon())
        {
            icon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            var x = new NotifyIcon();
            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.ContextMenuStrip.Items.Add("Upload New Image", null, (s, e) => new Form1().ShowDialog());
            icon.ContextMenuStrip.Items.Add("Force Update Cache", null, (s, e) => UpdateImages());
            icon.ContextMenuStrip.Items.Add("Clear Cache", null, (s, e) => ClearCaches());
            icon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Application.Exit());
            icon.Visible = true;
            icon.ShowBalloonTip(1000, "Ubisoft Profile Picture Tool", "Ubisoft Profile Picture Tool is running in the background", ToolTipIcon.Info);
            Application.Run();
            icon.Visible = false;
        }
    }
    
    public class ImageInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; }
    }

    private static void ClearCaches()
    {
        var profilePicturesCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games",
            "XDefiant", "profilepicturescache");
        var avatarsCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Ubisoft",
            "Ubisoft Game Launcher", "cache", "avatars");
        var notificationsCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Ubisoft",
            "Ubisoft Game Launcher", "cache", "notifications");
        foreach (var file in Directory.GetFiles(profilePicturesCache))
        {
            File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
            File.Delete(file);
        }
        foreach (var file in Directory.GetFiles(avatarsCache))
        {
            File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
            File.Delete(file);
        }
        foreach (var file in Directory.GetFiles(notificationsCache))
        {
            File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
            File.Delete(file);
        }
    }

    private static async void UpdateImages(bool background = false)
    {
        try
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "UbiPfpTool");
            var response = await client.GetAsync("https://api.github.com/repos/kran27/ubipfps/contents/pfps");
            var json = await response.Content.ReadAsStringAsync();
            var images = JsonSerializer.Deserialize<List<ImageInfo>>(json);
            foreach (var image in images)
            {
                image.Name = image.Name.Replace(".png", "");
                var imageResponse = await client.GetAsync(image.DownloadUrl);
                var imageStream = await imageResponse.Content.ReadAsStreamAsync();
                var imageBitmap = new Bitmap(imageStream);
                var image256 = new Bitmap(imageBitmap, new Size(256, 256));
                var image128 = new Bitmap(imageBitmap, new Size(128, 128));
                var image64 = new Bitmap(imageBitmap, new Size(64, 64));

                var xDefPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "My Games",
                    "XDefiant", "profilepicturescache", $"{image.Name}.png");
                var notiPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Ubisoft",
                    "Ubisoft Game Launcher", "cache", "notifications", $"-{image.Name}-default_256_256.png");
                var avatarsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Ubisoft",
                    "Ubisoft Game Launcher", "cache", "avatars");
                var avatars256Path = Path.Combine(avatarsPath, $"{image.Name}_256.png");
                var avatars128Path = Path.Combine(avatarsPath, $"{image.Name}_128.png");
                var avatars64Path = Path.Combine(avatarsPath, $"{image.Name}_64.png");
                if (File.Exists(xDefPath))
                {
                    File.SetAttributes(xDefPath, File.GetAttributes(xDefPath) & ~FileAttributes.ReadOnly);
                }

                image256.Save(xDefPath);
                File.SetAttributes(xDefPath, File.GetAttributes(xDefPath) | FileAttributes.ReadOnly);
                if (File.Exists(notiPath))
                {
                    File.SetAttributes(notiPath, File.GetAttributes(notiPath) & ~FileAttributes.ReadOnly);
                }

                image256.Save(notiPath);
                File.SetAttributes(notiPath, File.GetAttributes(notiPath) | FileAttributes.ReadOnly);
                if (File.Exists(avatars256Path))
                {
                    File.SetAttributes(avatars256Path, File.GetAttributes(avatars256Path) & ~FileAttributes.ReadOnly);
                }

                image256.Save(avatars256Path);
                File.SetAttributes(avatars256Path, File.GetAttributes(avatars256Path) | FileAttributes.ReadOnly);
                if (File.Exists(avatars128Path))
                {
                    File.SetAttributes(avatars128Path, File.GetAttributes(avatars128Path) & ~FileAttributes.ReadOnly);
                }

                image128.Save(avatars128Path);
                File.SetAttributes(avatars128Path, File.GetAttributes(avatars128Path) | FileAttributes.ReadOnly);
                if (File.Exists(avatars64Path))
                {
                    File.SetAttributes(avatars64Path, File.GetAttributes(avatars64Path) & ~FileAttributes.ReadOnly);
                }

                image64.Save(avatars64Path);
                File.SetAttributes(avatars64Path, File.GetAttributes(avatars64Path) | FileAttributes.ReadOnly);
            }
            if (!background) MessageBox.Show("Profile pictures updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception e)
        {
            if (!background) MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}