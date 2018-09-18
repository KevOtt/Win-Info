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
using System.Windows.Shapes;

namespace Win_Info.AdditionalQueries
{
    /// <summary>
    /// Interaction logic for ServicesQuery.xaml
    /// </summary>
    public partial class ServicesQuery : Window
    {
        private DataCollector dataCollector;
        private List<DataClasses.Service> services;

        public ServicesQuery(DataCollector obj)
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
            services = dataCollector.GetServiceData();
            listView_service.ItemsSource = services;
        }

        private void Process_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
