using System;
using System.Windows;
using System.Threading;
using System.ComponentModel;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for ProgressWin.xaml
    /// </summary>
    public partial class ProgressWin : Window
    {
        private int totalPics;

        private string unformatInfo;

        /// <summary>
        /// State of the flag, 0 means the progress is running,
        /// 1 means it may be closed by mouse click, 2 means other window want it closed.
        /// </summary>
        private int stateFlag;

        public ProgressWin(int totalPics)
        {
            this.totalPics = totalPics;
            InitializeComponent();

            unformatInfo = String.Format("进度: 共有图片{0}张, 已经导入{{0}}张...", this.totalPics);
            lblProgSummary.Content = String.Format(unformatInfo, 0);
            pgbProgInfo.Maximum = totalPics;

            stateFlag = 0;
        }

        public int FinishedPics
        {
            set 
            {
                pgbProgInfo.Value = value;
                lblProgSummary.Content = String.Format(unformatInfo, value);
            }
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            lock (this)
            {
                if (stateFlag == 2)
                {
                    // close immediately
                    return;
                }
                else if (stateFlag == 0)
                {
                    MessageBoxResult result = MessageBox.Show(
                    "关闭将停止导入(不影响已导入图片). 是否继续?", "温馨提示",
                     MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // should be closed, its parent window will close it at the proper time.
                        stateFlag = 1;
                        lblProgSummary.Content = "请等待当前正在处理的图片完成...";
                    }
                } 
            }

            // not closed.
            e.Cancel = true;
        }
        
        public int StateFlag
        {
            get { lock (this) { return stateFlag; } }
            set { lock (this) { stateFlag = value; } }
        }
    }
}
