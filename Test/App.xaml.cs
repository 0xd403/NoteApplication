namespace Test;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		var x = SecureStorage.GetAsync("firstPage").Result;

		// prima volta che accedo
		if (x == null)
		{
			SecureStorage.SetAsync("firstPage", "Register");
		}
		else if (x == "Register")
		{
			SecureStorage.SetAsync("firstPage", "Login");
		}

		MainPage = new AppShell();
	}
}
