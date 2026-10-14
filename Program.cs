namespace PowerShield_R;

static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		try
		{
			ApplicationConfiguration.Initialize();
			Application.Run(new FormMain());
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.ToString(), "Error al iniciar");
		}
	}
}