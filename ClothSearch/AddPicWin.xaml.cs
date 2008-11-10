using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Zju.View;
using Zju.Domain;
using Zju.Service;
using Zju.Util;
using Zju.Dao;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for AppPicWin.xaml
    /// </summary>
    public partial class AddPicWin : Window
    {
        private List<ColorItem> colorItems;
        private List<ShapeItem> shapeItems;

        private String keyPicFileName;

        private String addPicFileName;

        private IClothLibService clothLibService;

        private OpenFileDialog dlgOpenPic;

        public AddPicWin(String keyPicFileName)
        {
            colorItems = ViewHelper.NewColorItems;
            shapeItems = ViewHelper.NewShapeItems;
            this.Resources.Add("colorItems", colorItems);
            this.Resources.Add("shapeItems", shapeItems);

            InitializeComponent();

            this.keyPicFileName = keyPicFileName;
            if (String.IsNullOrEmpty(this.keyPicFileName))
            {
                btnAddImportKeyPic.IsEnabled = false;
            }

            // It should be done by dependency injection here!!
            clothLibService = new ClothLibService(new ClothDao());

            // initialize OpenFileDialog
            dlgOpenPic = new OpenFileDialog();
            dlgOpenPic.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dlgOpenPic.Title = "请选择新增的图片";
            dlgOpenPic.Filter = "jpeg (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|All Image files|*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.png;*.bmp;*.ico;*.tif;*.tiff|All files (*.*)|*.*";
        }

        public AddPicWin() : this(null)
        {

        }


        private void btnAddOpenPic_Click(object sender, RoutedEventArgs e)
        {
            if (dlgOpenPic.ShowDialog() == true)
            {
                showAddPic(dlgOpenPic.FileName);
            }
        }

        private void chkAddColors_Click(object sender, RoutedEventArgs e)
        {
            String Values = "";

            foreach (ColorItem ci in colorItems)
            {
                if (ci.Selected)
                {
                    Values += String.IsNullOrEmpty(Values) ? ci.Name : "," + ci.Name;
                }
            }

            cmbAddColors.Text = Values;
        }

        private void chkAddShapes_Click(object sender, RoutedEventArgs e)
        {
            String Values = "";

            foreach (ShapeItem ci in shapeItems)
            {
                if (ci.Selected)
                {
                    Values += String.IsNullOrEmpty(Values) ? ci.Name : "," + ci.Name;
                }
            }

            cmbAddShapes.Text = Values;
        }

        private void btnAddFileSave_Click(object sender, RoutedEventArgs e)
        {
            Cloth cloth = new Cloth();
            if (!String.IsNullOrEmpty(txtAddName.Text))
            {
                cloth.Name = txtAddName.Text;
            }
            if (!String.IsNullOrEmpty(txtAddPattern.Text))
            {
                cloth.Pattern = txtAddPattern.Text;
            }
            if (!String.IsNullOrEmpty(addPicFileName))
            {
                cloth.Path = addPicFileName;
            }

            ColorEnum colors = ColorEnum.NONE;
            foreach (ColorItem ci in colorItems)
            {
                if (ci.Selected)
                {
                    colors |= ci.Value;
                }
            }
            cloth.Colors = colors;

            ShapeEnum shapes = ShapeEnum.NONE;
            foreach (ShapeItem si in shapeItems)
            {
                if (si.Selected)
                {
                    shapes |= si.Value;
                }
            }
            cloth.Shapes = shapes;

            clothLibService.AddCloth(cloth);

            // close the window.
            this.Close();
        }

        private void btnAddImportKeyPic_Click(object sender, RoutedEventArgs e)
        {
            showAddPic(keyPicFileName);
        }

        private void btnAddFileCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Show a picture in the <code>imgAdded</code> Image control with the file <code>fileName</code>.
        /// </summary>
        /// <param name="fileName"></param>
        private void showAddPic(String fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                imgAdded.Source = bi;

                addPicFileName = fileName;
            }
        }

        public String KeyPicFileName
        {
            get { return this.keyPicFileName; }
            set { this.keyPicFileName = value; }
        }
    }
}
