using Microsoft.Maui.Controls;
using Socialize;
using Socialize.Classes;
using SocialMediaApp;
using System.Collections.ObjectModel;
using System.Text;

namespace SocialMediaApp
{
    public partial class page_home : ContentPage
    {
        private readonly Random randomizer = new Random();
        private readonly ObservableCollection<string> myItems = new ObservableCollection<string>();

        public page_home()
        {
            InitializeComponent();

            getdata();
        }
        private async void getdata()
        {
            var friendPosts = await APIHandler.Post.GetFromFriendsNewerThan(DateTime.UtcNow.AddDays(30));

            if (friendPosts != null)
            {
                if (friendPosts.Count > 0)
                {
                    foreach (SocialPost sp in friendPosts)
                    {
                        Grid spacing = new Grid
                        {
                            Padding = 0,
                            BackgroundColor = Color.FromArgb("#0d1735"),
                            Margin = new Thickness(-10, 0, -10, 20),

                            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(20) },

            },
                            ColumnDefinitions =
                    {
                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },

                    }
                        };
                        vsl_showposts.Add(spacing);
                        vsl_showposts.Add(await loaddatagrids(sp));

                    }
                }

            }
            else
            {
                lbl_noposts.IsVisible = true;
            }
        }

        private async Task<Grid> loaddatagrids(SocialPost post)
        {
            Grid posts = new Grid
            {
                Padding = 0,

                Margin = new Thickness(0, 0, 0, 0),

                RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(40) },
                new RowDefinition { Height = new GridLength(40) },
            },
                ColumnDefinitions =
                    {
                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star) },

                    }
            };



            SocialManagement sc = null;
            (bool, byte[], string) mediaResponse = (false, null, null);
            try
            {
                var user = await APIHandler.Users.GetByUserId(post.PostedBy);
                Label username = new Label()
                {

                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                    Text = user.username,
                    TextColor = Color.FromHex("c0cded"),
                };

                sc = await APIHandler.SocialManage.GetByUserId(post.PostedBy);

                if (sc != null)
                {
                    var file = await APIHandler.Post.GetFile(post.Media);

                    Image img = new Image() { };
                    posts.Add(img, 0, 1);
                }

                Label Description = new Label()
                {
                    Text = post.Description,
                    TextColor = Color.FromHex("c0cded"),
                };

                posts.Add(username, 0, 0);
                posts.Add(Description, 0, 2);

                return posts;
            }
            catch
            {

                throw;
            }
        }

        private void MyCollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            foreach (var s in GetItems(15))
            {
                myItems.Add(s);
            }
        }

        private List<string> GetItems(int numberOfItems)
        {
            var resultList = new List<string>();

            for (var i = 0; i <= numberOfItems; i++)
            {
                resultList.Add(randomizer.Next(10000, 99999).ToString());
            }

            return resultList;
        }
    }
}