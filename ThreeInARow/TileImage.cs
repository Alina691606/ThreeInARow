using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ThreeInARow
{
    class TileImage : Image
    {
        static BitmapImage blue = new BitmapImage(new Uri("pack://application:,,,/img/Blue.png"));
        static BitmapImage green = new BitmapImage(new Uri("pack://application:,,,/img/Green.png"));
        static BitmapImage pink = new BitmapImage(new Uri("pack://application:,,,/img/Pink.png"));
        static BitmapImage red = new BitmapImage(new Uri("pack://application:,,,/img/Red.png"));
        static BitmapImage yellow = new BitmapImage(new Uri("pack://application:,,,/img/Yellow.png"));

        public string ColorName;
        
        public TileImage(string color)
        {
            switch (color)
            {
                case "Blue":
                    Source = blue;
                    ColorName = color;
                    break;
                case "Green":
                    Source = green;
                    ColorName = color;
                    break;
                case "Pink":
                    Source = pink;
                    ColorName = color;
                    break;
                case "Red":
                    Source = red;
                    ColorName = color;
                    break;
                case "Yellow":
                    Source = yellow;
                    ColorName = color;
                    break;
            }
            AllowDrop = true;
            Margin = new System.Windows.Thickness(1);
        }
    }
}
