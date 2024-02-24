using BeesVault;
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


namespace BeehiveManagementSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Объект пчелиной матки управляет ульем
        /// </summary>
        private Queen queen = new Queen();

        public MainWindow()
        {
            InitializeComponent();
            statusReport.Text = queen.StatusReport; //TextBox выводит отчет матки о состоянии улья
        }
        /// <summary>
        /// При нажатии на кнопку вызывает метод новой рабочей смены у матки
        /// </summary>
        private void WorkShift_Click(object sender, RoutedEventArgs e)
        {
            queen.WorkTheNextShift();
            statusReport.Text = queen.StatusReport;
        }
        /// <summary>
        /// При нажатии на кнопку добаляет новую рабочую пчелу, в зависимости от того какая специальность была выбрана в селекторе
        /// </summary>
        private void AssignJob_Click(object sender, RoutedEventArgs e)
        {
            queen.AssignBee(jobSelector.Text);
            statusReport.Text = queen.StatusReport;
        }
    }
}
