using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DropDownCustomColorPicker
{   

    public partial class ColorPicker : UserControl
    {
        private bool IsMouseDownOverEllipse = false;
        private bool _shift = false;

        private Color _customColor = Colors.Transparent;
        
        public Color CustomColor
        {
            get { return _customColor; }
            set
            {
                if (_customColor != value)
                {
                    _customColor = value;
                    UpdatePreview();
                }
            }
        }
       
        public ColorPicker()
        {
            InitializeComponent();
            InitialWork();
            image.Source = loadBitmap(DropDownCustomColorPicker.Properties.Resources.ColorSwatchCircle);
            txtAlpha.LostFocus += new RoutedEventHandler(txtAlpha_TextChanged);
            txtR.LostFocus += new RoutedEventHandler(txtR_TextChanged);
            txtG.LostFocus += new RoutedEventHandler(txtG_TextChanged);
            txtB.LostFocus += new RoutedEventHandler(txtB_TextChanged);
            txtAll.LostFocus += new RoutedEventHandler(txtAll_TextChanged);
            txtAlpha.KeyDown += new KeyEventHandler(txtAlpha_KeyDown);
            txtR.KeyDown += new KeyEventHandler(txtR_KeyDown);
            txtG.KeyDown += new KeyEventHandler(txtG_KeyDown);
            txtB.KeyDown += new KeyEventHandler(txtB_KeyDown);
            txtAll.KeyDown += new KeyEventHandler(txtAll_KeyDown);
            CanColor.MouseMove += new MouseEventHandler(CanColor_MouseLeftButtonMove);
            CanColor.MouseLeftButtonDown += new MouseButtonEventHandler(CanColor_MouseLeftButtonDown);
            CanColor.MouseLeftButtonUp += new MouseButtonEventHandler(CanColor_MouseLeftButtonUp);
            EpPointer.MouseMove += new MouseEventHandler(EpPointer_MouseMove);
            EpPointer.MouseLeftButtonDown += new MouseButtonEventHandler(EpPointer_MouseLeftButtonDown);
            EpPointer.MouseLeftButtonUp += new MouseButtonEventHandler(EpPointer_MouseLeftButtonUp);
        }

        void EpPointer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDownOverEllipse = false;
        }

        private bool canChangeColor;
        void CanColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            canChangeColor = false;
            e.Handled = true;
        }

        void CanColor_MouseLeftButtonMove(object sender, MouseEventArgs e)
        {
            if(canChangeColor)
                ChangeColor();
            e.Handled = true;
        }

        void CanColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canChangeColor = true;
            e.Handled = true;
        }

        void EpPointer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDownOverEllipse = true;
        }

        void EpPointer_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDownOverEllipse)
            {
                ChangeColor();
            }
            e.Handled = true;
        }

        void txtAll_KeyDown(object sender, KeyEventArgs e)
        {           

            if (e.Key == Key.Enter)
            {
                try
                {
                    if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                    CustomColor = MakeColorFromHex(sender);
                    Reposition();
                }
                catch
                {
                }
            }
            else if (e.Key == Key.Tab)
            {
                txtAlpha.Focus();
            }
            
            string input = e.Key.ToString().Substring(1);
            if (string.IsNullOrEmpty(input))
            {
                input = e.Key.ToString();
            }
            if (input == "3" && _shift == true)
            {
                input = "#";
            }

            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                _shift = true;
            }
            else
            {
                _shift = false;
            }

            if (!(input == "#" || (input[0] >= 'A' && input[0] <= 'F') || (input[0] >= 'a' && input[0] <= 'F') || (input[0] >= '0' && input[0] <= '9')))
                e.Handled = true;
            if (input.Length > 1)
                e.Handled = true;
        }

        void txtAlpha_LostFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void txtB_KeyDown(object sender, KeyEventArgs e)
        {
            NumericValidation(e);
            NumericValidation(e);
            if (e.Key == Key.Tab)
            {
                txtAll.Focus();
            }

        }

        void txtG_KeyDown(object sender, KeyEventArgs e)
        {
            NumericValidation(e);
            if (e.Key == Key.Tab)
            {
                txtB.Focus();
            }
        }

        void txtR_KeyDown(object sender, KeyEventArgs e)
        {
            NumericValidation(e);
            if (e.Key == Key.Tab)
            {
                txtG.Focus();
            }
        }

        void txtAlpha_KeyDown(object sender, KeyEventArgs e)
        {
            NumericValidation(e);

            if (e.Key == Key.Tab)
            {
                txtR.Focus();
            }
        }

        void txtAll_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                CustomColor = MakeColorFromHex(sender);
                Reposition();
            }
            catch
            {
            }

        }

        private Color MakeColorFromHex(object sender)
        {
            try
            {
                ColorConverter cc = new ColorConverter();
                return (Color)cc.ConvertFrom(((TextBox)sender).Text);

            }
            catch
            {
                string alphaHex = CustomColor.A.ToString("X").PadLeft(2, '0');
                string redHex = CustomColor.R.ToString("X").PadLeft(2, '0');
                string greenHex = CustomColor.G.ToString("X").PadLeft(2, '0');
                string blueHex = CustomColor.B.ToString("X").PadLeft(2, '0');
                txtAll.Text = String.Format("#{0}{1}{2}{3}",
                alphaHex, redHex,
                greenHex, blueHex);


            }
            return _customColor;
        }

        void txtB_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                int val = Convert.ToInt32(((TextBox)sender).Text);
                if (val > 255)

                    ((TextBox)sender).Text = "255";
                else
                {
                    byte byteValue = Convert.ToByte(((TextBox)sender).Text);
                    CustomColor = MakeColorFromRGB();
                    Reposition();
                }
            }
            catch
            {
            }

        }

        private Color MakeColorFromRGB()
        {
            byte abyteValue = Convert.ToByte(txtAlpha.Text);
            byte rbyteValue = Convert.ToByte(txtR.Text);
            byte gbyteValue = Convert.ToByte(txtG.Text);
            byte bbyteValue = Convert.ToByte(txtB.Text);
            Color rgbColor =
                 Color.FromArgb(
                     abyteValue,
                     rbyteValue,
                     gbyteValue,
                     bbyteValue);
            return rgbColor;
        }

        void txtG_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                int val = Convert.ToInt32(((TextBox)sender).Text);
                if (val > 255)

                    ((TextBox)sender).Text = "255";
                else
                {
                    byte byteValue = Convert.ToByte(((TextBox)sender).Text);
                    CustomColor =
                       Color.FromArgb(
                            _customColor.A,
                           CustomColor.R,
                          byteValue,
                           CustomColor.B);
                    Reposition();

                }
            }
            catch
            {
            }
        }

        void txtR_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                int val = Convert.ToInt32(((TextBox)sender).Text);
                if (val > 255)

                    ((TextBox)sender).Text = "255";
                else
                {
                    byte byteValue = Convert.ToByte(((TextBox)sender).Text);
                    CustomColor =
                       Color.FromArgb(
                            _customColor.A,
                           byteValue,
                           CustomColor.G,
                           CustomColor.B);
                    Reposition();

                }
            }
            catch
            {
            }
        }

        void txtAlpha_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;
                int val = Convert.ToInt32(((TextBox)sender).Text);
                if (val > 255)

                    ((TextBox)sender).Text = "255";
                else
                {
                    byte byteValue = Convert.ToByte(((TextBox)sender).Text);
                    CustomColor =
                       Color.FromArgb(
                            byteValue,
                           CustomColor.R,
                           CustomColor.G,
                           CustomColor.B);

                }
            }
            catch
            {
            }
        }

        private void NumericValidation(System.Windows.Input.KeyEventArgs e)
        {
            string input = e.Key.ToString().Substring(1);     
            try
            {
                if (e.Key == Key.Enter)
                {
                    CustomColor = MakeColorFromRGB();
                    Reposition();
                }
                int inputDigit = Int32.Parse(input);
            }
            catch  
            {
                e.Handled = true; 
            }
        }

        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        private void InitialWork()
        {
            DefaultPicker.Items.Clear();
            CustomColors customColors = new CustomColors();
            foreach (var item in customColors.SelectableColors)
            {
                DefaultPicker.Items.Add(item);
            }
            DefaultPicker.SelectionChanged += new SelectionChangedEventHandler(DefaultPicker_SelectionChanged);
        }

        void DefaultPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DefaultPicker.SelectedValue != null)
            {
                _customColor = (Color)DefaultPicker.SelectedValue;
            }
            FrameworkElement frameworkElement = this;
            while (true)
            {
                if (frameworkElement == null) break;
                if (frameworkElement is ContextMenu)
                {
                    ((ContextMenu)frameworkElement).IsOpen = false;
                    break;
                }
                if (frameworkElement.Parent != null)
                    frameworkElement = (FrameworkElement)frameworkElement.Parent;
                else
                    break;
            }
        }

        private void ChangeColor()
        {
            try
            {
                CustomColor = GetColorFromImage((int)Mouse.GetPosition(CanColor).X, (int)Mouse.GetPosition(CanColor).Y);                
                MovePointer();                 
            }
            catch 
            {

            }
        }     

        private void Reposition()
        {

            for (int i = 0; i < CanColor.ActualWidth; i++)
            {
                bool flag = false;
                for (int j = 0; j < CanColor.ActualHeight; j++)
                {

                    try
                    {
                        Color Colorfromimagepoint = GetColorFromImage(i, j);
                        if (SimmilarColor(Colorfromimagepoint, _customColor))
                        {
                            MovePointerDuringReposition(i, j);
                            flag = true;
                            break;
                        }

                    }
                    catch 
                    {

                    }

                }
                if (flag) break;

            }


        }
        /// <summary>
        /// 1*1 pixel copy is based on an article by Lee Brimelow    
        /// http://thewpfblog.com/?p=62
        /// </summary>

        private Color GetColorFromImage(int i, int j)
        {
            CroppedBitmap cb = new CroppedBitmap(image.Source as BitmapSource,
                new Int32Rect(i,
                    j, 1, 1));
            byte[] color = new byte[4];
            cb.CopyPixels(color, 4, 0);
            Color Colorfromimagepoint = Color.FromArgb((byte)SdA.Value, color[2], color[1], color[0]);
            return Colorfromimagepoint;
        }

        private void MovePointerDuringReposition(int i, int j)
        {
            EpPointer.SetValue(Canvas.LeftProperty, (double)(i - 3));
            EpPointer.SetValue(Canvas.TopProperty, (double)(j - 3));
            EpPointer.InvalidateVisual();
            CanColor.InvalidateVisual();
        }
        private void MovePointer()
        {
            EpPointer.SetValue(Canvas.LeftProperty, (double)(Mouse.GetPosition(CanColor).X - 5));
            EpPointer.SetValue(Canvas.TopProperty, (double)(Mouse.GetPosition(CanColor).Y - 5));
            CanColor.InvalidateVisual();
        }

        private bool SimmilarColor(Color pointColor, Color selectedColor)
        {
            int diff = Math.Abs(pointColor.R - selectedColor.R) + Math.Abs(pointColor.G - selectedColor.G) + Math.Abs(pointColor.B - selectedColor.B);
            if (diff < 20) return true;
            else
                return false;
        }

        private void UpdatePreview()
        {
            lblPreview.Background = new SolidColorBrush(CustomColor);
            txtAlpha.Text = CustomColor.A.ToString();
            string alphaHex = CustomColor.A.ToString("X").PadLeft(2, '0');
            txtR.Text = CustomColor.R.ToString();
            string redHex = CustomColor.R.ToString("X").PadLeft(2, '0');
            txtG.Text = CustomColor.G.ToString();
            string greenHex = CustomColor.G.ToString("X").PadLeft(2, '0');
            txtB.Text = CustomColor.B.ToString();
            string blueHex = CustomColor.B.ToString("X").PadLeft(2, '0');
            txtAll.Text = String.Format("#{0}{1}{2}{3}",
            alphaHex, redHex,
            greenHex, blueHex);
            SdA.Value = CustomColor.A;
        }

        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void epDefaultcolor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            epCustomcolor.IsExpanded = false;

        }

        private void epCustomcolor_Expanded(object sender, RoutedEventArgs e)
        {
            epDefaultcolor.IsExpanded = false;
        }

        private void SdA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CustomColor = Color.FromArgb((byte)SdA.Value, CustomColor.R, CustomColor.G, CustomColor.B);
        }
    }

    class CustomColors
    {
        List<Color> _SelectableColors = null;

        public List<Color> SelectableColors
        {
            get { return _SelectableColors; }
            set { _SelectableColors = value; }
        }

        public CustomColors()
        {
            _SelectableColors = new List<Color>();
            Type ColorsType = typeof(Colors);
            PropertyInfo[] ColorsProperty = ColorsType.GetProperties();

            foreach (PropertyInfo property in ColorsProperty)
            {
                _SelectableColors.Add((Color)ColorConverter.ConvertFromString(property.Name));

            }
        }

    }

    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
