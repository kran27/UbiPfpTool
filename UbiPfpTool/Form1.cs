using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UbiPfpTool;

public partial class Form1 : Form
{
    private string uplayPath = Program.uplayPath;
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
            var notiPath = Path.Combine(uplayPath, "cache", "notifications", $"-{name}-default_256_256.png");
            var avatarsPath = Path.Combine(uplayPath, "cache", "avatars");
            var ubiPfpCache = Path.Combine(uplayPath, "cache", "ubipfps");
            var ubi256Path = Path.Combine(ubiPfpCache, $"{name}_256.png");
            var ubi128Path = Path.Combine(ubiPfpCache, $"{name}_128.png");
            var ubi64Path = Path.Combine(ubiPfpCache, $"{name}_64.png");
                
            var avatars256Path = Path.Combine(avatarsPath, $"{name}_256.png");
            var avatars128Path = Path.Combine(avatarsPath, $"{name}_128.png");
            var avatars64Path = Path.Combine(avatarsPath, $"{name}_64.png");
            scaled.Save(ubi256Path);
            image128.Save(ubi128Path);
            image64.Save(ubi64Path);
            scaled.Save(notiPath);
            scaled.Save(avatars256Path);
            image128.Save(avatars128Path);
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