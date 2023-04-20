using Fithub.Classes;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using SocialMediaApp.Properties;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;

namespace SocialMediaApp;

public partial class page_search : ContentPage
{
	public SocialManagementViewModel vm;
	public ViewList vList = new ViewList();
	public int i = 0;

    public page_search()
	{
		InitializeComponent();
		SocialManagement sm = Const.lst_sm[0];

		vList.List = new ObservableCollection<string>();
        this.BindingContext = vList;
	}



	public async Task ChangeUsername()
	{
		//await Task.Delay(2000);
	}

    private void addItem_Clicked(object sender, EventArgs e)
    {
		vList.List.Add("Test" + i);
		i++;
    }

    private void removeItem_Clicked(object sender, EventArgs e)
    {
		vList.List.RemoveAt(vList.List.Count - 1);
    }
}