using Ricettario.Common;
using Ricettario.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=321224

namespace Ricettario
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            search.SuggestionsRequested += search_SuggestionsRequested;
            search.QuerySubmitted += search_QuerySubmitted;
        }

        void search_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            Flyout fly = new Flyout();
            TextBlock tb=new TextBlock(){ Text="Hai cercato "+args.QueryText};
            fly.Content = tb;
            fly.ShowAt(search);
        }

        async void search_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            var groups=await RecipeDataSource.GetGroupsAsync();
            List<string> terms=new List<string>();
            
            foreach(var group in groups )
            {
                terms.AddRange(group.Items.Select(r => r.Title));
            }
            var query = terms.Where(x => x.StartsWith(args.QueryText, StringComparison.OrdinalIgnoreCase));
            args.Request.SearchSuggestionCollection.AppendQuerySuggestions(query);
        }



        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            //this.DefaultViewModel["Section3Items"] = sampleDataGroup;

            
            var gruppiRicette = await RecipeDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = gruppiRicette;
            
            foreach(RecipeDataGroup rgroup in gruppiRicette)
            {
                DefaultViewModel[rgroup.UniqueId] = rgroup;
            }

            RecipeDataGroup group = gruppiRicette.OrderBy(rnd => Guid.NewGuid()).First() as RecipeDataGroup;
            DefaultViewModel["TodayRecipe"]  = group.Items.OrderBy(r => Guid.NewGuid()).First() as RecipeDataItem;
            DefaultViewModel["RandomSection"] = group;
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((RecipeDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            if (e.ClickedItem is RecipeDataGroup)
            {
                this.Frame.Navigate(typeof(SectionPage), ((RecipeDataGroup)e.ClickedItem).UniqueId);
            }
            else
            {
                var itemId = ((RecipeDataItem)e.ClickedItem).UniqueId;
                this.Frame.Navigate(typeof(ItemPage), itemId);
            }
        }
        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth < 500)
            {
                theHub.Sections[0].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                theHub.Orientation = Orientation.Vertical;
            }
            else
            {
                theHub.Sections[0].Visibility = Windows.UI.Xaml.Visibility.Visible;
                theHub.Orientation = Orientation.Horizontal;
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var itemId = ((RecipeDataItem)DefaultViewModel["TodayRecipe"]).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
    }
}
