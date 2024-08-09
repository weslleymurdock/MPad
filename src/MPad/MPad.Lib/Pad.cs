using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Shapes;

namespace MPad.Lib;

/// <summary>
/// ContentView extended class library as Pad object
/// </summary>

public class Pad : ContentView
{
    #region Properties

    /// <summary>
    /// PadChanged event handler
    /// </summary>
    public event EventHandler<EventArgs>? PadChanged;

    /// <summary>
    /// Value changed command bindable property
    /// </summary>
    public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(
      nameof(ValueChangedCommand),
      typeof(RelayCommand),
      typeof(Pad),
      null,
      defaultBindingMode: BindingMode.TwoWay,
      propertyChanged: (bindable, oldvalue, newvalue) =>
      {
          var pad = (Pad)bindable;
          pad.ValueChangedCommand = (RelayCommand)newvalue;
      }
   );

    /// <summary>
    /// Value changed command property
    /// </summary>
    public RelayCommand ValueChangedCommand { get => (RelayCommand)GetValue(ValueChangedCommandProperty); set => SetValue(ValueChangedCommandProperty, value); }


    /// <summary>
    /// Diameter of stick area bindable property
    /// </summary>
    public static readonly BindableProperty DiameterProperty = BindableProperty.Create(
       nameof(Diameter),
       typeof(float),
       typeof(Pad),
       150.0f,
       defaultBindingMode: BindingMode.TwoWay,
       propertyChanged: (bindable, oldvalue, newvalue) =>
       {
           var pad = (Pad)bindable;
           pad.Diameter = (float)newvalue;
       }
    );

    /// <summary>
    /// Diameter of stick area property
    /// </summary>
    public float Diameter { get => (float)GetValue(DiameterProperty); set => SetValue(DiameterProperty, value); }

    /// <summary>
    /// Background color of stick area bindable property
    /// </summary>
    public static readonly BindableProperty BackgroundFrameColorProperty = BindableProperty.Create(
       nameof(BackgroundFrameColor),
       typeof(Color),
       typeof(Pad),
       Color.FromArgb("8b34eb"),
       defaultBindingMode: BindingMode.TwoWay,
       propertyChanged: (bindable, oldvalue, newvalue) =>
       {
           var pad = (Pad)bindable;
           pad.BackgroundFrameColor = (Color)newvalue;
       }
    );

    /// <summary>
    /// Background color of stick area property
    /// </summary>
    public Color BackgroundFrameColor { get => (Color)GetValue(BackgroundFrameColorProperty); set => SetValue(BackgroundFrameColorProperty, value); }

    /// <summary>
    /// Background color of stick bindable property
    /// </summary>
    public static readonly BindableProperty BackgroundMainFrameColorProperty = BindableProperty.Create(
       nameof(BackgroundMainFrameColor),
       typeof(Color),
       typeof(Pad),
       Color.FromArgb("#594862"),
       defaultBindingMode: BindingMode.TwoWay,
       propertyChanged: (bindable, oldvalue, newvalue) =>
       {
           var pad = (Pad)bindable;
           pad.BackgroundMainFrameColor = (Color)newvalue;
       }
    );

    /// <summary>
    /// Background color of stick property
    /// </summary>
    public Color BackgroundMainFrameColor { get => (Color)GetValue(BackgroundMainFrameColorProperty); set => SetValue(BackgroundMainFrameColorProperty, value); }
    /// <summary>
    /// X axis bindable property
    /// </summary>
    public static readonly BindableProperty XAxisProperty = BindableProperty.Create(
        nameof(XAxis),
        typeof(double),
        typeof(Pad),
        0.0,
        defaultBindingMode: BindingMode.TwoWay,
       propertyChanged: (bindable, oldvalue, newvalue) =>
       {
           var pad = (Pad)bindable;
           pad.XAxis = (double)newvalue;
       }
    );

    /// <summary>
    /// X axis property
    /// </summary>
    public double XAxis { get => (double)GetValue(XAxisProperty); set => SetValue(XAxisProperty, value); }

