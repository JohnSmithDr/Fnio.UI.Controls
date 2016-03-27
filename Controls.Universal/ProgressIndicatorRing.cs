using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Fnio.UI.Controls
{
    public class ProgressIndicatorRing : RangeBase
    {
        private bool inited;
        private Path indicatorPath;
        private PathFigure indicatorFigure;
        private ArcSegment indicatorArc;
        private Path trackPath;
        private EllipseGeometry trackEllipse;
        private TextBlock percentageText;

        #region TrackBrush
        public Brush TrackBrush
        {
            get { return (Brush)GetValue(TrackBrushProperty); }
            set { SetValue(TrackBrushProperty, value); }
        }

        public static DependencyProperty TrackBrushProperty { get; } =
            DependencyProperty.Register(
              "TrackBrush", typeof(Brush), typeof(ProgressIndicatorRing), new PropertyMetadata(null));
        #endregion

        #region IndicatorBrush
        public Brush IndicatorBrush
        {
            get { return (Brush)GetValue(IndicatorBrushProperty); }
            set { SetValue(IndicatorBrushProperty, value); }
        }

        public static DependencyProperty IndicatorBrushProperty { get; } =
            DependencyProperty.Register(
              "IndicatorBrush", typeof(Brush), typeof(ProgressIndicatorRing), new PropertyMetadata(null));
        #endregion

        #region Thickness
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static DependencyProperty ThicknessProperty { get; } =
            DependencyProperty.Register(
              "Thickness",
              typeof(double),
              typeof(ProgressIndicatorRing),
              new PropertyMetadata(0d, OnThicknessPropertyChanged));

        private static void OnThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ProgressIndicatorRing;
            obj?.OnThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        private void OnThicknessChanged(double oldValue, double newValue)
        {
            Render();
        }
        #endregion

        #region Radius
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static DependencyProperty RadiusProperty { get; } =
            DependencyProperty.Register(
              "Radius",
              typeof(double),
              typeof(ProgressIndicatorRing),
              new PropertyMetadata(0d, OnRadiusPropertyChanged));

        private static void OnRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ProgressIndicatorRing)?.OnRadiusChanged((double)e.OldValue, (double)e.NewValue);
        }

        private void OnRadiusChanged(double oldValue, double newValue)
        {
            Render();
        }
        #endregion

        public ProgressIndicatorRing()
        {
            DefaultStyleKey = typeof(ProgressIndicatorRing);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            indicatorPath = GetTemplateChild("indicatorPath") as Path;
            indicatorFigure = GetTemplateChild("indicatorFigure") as PathFigure;
            indicatorArc = GetTemplateChild("indicatorArc") as ArcSegment;
            trackPath = GetTemplateChild("trackPath") as Path;
            trackEllipse = GetTemplateChild("trackEllipse") as EllipseGeometry;
            percentageText = GetTemplateChild("percentageText") as TextBlock;
            inited = true;
            Render();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Render();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            Render();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            Render();
        }

        private void Render()
        {
            if (!inited) return;

            var perc = Maximum > 0 ? Value / Maximum : 0; // [0, 1]
            var angle = perc * 360;
            var radian = angle * Math.PI / 180;
            var outterRadius = Thickness / 2 + Radius;
            var centerPoint = new Point(outterRadius, outterRadius);
            var arcStartPoint = new Point(outterRadius, Thickness / 2);
            var arcEndPoint = new Point(outterRadius + Radius * Math.Sin(radian), outterRadius - Radius * Math.Cos(radian));
            var size = Radius * 2 + Thickness;

            // render text
            //
            percentageText.Text = string.Format("{0:0}%", perc * 100);

            // render track
            //
            trackPath.Width = trackPath.Height = size;
            trackEllipse.RadiusX = trackEllipse.RadiusY = Radius;
            trackEllipse.Center = centerPoint;
            trackPath.Stroke = (perc == 1) ? IndicatorBrush : TrackBrush;

            // render indicator
            //
            indicatorPath.Width = indicatorPath.Height = size;
            indicatorFigure.StartPoint = arcStartPoint;
            indicatorArc.Point = arcEndPoint;
            indicatorArc.Size = new Size(Radius, Radius);
            indicatorArc.IsLargeArc = angle > 180.0;
        }

        #region IProgressIndicator

        public void Show() => Visibility = Visibility.Visible;

        public void Hide() => Visibility = Visibility.Collapsed;

        public void ReportProgress(double progress) => Value = progress;

        #endregion
    }
}
