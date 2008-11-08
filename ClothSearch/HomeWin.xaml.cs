using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using Perst;
using Zju.Dao;
using Zju.Domain;
using Zju.View;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HomeWin : Window
    {
        private List<ColorItem> colorItems;
        private List<ShapeItem> shapeItems;

        private String keyPicFileName;

        public HomeWin()
        {
            colorItems = ViewHelper.NewColorItems;
            shapeItems = ViewHelper.NewShapeItems;
            this.Resources.Add("colorItems", colorItems);
            this.Resources.Add("shapeItems", shapeItems);

            InitializeComponent();
        }

        private void btnToolOpen_Click(object sender, RoutedEventArgs e)
        {
            AddPicWin addPicWin = new AddPicWin(keyPicFileName);
            addPicWin.ShowDialog();
        }

        private void btnOpenKeyPic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "jpeg (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|All Image files|*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.png;*.bmp;*.ico;*.tif;*.tiff|All files (*.*)|*.*";
            if (dlgOpenFile.ShowDialog() == true)
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(dlgOpenFile.FileName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                imgKeyPic.Source = bi;

                keyPicFileName = dlgOpenFile.FileName;
            }
        }

        private void chkColorInput_Click(object sender, RoutedEventArgs e)
        {
            String Values = "";

            foreach (ColorItem ci in colorItems)
            {
                if (ci.Selected)
                {
                    Values += String.IsNullOrEmpty(Values) ? ci.Name : "," + ci.Name;
                }
            }

            cmbColorInput.Text = Values;
        }

        private void chkShapeInput_Click(object sender, RoutedEventArgs e)
        {
            String Values = "";

            foreach (ShapeItem ci in shapeItems)
            {
                if (ci.Selected)
                {
                    Values += String.IsNullOrEmpty(Values) ? ci.Name : "," + ci.Name;
                }
            }

            cmbShapeInput.Text = Values;
        }

        private void btnToolImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnToolImportKey_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnToolSeeKey_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnToolExportKey_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnToolAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMatchAlgorithm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnResultDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnResultModify_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
