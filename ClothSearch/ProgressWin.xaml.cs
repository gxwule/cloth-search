using System;
using System.Windows;

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
