namespace UbiPfpTool;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tableLayoutPanel1 = new TableLayoutPanel();
        textBox1 = new TextBox();
        textBox2 = new TextBox();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        pictureBox1 = new PictureBox();
        button1 = new Button();
        tableLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel1.Controls.Add(textBox1, 0, 1);
        tableLayoutPanel1.Controls.Add(textBox2, 0, 3);
        tableLayoutPanel1.Controls.Add(label1, 0, 0);
        tableLayoutPanel1.Controls.Add(label2, 0, 2);
        tableLayoutPanel1.Controls.Add(label3, 1, 0);
        tableLayoutPanel1.Controls.Add(pictureBox1, 1, 1);
        tableLayoutPanel1.Controls.Add(button1, 0, 4);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 5;
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.Size = new Size(519, 277);
        tableLayoutPanel1.TabIndex = 0;
        // 
        // textBox1
        // 
        textBox1.Dock = DockStyle.Fill;
        textBox1.Location = new Point(3, 18);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(251, 23);
        textBox1.TabIndex = 0;
        // 
        // textBox2
        // 
        textBox2.Dock = DockStyle.Fill;
        textBox2.Location = new Point(3, 62);
        textBox2.Name = "textBox2";
        textBox2.Size = new Size(251, 23);
        textBox2.TabIndex = 1;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(3, 0);
        label1.Name = "label1";
        label1.Size = new Size(36, 15);
        label1.TabIndex = 2;
        label1.Text = "Email";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(3, 44);
        label2.Name = "label2";
        label2.Size = new Size(57, 15);
        label2.TabIndex = 3;
        label2.Text = "Password";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(260, 0);
        label3.Name = "label3";
        label3.Size = new Size(129, 15);
        label3.TabIndex = 4;
        label3.Text = "Image (click to upload)";
        // 
        // pictureBox1
        // 
        pictureBox1.Dock = DockStyle.Fill;
        pictureBox1.Location = new Point(260, 18);
        pictureBox1.Name = "pictureBox1";
        tableLayoutPanel1.SetRowSpan(pictureBox1, 4);
        pictureBox1.Size = new Size(256, 256);
        pictureBox1.TabIndex = 5;
        pictureBox1.TabStop = false;
        pictureBox1.Click += pictureBox1_Click;
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox1.BackColor = Color.LightGray;
        // 
        // button1
        // 
        button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        button1.Location = new Point(179, 91);
        button1.Name = "button1";
        button1.Size = new Size(75, 23);
        button1.TabIndex = 6;
        button1.Text = "Upload";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // Form1
        // 
        AcceptButton = button1;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(519, 277);
        Controls.Add(tableLayoutPanel1);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "Form1";
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Upload New Profile Picture";
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel1;
    private TextBox textBox1;
    private TextBox textBox2;
    private Label label1;
    private Label label2;
    private Label label3;
    private PictureBox pictureBox1;
    private Button button1;
}