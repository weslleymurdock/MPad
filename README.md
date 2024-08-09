# MPad - a simple .NET MAUI joypad control

MPad is a single joystick button that can be used in .NET MAUI Android and iOS apps. 
It provides 3 axis measurements, a event handler and command to consume new values.

## Usage

First add the package via nuget

```bash
dotnet add package MPad.Lib
```
Then, in MauiProgram add the UseMauiCompatibility method on CreateMauiApp method:

```csharp
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        })
        .UseMauiCompatibility();
```

After that, create the control on .xaml page and add the following lines on Content page node:

```csharp
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:m="clr-namespace:MPad.Lib;assembly=MPad.Lib"
             x:Class="MyApp.MainPage"
             xmlns:compat="clr-namespace:Microsoft.Maui.Controls.Compatibility;assembly=Microsoft.Maui.Controls">
```

That's it. The control supports MVVM pattern, using the XAxis, YAxis, ZAxis properties and command binded to a view model