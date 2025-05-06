// MonthlySubscriptionManager/Helpers/InputDialog.cs
using System.Windows;
using System.Windows.Controls;

namespace MonthlySubscriptionManager.WPF.Helpers
{
    public class InputDialog : Window
    {
        private TextBox textBox;

        public string Input { get; private set; }

        public InputDialog(string title, string prompt)
        {
            Title = title;
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new TextBlock
            {
                Text = prompt,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(label, 0);

            textBox = new TextBox
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(textBox, 1);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 75,
                Margin = new Thickness(0, 0, 5, 0),
                IsDefault = true
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 75,
                IsCancel = true
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(buttonPanel, 2);

            grid.Children.Add(label);
            grid.Children.Add(textBox);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Input = textBox.Text;
            DialogResult = true;
            Close();
        }
    }
}