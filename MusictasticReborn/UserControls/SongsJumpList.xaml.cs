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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using MusictasticReborn.BusinessLayer.Extensions;
using MusictasticReborn.BusinessLayer.Models;
using MusictasticReborn.UserControls.Extensions;

namespace MusictasticReborn.UserControls
{
    public sealed partial class SongsJumpList : UserControl
    {
        private List<IJumpListItem> _allItems;

        private List<IJumpListItem> _headerItems;

        public event EventHandler<SongSelectedEventArgs> NewSongSelected;

        public SongsJumpList()
        {
            this.InitializeComponent();

            _allItems = new List<IJumpListItem>();
            _headerItems = new List<IJumpListItem>();
        }
        
        public void Initialize(IList<IJumpListItem> items)
        {
            _allItems.AddRange(items);

            _headerItems.AddRange(items.Where(item => item.IsHeader));

            MainList.ItemsSource = _allItems;

            SecondList.ItemsSource = _headerItems;
        }

        private void OnNewSongSelected(SongModel song)
        {
            NewSongSelected?.Invoke(this, new SongSelectedEventArgs() {Song = song});
        }

        public bool IsSecondListViewOpened => SecondList.Visibility == Windows.UI.Xaml.Visibility.Visible;

        private void HeaderItem_Tap(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as Grid)?.DataContext as IJumpListItem;

            SecondList.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            MainList.Opacity = 1;

            MainList.IsEnabled = true;

            MainList.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
        }

        private void Item_Tap(object sender, TappedRoutedEventArgs e)
        {
            var item = (sender as Grid)?.DataContext as IJumpListItem;

            if (item.IsHeader)
            {
                SecondList.Visibility = Windows.UI.Xaml.Visibility.Visible;

                MainList.IsEnabled = false;

                MainList.Opacity = 0.3;
            }
            else
            {
                var selectedSong = item as SongModel;

                if (selectedSong != null)
                {
                    OnNewSongSelected(selectedSong);
                }
            }
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var listView = sender as ListView;

            if (listView != null)
            {
                listView.Width = e.NewSize.Width;
            }
        }
    }
}
