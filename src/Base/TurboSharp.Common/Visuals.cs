using Terminal.Gui;

namespace TurboSharp.Common
{
    public static class Visuals
    {
        public static ColorScheme GetBaseColor()
        {
            return Colors.ColorSchemes[nameof(Colors.Base)];
        }

        public static ColorScheme CreateTextColor()
        {
            var scheme = new ColorScheme();
            var driver = Application.Driver;
            scheme.Normal = driver.MakeAttribute(Color.White, Color.Blue);
            scheme.Focus = driver.MakeAttribute(Color.White, Color.Blue);
            return scheme;
        }
    }
}