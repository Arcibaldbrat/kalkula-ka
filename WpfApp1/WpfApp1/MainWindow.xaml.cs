using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _current = "0";
        private double? _accumulator;
        private char? _pendingOperator;
        private bool _resetOnNextDigit;

        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay();

            // Optional keyboard support
            PreviewKeyDown += OnPreviewKeyDown;
        }

        // Button click handlers wired from XAML
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            string content = b.Content?.ToString() ?? string.Empty;
            ProcessDigit(content);
        }

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b) return;
            string s = b.Content?.ToString() ?? string.Empty;
            char op = s switch
            {
                "×" => '*',
                "÷" => '/',
                "−" => '-',
                "-" => '-',
                "+" => '+',
                _ => throw new InvalidOperationException("Unknown operator")
            };

            ProcessOperator(op);
        }

        private void Equals_Click(object sender, RoutedEventArgs e) => DoEquals();
        private void Clear_Click(object sender, RoutedEventArgs e) => DoClear();
        private void Backspace_Click(object sender, RoutedEventArgs e) => DoBackspace();
        private void ToggleSign_Click(object sender, RoutedEventArgs e) => ToggleSign();

        // Core logic
        private void ProcessDigit(string content)
        {
            if (_resetOnNextDigit)
            {
                _current = "0";
                _resetOnNextDigit = false;
            }

            if (content == ".")
            {
                if (!_current.Contains("."))
                    _current += ".";
            }
            else
            {
                // expect digits "0".."9"
                if (_current == "0")
                    _current = content;
                else
                    _current += content;
            }

            UpdateDisplay();
        }

        private void ProcessOperator(char op)
        {
            if (!_accumulator.HasValue)
            {
                if (double.TryParse(_current, NumberStyles.Float, CultureInfo.InvariantCulture, out var left))
                {
                    _accumulator = left;
                }
                else
                {
                    ShowError();
                    return;
                }
            }
            else if (_pendingOperator.HasValue)
            {
                if (!Compute())
                {
                    ShowError();
                    return;
                }
            }

            _pendingOperator = op;
            _resetOnNextDigit = true;
        }

        private void DoEquals()
        {
            if (!_pendingOperator.HasValue || !_accumulator.HasValue)
                return;

            if (!Compute())
            {
                ShowError();
                return;
            }

            _pendingOperator = null;
            _accumulator = double.TryParse(_current, NumberStyles.Float, CultureInfo.InvariantCulture, out var r) ? r : null;
            _resetOnNextDigit = true;
        }

        private bool Compute()
        {
            if (!_pendingOperator.HasValue || !_accumulator.HasValue) return false;
            if (!double.TryParse(_current, NumberStyles.Float, CultureInfo.InvariantCulture, out var right)) return false;

            double left = _accumulator.Value;
            double result = _pendingOperator.Value switch
            {
                '+' => left + right,
                '-' => left - right,
                '*' => left * right,
                '/' => right == 0 ? double.NaN : left / right,
                _ => double.NaN
            };

            if (double.IsNaN(result) || double.IsInfinity(result))
                return false;

            _current = result.ToString("G15", CultureInfo.InvariantCulture);
            UpdateDisplay();
            return true;
        }

        private void DoClear()
        {
            _current = "0";
            _accumulator = null;
            _pendingOperator = null;
            _resetOnNextDigit = false;
            UpdateDisplay();
        }

        private void DoBackspace()
        {
            if (_resetOnNextDigit)
            {
                _current = "0";
                _resetOnNextDigit = false;
                UpdateDisplay();
                return;
            }

            if (_current.Length <= 1)
            {
                _current = "0";
            }
            else
            {
                _current = _current[..^1];
                if (_current == "-" || _current == "-0")
                    _current = "0";
            }

            UpdateDisplay();
        }

        private void ToggleSign()
        {
            if (_current == "0") return;

            if (_current.StartsWith("-", StringComparison.Ordinal))
                _current = _current.Substring(1);
            else
                _current = "-" + _current;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Display is defined in MainWindow.xaml
            Display.Text = _current;
        }

        private void ShowError()
        {
            Display.Text = "Error";
            _current = "0";
            _accumulator = null;
            _pendingOperator = null;
            _resetOnNextDigit = true;
        }

        // Optional keyboard support mapped to the same logic
        private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                int d = e.Key - Key.D0;
                ProcessDigit(d.ToString());
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                int d = e.Key - Key.NumPad0;
                ProcessDigit(d.ToString());
                e.Handled = true;
                return;
            }

            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                ProcessDigit(".");
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Add || (e.Key == Key.OemPlus && (Keyboard.Modifiers & ModifierKeys.Shift) != 0))
            {
                ProcessOperator('+');
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                ProcessOperator('-');
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Multiply)
            {
                ProcessOperator('*');
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Divide || e.Key == Key.Oem2)
            {
                ProcessOperator('/');
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                DoEquals();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Back)
            {
                DoBackspace();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Delete || e.Key == Key.Escape)
            {
                DoClear();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.S)
            {
                ToggleSign();
                e.Handled = true;
            }
        }
    }
}