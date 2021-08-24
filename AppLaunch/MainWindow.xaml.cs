using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AppLaunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLaunchItems();
        }

        void LoadLaunchItems()
        {
            LaunchItems.LoadJson();

            foreach (var element in LaunchItems.GetElements())
            {
                var template = CreateTemplate(element.IconImage, element.Source.Name, element.Source.AppLink);
                root.Children.Add(template);
            }
        }

        private static UIElement CreateTemplate(Image imageSourceView, string applicationName, string applicationShellScript)
        {
            imageSourceView.Width = 80;
            imageSourceView.Height = 80;

            var textBlock = new TextBlock();
            textBlock.Width = 100;
            textBlock.Height = 20;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.Text = applicationName;

            // Make template with hard-coded sources.
            Button button = new Button()
            {
                Width = 100,
                Height = 100,
                Content = new Canvas()
                {
                    Width = 100,
                    Height = 100,
                    Children =
                    {
                        imageSourceView,
                        textBlock
                    }
                }
            };

            // Setup dependency properties.
            imageSourceView.SetValue(Canvas.LeftProperty, 10.0);
            textBlock.SetValue(Canvas.TopProperty, 80.0);

            button.Click += (sender, ev) =>
            {
                try
                {
                    if (!Path.IsPathRooted(applicationShellScript))
                    {
                        applicationShellScript = Path.Combine(Directory.GetCurrentDirectory(), "Content", applicationShellScript);
                    }

                    // Use shell execute.
                    Process applicationProc = new();
                    applicationProc.StartInfo = new()
                    {
                        FileName = $"{applicationShellScript}",
                        UseShellExecute = true
                    };

                    if (!applicationProc.Start())
                    {
                        MessageBox.Show($"Could not start process with shell script: {applicationShellScript}.", "Error");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An exception occurred while process is beginning:\n{e.Message}");
                }
            };

            Border border = new()
            {
                Margin = new Thickness(10.0, 10.0, 0.0, 0.0),
                Child = button
            };

            return border;
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
