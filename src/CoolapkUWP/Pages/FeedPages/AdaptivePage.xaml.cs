﻿using CoolapkUWP.ViewModels.AdaptivePage;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoolapkUWP.Pages.FeedPages
{
    public sealed partial class AdaptivePage : Page
    {
        private ViewModel provider;

        public AdaptivePage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            titleBar.ShowProgressRing();
            provider = e.Parameter as ViewModel;

            FeedList.ItemsSource = provider.Models;
            await Refresh(-2);
            titleBar.Title = provider.Title;
            _ = scrollViewer.ChangeView(null, provider.VerticalOffsets[0], null, true);
            titleBar.HideProgressRing();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            provider.VerticalOffsets[0] = scrollViewer.VerticalOffset;
            titleBar.Title = string.Empty;

            base.OnNavigatingFrom(e);
        }

        private void VScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!e.IsIntermediate && scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                _ = Refresh();
            }
        }

        private async Task Refresh(int p = -1)
        {
            titleBar.ShowProgressRing();
            if (p == -2)
            {
                _ = (scrollViewer?.ChangeView(null, 0, null));
                titleBar.Title = provider.Title;
            }
            await provider.Refresh(p);
            if (p == -2 && scrollViewer.VerticalOffset != 0)
            {
                _ = (scrollViewer?.ChangeView(null, 0, null));
            }
            titleBar.HideProgressRing();
        }

        private void TitleBar_BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void TitleBar_RefreshEvent(object sender, RoutedEventArgs e) => _ = Refresh(-2);

        private async void RefreshContainer_RefreshRequested(RefreshContainer _, RefreshRequestedEventArgs args)
        {
            using (Windows.Foundation.Deferral RefreshCompletionDeferral = args.GetDeferral())
            {
                await Refresh(-2);
            }
        }
    }
}