using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DrawerLayout;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using MusictasticReborn.BusinessLayer;
using MusictasticReborn.Pages;
using MusictasticReborn.ViewModels;

namespace MusictasticReborn
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayingPage : Page
    {
        private PlayingNowVm _vm;

        public PlayingPage()
        {
            this.InitializeComponent();

            _vm = new PlayingNowVm();

            DataContext = _vm;

            Loaded += delegate
            {
                MusicPlayerWrapper.Instance.Dispatcher = this.Dispatcher;
            };
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateArtAndTrackName();

            //Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
        }

        private void UpdateArtAndTrackName()
        {
            if (!String.IsNullOrEmpty(MusicPlayerWrapper.Instance.AlbumArt))
                AlbumArtImg.Source = new BitmapImage(new Uri(MusicPlayerWrapper.Instance.AlbumArt, UriKind.Absolute));

            TrackNameTbx.Text = MusicPlayerWrapper.Instance.GetCurrentTrack();
        }
        
        private void PlayPauseBtn_Tap(object sender, TappedRoutedEventArgs e)
        {
            _vm.TogglePlayPause();
        }

        private void Previous_Tap(object sender, TappedRoutedEventArgs e)
        {
            MusicPlayerWrapper.Instance.Skip();
            UpdateArtAndTrackName();
        }

        private void Next_Tap(object sender, TappedRoutedEventArgs e)
        {
            MusicPlayerWrapper.Instance.Next();
            UpdateArtAndTrackName();
        }

        private async void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await SafeNavigation(typeof (SettingsPage));
        }

        private async void MusicLibrary_Tap(object sender, TappedRoutedEventArgs e)
        {
            await SafeNavigation(typeof (MusicLibraryPage));
        }

        private async Task SafeNavigation(Type target)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(target));
        }
    }
}
