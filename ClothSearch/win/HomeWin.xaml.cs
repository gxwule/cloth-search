﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using Microsoft.Win32;
using Zju.Dao;
using Zju.Domain;
using Zju.Image;
using Zju.Service;
using Zju.Util;
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

        private IClothLibService clothLibService;

        private IClothSearchService clothSearchService;

        private ImageMatcher imageMatcher;

        /// <summary>
        /// The Cloth object of opened key picture. Its property <code>Path</code> should be assigned.
        /// If it clears, <code>keyCloth</code> should be set <code>null</code>.
        /// </summary>
        private Cloth keyCloth;

        private AlgorithmDesc aDesc;

        private OpenFileDialog dlgOpenKeyPic;

        private OpenFileDialog dlgOpenPics;

        private ProgressWin progressWin;

        private MatchAlgorithmWin matchAlgorithmWin;

        private delegate void AsynOpenUI(int nTotal);

        private delegate void AsynUpdateUI();

        //private delegate void AsynImportPics(List<String> picNames);

        private System.Windows.Forms.FolderBrowserDialog dlgOpenPicFolder;

        /// <summary>
        /// pages.
        /// </summary>
        private const int picsPerPage = 28;
        private List<Cloth> searchedClothes;
        private int curPage;
        /// <summary>
        /// totalPage = (seachedClothes.Count + picsPerPage - 1) / picsPerPage
        /// </summary>
        private int totalPage;

        /// <summary>
        /// the selected cloth in the result clothes.
        /// </summary>
        private Cloth selectedCloth;

        private const string imageNamePrefix = "img";
        private const string reImageNamePrefix = "r";

        /// <summary>
        ///  the finished picture when import
        /// </summary>
        private int nFinished;

        /// <summary>
        ///  Thread to import pictures.
        /// </summary>
        //private Thread importPicsThread;

        /// <summary>
        /// The picture file names to be imported, used in the thread <code>importPicsThread</code>.
        /// </summary>
        //private List<String> picNames;

        //public static int count = 0;
        public HomeWin()
        {
            colorItems = ViewHelper.NewColorItems;
            shapeItems = ViewHelper.NewShapeItems;
            this.Resources.Add("colorItems", colorItems);
            this.Resources.Add("shapeItems", shapeItems);

            InitializeComponent();

            btnSearch.IsEnabled = false;
            rbtnPic.IsChecked = true;

            dlgOpenKeyPic = newOpenFileDialog();
            dlgOpenKeyPic.Title = "请选择关键图";

            dlgOpenPics = newOpenFileDialog();
            dlgOpenPics.Title = "请选择多张图片进行导入";
            dlgOpenPics.Multiselect = true;

            dlgOpenPicFolder = new System.Windows.Forms.FolderBrowserDialog();
            dlgOpenPicFolder.Description = "请选择文件夹以导入其下的所有图片(JPG, PNG, BMP)";
            dlgOpenPicFolder.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //dlgOpenPicFolder.RootFolder = Environment.SpecialFolder.MyPictures;

            // It should be done by dependency injection here!!
            clothLibService = new ClothLibService(new ClothDao());
            clothSearchService = new ClothSearchService(new ClothDao());
            imageMatcher = ViewHelper.ImageMatcher;

            aDesc = new AlgorithmDesc();

            //picNames = new List<string>();
        }

        private void btnToolOpen_Click(object sender, RoutedEventArgs e)
        {
            AddPicWin addPicWin = new AddPicWin(keyCloth);
            addPicWin.Owner = this;
            addPicWin.ShowDialog();
        }

        private void btnOpenKeyPic_Click(object sender, RoutedEventArgs e)
        {
            if (dlgOpenKeyPic.ShowDialog() == true)
            {
                BitmapImage bi = ViewHelper.NewBitmapImage(dlgOpenKeyPic.FileName);
                if (bi == null)
                {
                    MessageBox.Show("您选择的文件无法识别, 可能不是图片文件.", "显示关键图...");
                    return;
                }

                imgKeyPic.Source = bi;

                keyCloth = new Cloth();
                keyCloth.Path = dlgOpenKeyPic.FileName;
                keyCloth.Pattern = ViewHelper.ExtractPattern(keyCloth.Path);
                keyCloth.Name = keyCloth.Pattern;

                keyCloth.ColorVector = imageMatcher.ExtractColorVector(keyCloth.Path, ViewConstants.IgnoreColors);
                keyCloth.TextureVector = imageMatcher.ExtractTextureVector(keyCloth.Path);
                keyCloth.GaborVector = imageMatcher.ExtractGaborVector(keyCloth.Path);
                keyCloth.CooccurrenceVector = imageMatcher.ExtractCooccurrenceVector(keyCloth.Path);

                updateSearchButton();
            }
        }

        private OpenFileDialog newOpenFileDialog()
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            //dlgOpenFile.Filter = "jpeg (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|All Image files|*.jpg;*.jpeg;*.jpe;*.jfif;*.gif;*.png;*.bmp;*.ico;*.tif;*.tiff|All files (*.*)|*.*";
            dlgOpenFile.Filter = "jpeg (*.jpg;*.jpeg;*.jpe;*.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif|所有支持的图片文件 (JPG;BMP;PNG;TIF;...)|*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.bmp;*.dib;*.tif;*.tiff";
            dlgOpenFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dlgOpenFile.FilterIndex = 2; // begin from 1
            return dlgOpenFile;
        }

        private void btnToolImport_Click(object sender, RoutedEventArgs e)
        {
            if (dlgOpenPics.ShowDialog() == true)
            {
                String[] selectedFiles = dlgOpenPics.FileNames;
                int nFiles = selectedFiles.Length;
                if (nFiles == 0)
                {
                    MessageBox.Show("您未先选择任何图片, 请先选择.", "导入图片...");
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
                string[] tifFiles = Directory.GetFiles(selectedPath, "*.tif");
                string[] pngFiles = Directory.GetFiles(selectedPath, "*.png");
                string[] bmpFiles = Directory.GetFiles(selectedPath, "*.bmp");
                int nFiles = jpgFiles.Length + tifFiles.Length + pngFiles.Length + bmpFiles.Length;
                if (nFiles == 0)
                {
                    MessageBox.Show("您选择的文件夹中未包含任何图片, 请重新选择.", "导入图片...");
                    return;
                }

                List<String> picNames = new List<string>(nFiles);
                picNames.AddRange(jpgFiles);
                picNames.AddRange(tifFiles);
                picNames.AddRange(pngFiles);
                picNames.AddRange(bmpFiles);

                // save to database and show progress bar asynchronously.
                asynImportClothPics(picNames);
            }
        }

        private void asynImportClothPics(List<String> picNames)
        {
            //importPicsThread = new AsynImportPics(importClothPics);
            //importPicsThread.BeginInvoke(picNames, null, null);
            ParameterizedThreadStart threadDelegate = new ParameterizedThreadStart(importClothPics);
            Thread importPicsThread = new Thread(threadDelegate);
            importPicsThread.IsBackground = true;
            importPicsThread.Start(picNames);
        }

        private void importClothPics(Object obj)
        {
            //ClothUtil.Log.WriteLine("begin importClothPics");
            List<String> picNames = (List<String>)obj;

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new AsynOpenUI(showProgressDialog), picNames.Count);
            // batch add pictures: add 10 pictures every time.
            int step = 1;
            List<Cloth> clothes = new List<Cloth>(step);
            // finished pictures
            nFinished = 0;
            foreach (String picName in picNames)
            {
                if (progressWin != null  && progressWin.StateFlag == 1)
                {
                    // stop import
                    break;
                }

                Cloth cloth = generateClothObject(picName);
                
                clothes.Add(cloth);
                if (++nFinished % step == 0)
                {
                    clothLibService.InsertAll(clothes);
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new AsynUpdateUI(updateProgressWin));
                    clothes.Clear();
                }

                
            }
            if (clothes.Count > 0)
            {
                clothLibService.InsertAll(clothes);
            }

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new AsynUpdateUI(closeProgressWin));

        }

        private Cloth generateClothObject(string picName)
        {
            Cloth cloth = new Cloth();

            cloth.Path = picName;
            cloth.Pattern = ViewHelper.ExtractPattern(cloth.Path);
            cloth.Name = cloth.Pattern;

            //ClothUtil.Log.WriteLine("begin ExtractFeatures");
            ViewHelper.ExtractFeatures(cloth);

            return cloth;
        }

        private void showProgressDialog(int nTotal)
        {
            progressWin = new ProgressWin(nTotal);
            progressWin.Owner = this;
            progressWin.ShowDialog();

            //importPicsThread.Abort();

            MessageBox.Show(String.Format("成功导入{0}张图片", nFinished), "祝贺您");
        }

        private void updateProgressWin()
        {
            progressWin.FinishedPics = nFinished;
        }
        
        private void closeProgressWin()
        {
            // set the closing event should not be cancelled.
            progressWin.StateFlag = 2;
            progressWin.Close();
            progressWin = null;
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
            // window for selecting match algorithms.
            matchAlgorithmWin = new MatchAlgorithmWin(aDesc);
            matchAlgorithmWin.Owner = this;
            matchAlgorithmWin.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            /*if (++count >= 1000)
            {
                MessageBox.Show("系统未注册, 请与供应商联系, 谢谢.");
                this.Close();
            }*/

            if (true == rbtnPic.IsChecked)
            {
                if (null == keyCloth || string.IsNullOrEmpty(keyCloth.Path))
                {
                    MessageBox.Show("图片搜索必须先指定关键图.", "搜索图片...");
                    return;
                }
                lblSearchResultInfo.Content = "正在通过图片内容搜索请稍候...";
                searchedClothes = searchByPic();
            }
            else if (true == rbtnText.IsChecked)
            {
                lblSearchResultInfo.Content = "正在通过文字搜索请稍候...";
                searchedClothes = searchByText();
            }
            else if (true == rbtnCombine.IsChecked)
            {
                lblSearchResultInfo.Content = "正在进行联合搜索请稍候...";
                searchedClothes = searchByCombine();
            }
            
            updatePicResults();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Result list. Null if no search executed.</returns>
        private List<Cloth> searchByPic()
        {
            if (null == keyCloth || string.IsNullOrEmpty(keyCloth.Path))
            {
                return null;
            }

            List<Cloth> clothes = new List<Cloth>();
            int index = ViewHelper.RecallLevelToIndex(aDesc.RLevel);
            switch (aDesc.AType)
            {
                case AlgorithmType.Color1:
                    int[] colorVector = keyCloth.ColorVector != null ? keyCloth.ColorVector
                        : imageMatcher.ExtractColorVector(keyCloth.Path, ViewConstants.IgnoreColors);
                    if (colorVector == null)
                    {
                        MessageBox.Show("无法识别指定图片文件, 请检查该文件是否正确.", "提取颜色特征...");
                        return null;
                    }

                    if (clothSearchService.GetColorMDLimit() != SearchConstants.ColorMDLimits[index])
                    {
                        clothSearchService.SetColorMDLimit(SearchConstants.ColorMDLimits[index]);
                    }
                    clothes = clothSearchService.SearchByPicColor(colorVector);
                    break;
                case AlgorithmType.Texture1:
                    float[] gaborVector = keyCloth.GaborVector != null ? keyCloth.GaborVector
                        : imageMatcher.ExtractGaborVector(keyCloth.Path);
                    if (null == gaborVector)
                    {
                        MessageBox.Show("您选择的文件无法识别, 可能不是图片文件.", "提取Gabor纹理...");
                        return null;
                    }
                    
                    if (clothSearchService.GetGaborMDLimit() != SearchConstants.GaborMDLimits[index])
                    {
                        clothSearchService.SetGaborMDLimit(SearchConstants.GaborMDLimits[index]);
                    }
                    clothes = clothSearchService.SearchByPicGabor(gaborVector);
                    break;
                case AlgorithmType.Texture2:
                    float[] textureVector = keyCloth.TextureVector != null ? keyCloth.TextureVector
                        : imageMatcher.ExtractTextureVector(keyCloth.Path);
                    if (null == textureVector)
                    {
                        MessageBox.Show("您选择的文件无法识别, 可能不是图片文件.", "提取Daubechies纹理...");
                        return null;
                    }

                    if (clothSearchService.GetTextureMDLimit() != SearchConstants.TextureMDLimits[index])
                    {
                        clothSearchService.SetTextureMDLimit(SearchConstants.TextureMDLimits[index]);
                    }
                    clothes = clothSearchService.SearchByPicTexture(textureVector);
                    break;
                case AlgorithmType.Texture3:
                default:
                    float[] cooccurrenceVector = keyCloth.CooccurrenceVector != null ? keyCloth.CooccurrenceVector
                        : imageMatcher.ExtractCooccurrenceVector(keyCloth.Path);
                    if (null == cooccurrenceVector)
                    {
                        MessageBox.Show("您选择的文件无法识别, 可能不是图片文件.", "提取Cooccurrence纹理...");
                        return null;
                    }
                    
                    if (clothSearchService.GetCooccurrenceMDLimit() != SearchConstants.CooccurrenceMDLimits[index])
                    {
                        clothSearchService.SetCooccurrenceMDLimit(SearchConstants.CooccurrenceMDLimits[index]);
                    }
                    clothes = clothSearchService.SearchByPicCooccurrence(cooccurrenceVector);
                    break;
            }

            return clothes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        private List<Cloth> searchByCombine()
        {
            List<List<Cloth>> clothLists = new List<List<Cloth>>();
            List<Cloth> clothesByText = searchByText();
            if (clothesByText == null || clothesByText.Count == 0)
            {
                return new List<Cloth>();
            }

            clothLists.Add(clothesByText);

            clothLists.Add(searchByPic());

            return ClothUtil.IntersectClothLists(clothLists);
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
            if (null == searchedClothes || searchedClothes.Count == 0)
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
                    wpanResultPics.Children.Add(newBorder(searchedClothes[begin+i].Path, begin+i, 100, 100));
                }
            }
        }

        private Border newBorder(String picName, int index, int weight, int height)
        {
            Border border = new Border();
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.SteelBlue;
            border.Child = newImage(picName, index, weight, height);

            return border;
        }

        private Image newImage(String picName, int index, int weight, int height)
        {
            Image img = new Image();
            img.Stretch = Stretch.Uniform;
            img.Width = Width;
            img.Height = height;

            if (!String.IsNullOrEmpty(picName))
            {
                img.Source = ViewHelper.NewBitmapImage(picName);
                img.Name = imageNamePrefix + index.ToString();
                if (img.Source != null)
                {
                    img.MouseDown += new MouseButtonEventHandler(resultImg_MouseDown);
                }
            }

            return img;
        }

        private void resultImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            imgCurrentResult.Source = image.Source;
            //imgCurrentResult.Name = reImageNamePrefix + image.Name;

            selectedCloth = searchedClothes[int.Parse(image.Name.Substring(imageNamePrefix.Length))];

            txtModifyName.Text = string.IsNullOrEmpty(selectedCloth.Name) ? "" : selectedCloth.Name;

            txtModifyPattern.Text = string.IsNullOrEmpty(selectedCloth.Pattern) ? "" : selectedCloth.Pattern;
            
            txtModifyName.IsEnabled = true;
            txtModifyPattern.IsEnabled = true;
            btnResultDelete.IsEnabled = true;
            btnResultModify.IsEnabled = true;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            imgKeyPic.Source = null;
            keyCloth = null;

            updateSearchButton();
        }

        private void btnResultDelete_Click(object sender, RoutedEventArgs e)
        {
            //Cloth cloth = searchedClothes[int.Parse(imgCurrentResult.Name.Substring(reImageNamePrefix.Length + imageNamePrefix.Length))];
            bool isIn = searchedClothes.Remove(selectedCloth);
            clothLibService.Delete(selectedCloth.Oid);

            imgCurrentResult.Source = null;

            txtModifyName.Text = "";
            txtModifyName.IsEnabled = false;

            txtModifyPattern.Text = "";
            txtModifyPattern.IsEnabled = false;

            btnResultDelete.IsEnabled = false;
            btnResultModify.IsEnabled = false;

            if (isIn)
            {   // the removed selected cloth is in the searched clothes
                updatePicResults();
            }
        }

        private void btnResultModify_Click(object sender, RoutedEventArgs e)
        {
            //Cloth cloth = searchedClothes[int.Parse(imgCurrentResult.Name.Substring(reImageNamePrefix.Length + imageNamePrefix.Length))];

            Cloth newCloth = new Cloth(selectedCloth);
            newCloth.Name = string.IsNullOrEmpty(txtModifyName.Text) ? null : txtModifyName.Text;
            newCloth.Pattern = string.IsNullOrEmpty(txtModifyPattern.Text) ? null : txtModifyPattern.Text;

            clothLibService.Update(selectedCloth, newCloth);
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

            updateSearchButton();
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

            updateSearchButton();
        }
        /*
                private void cmbInput_MouseEnter(object sender, MouseEventArgs e)
                {
                    if (sender is ComboBox)
                    {
                        ((ComboBox)sender).IsDropDownOpen = true;
                    }
                }
        */
        private void rbtnCombine_Checked(object sender, RoutedEventArgs e)
        {
            updateSearchButtonByCombine();
        }

        private void rbtnText_Checked(object sender, RoutedEventArgs e)
        {
            updateSearchButtonByText();
        }

        private bool canSearchByText()
        {
            bool cando = false;

            if (!string.IsNullOrEmpty(txtSearchInput.Text))
            {
                cando = true;
            }

            if (!cando)
            {
                foreach (ColorItem ci in colorItems)
                {
                    if (ci.Selected)
                    {
                        cando = true;
                        break;
                    }
                }
            }

            if (!cando)
            {
                foreach (ShapeItem si in shapeItems)
                {
                    if (si.Selected)
                    {
                        cando = true;
                        break;
                    }
                }
            }

            return cando;
        }

        private void rbtnPic_Checked(object sender, RoutedEventArgs e)
        {
            updateSearchButtonByPic();
        }

        private bool canSearchByPic()
        {
            bool cando = false;

            if (null != keyCloth && !string.IsNullOrEmpty(keyCloth.Path))
            {
                cando = true;
            }

            return cando;
        }

        private void txtSearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateSearchButton();
        }

        private void updateSearchButtonByText()
        {
            bool cando = canSearchByText();

            if (btnSearch.IsEnabled != cando)
            {
                btnSearch.IsEnabled = cando;
            }
        }

        private void updateSearchButtonByPic()
        {
            bool cando = canSearchByPic();

            if (btnSearch.IsEnabled != cando)
            {
                btnSearch.IsEnabled = cando;
            }
        }

        private void updateSearchButtonByCombine()
        {
            bool cando = canSearchByText() && canSearchByPic();

            if (btnSearch.IsEnabled != cando)
            {
                btnSearch.IsEnabled = cando;
            }
        }

        private void updateSearchButton()
        {
            if (true == rbtnPic.IsChecked)
            {
                updateSearchButtonByPic();
            }
            else if (true == rbtnText.IsChecked)
            {
                updateSearchButtonByText();
            }
            else if (true == rbtnCombine.IsChecked)
            {
                updateSearchButtonByCombine();
            }
        }
    }
}