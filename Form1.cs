namespace PowerShield_R;

public partial class FormMain : Form
{

	const string VERSION = "1.0.0";
	PictureBox? pictureBox;
	public Bitmap? ps_image;
	Random rnd = new Random();

	int R, G, B;

	int screenWidth = Screen.PrimaryScreen?.Bounds.Width ?? 1920, screenHeight = Screen.PrimaryScreen?.Bounds.Height ?? 1080;

	const string PS_IMAGE_PATH = ".\\assets\\ps.png";

	private void InitializeComponent()
	{
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(screenWidth, screenHeight);
		Text = $"PowerShield-R v{VERSION} ({BuildInfo.BUILD})";

		BackColor = Color.FromArgb(R, G, B);
		WindowState = FormWindowState.Maximized;
		WindowState = FormWindowState.Maximized;
		FormBorderStyle = FormBorderStyle.None;
		TopMost = true;
	}

	public void ShowImage(string imagePath)
	{
		pictureBox = new PictureBox();

		Bitmap bitmap = new Bitmap(imagePath);
		pictureBox.Location = new Point(20, 20);
		pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
		pictureBox.Size = new Size(40, 40);

		pictureBox.Image = (Image)bitmap;
		this.Controls.Add(pictureBox);
	}

	public void AddLabel(string text, int x, int y, int fontSize, Color color)
	{
		Label label = new Label();
		label.Text = text;
		label.Location = new Point(x, y);
		label.AutoSize = true;
		label.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
		label.ForeColor = color;
		this.Controls.Add(label);
	}

	public FormMain()
	{

		G = rnd.Next(256);
		B = rnd.Next(256);
		R = rnd.Next(256);

		int FontColor = R + G + B > 382 ? 0 : 255;

		Console.WriteLine("PowerShield-R running " + VERSION + " " + BuildInfo.BUILD);

		InitializeComponent();

		ShowImage(PS_IMAGE_PATH);
		AddLabel($"PowerShield-R", 60, 20, 20, Color.FromArgb(FontColor, FontColor, FontColor));
		AddLabel($"v{VERSION} ({BuildInfo.BUILD})", 20, screenHeight - 60, 10, Color.FromArgb(FontColor, FontColor, FontColor));

	}

}
