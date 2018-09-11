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
using System.Windows.Shapes;

namespace Win_Info.AdditionalQueries
{
    /// <summary>
    /// Interaction logic for ProcessQuery.xaml
    /// </summary>
    public partial class ProcessQuery : Window
    {
        private DataCollector dataCollector;
        private List<DataClasses.Process> processes;

        public ProcessQuery(DataCollector obj)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException();
            }
            else
            {
                dataCollector = obj;
                
            }
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            processes = dataCollector.GetProcessData();
            listView_process.ItemsSource = processes;
        }

        private void Process_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
