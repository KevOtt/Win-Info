/*
 Copyright (c) 2018 Kevin Ott
 Licensed under the MIT License
 See the LICENSE file in the project root for more information. 
*/

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
using System.Threading;

namespace Win_Info
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string serverName;
        private Credential credential = new Credential();
        private ConnectionHandler connectionHandler = new ConnectionHandler();
        private DataClasses.ConnectionInfo connectinfo = new DataClasses.ConnectionInfo();
        private List<DataClasses.CPU> cpus = new List<DataClasses.CPU>();
        private List<DataClasses.Disk> disks = new List<DataClasses.Disk>();
        private List<DataClasses.NIC> nics = new List<DataClasses.NIC>();
        private List<DataClasses.PageFile> pageFiles = new List<DataClasses.PageFile>();
        private List<DataClasses.PhysicalDisk> physicalDisks = new List<DataClasses.PhysicalDisk>();
        private List<string> features = new List<string>();
        private DataCollector dataCollector;
        private BackgroundWorker worker;

        private DataClasses.SystemInfo systemInfo = new DataClasses.SystemInfo();

        public object This { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            connectinfo.ConnectStatus = "Not Connected";
            groupbox_connectioninfo.DataContext = connectinfo;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            // Check if target server is blank
            if (TargetServer.Text == "")
            {
                MessageBox.Show("No target system specified", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            serverName = TargetServer.Text;
            DisableControls();

            // Check credential options
            if (radioButton_SavedCred.IsChecked == true)
            {
                credential.usedefault = false;

                // If the target server is local host saved creds cannot be used
                if (TargetServer.Text == "localhost" || TargetServer.Text == System.Environment.MachineName)
                {
                    MessageBox.Show("WMI does not allow alternate credentials to be used to connect to localhost, try launching the application as the alternate user."
                        , "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                }
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

            // Connect to system
            connectinfo.ConnectStatus = "Connecting...";
            Connection();
        }

        private void Connection()
        {
            button_Connect.IsEnabled = false;
            // Attempt to open WMI connection to server
            connectionHandler.CreateConnection(serverName, credential);
            // Check we have a valid connection
            if (connectionHandler.validConnection != true)
            {
                MessageBox.Show((connectionHandler.ConnectionError), "Connection Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                connectinfo.ConnectStatus = "Connection Error";
                button_Connect.IsEnabled = true;
                return;
            }

            // Set last connection date
            connectinfo.LastConnection = (DateTime.Now).ToString("MM / dd / yyyy hh: mm:ss tt");

            // Get data in a separate thread
            connectinfo.ConnectStatus = "Querying Data....";
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_GetData;
            worker.RunWorkerCompleted += worker_RefreshData;
            worker.RunWorkerAsync();
        }


        private void GetData()
        {
            dataCollector = new DataCollector(connectionHandler);
            cpus = dataCollector.GetCPUData();
            disks = dataCollector.GetDiskData();
            nics = dataCollector.GetNICData();
            pageFiles = dataCollector.GetPageFileData();
            physicalDisks = dataCollector.GetPhysicalDiskData();
            features = dataCollector.GetFeatureData();
            systemInfo = dataCollector.GetSystemInfo();
        }

        private void RefreshView()
        {
            listView_CPU.ItemsSource = cpus;
            listView_disk.ItemsSource = disks;
            listView_PageFile.ItemsSource = pageFiles;
            listView_PDisk.ItemsSource = physicalDisks;
            listView_Roles.ItemsSource = features;
            groupBox_BasicInfo.DataContext = groupbox_AdvInfo.DataContext = groupbox_Configuration.DataContext = groupbox_Memory.DataContext = systemInfo;
            RefreshNICView();
            EnableControls();
        }

        // Separating this method from RefreshView() for the physical adapter switch
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

        // Post data collection actions
        private void worker_RefreshData(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                RefreshView();

                if (e.Error != null)
                {
                    MessageBox.Show(("Failed to query data: " + e.Error.Message), "Connection Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
                    connectinfo.ConnectStatus = ("Aborted");
                }
                else
                {
                    connectinfo.ConnectStatus = ("Connected to " + serverName);
                }
                button_ConnectionRefresh.IsEnabled = true;
            });
        }

        // Call cred prompt dialog to obtain a saved credential
        private bool PromptForCredential()
        {
            CredentialPrompt credentialprompt = new CredentialPrompt();
            credentialprompt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
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
            listView_CPU.ItemsSource = listView_disk.ItemsSource = listView_PageFile.ItemsSource = listView_PDisk.ItemsSource = 
                listView_Roles.ItemsSource = listView_NIC.ItemsSource = null;
            Grid_Info.IsEnabled = groupbox_Queries.IsEnabled = false;
            Grid_Info.Opacity = 50;
        }

        private void EnableControls()
        {
            Grid_Info.Opacity = 100;
            Grid_Info.IsEnabled = groupbox_Queries.IsEnabled = true;
        }

# region EventHandlers

        private void TargetServer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (button_Connect.IsEnabled != true)
            {
                button_Connect.IsEnabled = true;
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
            process.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            process.ShowDialog();
        }

        private void button_queryServices_Click(object sender, RoutedEventArgs e)
        {
            var service = new AdditionalQueries.ServicesQuery(dataCollector);
            service.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            service.ShowDialog();
        }

        private void button_queryUpdates_Click(object sender, RoutedEventArgs e)
        {
            var update = new AdditionalQueries.UpdatesQuery(dataCollector);
            update.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            update.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_ConnectionRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (serverName != null)
            {
                Connection();
            }
        }

        private void Credential_Prompt_Click(object sender, RoutedEventArgs e)
        {
            PromptForCredential();
        }

        private void Help_About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(("Win Info v1.0 - Copyright (c) 2018 Kevin Ott" + "\n" + 
                "Licensed under the MIT License" + "\n" + "\n" + "https://github.com/kevott"), 
                "About Win Info", MessageBoxButton.OK, MessageBoxImage.None);
        }


        // Re-enable connect button if cred option changes
        private void radioButtonCred_Changed(object sender, RoutedEventArgs e)
        {
            if (TargetServer.Text != null && button_Connect.IsEnabled == false)
            {
                button_Connect.IsEnabled = true;
            }
        }

        #endregion

    }
}
