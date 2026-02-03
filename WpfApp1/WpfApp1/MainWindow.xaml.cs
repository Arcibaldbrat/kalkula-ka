using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Kalkulacka
{
    public partial class MainWindow : Window
    {
        double firstNumber = 0;
        string operation = "";
        bool newNumber = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        void Number_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (newNumber)
            {
                Display.Text = btn.Content.ToString();
                newNumber = false;
            }
            else
                Display.Text += btn.Content;
        }

        void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (!Display.Text.Contains(","))
                Display.Text += ",";
        }

        void Operator_Click(object sender, RoutedEventArgs e)
        {
            firstNumber = double.Parse(Display.Text);
            operation = ((Button)sender).Content.ToString();
            newNumber = true;
        }

        void Equals_Click(object sender, RoutedEventArgs e)
        {
            double secondNumber = double.Parse(Display.Text);
            double result = 0;

            switch (operation)
            {
                case "+": result = firstNumber + secondNumber; break;
                case "−": result = firstNumber - secondNumber; break;
                case "×": result = firstNumber * secondNumber; break;
                case "÷": result = secondNumber == 0 ? 0 : firstNumber / secondNumber; break;
                case "%": result = firstNumber % secondNumber; break;
            }

            Display.Text = result.ToString(CultureInfo.CurrentCulture);
            newNumber = true;
        }

        void C_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            firstNumber = 0;
            operation = "";
            newNumber = true;
        }

        void CE_Click(object sender, RoutedEventArgs e)
        {
            Display.Text = "0";
            newNumber = true;
        }

        void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (Display.Text.Length > 1)
                Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
            else
                Display.Text = "0";
        }

        void Sign_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(Display.Text, out double value))
                Display.Text = (-value).ToString();
        }

        void Square_Click(object sender, RoutedEventArgs e)
        {
            double x = double.Parse(Display.Text);
            Display.Text = (x * x).ToString();
        }

        void Sqrt_Click(object sender, RoutedEventArgs e)
        {
            double x = double.Parse(Display.Text);
            Display.Text = Math.Sqrt(x).ToString();
        }

        void Reciprocal_Click(object sender, RoutedEventArgs e)
        {
            double x = double.Parse(Display.Text);
            Display.Text = (1 / x).ToString();
        }
    }
}
