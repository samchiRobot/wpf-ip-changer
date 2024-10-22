﻿using System;
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

namespace IP_changer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.MainViewModel();
        }

        private void ListBoxProgramLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var idx = ListBoxProgramLog.SelectedIndex;
            if(idx>0)
            {
                var item = ListBoxProgramLog.Items.GetItemAt(idx);
                ListBoxProgramLog.ScrollIntoView(item);
            }
        }
    }
}
