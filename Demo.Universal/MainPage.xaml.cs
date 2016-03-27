using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Fnio.UI.Controls.Demo.Universal.Views;

namespace Fnio.UI.Controls.Demo.Universal
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                ContentFrame.Navigate(typeof (DefaultView));
            };

            ContentFrame.Navigating += (sender, e) => VisualStateManager.GoToState(this, "ProgressStateLoading", true);

            ContentFrame.Navigated += (sender, e) => VisualStateManager.GoToState(this, "ProgressStateNone", true);

            DemoSelector.SelectionChanged += (sender, e) =>
            {
                var item = DemoSelector.SelectedValue as string;

                if (string.IsNullOrEmpty(item))
                {
                    return;
                }

                var viewType = Views[item];

                if (viewType == null)
                {
                    return;
                }

                ContentFrame.Navigate(viewType);
            };
        }

        private static readonly Dictionary<string, Type> Views = new Dictionary<string, Type>
        {
            { "ImageLoaderView", typeof(ImageLoaderView) },
            { "ProgressIndicatorRingView", typeof(ProgressIndicatorRingView) },
        };
    }
}
