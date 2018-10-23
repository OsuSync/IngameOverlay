using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace IngameOverlay.Gui
{
    /// <summary>
    /// OverlayEditor.xaml 的交互逻辑
    /// </summary>
    partial class OverlayEditor : Window
    {
        public static IEnumerable<string> AvailableStatus { get; } = typeof(VisibleStatus).GetEnumNames();

        class ConfigItemProxy : INotifyPropertyChanged
        {
            private OverlayConfigItem _object;

            #region Property
            public string Mmf
            {
                get => _object.Mmf;
                set
                {
                    _object.Mmf = value;
                    OnPropertyChanged(nameof(Mmf));
                }
            }
            
            public string FontPath
            {
                get => _object.FontPath;
                set
                {
                    _object.FontPath = value;
                    OnPropertyChanged(nameof(FontPath));
                }
            }

            public Color TextColor
            {
                get => F4VToColor(_object.TextRgba);
                set
                {
                    ColorToF4V(value, _object.TextRgba);
                    OnPropertyChanged(nameof(TextColor));
                }
            }

            public Color BackgroundColor
            {
                get => F4VToColor(_object.BackgroundRgba);
                set
                {
                    ColorToF4V(value, _object.BackgroundRgba);
                    OnPropertyChanged(nameof(BackgroundColor));
                }
            }

            public Color BorderColor
            {
                get => F4VToColor(_object.BorderRgba);
                set
                {
                    ColorToF4V(value, _object.BorderRgba);
                    OnPropertyChanged(nameof(BorderColor));
                }
            }

            public float PivotX
            {
                get => _object.Pivot[0];
                set
                {
                    _object.Pivot[0] = value;
                    OnPropertyChanged(nameof(PivotX));
                }
            }

            public float PivotY
            {
                get => _object.Pivot[1];
                set
                {
                    _object.Pivot[1] = value;
                    OnPropertyChanged(nameof(PivotY));
                }
            }

            public float FontSize
            {
                get => _object.FontSize;
                set
                {
                    _object.FontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                }
            }


            public float FontScale
            {
                get => _object.FontScale;
                set
                {
                    _object.FontScale = value;
                    OnPropertyChanged(nameof(FontScale));
                }
            }

            public int PositionX
            {
                get => _object.Position[0];
                set
                {
                    _object.Position[0] = value;
                    OnPropertyChanged(nameof(PositionX));
                }
            }

            public int PositionY
            {
                get => _object.Position[1];
                set
                {
                    _object.Position[1] = value;
                    OnPropertyChanged(nameof(PositionY));
            }
            }

            public bool BreakTimeCheckBoxEnable => VisibleStatus.Contains("Playing");

            public bool BreakTime
            {
                get => _object.BreakTime;
                set
                {
                    _object.BreakTime = value;
                    OnPropertyChanged(nameof(BreakTime));
                }
            }

            public string VisibleStatus
            {
                get => string.Join(",",_object.VisibleStatus);
                set
                {
                    List<string> list = value.Split(',').ToList();
                    _object.VisibleStatus = list;
                    OnPropertyChanged(nameof(VisibleStatus));
                    OnPropertyChanged(nameof(BreakTimeCheckBoxEnable));
                }
            }

            public int PositionXSliderMaxValue => (int)SystemParameters.VirtualScreenWidth;
            public int PositionYSliderMaxValue => (int)SystemParameters.VirtualScreenHeight;

            #endregion

            public ConfigItemProxy(OverlayConfigItem obj, OverlayEditor win)
            {
                _object = obj;
                DeleteItem = new DeleteCommand(win);
            }

            private static Color F4VToColor(float[] f) => Color.FromArgb((byte) (f[3] * 255), (byte) (f[0] * 255),(byte) (f[1] * 255), (byte) (f[2] * 255));

            private static void ColorToF4V(Color color, float[] f)
            {
                f[0] = color.R / 255.0f;
                f[1] = color.G / 255.0f;
                f[2] = color.B / 255.0f;
                f[3] = color.A / 255.0f;
            }


            public event PropertyChangedEventHandler PropertyChanged;

            private static readonly List<string> _needUpdateFontPropertyList = new List<string>()
            {
                nameof(Mmf),
                nameof(FontPath),
                nameof(FontSize),
            };

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                Setting.OverlayConfigs.WriteToMmf(_needUpdateFontPropertyList.Contains(propertyName));
            }

            public enum ColorType
            {
                Text,
                Background,
                Border
            }

            public SelectFontCommand SelectFont { get; set; } = new SelectFontCommand();
            public DeleteCommand DeleteItem { get; set; }

            public class SelectFontCommand : ICommand
            {
                #region font name to font path
                private static Dictionary<string, string> _fontNameToFontPathDict = new Dictionary<string, string>();
                private static Dictionary<string, string> _fontPathToFontNameDict = new Dictionary<string, string>();

                static SelectFontCommand()
                {
                    string fontsdir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                    RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts\");
                    foreach (var key in rk.GetValueNames())
                    {
                        string k = key.Replace(" (TrueType)", "");
                        string v = System.IO.Path.Combine(fontsdir, rk.GetValue(key).ToString()).ToLower();
                        try
                        {
                            _fontNameToFontPathDict.Add(k, v);
                            _fontPathToFontNameDict.Add(v, k);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }

                #endregion

                public bool CanExecute(object parameter) => true;

                public void Execute(object parameter)
                {
                    ConfigItemProxy item = parameter as ConfigItemProxy;

                    var fontWindow = new System.Windows.Forms.FontDialog();
                    if (!string.IsNullOrWhiteSpace(item.FontPath))
                    {
                        try
                        {
                            fontWindow.Font = new Font(new FontFamily(_fontPathToFontNameDict[item.FontPath.ToLower()]), item.FontSize);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Not found font from font path!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }

                    if (fontWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            item.FontPath = _fontNameToFontPathDict[fontWindow.Font.Name];
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Font path not found! Please enter the font path manually or try to select another font.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        item.FontSize = (int)Math.Round(fontWindow.Font.Size,MidpointRounding.AwayFromZero);
                    }
                }

                public event EventHandler CanExecuteChanged
                {
                    add { }
                    remove { }
                }
            }
            public class DeleteCommand : ICommand
            {
                private OverlayEditor m_window;

                public event EventHandler CanExecuteChanged
                {
                    add { }
                    remove { }
                }

                public DeleteCommand(OverlayEditor window)
                {
                    m_window = window;
                }

                public bool CanExecute(object parameter) => true;

                public void Execute(object parameter)
                {
                    var proxy = parameter as ConfigItemProxy;

                    m_window._observableCollection.Remove(proxy);
                    Setting.OverlayConfigs.OverlayConfigItems.Remove(proxy._object);
                    Setting.OverlayConfigs.WriteToMmf();
                }
            }
        }

        private ObservableCollection<ConfigItemProxy> _observableCollection;

        public OverlayEditor()
        {
            InitializeComponent();
            _observableCollection = new ObservableCollection<ConfigItemProxy>(Setting.OverlayConfigs.OverlayConfigItems.Select(c=>new ConfigItemProxy(c,this)));

            ConfigList.ItemsSource = _observableCollection;
        }

        private void OverlayEditor_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void AddNewItemButton_Click(object sender, RoutedEventArgs e)
        {
            var config = new OverlayConfigItem();
            var proxy = new ConfigItemProxy(config,this);
            Setting.OverlayConfigs.OverlayConfigItems.Add(config);
            _observableCollection.Add(proxy);
            Setting.OverlayConfigs.WriteToMmf();
        }

        private static readonly Regex _integer_regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static readonly Regex _float_regex = new Regex(@"^\d+(\.\d*)?$"); //regex that matches disallowed text
        private static bool IsIntegerTextAllowed(string text) => !_integer_regex.IsMatch(text);
        private static bool IsFloatTextAllowed(string text) => _float_regex.IsMatch(text);


        private void TextBoxIntegerPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsIntegerTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void TextBoxFloatPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                System.Windows.Controls.TextBox t = sender as System.Windows.Controls.TextBox;
                string nextText = t.Text + (String)e.DataObject.GetData(typeof(String));
                if (!IsFloatTextAllowed(nextText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void IntegerIntput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsIntegerTextAllowed(e.Text);
        }

        private void FloatInput(object sender, TextCompositionEventArgs e)
        {
            System.Windows.Controls.TextBox t = sender as System.Windows.Controls.TextBox;
            string nextText = t.Text + e.Text;
            e.Handled = !IsFloatTextAllowed(nextText);
        }
    }
}
