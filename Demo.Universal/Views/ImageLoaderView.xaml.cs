using System;
using System.Diagnostics;
using System.Linq;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace Fnio.UI.Controls.Demo.Universal.Views
{
    public sealed partial class ImageLoaderView : Page
    {
        public ImageLoaderView()
        {
            this.InitializeComponent();

            var stretchValues = Enum
                .GetValues(typeof (Stretch))
                .Cast<Stretch>()
                .Select(s => s.ToString())
                .OrderBy(s => s)
                .ToList();
            var stretchIndex = stretchValues.IndexOf(DemoImageLoader.Stretch.ToString());
            StretchSelector.ItemsSource = stretchValues;
            StretchSelector.SelectedIndex = stretchIndex;

            LoadFileButton.Click += async (sender, e) =>
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                var file = await picker.PickSingleFileAsync();

                if (file == null)
                {
                    return;
                }

                StorageApplicationPermissions.FutureAccessList.Add(file);

                FileInputText.Text = file.Path;
                DemoImageLoader.SourceUri = _uriSource = new Uri(file.Path);
                Debug.WriteLine($"change source uri to: {DemoImageLoader.SourceUri}");
            };

            LoadUrlButton.Click += (sender, e) =>
            {
                var uriText = UrlInputText.Text;

                if (string.IsNullOrEmpty(uriText))
                {
                    return;
                }

                try
                {
                    DemoImageLoader.SourceUri = _uriSource = new Uri(uriText);
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }
            };

            StretchSelector.SelectionChanged += (sender, e) =>
            {
                var item = StretchSelector.SelectedValue as string;

                if (string.IsNullOrEmpty(item))
                {
                    return;
                }

                DemoImageLoader.Stretch = (Stretch) Enum.Parse(typeof (Stretch), item);
                Debug.WriteLine($"change stretch to: {DemoImageLoader.Stretch}");
            };

            DemoImageLoader.ImageFailed += (sender, e) =>
            {
                var dialog = new MessageDialog($"Failed to load image from: {_uriSource}", "ERROR");
                var run = dialog.ShowAsync();
            };
        }

        private Uri _uriSource;
    }
}
