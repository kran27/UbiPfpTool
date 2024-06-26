using System.Collections.Concurrent;
using System.Drawing.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Win32;

namespace UbiPfpTool;
using System.Windows.Forms;

static class Program
{
    public static bool updating = false;
    public static string uplayPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Ubisoft\Launcher", "InstallDir", "C:\\Program Files (x86)\\Ubisoft\\Ubisoft Game Launcher\\") as string;
    
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        ClearUbiPfpCache();
        UpdateImages(true);
        
        var watcher = new FileSystemWatcher(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Ubisoft",
            "Ubisoft Game Launcher", "cache", "avatars"));
        watcher.Changed += OnChanged;
        watcher.EnableRaisingEvents = true;

        using (var icon = new NotifyIcon())
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
    
    static List<string> recentlyUpdatedFiles = [];
    static object lockObject = new();

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        lock (lockObject)
        {
            if (recentlyUpdatedFiles.Contains(e.FullPath))
            {
                recentlyUpdatedFiles.Remove(e.FullPath);
                return;
            }
            
            UpdateFromCache(sender, e);

            recentlyUpdatedFiles.Add(e.FullPath);
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, args) =>
            {
                lock (lockObject) { recentlyUpdatedFiles.Remove(e.FullPath); }
                timer.Dispose();
            };
            timer.Start();
        }
    }

    private static void UpdateFromCache(object sender, FileSystemEventArgs e)
    {
        var otherCacheFilePath = e.FullPath.Replace("cache\\avatars", "cache\\ubipfps");
        var success = false;
        while (!success){
            try
            {
                File.Copy(otherCacheFilePath, e.FullPath, true);
                success = true;
            }
            catch { }
            Thread.Sleep(100);
        }
    }

    private class ImageInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; }
    }
    
    private static void ClearUbiPfpCache()
    {
        var ubiPfpCache = Path.Combine(uplayPath, "cache", "ubipfps");
        foreach (var file in Directory.GetFiles(ubiPfpCache))
        {
            var fileInAvatars = Path.Combine(uplayPath, "cache", "avatars", Path.GetFileName(file));
            if (File.Exists(fileInAvatars))
                File.Delete(fileInAvatars);
            File.Delete(file);
        }
    }

    private static void ClearCaches()
    {
        var avatarsCache = Path.Combine(uplayPath, "cache", "avatars");
        var notificationsCache = Path.Combine(uplayPath, "cache", "notifications");
        foreach (var file in Directory.GetFiles(avatarsCache))
            File.Delete(file);
        foreach (var file in Directory.GetFiles(notificationsCache))
            File.Delete(file);
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
                
                var notiPath = Path.Combine(uplayPath, "cache", "notifications", $"-{image.Name}-default_256_256.png");
                var avatarsPath = Path.Combine(uplayPath, "cache", "avatars");
                var ubiPfpCache = Path.Combine(uplayPath, "cache", "ubipfps");
                if (!Directory.Exists(ubiPfpCache)) Directory.CreateDirectory(ubiPfpCache);
                var ubi256Path = Path.Combine(ubiPfpCache, $"{image.Name}_256.png");
                var ubi128Path = Path.Combine(ubiPfpCache, $"{image.Name}_128.png");
                var ubi64Path = Path.Combine(ubiPfpCache, $"{image.Name}_64.png");
                
                var avatars256Path = Path.Combine(avatarsPath, $"{image.Name}_256.png");
                var avatars128Path = Path.Combine(avatarsPath, $"{image.Name}_128.png");
                var avatars64Path = Path.Combine(avatarsPath, $"{image.Name}_64.png");
                
                image256.Save(ubi256Path);
                image128.Save(ubi128Path);
                image64.Save(ubi64Path);
                image256.Save(notiPath);
                image256.Save(avatars256Path);
                image128.Save(avatars128Path);
                image64.Save(avatars64Path);
            }
            if (!background) MessageBox.Show("Profile pictures updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception e)
        {
            if (!background) MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}