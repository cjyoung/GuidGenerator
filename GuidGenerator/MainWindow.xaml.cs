using System;
using System.Windows;

namespace GuidGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnGenerate_Click(null, null);
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            txtGuid.Text = Guid.NewGuid().ToString().ToUpper();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtGuid.Text);
        }
    }
}
