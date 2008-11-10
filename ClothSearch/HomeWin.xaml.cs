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
using System.IO;
using Zju.Service;
using Zju.Util;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HomeWin : Window
    {
        private List<ColorItem> colorItems;
        private List<ShapeItem> shapeItems;

        private IClothLibService clothLibService;

        private IClothSearchService clothSearchService;

        private String keyPicFileName;

        private OpenFileDialog dlgOpenKeyPic;

        private OpenFileDialog dlgOpenPics;

        private ProgressWin progressWin;

        private delegate void AsynUpdateUI(int nFinished);

        private delegate void AsynImportPics(List<String> picNames);

        private System.Windows.Forms.FolderBrowserDialog dlgOpenPicFolder;

        // pages
        private const int picsPerPage = 28;
        private List<Cloth> searchedClothes;
        private int curPage;
        // totalPage = (seachedClothes.Count + picsPerPage - 1) / picsPerPage
        private int totalPage;

        public HomeWin()
        {
            colorItems = ViewHelper.NewColorItems;
            shapeItems = ViewHelper.NewShapeItems;
            this.Resources.Add("colorItems", colorItems);
            this.Resources.Add("shapeItems", shapeItems);

            InitializeComponent();

            dlgOpenKeyPic = newOpenFileDialog();
            dlgOpenKeyPic.Title = "请选择关键图";

            dlgOpenPics = newOpenFileDialog();
            dlgOpenPics.Title = "请选择多张图片进行导入";
            dlgOpenPics.Multiselect = true;

            dlgOpenPicFolder = new System.Windows.Forms.FolderBrowserDialog();
            dlgOpenPicFolder.Description = "请选择文件夹以导入其下的所有图片(JPG, GIF, PNG, BMP)";
            dlgOpenPicFolder.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //dlgOpenPicFolder.RootFolder = Environment.SpecialFolder.MyPictures;

            // It should be done by dependency injection here!!
            clothLibService = new ClothLibService(new ClothDao());
            clothSearchService = new ClothSearchService(new ClothDao());
        }

        private void btnToolOpen_Click(object sender, RoutedEventArgs e)
        {
            AddPicWin addPicWin = new AddPicWin(keyPicFileName);
            addPicWin.Owner = this;
            addPicWin.ShowDialog();
        }

        private void btnOpenKeyPic_Click(object sender, RoutedEventArgs e)
        {
            if (dlgOpenKeyPic.ShowDialog() == true)
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(dlgOpenKeyPic.FileName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                imgKeyPic.Source = bi;

                keyPicFileName = dlgOpenKeyPic.FileName;
            }
        }

        private OpenFileDialog newOpenFileDialog()
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "jpeg (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|All Image files|*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.png;*.bmp;*.ico;*.tif;*.tiff|All files (*.*)|*.*";
            dlgOpenFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            return dlgOpenFile;
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
            if (dlgOpenPics.ShowDialog() == true)
            {
                String[] selectedFiles = dlgOpenPics.FileNames;
                int nFiles = selectedFiles.Length;
                if (nFiles == 0)
                {
                    MessageBox.Show("您未先选择任何图片, 请重新选择.", "温馨提醒");
                    return;
                }

                List<String> picNames = new List<string>(nFiles);
                picNames.AddRange(selectedFiles);

                // save to database and show progress bar asynchronously.
                asynImportClothPics(picNames);
            }
        }

        private void btnToolImportFolder_Click(object sender, RoutedEventArgs e)
        {
            if (dlgOpenPicFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String selectedPath = dlgOpenPicFolder.SelectedPath;
                string[] jpgFiles = Directory.GetFiles(selectedPath, "*.jpg");
                string[] gifFiles = Directory.GetFiles(selectedPath, "*.gif");
                string[] pngFiles = Directory.GetFiles(selectedPath, "*.png");
                string[] bmpFiles = Directory.GetFiles(selectedPath, "*.bmp");
                int nFiles = jpgFiles.Length + gifFiles.Length + pngFiles.Length + bmpFiles.Length;
                if (nFiles == 0)
                {
                    MessageBox.Show("您选择的文件夹中未包含任何图片, 请重新选择.", "温馨提醒");
                    return;
                }

                List<String> picNames = new List<string>(nFiles);
                picNames.AddRange(jpgFiles);
                picNames.AddRange(gifFiles);
                picNames.AddRange(pngFiles);
                picNames.AddRange(bmpFiles);

                // save to database and show progress bar asynchronously.
                asynImportClothPics(picNames);
            }
        }

        private void asynImportClothPics(List<String> picNames)
        {
            new AsynImportPics(importClothPics).BeginInvoke(picNames, null, null);
        }

        private void importClothPics(List<String> picNames)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new AsynUpdateUI(showProgressDialog), picNames.Count);
            // batch add pictures: add 10 pictures every time.
            int step = 10;
            List<Cloth> clothes = new List<Cloth>(step);
            // finished pictures
            int nFinished = 0;
            foreach (String picName in picNames)
            {
                Cloth cloth = new Cloth();
                cloth.Path = picName;
                clothes.Add(cloth);
                if (++nFinished % step == 0)
                {
                    clothLibService.AddClothes(clothes);
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new AsynUpdateUI(updateProgressWin), nFinished);
                    clothes.Clear();
                }
            }
            if (clothes.Count > 0)
            {
                clothLibService.AddClothes(clothes);
            }

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new AsynUpdateUI(closeProgressWin), nFinished);
        }

        private void showProgressDialog(int nTotal)
        {
            progressWin = new ProgressWin(nTotal);
            progressWin.Owner = this;
            progressWin.ShowDialog();
        }

        private void updateProgressWin(int nFinished)
        {
            progressWin.FinishedPics = nFinished;
        }
        
        private void closeProgressWin(int nFinished)
        {
            progressWin.Close();
            MessageBox.Show(String.Format("成功导入{0}张图片", nFinished), "祝贺您");
        }

        private void btnToolImportKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgImportKey = new OpenFileDialog();
            dlgImportKey.Filter = "key file(*.key)|*.key";
            dlgImportKey.Title = "请选择注册码文件导入";
            if (dlgImportKey.ShowDialog() == true)
            {
                // do something
            }
        }

        private void btnToolSeeKey_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("abcdefghijklmnopqrstuvwxyz", "您的注册码");
        }

        private void btnToolExportKey_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgExportKey = new SaveFileDialog();
            dlgExportKey.Filter = "key file(*.key)|*.key";
            dlgExportKey.Title = "请指定导出的注册码文件名";
            if (dlgExportKey.ShowDialog() == true)
            {
                // do something
            }
        }

        private void btnToolAbout_Click(object sender, RoutedEventArgs e)
        {
            ClothSearchAboutBox aboutBox = new ClothSearchAboutBox();
            aboutBox.ShowDialog();
        }

        private void btnMatchAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            IndeterminateProgressWin progressWin = new IndeterminateProgressWin("不用傻等", "正在开发中, 请直接关闭本对话框.");
            progressWin.Owner = this;
            progressWin.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lblSearchResultInfo.Content = "正在搜索请稍候...";
            searchedClothes = searchByText();

            updatePicResults();
        }

        private List<Cloth> searchByText()
        {
            string words = string.IsNullOrEmpty(txtSearchInput.Text) ? null : txtSearchInput.Text;
            
            ColorEnum colors = ColorEnum.NONE;
            foreach (ColorItem ci in colorItems)
            {
                if (ci.Selected)
                {
                    colors |= ci.Value;
                }
            }

            ShapeEnum shapes = ShapeEnum.NONE;
            foreach (ShapeItem si in shapeItems)
            {
                if (si.Selected)
                {
                    shapes |= si.Value;
                }
            }

            return clothSearchService.SearchByText(words, colors, shapes);
        }

        /// <summary>
        /// Update the WrapPanel of result pictures with <code>searchedClothes</code> of the class.
        /// </summary>
        private void updatePicResults()
        {
            curPage = 0;
            totalPage = searchedClothes == null ? 0 : (searchedClothes.Count + picsPerPage - 1) / picsPerPage;
            
            updatePageOfPicResults();
        }

        private void updatePageOfPicResults()
        {
            bool isLast = (curPage >= totalPage - 1);
            bool isFirst = (curPage <= 0);
            // update page button
            if (btnPrePage.IsEnabled == isFirst)
            {
                btnPrePage.IsEnabled = !isFirst;
            }
            if (btnNextPage.IsEnabled == isLast)
            {
                btnNextPage.IsEnabled = !isLast;
            }

            // update text info
            if (searchedClothes.Count == 0)
            {
                lblSearchResultInfo.Content = "未搜索到任何结果.";
            }
            else
            {
                lblSearchResultInfo.Content = String.Format("搜索到{0}个布料, 共{1}页, 当前显示第{2}页:", searchedClothes.Count, totalPage, curPage + 1);
            }
            
            // update page pictures
            wpanResultPics.Children.Clear();
            if (curPage < totalPage)
            {
                int begin = curPage * picsPerPage;
                int num = isLast ? searchedClothes.Count - begin : picsPerPage;
                for (int i = 0; i < num; ++i)
                {
                    wpanResultPics.Children.Add(newBorder(searchedClothes[begin+i].Path, 100, 100));
                }
            }
        }

        private Border newBorder(String picName, int weight, int height)
        {
            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.SteelBlue;
            border.Child = newImage(picName, weight, height);

            return border;
        }

        private Image newImage(String picName, int weight, int height)
        {
            Image img = new Image();
            img.Stretch = Stretch.Uniform;
            img.Width = Width;
            img.Height = height;

            if (!String.IsNullOrEmpty(picName))
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(picName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                img.Source = bi;

                img.MouseDown += new MouseButtonEventHandler(resultImg_MouseDown);
            }

            return img;
        }

        private void resultImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imgCurrentResult.Source = ((Image)sender).Source;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            imgKeyPic.Source = null;
            keyPicFileName = null;
        }

        private void btnResultDelete_Click(object sender, RoutedEventArgs e)
        {
            IndeterminateProgressWin progressWin = new IndeterminateProgressWin("不用傻等", "正在开发中, 请直接关闭本对话框.");
            progressWin.Owner = this;
            progressWin.ShowDialog();
        }

        private void btnResultModify_Click(object sender, RoutedEventArgs e)
        {
            IndeterminateProgressWin progressWin = new IndeterminateProgressWin("不用傻等", "正在开发中, 请直接关闭本对话框.");
            progressWin.Owner = this;
            progressWin.ShowDialog();
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            curPage = 0;
            updatePageOfPicResults();
        }

        private void btnPrePage_Click(object sender, RoutedEventArgs e)
        {
            curPage = curPage > 0 ? curPage - 1 : 0;
            updatePageOfPicResults();
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            curPage = curPage < totalPage - 1 ? curPage + 1 : (totalPage > 0 ? totalPage - 1 : 0);
            updatePageOfPicResults();
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            curPage = totalPage > 0 ? totalPage - 1 : 0;
            updatePageOfPicResults();
        }

    }
}
