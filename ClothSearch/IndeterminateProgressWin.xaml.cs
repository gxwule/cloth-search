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
    /// Interaction logic for IndeterminateProgressWin.xaml
    /// </summary>
    public partial class IndeterminateProgressWin : Window
    {
        public IndeterminateProgressWin(String title, String hint)
        {
            InitializeComponent();

            this.Title = title;
            lblProgSummary.Content = hint;
        }
    }
}
