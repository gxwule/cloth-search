using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for ProgressWin.xaml
    /// </summary>
    public partial class ProgressWin : Window
    {
        private int totalPics;

        private string unformatInfo;

        public ProgressWin(int totalPics)
        {
            this.totalPics = totalPics;
            InitializeComponent();

            unformatInfo = String.Format("进度: 共有图片{0}张, 已经导入{{0}}张...", this.totalPics);
            lblProgSummary.Content = String.Format(unformatInfo, 0);
            pgbProgInfo.Maximum = totalPics;
        }

        public int FinishedPics
        {
            set 
            {
                pgbProgInfo.Value = value;
                lblProgSummary.Content = String.Format(unformatInfo, value);
            }
        }
    }
}
