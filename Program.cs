namespace PowerShield_R;

#if DEBUG
using System.Runtime.InteropServices;
#endif

static class Program
{

#if DEBUG
	[DllImport("kernel32.dll")] static extern bool AllocConsole();
#endif
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
#if DEBUG
		AllocConsole();
#endif
		try
		{
			ApplicationConfiguration.Initialize();
			Application.Run(new FormMain());
		}
		catch (Exception ex)
		{
			MessageBox.Show(
				ex.ToString(),
				"PowerShield-R",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error,
				MessageBoxDefaultButton.Button1
			);
		}
	}
}