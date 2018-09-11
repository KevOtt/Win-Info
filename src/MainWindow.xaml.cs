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
using System.ComponentModel;

namespace Win_Info
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Credential credential = new Credential();
        private ConnectionHandler connectionHandler = new ConnectionHandler();
        private DataClasses.ConnectionInfo connectinfo = new DataClasses.ConnectionInfo();
        private List<DataClasses.CPU> cpus = new List<DataClasses.CPU>();
        private List<DataClasses.Disk> disks = new List<DataClasses.Disk>();
        private List<DataClasses.NIC> nics = new List<DataClasses.NIC>();
        private List<DataClasses.PageFile> pageFiles = new List<DataClasses.PageFile>();
        private List<DataClasses.PhysicalDisk> physicalDisks = new List<DataClasses.PhysicalDisk>();
        private DataCollector dataCollector;

        private DataClasses.SystemInfo systemInfo = new DataClasses.SystemInfo();

        public object This { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            groupbox_connectioninfo.DataContext = connectinfo;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            // Check if server is blank
            if (TargetServer.Text == "")
            {
                MessageBox.Show("No target server specified", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check credential options
            if (radioButton_SavedCred.IsChecked == true)
            {
                credential.usedefault = false;
            }
            else
            {
                credential.usedefault = true;
            }
            // Prompt for credential if needed
            if (credential.usedefault == false & credential.userName == null)
            {
                // If credentials are not updated, exit connect
                if (!(PromptForCredential()))
                {
                    return;
                }
            }
            connectinfo.ConnectStatus = "Connecting...";

            ConnectServer();

            // Establish the connection and get data in a separate thread
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_GetData;
            worker.RunWorkerCompleted += worker_RefreshData;
            worker.RunWorkerAsync();
        }

        // Things we want to do in the background
        private void ConnectServer() { 
            // Create Connection
            groupbox_connectioninfo.DataContext = connectinfo;
            Connect.IsEnabled = false;
            connectionHandler.CreateConnection(TargetServer.Text, credential);
        }

        private void GetData()
        {
            dataCollector = new DataCollector(connectionHandler);
            cpus = dataCollector.GetCPUData();
            disks = dataCollector.GetDiskData();
            nics = dataCollector.GetNICData();
            pageFiles = dataCollector.GetPageFileData();
            physicalDisks = dataCollector.GetPhysicalDiskData();
            systemInfo = dataCollector.GetSystemInfo();
        }

        private void RefreshView()
        {
            EnableControls();

            listView_CPU.ItemsSource = cpus;
            listView_disk.ItemsSource = disks;
            listView_PageFile.ItemsSource = pageFiles;
            listView_PDisk.ItemsSource = physicalDisks;
            groupBox_BasicInfo.DataContext = groupbox_AdvInfo.DataContext = groupbox_Configuration.DataContext = groupbox_Memory.DataContext = systemInfo;
            RefreshNICView();
        }

        // Separating this call for the physical adapter button
        private void RefreshNICView()
        {
            if (showPhysicalNIC.IsChecked == true)
            {
                listView_NIC.ItemsSource = nics.Where(NIC => NIC.PhysicalAdapter == true);
            }
            else
            {
                listView_NIC.ItemsSource = nics;
            }
        }

        // Background worker for data collection
        private void worker_GetData(object sender, DoWorkEventArgs e)
        {
            GetData();
        }

        private void worker_RefreshData(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                RefreshView();
            });
        }

        // Call cred prompt dialog to obtain a saved credential
        private bool PromptForCredential()
        {
            CredentialPrompt credentialprompt = new CredentialPrompt();
            credentialprompt.ShowDialog();
            // If we have a successfully obtained, update cred object return true
            if (credentialprompt.DialogResult == true)
            {
                credential = credentialprompt.Credential;
                return true;
            }
            return false;
        }

        private void DisableControls()
        {
            groupBox_BasicInfo.DataContext = groupbox_AdvInfo.DataContext = groupbox_Configuration.DataContext = groupbox_Memory.DataContext = null;
            Grid_Info.Opacity = 50;
            Grid_Info.IsEnabled = false;
        }

        private void EnableControls()
        {
            Grid_Info.Opacity = 100;
            Grid_Info.IsEnabled = true;
        }

# region EventHandlers
        protected virtual void OnDataReturned(EventArgs e)
        {
            RefreshView();
        }

        private void TargetServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Connect.IsEnabled != true)
            {
                Connect.IsEnabled = true;
            }
        }

        private void TargetServer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Connect_Click(this, new RoutedEventArgs());
            }
        }

        private void showPhysicalNIC_Changed(object sender, RoutedEventArgs e)
        {
            RefreshNICView();
        }

        private void button_queryProcesses_Click(object sender, RoutedEventArgs e)
        {
            var process = new AdditionalQueries.ProcessQuery(dataCollector);
            process.ShowDialog();
        }

        private void button_queryServices_Click(object sender, RoutedEventArgs e)
        {
            var service = new AdditionalQueries.ServicesQuery(dataCollector);
            service.ShowDialog();
        }

        private void button_queryUpdates_Click(object sender, RoutedEventArgs e)
        {
            var update = new AdditionalQueries.UpdatesQuery(dataCollector);
            update.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
#endregion

    }
}
