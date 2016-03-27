using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Fnio.UI.Controls
{
    [TemplatePart(Name = "ImageContainer", Type = typeof(Image))]
    public class ImageLoader : Control
    {
        /// <summary>
        /// Invokes when the image is loading from source uri.
        /// </summary>
        public event DownloadProgressEventHandler ImageLoading;

        /// <summary>
        /// Invokes when the image is displayed.
        /// </summary>
        public event RoutedEventHandler ImageOpened;

        /// <summary>
        /// Invokes when the image is failed to display.
        /// </summary>
        public event ExceptionRoutedEventHandler ImageFailed;

        #region ImageContainer property

        /// <summary>
        /// The Control to display the image.
        /// </summary>
        private Image ImageContainer
        {
            get { return _imageContainer; }
            set
            {
                if (_imageContainer != null)
                {
                    _imageContainer.ImageOpened -= OnImageOpened;
                    _imageContainer.ImageFailed -= OnImageFailed;
                }

                _imageContainer = value;

                if (_imageContainer != null)
                {
                    _imageContainer.ImageOpened += OnImageOpened;
                    _imageContainer.ImageFailed += OnImageFailed;
                }
            }
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            EndLoading();
            ImageOpened?.Invoke(this, e);
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            EndLoading();
            ImageFailed?.Invoke(this, e);
        }

        private Image _imageContainer;

        #endregion

        #region Bitmap property

        /// <summary>
        ///The image source.
        /// </summary>
        private BitmapImage ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource != null)
                {
                    _imageSource.DownloadProgress -= OnImageDownloadProgress;
                }

                _imageSource = value;

                if (_imageSource != null)
                {
                    _imageSource.DownloadProgress += OnImageDownloadProgress;
                }
            }
        }

        private void OnImageDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            IsLoading = true;
            ImageLoading?.Invoke(this, e);
        }

        private BitmapImage _imageSource;

        #endregion

        #region Stretch dependency property

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                "Stretch",
                typeof(Stretch),
                typeof(ImageLoader),
                new PropertyMetadata(Stretch.Uniform));

        #endregion

        #region SourceUri dependency property

        public Uri SourceUri
        {
            get { return (Uri)GetValue(SourceUriProperty); }
            set { SetValue(SourceUriProperty, value); }
        }

        public static readonly DependencyProperty SourceUriProperty =
            DependencyProperty.Register(
                "SourceUri",
                typeof(Uri),
                typeof(ImageLoader),
                new PropertyMetadata(null, OnSourceUriChanged));

        private static void OnSourceUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
            => (d as ImageLoader)?.OnSourceUriChanged(e.NewValue as Uri);

        private void OnSourceUriChanged(Uri newValue)
        {
            if (!_inited) return;
            BeginLoading(newValue);
        }

        #endregion

        #region IsLoading dependency property

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            private set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                "IsLoading",
                typeof(bool),
                typeof(ImageLoader),
                new PropertyMetadata(false)); 

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ImageContainer = GetTemplateChild("ImageContainer") as Image;

            _inited = true;
        }

        private void BeginLoading(Uri source)
        {
            if (source == null)
            {
                ImageSource = null;
                ImageContainer.Source = null;
                IsLoading = false;
                return;
            }

            IsLoading = true;
            ImageSource = new BitmapImage(source);
            ImageContainer.Source = ImageSource;
        }

        private void EndLoading()
        {
            IsLoading = false;
        }

        private bool _inited;
    }
}
