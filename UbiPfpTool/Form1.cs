using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UbiPfpTool;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        AllowDrop = true;
        DragEnter += (sender, e) =>
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        };
        DragDrop += (sender, e) =>
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1 && (files[0].EndsWith(".png") || files[0].EndsWith(".jpg") || files[0].EndsWith(".jpeg")))
            {
                pictureBox1.ImageLocation = files[0];
            }
        };
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
        var ofd = new OpenFileDialog();
        ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;";
        if (ofd.ShowDialog() != DialogResult.OK) return;
        pictureBox1.ImageLocation = ofd.FileName;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (pictureBox1.ImageLocation == null)
        {
            MessageBox.Show("Please select an image first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || !Regex.IsMatch(textBox1.Text, @"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,})+)$"))
        {
            MessageBox.Show("Please enter a valid login", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        
        var img = Image.FromFile(pictureBox1.ImageLocation);
        var scaled = new Bitmap(img, new Size(256, 256));
        img.Dispose();
        var ms = new MemoryStream();
        scaled.Save(ms, ImageFormat.Png);
        var base64 = Convert.ToBase64String(ms.ToArray());
        ms.Dispose();
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://pfps.r6s.skin/");
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            email = textBox1.Text,
            password = textBox2.Text,
            image = base64
        }), Encoding.UTF8, "application/json");
        var response = client.Send(request);
        if (response.IsSuccessStatusCode)
        {
            var name = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result)["userId"];
            
            var image128 = new Bitmap(scaled, new Size(128, 128));
            var image64 = new Bitmap(scaled, new Size(64, 64));
            var xDefPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games",
                "XDefiant", "profilepicturescache", $"{name}.png");
            var notiPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Ubisoft",
                "Ubisoft Game Launcher", "cache", "notifications", $"-{name}-default_256_256.png");
            var avatarsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Ubisoft",
                "Ubisoft Game Launcher", "cache", "avatars");
            var avatars256Path = Path.Combine(avatarsPath, $"{name}_256.png");
            var avatars128Path = Path.Combine(avatarsPath, $"{name}_128.png");
            var avatars64Path = Path.Combine(avatarsPath, $"{name}_64.png");
            if (File.Exists(xDefPath)) { File.SetAttributes(xDefPath, File.GetAttributes(xDefPath) & ~FileAttributes.ReadOnly); }
            scaled.Save(xDefPath);
            if (File.Exists(notiPath)) { File.SetAttributes(notiPath, File.GetAttributes(notiPath) & ~FileAttributes.ReadOnly); }
            scaled.Save(notiPath);
            if (File.Exists(avatars256Path)) { File.SetAttributes(avatars256Path, File.GetAttributes(avatars256Path) & ~FileAttributes.ReadOnly); }
            scaled.Save(avatars256Path);
            if (File.Exists(avatars128Path)) { File.SetAttributes(avatars128Path, File.GetAttributes(avatars128Path) & ~FileAttributes.ReadOnly); }
            image128.Save(avatars128Path);
            if (File.Exists(avatars64Path)) { File.SetAttributes(avatars64Path, File.GetAttributes(avatars64Path) & ~FileAttributes.ReadOnly); }
            image64.Save(avatars64Path);
            MessageBox.Show("Profile picture updated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            var error = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result)["error"];
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        scaled.Dispose();
    }
}