    /// <summary>
    /// Y axis bindable property
    /// </summary>
    public static readonly BindableProperty YAxisProperty = BindableProperty.Create(
        nameof(YAxis),
        typeof(double),
        typeof(Pad),
        0.0,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldvalue, newvalue) =>
        {
            var pad = (Pad)bindable;
            pad.YAxis = (double)newvalue;
        }
    );

    /// <summary>
    /// Y axis property
    /// </summary>
    public double YAxis { get => (double)GetValue(YAxisProperty); set => SetValue(YAxisProperty, value); }

    /// <summary>
    /// Z axis bindable property
    /// </summary>
    public static readonly BindableProperty ZAxisProperty = BindableProperty.Create(
        nameof(ZAxis),
        typeof(bool),
        typeof(Pad),
        false, defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, oldvalue, newvalue) =>
        {
            var pad = (Pad)bindable;
            pad.ZAxis = (bool)newvalue;
        }
    );

    /// <summary>
    /// Z Axis Property
    /// </summary>
    public bool ZAxis { get => (bool)GetValue(ZAxisProperty); set { SetValue(ZAxisProperty, value); } }
    #endregion
    #region Variables
    private double previousX = 0.0, previousY = 0.0;
    private readonly Ellipse _main, _back;
    private readonly Func<double, double, double> _screenOffset;
    private readonly string _centerAnimation = "Center Animation";
    #endregion

    /// <summary>
    /// Default constructor
    /// </summary>
    public Pad()
    {
        _screenOffset = DeviceInfo.Platform == DevicePlatform.Android ? ((a, b) => b) : (a, b) => a - b;

        var mainLayout = new RelativeLayout();
        _back = new Ellipse { BackgroundColor = BackgroundFrameColor ?? Color.FromArgb("8b34eb") };
        mainLayout.Children.Add(
                _back,
                Constraint.RelativeToParent(p => p.Width / 2.0 - Diameter / 2.0),
                Constraint.RelativeToParent(p => p.Height / 2.0 - Diameter / 2.0),
                Constraint.RelativeToParent(p => Diameter),
                Constraint.RelativeToParent(p => Diameter)
            );

        _main = new Ellipse { BackgroundColor = BackgroundMainFrameColor ?? Color.FromArgb("#594862") };
        mainLayout.Children.Add(
                _main,
                Constraint.RelativeToParent(p => p.Width / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                Constraint.RelativeToParent(p => p.Height / 2.0 - (0.1 * Math.Min(p.Width, p.Height))),
                Constraint.RelativeToParent(p =>
                {
                    var desiredWidth = 0.2 * Math.Min(p.Width, p.Height);
                    return desiredWidth;
                }),
                Constraint.RelativeToParent(p => 0.2 * Math.Min(p.Width, p.Height))
            );
        var thumbDragGestureRecognizer = new PanGestureRecognizer();
        thumbDragGestureRecognizer.PanUpdated += OnThumbDragged;

        mainLayout.GestureRecognizers.Add(thumbDragGestureRecognizer);
        this.Content = mainLayout;
    }

    #region Events
    /// <summary>
    /// OnThumbDragged gesture event
    /// </summary>
    /// <param name="sender">PanGestureRecognizer</param>
    /// <param name="e">Pan Gesture event args</param>
    private void OnThumbDragged(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:

                this.AbortAnimation(_centerAnimation);
                ZAxis = true;
                previousX = e.TotalX;
                previousY = e.TotalY;
                _main.Scale = 0.75;
                break;

            case GestureStatus.Running:
                this.AbortAnimation(_centerAnimation);

                var newX = _main.TranslationX + _screenOffset(e.TotalX, previousX);
                var newY = _main.TranslationY + _screenOffset(e.TotalY, previousY);
                var newRadius = Math.Sqrt(Math.Pow(newX, 2) + Math.Pow(newY, 2));
                var newTheta = Math.Atan2(newY, newX);

                newRadius = Math.Min(_back.Width / 2.0, newRadius);
                _main.TranslationX = newRadius * Math.Cos(newTheta);
                _main.TranslationY = newRadius * Math.Sin(newTheta);

                UpdateControllerOutputs();

                previousX = e.TotalX;
                previousY = e.TotalY;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _main.Scale = 1.0;
                CenterAnimate();
                ResetControllerOutputs();
                break;
        }

        this.PadChanged?.Invoke(this, EventArgs.Empty);
        this.ValueChangedCommand?.Execute(this);

    }
    /// <summary>
    /// OnParentSet Event
    /// </summary>
    protected override void OnParentSet() => base.OnParentSet();
    #endregion

    #region Methods
    /// <summary>
    /// Animation to center back the _main elipse 
    /// </summary>
    private void CenterAnimate() => new Animation(d =>
    {
        _main.TranslationX = d * _main.TranslationX;
        _main.TranslationY = d * _main.TranslationY;
    }, 1.0, 0.0, Easing.SpringOut).Commit(this, _centerAnimation, length: 350, rate: 8);

    /// <summary>
    /// Reset joystick axis values
    /// </summary>
    private void ResetControllerOutputs()
    {
        XAxis = 0;
        YAxis = 0;
        ZAxis = false;
    }

    /// <summary>
    /// Update joystick axis values
    /// </summary>
    private void UpdateControllerOutputs()
    {
        var smallerDimension = Math.Min(_back.Width, _back.Height);
        XAxis = _main.TranslationX / (smallerDimension / 2.0);
        YAxis = _main.TranslationY / (smallerDimension / 2.0);
        YAxis *= -1; // needed to change up and down direction values
        ZAxis = true;
    }
    #endregion

}