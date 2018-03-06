using Client.Util;
using System.Windows.Media;

namespace Client.Styles
{
    public static class StyleConstants
    {
        public static System.Drawing.Color ProductColor = System.Drawing.Color.Blue;
        public static System.Drawing.Color ReactantColor = System.Drawing.Color.Green;

        public static SolidColorBrush AllBrush = new SolidColorBrush(System.Drawing.Color.Black.ToMediaColor());
        public static SolidColorBrush ProductBrush = new SolidColorBrush(ProductColor.ToMediaColor());
        public static SolidColorBrush ReactantBrush = new SolidColorBrush(ReactantColor.ToMediaColor());
        public static SolidColorBrush SolventBrush = new SolidColorBrush(System.Drawing.Color.Orange.ToMediaColor());
        public static SolidColorBrush AgentBrush = new SolidColorBrush(System.Drawing.Color.Brown.ToMediaColor());
        public static SolidColorBrush CatalystBrush = new SolidColorBrush(System.Drawing.Color.Violet.ToMediaColor());

        public static SolidColorBrush ErrorBrush = new SolidColorBrush(System.Drawing.Color.Red.ToMediaColor());
        public static SolidColorBrush WarningBrush = new SolidColorBrush(System.Drawing.Color.Orange.ToMediaColor());
        public static SolidColorBrush WhiteBrush = new SolidColorBrush(System.Drawing.Color.White.ToMediaColor());
        public static SolidColorBrush BlackBrush = new SolidColorBrush(System.Drawing.Color.Black.ToMediaColor());
        public static SolidColorBrush NormalBrush = new SolidColorBrush(System.Drawing.Color.Transparent.ToMediaColor());

        public static SolidColorBrush IncompleteRxnBrush = new SolidColorBrush(System.Drawing.Color.Red.ToMediaColor());
        public static SolidColorBrush CompleteRxnBrush = new SolidColorBrush(System.Drawing.Color.Green.ToMediaColor());
    }
}
