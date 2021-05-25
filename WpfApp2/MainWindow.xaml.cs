using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WpfApp2.Log;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitLogger();
            InitUdpThread();
            showDesktop = Method1;
            Logger.LogMessage(Severity.Info, $"start process, Main Thread id: {Thread.CurrentThread.ManagedThreadId}");
        }

        private void InitLogger()
        {
            var file = new FileLogger("log.txt");
            Logger.LogMessage(Severity.Info, "Init logger success");
        }

        private void InitUdpThread()
        {
            Thread udpThread = new Thread(new ThreadStart(GetUdpMessage));
            udpThread.IsBackground = true;
            udpThread.Start();
        }

        private void GetUdpMessage()
        {
            UdpClient udpClient = null;
            try
            {
                udpClient = new UdpClient(10001);
            }
            catch (Exception)
            {
                Logger.LogMessage(Severity.Error, "create udp client failed");
                return;
            }
            Logger.LogMessage(Severity.Info, "create udp client success");

            IPEndPoint remotePoint = null;
            while (true)
            {
                try
                {
                    byte[] receiveData = udpClient.Receive(ref remotePoint);
                    string receiveString = Encoding.Default.GetString(receiveData);
                    Logger.LogMessage(Severity.Info, $"receive udp message: {receiveString}");

                    if (receiveString.ToLower().Contains("showdesktop"))
                        showDesktop?.Invoke();
                }
                catch (Exception e)
                {
                    Logger.LogMessage(Severity.Error, e.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Name)
                {
                    case "method1":
                        showDesktop = Method1;
                        Logger.LogMessage(Severity.Info, "turn to method1");
                        break;
                    case "method2":
                        showDesktop = Method2;
                        Logger.LogMessage(Severity.Info, "turn to method2");
                        break;
                    case "activeFirst":
                        showDesktop = ActiveFirst;
                        Logger.LogMessage(Severity.Info, "turn to activeFirst method");
                        break;
                    default:
                        break;
                }
            }
        }

        private void Method1()
        {
            Thread newSta = new Thread(()=>
            {
                Shell32.ShellClass objShel = new Shell32.ShellClass();
                objShel.ToggleDesktop();
                Logger.LogMessage(Severity.Info, $"Current Thread id: {Thread.CurrentThread.ManagedThreadId}");
            });
            newSta.TrySetApartmentState(ApartmentState.STA);
            newSta.Start();
        }

        private void Method2()
        {
            Type shellType = Type.GetTypeFromProgID("Shell.Application");
            object shellObject = System.Activator.CreateInstance(shellType);
            shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod, null, shellObject, null);
            Logger.LogMessage(Severity.Info, $"Current Thread id: {Thread.CurrentThread.ManagedThreadId}");
        }

        private void ActiveFirst()
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                Win32Api.SetWindowToForegroundWithAttachThreadInput(this);
                Method2();
            }));
        }

        private Action showDesktop;
    }
}
