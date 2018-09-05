using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows.Threading;

namespace Wif
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process newprocess = new Process();
        public DispatcherTimer dispacthedTime = new DispatcherTimer();
        public MainWindow()
        {
            newprocess.StartInfo.UseShellExecute = false;
            newprocess.StartInfo.CreateNoWindow = true;
            newprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            InitializeComponent();
            dispacthedTime.Tick += DispacthedTime_Tick;
            dispacthedTime.Interval += new TimeSpan(0, 0, 1);
        }
        private void DispacthedTime_Tick(object sender, EventArgs e)
        {
            GetNumberOfUsers();
        }

        public bool isUerAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch(UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
        public void Process_Start_1()
        {
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan stop hostednetwork";
            try
            { 
                using(Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    Process_Start_2();
                }
            }
            catch { }
        }
        public void Process_Start_2()
        {
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan set hostednetwork ssid="+SSID_textBox.Text+" key="+Password_textBox.Text;
            try
            {
                using (Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    Process_Start_3();
                }
            }
            catch { }
        }
        public void Process_Start_3()
        {
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan start hostednetwork";
            try
            {
                using (Process execute = Process.Start(newprocess.StartInfo))
                {
                    execute.WaitForExit();
                    Play_Stop_Button.Content = "Stop";
                    SSID_textBox.IsEnabled = false;
                    Password_textBox.IsEnabled = false;
                    dispacthedTime.Start();
                }
            }
            catch { }
            DirectoryInfo di = new DirectoryInfo(@"C:\MyWifiFolder");
            if (!di.Exists)
                Directory.CreateDirectory(@"C:\MyWifiFolder");
            FileInfo fi = new FileInfo(@"C:\MyWifiFolder\Info.txt");
            if (!fi.Exists)
            {
                Stream ss = File.Create(@"C:\MyWifiFolder\Info.txt");
                ss.Close();
            }
            comboBox.IsEnabled = false;
        }
        public void Process_Stop()
        {
            newprocess.StartInfo.FileName = "netsh";
            newprocess.StartInfo.Arguments = "wlan stop hostednetwork";
            try
            {
                using (Process execute = Process.Start(newprocess.StartInfo))
                {
                    
                    execute.WaitForExit();
                    Play_Stop_Button.Content = "Start";
                    SSID_textBox.IsEnabled = true;
                    if(none.IsEnabled)
                        Password_textBox.IsEnabled = false;
                    else
                        Password_textBox.IsEnabled = true;
                    comboBox.IsEnabled = true;
                }
            }
            catch { }
        }

        private void Play_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if((String)Play_Stop_Button.Content == "Start")
            {
                Process_Start_1();
            }
            else
            {
                dispacthedTime.Stop();
                No_Of_Users.Content = "";
                Process_Stop();
            }
        }
        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetNumberOfUsers();
        }

        public void GetNumberOfUsers()
        {
            string message;
            newprocess.StartInfo.RedirectStandardInput = true;
            newprocess.StartInfo.RedirectStandardOutput = true;
            try
            {
                newprocess.StartInfo.FileName = "netsh";
                newprocess.StartInfo.Arguments = "wlan show hostednetwork";
                message = Process.Start(newprocess.StartInfo).StandardOutput.ReadToEnd().ToString();
                
                try
                {
                    File.WriteAllText(@"C:\MyWifiFolder\Info.txt", message);
                }
                catch (Exception e) { MessageBox.Show(e.Message); }
            }
            catch(Exception f) { MessageBox.Show(f.Message); }
            string[] users = File.ReadAllLines(@"C:\MyWifiFolder\Info.txt");
            string noOfUSers = users[15].Remove(0, 29);
            No_Of_Users.Content =  noOfUSers;
        }

        private void Combo_Selected(object sender, RoutedEventArgs e)
        {
            if(((ComboBoxItem)sender).Name == "none")
            {
                Password_textBox.IsEnabled = false;
                Password_textBox.Text = string.Empty;
            }
            else
            {
                Password_textBox.IsEnabled= true;
            }
        }
    }
}