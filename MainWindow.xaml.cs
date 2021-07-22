using CowinWatcher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CowinWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

    }
  
    public class Column
    {
        public string Header { get; set; }
        public object DataField { get; set; }
    }

    public class CenterInfo
    {
        public Center CenterDetail { get; set; }
        public List<Session> SessionDay1 { get; set; }
        public List<Session> SessionDay2 { get; set; }
        public List<Session> SessionDay3 { get; set; }
        public List<Session> SessionDay4 { get; set; }
        public List<Session> SessionDay5 { get; set; }
        public List<Session> SessionDay6 { get; set; }

    }




}
