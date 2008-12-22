using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Zju.Domain;
using Zju.Service;
using Zju.Util;
using Zju.View;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for AppPicWin.xaml
    /// </summary>
    public partial class AddPicWin : Window
    {
        private List<ColorItem> colorItems;
        private List<ShapeItem> shapeItems;

        private Cloth keyCloth;

        private String addPicFileName;

        private IClothLibService clothLibService;

        private OpenFileDialog dlgOpenPic;

        public AddPicWin(Cloth keyCloth)
        {
            colorItems = ViewHelper.NewColorItems;
            shapeItems = ViewHelper.NewShapeItems;
            this.Resources.Add("colorItems", colorItems);
            this.Resources.Add("shapeItems", shapeItems);

            InitializeComponent();

            txtAddName.Text = "自动";
            txtAddName.IsEnabled = false;

            this.keyCloth = keyCloth;
            if (null == keyCloth || String.IsNullOrEmpty(this.keyCloth.Path))
            {
                btnAddImportKeyPic.IsEnabled = false;
            }

            btnAddFileSave.IsEnabled = false;

            // It should be done by dependency injection here!!
            clothLibService = new ClothLibService();

            // initialize OpenFileDialog
            dlgOpenPic = new OpenFileDialog();
            dlgOpenPic.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dlgOpenPic.Title = "请选择新增的图片";
            dlgOpenPic.Filter = "jpeg (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|所有支持的图片文件 (JPG;BMP;PNG;TIF;...)|*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.bmp;*.dib;*.tif;*.tiff";
            dlgOpenPic.FilterIndex = 2; // begin from 1
        }

        public AddPicWin() : this(null)
        {

        }


        private void btnAddOpenPic_Click(object sender, RoutedEventArgs e)
        {
            if (dlgOpenPic.ShowDialog() == true)
            {
                showAddPicInfo(dlgOpenPic.FileName);
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
            if (String.IsNullOrEmpty(addPicFileName))
            {
                // should not be here.
                MessageBox.Show("请先选择一张图片.", "温馨提醒");
                return;
            }
            Cloth cloth = new Cloth();

            // whether use the key cloth.
            bool useKeyPic = (null != keyCloth && keyCloth.Path == addPicFileName);

            cloth.Path = addPicFileName;

            if (!String.IsNullOrEmpty(txtAddPattern.Text))
            {
                cloth.Pattern = txtAddPattern.Text;
            }
            cloth.Name =  useKeyPic && keyCloth.Name != null ? keyCloth.Name : ClothUtil.ExtractPattern(addPicFileName);

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

            // feature vectors
            //cloth.RGBSeparateColorVector = (useKeyPic && keyCloth.RGBSeparateColorVector != null)
            //    ? keyCloth.RGBSeparateColorVector : ClothUtil.ImageMatcherInst.ExtractRGBSeparateColorVector(cloth.Path, 8, SearchConstants.IgnoreColors);
            cloth.RGBColorVector = (useKeyPic && keyCloth.RGBColorVector != null)
                ? keyCloth.RGBColorVector : ClothUtil.ImageMatcherInst.ExtractRGBColorVector(cloth.Path, 3, SearchConstants.IgnoreColors);
            cloth.HSVAynsColorVector = (useKeyPic && keyCloth.HSVAynsColorVector != null)
                ? keyCloth.HSVAynsColorVector : ClothUtil.ImageMatcherInst.ExtractHSVAynsColorVector(cloth.Path, 0, SearchConstants.IgnoreColors);
            cloth.HSVColorVector = (useKeyPic && keyCloth.HSVColorVector != null)
                ? keyCloth.HSVColorVector : ClothUtil.ImageMatcherInst.ExtractHSVColorVector(cloth.Path, 3, SearchConstants.IgnoreColors);
            if (cloth.HSVColorVector != null)
            {
                cloth.ColorNum = ClothUtil.getColorNumber(cloth.HSVColorVector, 0.07f);
            }
            
            cloth.DaubechiesWaveletVector = (useKeyPic && keyCloth.DaubechiesWaveletVector != null)
                ? keyCloth.DaubechiesWaveletVector : ClothUtil.ImageMatcherInst.ExtractDaubechiesWaveletVector(cloth.Path);
            //cloth.GaborVector = (useKeyPic && keyCloth.GaborVector != null)
            //    ? keyCloth.GaborVector : ClothUtil.ImageMatcherInst.ExtractGaborVector(cloth.Path);
            //cloth.CooccurrenceVector = (useKeyPic && keyCloth.CooccurrenceVector != null)
            //    ? keyCloth.CooccurrenceVector : ClothUtil.ImageMatcherInst.ExtractCooccurrenceVector(cloth.Path);

            clothLibService.Insert(cloth);

            // close the window.
            this.Close();
        }

        private void btnAddImportKeyPic_Click(object sender, RoutedEventArgs e)
        {
            showAddPicInfo(keyCloth.Path, keyCloth.Pattern);
        }

        private void btnAddFileCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Show a picture in the <code>imgAdded</code> Image control with the file <code>fileName</code>.
        /// </summary>
        /// <param name="fileName"></param>
        private void showAddPicInfo(String fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                string pattern = ClothUtil.ExtractPattern(fileName);
                // use pattern as name currently.
                showAddPicInfo(fileName, pattern);
            }
        }

        private void showAddPicInfo(String fileName, String pattern)
        {
            if (!String.IsNullOrEmpty(fileName))
            {

                BitmapImage bi = ViewHelper.NewBitmapImage(fileName);
                if (bi == null)
                {
                    MessageBox.Show("您选择的不是图片文件, 请重新选择.", "温馨提示");
                    return;
                }

                imgAdded.Source = bi;

                addPicFileName = fileName;

                txtAddPattern.Text = pattern;

                btnAddFileSave.IsEnabled = true;
            }
        }


        public Cloth KeyCloth
        {
            get { return this.keyCloth; }
            set { this.keyCloth = value; }
        }

        private void cmbInput_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is ComboBox)
            {
                ((ComboBox)sender).IsDropDownOpen = true;
            }
        }
    }
}
