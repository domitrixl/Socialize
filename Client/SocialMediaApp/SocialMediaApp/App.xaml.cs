using Microsoft.Maui.Controls.PlatformConfiguration;
using Socialize;

namespace SocialMediaApp;


public partial class App : Application
{
    bool logged = Preferences.Default.Get("loggedin", false);
    public App()
    {

        InitializeComponent();

        if (logged)
        {
            //gets bearer from prefs and sets it for requests
            string bearer = Preferences.Default.Get("bearer", "");
            APIHandler.SetBearer(bearer);
            MainPage = new AppShell();
        }
        else
        {
            MainPage = new page_login();
        }
    }

}