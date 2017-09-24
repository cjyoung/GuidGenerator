using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace GuidGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _clickCounter = 0;
        private DispatcherTimer _clickTimer = new DispatcherTimer();
        private NotifyIcon ni = new NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();
            InitializeNotificationIcon();
            GenerateNewGuid();

            //capture minimize event 
            this.StateChanged += MainWindow_StateChanged;

            //this is needed for the single/double click checker
            _clickTimer.Interval = TimeSpan.FromMilliseconds(SystemInformation.DoubleClickTime);
            _clickTimer.Tick += _clickTimer_Elapsed;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    ni.ContextMenu.MenuItems[0].Text = "Minimize";
                    break;
                case WindowState.Minimized:
                    Hide();
                    ni.ContextMenu.MenuItems[0].Text = "Restore";
                    ni.ShowBalloonTip(1, null, "Minimized to notifications area", ToolTipIcon.None);
                    break;
                case WindowState.Maximized:
                    break;
                default:
                    break;
            }

        }

        private void InitializeNotificationIcon()
        {

            ni.Icon = System.Drawing.Icon.ExtractAssociatedIcon(
                System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            ni.Visible = true;

            //using a timer to distinguish between single and double clicks
            ni.MouseDown += Ni_MouseDown;

            ContextMenu ctx = new ContextMenu();

            MenuItem mnuExit = new MenuItem() { Text = "E&xit" };
            mnuExit.Click += delegate (object o, EventArgs e) { Exit(); };

            MenuItem mnuGenerate = new MenuItem() { Text = "&Generate and Copy" };
            mnuGenerate.Click += delegate (object o, EventArgs e) { GenerateAndCopy(); };

            MenuItem mnuRestoreHide = new MenuItem() { Text = "Minimize" };
            mnuRestoreHide.Click += MnuRestoreHide_Click;

            ctx.MenuItems.Add(mnuRestoreHide);
            ctx.MenuItems.Add(mnuGenerate);
            ctx.MenuItems.Add("-");
            ctx.MenuItems.Add(mnuExit);

            ni.ContextMenu = ctx;
        }

        private void MnuRestoreHide_Click(object sender, EventArgs e)
        {
            ChangeWindowState();
        }

        private void Ni_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _clickTimer.Stop();
                _clickCounter++;
                //Trace.WriteLine("count: " + _clickCounter.ToString());
                _clickTimer.Start();
            }
        }

        private void _clickTimer_Elapsed(object sender, EventArgs e)
        {
            _clickTimer.Stop();

            if (_clickCounter == 1)
            {
                Ni_Click();
            }
            else
            {
                Ni_DoubleClick();
            }

            _clickCounter = 0;
        }

        private void Ni_Click()
        {
            GenerateAndCopy();
            //Trace.WriteLine("single click");
        }

        private void Ni_DoubleClick()
        {
            //minimize/restore on double-click
            ChangeWindowState();

            //Trace.WriteLine("double click");
        }

        private void ChangeWindowState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Minimized;
                    Hide();
                    break;
                case WindowState.Minimized:
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                    break;
                case WindowState.Maximized:
                    break;
                default:
                    break;
            }
        }

        private void GenerateAndCopy()
        {
            GenerateNewGuid();
            CopyGuidToClipboard();
            ni.ShowBalloonTip(1, null, "New Guid generated and copied to clipboard", ToolTipIcon.None);
        }

        private void Exit()
        {
            ni.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewGuid();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            CopyGuidToClipboard();
            ni.ShowBalloonTip(1, null, "Guid copied to clipboard", ToolTipIcon.None);
        }

        private void CopyGuidToClipboard()
        {
            System.Windows.Clipboard.SetText(txtGuid.Text);
        }

        private void GenerateNewGuid()
        {
            txtGuid.Text = Guid.NewGuid().ToString().ToUpper();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }
    }
}
