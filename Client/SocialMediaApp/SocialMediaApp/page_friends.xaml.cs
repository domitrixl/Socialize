using Socialize;
namespace SocialMediaApp;

public partial class page_friends : ContentPage
{
    private readonly string bearer = null;
    public page_friends()
    {
        InitializeComponent();
        bearer = Preferences.Default.Get("bearer", "");
        loadfriendrequests();
    }

    private async void btn_back_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void loadfriendrequests()
    {
        var socialSelf = await APIHandler.SocialManage.Self();

        if (socialSelf != null && socialSelf.FRequestsRecieved != null)
        {
            Grid spacing = new Grid
            {
                Padding = 0,
                Margin = new Thickness(0, 0, 0, 0),

                RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(10) },
            },
            };

            foreach (string userId in socialSelf.FRequestsRecieved)
            {
                var uz = await APIHandler.Users.GetByUserId(userId);
                vsl_friendreq.Children.Add(FriendRequests(uz.username, userId));
                vsl_friendreq.Children.Add(spacing);
            }
        }
    }

    private Grid FriendRequests(string username, string userId)
    {
        Grid Requests = new Grid
        {
            Padding = 0,

            Margin = new Thickness(0, 0, 0, 0),

            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(40) },

            },
            ColumnDefinitions =
                    {
                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition{ Width = new GridLength(4, GridUnitType.Star) },
                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },
                    }
        };


        TapGestureRecognizer add = new TapGestureRecognizer();
        add.Tapped += async (sender, e) =>
        {
            bool accept = await APIHandler.SocialManage.AcceptFriendRequest(userId);

            if (accept)
            {
                Label l = (Label)sender;
                Grid g = l.Parent as Grid;
                vsl_friendreq.Children.Remove(g);
            }
            else
            {
                await DisplayAlert("Error", "Could not add to friends", "OK");
            }
        };

        Label accept = new Label();
        accept.Text = "\u2713";
        accept.TextColor = Color.FromHex("c0cded");
        accept.VerticalOptions = LayoutOptions.Center;
        accept.HorizontalOptions = LayoutOptions.End;
        accept.GestureRecognizers.Add(add);

        Label username1 = new Label();
        username1.Text = userId;
        username1.TextColor = Color.FromHex("c0cded");
        username1.VerticalOptions = LayoutOptions.Center;
        username1.HorizontalOptions = LayoutOptions.Center;

        Requests.Add(username1, 0, 0);
        Requests.Add(accept, 2, 0);

        return Requests;
    }
}