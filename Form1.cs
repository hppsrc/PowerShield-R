namespace PowerShield_R;

using System;
using Microsoft.Win32;
using System.Security.Principal;
using System.Diagnostics;

public partial class FormMain : Form
{

#if DEBUG // global debug label
	private Label DUMP_DEBUG_TEXT = new Label();
#endif

	// Classes definitions
	PictureBox? PictureBoxVar;
	Random? rnd = new Random();

	DateTime dateTime;

	// Constants
	const string Version = "0.2.0";
	const string SemiPrivateAppName = "PwshldR";
	const string runKey = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";
	const string TargetPath = $"C:\\Windows\\{SemiPrivateAppName}";

	// Variables
	int R, G, B;
	readonly string SelfEXE = Environment.ProcessPath!;
	int screenWidth = Screen.PrimaryScreen?.Bounds.Width ?? 1920, screenHeight = Screen.PrimaryScreen?.Bounds.Height ?? 1080;

	private void Log(string message)
	{
		dateTime = DateTime.Now;
		// dateTime = dateTime?.ToString("HH:mm:ss:fff");
		//var dateTime = DateTime.Now; uncomment to get the current date and time
		// or use this fixed date-time: 25 April 2025 05:07:08.730
		dateTime = DateTime.Now;
		Console.WriteLine($"[{dateTime.ToString("ddd, dd MMM yyyy h:mm:ss:fff")}] {message}");
	}

	private void InitializeComponent()
	{
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(screenWidth, screenHeight);
		Text = $"PowerShield-R v{Version} ({BuildInfo.BUILD})";

		BackColor = Color.FromArgb(R, G, B);
		WindowState = FormWindowState.Maximized;
		WindowState = FormWindowState.Maximized;
		FormBorderStyle = FormBorderStyle.None;
		TopMost = true;
#if DEBUG // disable topmost in debug mode to allow debugging
		TopMost = false;
#endif
	}

