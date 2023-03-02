using Microsoft.Maui.Controls.Compatibility;

namespace MPad.Lib;
/// <summary>
/// ContentView extended Pad class library
/// </summary>
public partial class Pad : ContentView
{
    //avoid another OS's
#if (!MACCATALYST && !TIZEN && !WINDOWS)
    #region Properties
    public static readonly BindableProperty XAxisProperty = BindableProperty.Create(
        nameof(XAxis),
        typeof(double),
        typeof(Pad),
        0.0
    );

    public double XAxis { get => (double)GetValue(XAxisProperty); set => SetValue(XAxisProperty, value); }

    public static readonly BindableProperty YAxisProperty = BindableProperty.Create(
        nameof(YAxis),
        typeof(double),
        typeof(Pad),
        0.0
    );

    public double YAxis { get => (double)GetValue(YAxisProperty); set => SetValue(YAxisProperty, value); }

    public static readonly BindableProperty ZAxisProperty = BindableProperty.Create(
        nameof(ZAxis),
        typeof(double),
        typeof(Pad),
        0.0
    );

    public double ZAxis { get => (double)GetValue(ZAxisProperty); set { OnThumbPress(); SetValue(ZAxisProperty, value); } }
    #endregion
    #region Variables
    private double previousX = 0.0, previousY = 0.0;
    private readonly Frame _mainFrame, _backgroundFrame;
    private readonly Func<double, double, double> _screenOffset;
    private readonly string _centerAnimation = "Center Animation";
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public Pad()
    {
        _screenOffset = DeviceInfo.Platform == DevicePlatform.Android ? ((a, b) => a) : (a, b) => a - b;

        var mainLayout = new RelativeLayout();

        _backgroundFrame = new Frame { BackgroundColor = Color.FromArgb("663399") };
        mainLayout.Children.Add(
                _backgroundFrame,
                Constraint.RelativeToParent(p => p.Width / 2.0 - Math.Min(p.Width, p.Height) / 2.0),
                Constraint.RelativeToParent(p => p.Height / 2.0 - Math.Min(p.Width, p.Height) / 2.0),
                Constraint.RelativeToParent(p =>
                {
                    _backgroundFrame.CornerRadius = (float)(Math.Min(p.Width, p.Height) / 2.0);
                    return Math.Min(p.Width, p.Height);
                }
                    ),
                Constraint.RelativeToParent(p => Math.Min(p.Width, p.Height))
            );

        _mainFrame = new Frame { BackgroundColor = Color.FromArgb("#6600CC") };
        var thumbDragGestureRecognizer = new PanGestureRecognizer();
        thumbDragGestureRecognizer.PanUpdated += OnThumbDragged;

        _mainFrame.GestureRecognizers.Add(thumbDragGestureRecognizer);
        mainLayout.Children.Add(
                _mainFrame,
                Constraint.RelativeToParent(p => p.Width / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                Constraint.RelativeToParent(p => p.Height / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                Constraint.RelativeToParent(p =>
                {
                    var desiredWidth = 0.2 * Math.Min(p.Width, p.Height);

                    _mainFrame.CornerRadius = (float)(desiredWidth / 2.0);
                    return desiredWidth;
                }),
                Constraint.RelativeToParent(p => 0.2 * Math.Min(p.Width, p.Height))
            );

        this.Content = mainLayout;
    }

    #region Events
    private void OnThumbDragged(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:

                this.AbortAnimation(_centerAnimation);
                ZAxis = 1.0;
                previousX = e.TotalX;
                previousY = e.TotalY;
                _mainFrame.Scale = 0.85;
                break;

            case GestureStatus.Running:
                var newX = _mainFrame.TranslationX + _screenOffset(e.TotalX, previousX);
                var newY = _mainFrame.TranslationY + _screenOffset(e.TotalY, previousY);
                ZAxis = 1.0;

                var newRadius = Math.Sqrt(Math.Pow(newX, 2) + Math.Pow(newY, 2));
                var newTheta = Math.Atan2(newY, newX);

                newRadius = Math.Min(_backgroundFrame.Width / 2.0, newRadius);

                _mainFrame.TranslationX = newRadius * Math.Cos(newTheta);
                _mainFrame.TranslationY = newRadius * Math.Sin(newTheta);

                UpdateControllerOutputs();

                previousX = e.TotalX;
                previousY = e.TotalY;
                break;

            case GestureStatus.Completed:
                _mainFrame.Scale = 1.0;
                CenterAnimate();

                ZAxis = 0.0;

                break;

            case GestureStatus.Canceled:
                _mainFrame.Scale = 1.0;
                CenterAnimate();
                ZAxis = 0.0;
                break;
        }
    }

    private void OnThumbPress() => _mainFrame.BackgroundColor = (ZAxis > 0.0) ? Color.FromArgb("#4C0099") : Color.FromArgb("#6600CC");

    protected override void OnParentSet() => base.OnParentSet();
    #endregion

    #region Methods
    private void CenterAnimate() => new Animation(d =>
    {
        _mainFrame.TranslationX = d * _mainFrame.TranslationX;
        _mainFrame.TranslationY = d * _mainFrame.TranslationY;
    }, 1.0, 0.0, Easing.CubicIn).Commit(this, _centerAnimation, length: 150);

    private void UpdateControllerOutputs()
    {
        var smallerDimension = Math.Min(_backgroundFrame.Width, _backgroundFrame.Height);
        XAxis = _mainFrame.TranslationX / (smallerDimension / 2.0);
        YAxis = _mainFrame.TranslationY / (smallerDimension / 2.0);
    }
    #endregion

#endif
}