	public void ShowImage(string imagePath)
	{
		PictureBoxVar ??= new PictureBox();

		Bitmap BitmapVar = new Bitmap(imagePath);

		PictureBoxVar.Location = new Point(20, 20);
		PictureBoxVar.SizeMode = PictureBoxSizeMode.Zoom;
		PictureBoxVar.Size = new Size(40, 40);

		PictureBoxVar.Image = (Image)BitmapVar;
		this.Controls.Add(PictureBoxVar);
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

	public void Restart(string args)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = SelfEXE,
			Arguments = args,
			UseShellExecute = true
		});

		Environment.Exit(0);
	}

	public void RestartAsAdmin(string args)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = SelfEXE,
			Arguments = args,
			Verb = "runas",
			UseShellExecute = true
		});

		Environment.Exit(0);
	}

	public void ExitApplication(int exitCode = 0)
	{
		Environment.Exit(exitCode);
	}

	public System.Windows.Forms.DialogResult Msgbox(string message, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information)
	{
		return MessageBox.Show(
			this,
			message,
			"PowerShield-R",
			buttons,
			icon,
			MessageBoxDefaultButton.Button1
		);
	}

	public void UninstallPwshldR()
	{
		try
		{
			using (var key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
			{
				key?.DeleteValue(SemiPrivateAppName, false);
			}
			Log("Removed from startup reg key");
		}
		catch (Exception ex)
		{
			Log("Error removing from startup reg key: " + ex.ToString());
			Msgbox(
				"Error removing from startup reg key:\n" + ex.ToString(),
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
		}

		try
		{
			if (Directory.Exists(TargetPath))
			{
				Directory.Delete(TargetPath, true);
				Log("Deleted target path: " + TargetPath);
			}
			else
			{
				Log("Target path does not exist: " + TargetPath);
			}
		}
		catch (Exception ex)
		{
			Log("Error deleting target path: " + ex.ToString());
			Msgbox(
				"Error deleting target path:\n" + ex.ToString(),
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
		}

		Log("Uninstallation complete.");
		Msgbox("PowerShield-R was successfully uninstalled.");
	}


	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
	{

		Console.WriteLine("Key pressed: " + keyData.ToString());
		// ignore, user can press numbers to unlock the screen
		if (
			keyData >= Keys.D0 && keyData <= Keys.D9 ||
			keyData == Keys.CapsLock || keyData == Keys.NumLock ||
			keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9)
		{
#if DEBUG // update debug label with allowed key pressed
			DUMP_DEBUG_TEXT.Text = keyData.ToString() + " allowed";
#endif
			return base.ProcessCmdKey(ref msg, keyData);
		}

		else // disable any other key presses
		{
#if DEBUG // update debug label with disabled key pressed
			DUMP_DEBUG_TEXT.Text = keyData.ToString() + " DISABLED!";
#endif
			return true;
		}
	}

	public FormMain()
	{

		Log("PowerShield-R running " + Version + " " + BuildInfo.BUILD);
		Log("Args: " + string.Join(" ", Environment.GetCommandLineArgs()));
		Log("SelfEXE: " + SelfEXE);
		Log("PWD: " + Directory.GetCurrentDirectory());

		// Check admin
		bool isElevated;
		using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
		{
			WindowsPrincipal principal = new WindowsPrincipal(identity);
			isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		if (Environment.GetCommandLineArgs().Contains("--uninstall"))
		{
			Log("Uninstall flag");
			if (!isElevated)
			{
				Log("Uninstall flag but user is not admin");
				Msgbox(
					"Admin privileges are required to uninstall PowerShield-R.",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				ExitApplication(1);
			}
			UninstallPwshldR();
			ExitApplication(1);
		}

		Log("User is admin? " + isElevated.ToString().ToUpper());
		Log("Run regkey: " + Registry.GetValue(runKey, SemiPrivateAppName, null)?.ToString());

		// Is Run regkey set and target path exists?
		if (Registry.GetValue(runKey, SemiPrivateAppName, null) == null || !Path.Exists(TargetPath))
		{   // It is not
			Log("No startup key reg");
			Log("Show messagebox for restart");

			// Is the restart flag set?
			DialogResult result;
			if (!Environment.GetCommandLineArgs().Contains("--install"))
			{   // If not show messagebox
				result = Msgbox(
					"It seems like PowerShield-R is not installed.\nDo you want to install it now?",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question
				);
			}
			else
			{   // If so, set result to yes
				result = DialogResult.Yes;
			}

			// Is result yes?
			if (result == DialogResult.Yes)
			{
				// If so
				Log("User said yes");
				if (!isElevated)
				{
					Log("User is not admin");
					RestartAsAdmin("--install");
				}
				Log("User is admin, installing now");

				Registry.SetValue(runKey, SemiPrivateAppName, $"{TargetPath}\\{SemiPrivateAppName}.exe");

				Log("Added to startup reg key");

				Directory.CreateDirectory(TargetPath);
				File.Copy(SelfEXE, $"{TargetPath}\\{SemiPrivateAppName}.exe", true);

				foreach (string dir in Directory.GetDirectories($"{Directory.GetCurrentDirectory()}", "*", SearchOption.AllDirectories))
				{
					Directory.CreateDirectory(Path.Combine(TargetPath, Path.GetRelativePath($"{Directory.GetCurrentDirectory()}", dir)));
					Log($"Created directory: {Path.Combine(TargetPath, Path.GetRelativePath($"{Directory.GetCurrentDirectory()}", dir))}");
					Log("dir exists? " + Directory.Exists(Path.Combine(TargetPath, Path.GetRelativePath($"{Directory.GetCurrentDirectory()}", dir))).ToString().ToUpper());
				}

				foreach (string file in Directory.GetFiles($"{Directory.GetCurrentDirectory()}", "*.*", SearchOption.AllDirectories))
				{
					string destFile = Path.Combine(TargetPath, Path.GetRelativePath($"{Directory.GetCurrentDirectory()}", file));
					File.Copy(file, destFile, true);
					Log($"Copied file: {destFile}");
					Log("file exists? " + File.Exists(destFile).ToString().ToUpper());
				}

#if DEBUG  // Debug version installed warning
				Log("RUNNING DEBUG VERSION ");
				Msgbox(
					"DEBUG VERSION WAS INSTALLED",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
				ExitApplication();
#endif
				Msgbox("PowerShield-R was successfully installed.");
				ExitApplication();
			}
			else
			{
				Log("User said no");
				Msgbox("PowerShield-R installation was canceled.");
				Log("Exiting");
				ExitApplication(1);
			}

		}

		Log("Startup reg key found");

		if (Path.Exists(TargetPath))
		{
			Log("Target path exists");
		}
		else
		{
			Log("Target path does not exist");
			Msgbox(
				"PowerShield-R is broken.\nPlease reinstall.",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
				);
			ExitApplication(1);
		}

		if (isElevated)
		{
			Log("User is admin but completed reg run check");
			Msgbox(
				"PowerShield-R is running as admin, this is not required.\nThe application will now close.",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
			);
			ExitApplication(1);
		}

		if (!string.Equals(Directory.GetCurrentDirectory(), TargetPath, StringComparison.OrdinalIgnoreCase))
		{
			Log("User is not on target path, performing cd");

			if (!Directory.Exists(TargetPath))
			{
				Log("For some reason path didn't exist, creating");
				Directory.CreateDirectory(TargetPath);
			}

			Log("Path set at");
			Directory.SetCurrentDirectory(TargetPath);
			Log("NEW PWD: " + Directory.GetCurrentDirectory());
		}

		G = rnd.Next(256);
		B = rnd.Next(256);
		R = rnd.Next(256);
		Log($"Random color generated: R={R}, G={G}, B={B}");
#if DEBUG // set background to black in debug mode to make it easier to read the debug text
		G = 0;
		B = 0;
		R = 0;
		Log("Color set to black in debug mode");
#endif
		int FontColor = R + G + B > 382 ? 0 : 255;
		Log($"Font color set to {(FontColor == 0 ? "black" : "white")}");

		Color C_FontColor = Color.FromArgb(FontColor, FontColor, FontColor);

		InitializeComponent();

		ShowImage(".\\assets\\ps.png");
		AddLabel($"PowerShield-R", 60, 20, 20, C_FontColor);

#if DEBUG // global debug label
		AddLabel($"DEBUG MODE", 60, 40, 20, C_FontColor);

		DUMP_DEBUG_TEXT?.Text = "DUMP DEBUG TEXT";
		DUMP_DEBUG_TEXT?.Location = new Point(100, 100);
		DUMP_DEBUG_TEXT?.AutoSize = true;
		DUMP_DEBUG_TEXT?.Font = new Font("Segoe UI", 30, FontStyle.Bold);
		DUMP_DEBUG_TEXT?.ForeColor = C_FontColor;
		Controls.Add(DUMP_DEBUG_TEXT);
#endif
		AddLabel($"v{Version} ({BuildInfo.BUILD})", 20, screenHeight - 60, 10, C_FontColor);

#if DEBUG // add a button to kill the application in debug mode
		Button KILL_DEBUG_BUTTON = new Button();
		KILL_DEBUG_BUTTON.DialogResult = DialogResult.OK;
		KILL_DEBUG_BUTTON.Text = "KILL";
		KILL_DEBUG_BUTTON.ForeColor = C_FontColor;
		KILL_DEBUG_BUTTON.Location = new Point(screenWidth - 100, screenHeight - 50);
		KILL_DEBUG_BUTTON.Size = new Size(80, 30);
		KILL_DEBUG_BUTTON.Click += (sender, e) => { ExitApplication(); };
		Controls.Add(KILL_DEBUG_BUTTON);
#endif

	}

}
