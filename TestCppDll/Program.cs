using System;
using System.Collections.Generic;
using System.Text;
using Zju.Image;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace TestCppDll
{
    class Program
    {

        private static float calcManhattanDistance(int[] v1, int[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
            {
                return int.MaxValue;
            }

            float mds = 0.0f;
            int n = v1.Length;
            for (int i = 0; i < n; ++i)
            {
                mds += (v1[i] >= v2[i] ? v1[i] - v2[i] : v2[i] - v1[i]);
            }

            return mds / n;
        }

        private static float calcManhattanDistance(float[] v1, float[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
            {
                return float.MaxValue;
            }

            float mds = 0.0f;
            int n = v1.Length;
            for (int i = 0; i < n; ++i)
            {
                mds += (v1[i] >= v2[i] ? v1[i] - v2[i] : v2[i] - v1[i]);
            }

            return mds / n;
        }

        static void testClothSearch()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();
            im.LuvInit(@"E:\projects\ClothSearch\codes\trunk\data\luv.dat");

            String selectedPath = @"E:\pic_skindetect\img_lib";
            string[] jpgFiles = Directory.GetFiles(selectedPath, "*.jpg");
            string[] gifFiles = Directory.GetFiles(selectedPath, "*.gif");
            string[] pngFiles = Directory.GetFiles(selectedPath, "*.png");
            string[] bmpFiles = Directory.GetFiles(selectedPath, "*.bmp");
            int nFiles = jpgFiles.Length + gifFiles.Length + pngFiles.Length + bmpFiles.Length;
            if (nFiles == 0)
            {
                return;
            }

            List<String> picNames = new List<string>(nFiles);
            picNames.AddRange(jpgFiles);
            picNames.AddRange(gifFiles);
            picNames.AddRange(pngFiles);
            picNames.AddRange(bmpFiles);

            Console.WriteLine("begin extract");
            List<float[]> tcs = new List<float[]>();
            List<float[]> ccs = new List<float[]>();
            List<string> fns = new List<string>();
            foreach (String picName in picNames)
            {
                ccs.Add(im.ExtractRGBSeparateColorVector(picName, 8, new int[]{-1}));
                tcs.Add(im.ExtractDaubechiesWaveletVector(picName));
                fns.Add(picName);
            }

            Console.WriteLine("begin calculate color md");
            StreamWriter sw = File.CreateText(@"E:\projects\ClothSearch\codes\trunk\data\testcolor.txt");
            List<float> ccsMd = new List<float>();
            for (int i = 0; i < ccs.Count; ++i)
            {
                for (int j=0; j<ccs.Count; ++j)
                {
                    ccsMd.Add(calcManhattanDistance(ccs[i], ccs[j]));
                }
                ccsMd.Sort();
                sw.WriteLine(fns[i]);
                foreach (float md in ccsMd)
                {
                    sw.WriteLine(md);
                }
                ccsMd.Clear();
            }
            sw.Close();


            Console.WriteLine("begin calculate texture md");
            sw = File.CreateText(@"E:\projects\ClothSearch\codes\trunk\data\testtexture.txt");
            List<float> tcsMd = new List<float>();
            for (int i = 0; i < tcs.Count; ++i)
            {
                for (int j=0; j<tcs.Count; ++j)
                {
                    tcsMd.Add(calcManhattanDistance(tcs[i], tcs[j]));
                }

                tcsMd.Sort();
                sw.WriteLine(fns[i]);
                foreach (float md in tcsMd)
                {
                    sw.WriteLine(md);
                }
                tcsMd.Clear();
            }

            sw.Close();
        }

        static void testClothSearch2()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();

            String selectedPath = @"E:\pic_skindetect\img_lib";
            string[] jpgFiles = Directory.GetFiles(selectedPath, "*.jpg");
            string[] gifFiles = Directory.GetFiles(selectedPath, "*.gif");
            string[] pngFiles = Directory.GetFiles(selectedPath, "*.png");
            string[] bmpFiles = Directory.GetFiles(selectedPath, "*.bmp");
            int nFiles = jpgFiles.Length + gifFiles.Length + pngFiles.Length + bmpFiles.Length;
            if (nFiles == 0)
            {
                return;
            }

            List<String> picNames = new List<string>(nFiles);
            picNames.AddRange(jpgFiles);
            picNames.AddRange(gifFiles);
            picNames.AddRange(pngFiles);
            picNames.AddRange(bmpFiles);

            Console.WriteLine("begin extract");
            List<float[]> tcs = new List<float[]>();
            List<float[]> ccs = new List<float[]>();
            List<string> fns = new List<string>();
            foreach (String picName in picNames)
            {
                //ccs.Add(im.ExtractCooccurrenceVector(picName));
                tcs.Add(im.ExtractGaborVector(picName));
                fns.Add(picName);
            }

            Console.WriteLine("begin calculate color md");
            StreamWriter sw = File.CreateText(@"E:\projects\ClothSearch\codes\trunk\data\testCooccurrence.txt");
            List<float> ccsMd = new List<float>();
            for (int i = 0; i < ccs.Count; ++i)
            {
                for (int j = i+1; j < ccs.Count; ++j)
                {
                    ccsMd.Add(calcManhattanDistance(ccs[i], ccs[j]));
                }
            }
            ccsMd.Sort();
            foreach (float md in ccsMd)
            {
                sw.WriteLine(md);
            }
            sw.Close();

            /*
            Console.WriteLine("begin calculate texture md");
            sw = File.CreateText(@"E:\projects\ClothSearch\codes\trunk\data\testGabor.txt");
            List<float> tcsMd = new List<float>();
            for (int i = 0; i < tcs.Count; ++i)
            {
                for (int j = i+1; j < tcs.Count; ++j)
                {
                    tcsMd.Add(calcManhattanDistance(tcs[i], tcs[j]));
                }

                
            }
            tcsMd.Sort();
            foreach (float md in tcsMd)
            {
                sw.WriteLine(md);
            }

            sw.Close();*/
        }

        static void testExtractFeatures()
        {
            Console.WriteLine("hello");
            ImageMatcher im = new ImageMatcher();
            String s = @"F:\photos\cc\08-06-23写真\ 003.JPG";
            //string s = @"E:\pic_skindetect\img_lib\20081839149581.jpg";
            im.LuvInit(@"E:\projects\ClothSearch\codes\trunk\data\luv.dat");
            float[] v = im.ExtractDaubechiesWaveletVector(s);
            float sum = 0;
            foreach (float i in v)
            {
                Console.Write("{0} ", i);
                sum += i;
            }
            Console.WriteLine();
            Console.WriteLine("{0}", sum);
            /*            Console.WriteLine(ImageMatcher.testAdd(10, 20).ToString());
                        ImageMatcher im = new ImageMatcher();
                        Console.WriteLine(im.testAdd2("abcd", 20).ToString());

            /*            Type t = Type.GetTypeFromProgID("ZjuImage.ImageMatcher");
                        object voice = Activator.CreateInstance(t);
                        object[] args = new Object[2];
                        args[0] = "Hello, World!";
                        args[1] = 0;
                        t.InvokeMember("Speak", BindingFlags.InvokeMethod, null, voice, args);
                        im.extractColorVector("abcd");*/
        }

        static void testGabor()
        {
            Console.WriteLine("testGabor begin");

            ImageMatcher im = new ImageMatcher();
            string s = @"E:\pic_skindetect\img_lib\20081839149581.jpg";
            float[] v = im.ExtractGaborVector(s);
            foreach (float i in v)
            {
                Console.Write("{0} ", i);
            }

            Console.WriteLine();
            Console.WriteLine("testGabor end");
        }

        static void testCooccurrence()
        {
            Console.WriteLine("testCooccurrence begin");

            ImageMatcher im = new ImageMatcher();
            string s = @"E:\testpics\test\2.bmp";
            float[] v = im.ExtractCooccurrenceVector(s);
            foreach (float i in v)
            {
                Console.Write("{0} ", i);
            }

            Console.WriteLine();
            Console.WriteLine("testCooccurrence end");
        }

        static void testExtractRGBColor()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();

            String selectedPath = @"F:\jpgtest";
            string[] jpgFiles = Directory.GetFiles(selectedPath, "*.jpg");
            string[] gifFiles = Directory.GetFiles(selectedPath, "*.gif");
            string[] pngFiles = Directory.GetFiles(selectedPath, "*.png");
            string[] bmpFiles = Directory.GetFiles(selectedPath, "*.bmp");
            int nFiles = jpgFiles.Length + gifFiles.Length + pngFiles.Length + bmpFiles.Length;
            if (nFiles == 0)
            {
                return;
            }

            List<String> picNames = new List<string>(nFiles);
            picNames.AddRange(jpgFiles);
            picNames.AddRange(gifFiles);
            picNames.AddRange(pngFiles);
            picNames.AddRange(bmpFiles);

            Console.WriteLine("begin extract");
            string pre = @"E:\projects\ClothSearch\codes\trunk\data\RGBColor\HSVAsynColor_float_11x4x4.txt";

            float[][] vs = new float[picNames.Count][];
            int count = 0;
            foreach (String picName in picNames)
            {
                vs[count++] = im.ExtractHSVAynsColorVector(picName, 0, new int[] { -1 });
            }

            //int index = picName.LastIndexOf('\\');
            //StreamWriter sw = File.CreateText(pre + picName.Substring(index + 1) + ".txt");
            StreamWriter sw = File.CreateText(pre);
            for (int j = 0; j < vs[0].Length; j++)
            {
                for (int i=0; i<vs.Length; i++)
                {
                    sw.Write("{0},", vs[i][j]);
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        private void testRef()
        {
            StateFlag = 10;
            //myRef(ref StateFlag);
            Console.WriteLine("StateFlag={0}", StateFlag);
        }

        private void myRef(ref int a)
        {
            Console.WriteLine("a={0}", a);
            a = 100;
            Console.WriteLine("a={0}", a);
        }

        static int getColorNumber(float[] v, float limit)
        {
            int count = 0;
            foreach (float f in v)
            {
                if (f >= limit)
                {
                    ++count;
                }
            }
            return count;
        }

        private static int[] cnByMan = {4, 4, 4, 4, 3, 3, 3, 3, 3, 3, 2, 3, 2, 2, 3, 4, 3, 3, 3, 3,
                                3, 5, 3, 3, 3, 5, 4, 4, 3, 4, 7, 4, 5, 4, 2, 3, 3, 4, 5, 5,
                                6, 4, 5, 6, 4, 4, 6, 5, 4, 6, 6, 4, 4, 4, 4, 5, 5, 8, 8, 3,
                                4, 4, 4, 4, 4, 7, 5, 5, 6, 5, 6, 6, 5, 5, 5, 4, 3, 5, 5, 4,
                                4, 5, 4, 3, 3, 3, 4, 5, 4, 5, 4, 4, 5, 4, 5, 6, 3, 3, 5, 3};
        private static void testColorNumberSuccess()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();

            String selectedPath = @"F:\jpgtest\";
            String ext = ".jpg";
            
            string retest = @"E:\projects\ClothSearch\codes\trunk\data\RGBColor\retest_rate2.txt";

            StreamWriter sw = File.CreateText(retest);
            sw.AutoFlush = true;
            float avrByMan = 0.0f;
            float avrRByMan = 0.0f;
            calcAvr(cnByMan, ref avrByMan, ref avrRByMan);

            sw.WriteLine("图片: {0}, 平均每张图片颜色数: {1}, 偏差: {2}", 100, avrByMan, avrRByMan);

            sw.WriteLine("vec  /lim  /Algo, avr-avrByMan, avrR-avrRByMan, suc_rate, mod_suc_rate, maihadun, ejilide");

           // float limit = 0.05f;
           // int vecnum = 8;
            int[] cnrgb = new int[100];
            int[] cnhsv = new int[100];
            int[] cnhls = new int[100];
            for (int vecnum = 2; vecnum <= 10; ++vecnum)
            {
                for (float limit=0.05f; limit <=0.1f; limit+=0.01f)
                {                
                    for (int i = 0; i < 100; ++i)
                    {
                        string picName = selectedPath + (i + 1) + ext;
                        float[] vrgb = im.ExtractRGBColorVector(picName, vecnum, new int[] { -1 });
                        float[] vhsv = im.ExtractHSVColorVector(picName, vecnum, new int[] { -1 });
                        float[] vhls = im.ExtractHLSColorVector(picName, vecnum, new int[] { -1 });
                        if (vrgb == null || vhsv == null || vhls == null)
                        {
                            Console.WriteLine("Error of {0}{1}", i + 1, ext);
                            continue;
                        }
                        cnrgb[i] = getColorNumber(vrgb, limit);
                        cnhsv[i] = getColorNumber(vhsv, limit);
                        cnhls[i] = getColorNumber(vhls, limit);
                        //sw.WriteLine("{0}, {1}, {2}", getColorNumber(vrgb, limit), getColorNumber(vhsv, limit), getColorNumber(vhls, limit));
                    }
                    float avr = 0.0f;
                    float avrR = 0.0f;
                    calcAvr(cnrgb, ref avr, ref avrR);
                    string format = "{0,-5}/{1,-5}/{2} , {3,-12}, {4,-14}, {5,-8}, {6,-12}, {7,-8}, {8,-7}";
                    sw.WriteLine(format, vecnum, limit, "rgb", avr - avrByMan, avrR - avrRByMan,
                        calcSucRate(cnByMan, cnrgb, 0.0f), calcSucRate(cnByMan, cnrgb, avr - avrByMan),
                    calcMaihadun(cnByMan, cnrgb), calEjilide(cnByMan, cnrgb));

                    calcAvr(cnhsv, ref avr, ref avrR);
                    sw.WriteLine(format, vecnum, limit, "hsv", avr - avrByMan, avrR - avrRByMan,
                        calcSucRate(cnByMan, cnhsv, 0.0f), calcSucRate(cnByMan, cnhsv, avr - avrByMan),
                    calcMaihadun(cnByMan, cnhsv), calEjilide(cnByMan, cnhsv));

                    calcAvr(cnhls, ref avr, ref avrR);
                    sw.WriteLine(format, vecnum, limit, "hls", avr - avrByMan, avrR - avrRByMan,
                        calcSucRate(cnByMan, cnhls, 0.0f), calcSucRate(cnByMan, cnhls, avr - avrByMan),
                    calcMaihadun(cnByMan, cnhls), calEjilide(cnByMan, cnhls));
                }
            }
            sw.Close();
        }

        private static void testHSVAsynColorNumberSuccess()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();

            String selectedPath = @"F:\jpgtest\";
            String ext = ".jpg";

            string retest = @"E:\projects\ClothSearch\codes\trunk\data\RGBColor\retest_rate_hsv_aync2.txt";

            StreamWriter sw = File.CreateText(retest);
            sw.AutoFlush = true;
            float avrByMan = 0.0f;
            float avrRByMan = 0.0f;
            calcAvr(cnByMan, ref avrByMan, ref avrRByMan);

            sw.WriteLine("图片: {0}, 平均每张图片颜色数: {1}, 偏差: {2}", 100, avrByMan, avrRByMan);

            sw.WriteLine("vec  /lim  /Algo, avr-avrByMan, avrR-avrRByMan, suc_rate, mod_suc_rate, maihadun, ejilide");

            // float limit = 0.05f;
            // int vecnum = 8;
            
            int[] cnhsv = new int[100];
            for (float limit = 0.05f; limit <= 0.1f; limit += 0.01f)
            {
                for (int i = 0; i < 100; ++i)
                {
                    string picName = selectedPath + (i + 1) + ext;
                    float[] vhsv = im.ExtractHSVAynsColorVector(picName, 0, new int[] { -1 });
                    if (vhsv == null)
                    {
                        Console.WriteLine("Error of {0}{1}", i + 1, ext);
                        continue;
                    }
                    cnhsv[i] = getColorNumber(vhsv, limit);
                    //sw.WriteLine("{0}, {1}, {2}", getColorNumber(vrgb, limit), getColorNumber(vhsv, limit), getColorNumber(vhls, limit));
                }
                float avr = 0.0f;
                float avrR = 0.0f;
                string format = "{0,-5}/{1,-5}/{2} , {3,-12}, {4,-14}, {5,-8}, {6,-12}, {7,-8}, {8,-7}";
                
                calcAvr(cnhsv, ref avr, ref avrR);
                sw.WriteLine(format, 176, limit, "hsva", avr - avrByMan, avrR - avrRByMan, 
                    calcSucRate(cnByMan, cnhsv, 0.0f), calcSucRate(cnByMan, cnhsv, avr - avrByMan),
                    calcMaihadun(cnByMan, cnhsv), calEjilide(cnByMan, cnhsv));
            }

            sw.Close();
        }

        private static int calcMaihadun(int[] v1, int[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
            {
                return int.MaxValue;
            }

            int n = v1.Length;
            int total = 0;
            for (int i=0; i<n; ++i)
            {
                total += (v1[i] > v2[i] ? v1[i] - v2[i] : v2[i] - v1[i]);
            }

            return total;
        }

        private static int calEjilide(int[] v1, int[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
            {
                return int.MaxValue;
            }

            int n = v1.Length;
            int total = 0;
            for (int i = 0; i < n; ++i)
            {
                int t = v1[i] - v2[i];
                total += t * t;
            }

            return total;
        }

        private static void calcAvr(int[] v, ref float avr, ref float avrR)
        {
            int sum = 0;
            foreach (int i in v)
            {
                sum += i;
            }
            avr = sum / 100.0f;
            float sumR = 0.0f;
            foreach (int i in v)
            {
                sumR += (i > avr ? i - avr : avr - i);
            }
            avrR = sumR / 100.0f;
        }

        private static float calcSucRate(int[] v1, int[] v2, float vr)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
            {
                return 0.0f;
            }

            int suc = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                float r = v1[i] - (v2[i] - vr);
                if (r >= -1.0f && r <= 1.0f)
                {
                    ++suc;
                }
            }

            return ((float)suc) / v1.Length;
        }

        static void testCalcColorNumber()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();

            String selectedPath = @"F:\jpgtest\";
            String ext = ".jpg";
            float limit = 0.1f;
            int vecnum = 8;
            string retest = @"E:\projects\ClothSearch\codes\trunk\data\RGBColor\retest_8_0.1.txt";

            StreamWriter sw = File.CreateText(retest);
            for (int i=1; i<=100; ++i)
            {
                string picName = selectedPath + i + ext;
                float[] vrgb = im.ExtractRGBColorVector(picName, vecnum, new int[] { -1 });
                float[] vhsv = im.ExtractHSVColorVector(picName, vecnum, new int[] { -1 });
                float[] vhls = im.ExtractHLSColorVector(picName, vecnum, new int[] { -1 });
                if (vrgb == null || vhsv == null || vhls == null)
                {
                    Console.WriteLine("Error of {0}{1}", i, ext);
                    continue;
                }
                sw.WriteLine("{0}, {1}, {2}, {3}", cnByMan[i-1], getColorNumber(vrgb, limit), getColorNumber(vhsv, limit), getColorNumber(vhls, limit));
            }
            sw.Close();
        }

        static void testCalcHsvAsynNumber()
        {
            Console.WriteLine("begin");
            ImageMatcher im = new ImageMatcher();

            String selectedPath = @"F:\jpgtest\";
            String ext = ".jpg";
            float limit = 0.1f;
            int vecnum = 8;
            string retest = @"E:\projects\ClothSearch\codes\trunk\data\RGBColor\retest_hsvasyn_number.txt";

            StreamWriter sw = File.CreateText(retest);
            for (int i = 1; i <= 100; ++i)
            {
                string picName = selectedPath + i + ext;
                float[] vhsv = im.ExtractHSVAynsColorVector(picName, 0, new int[] { -1 });
                if (vhsv == null)
                {
                    Console.WriteLine("Error of {0}{1}", i, ext);
                    continue;
                }
                sw.WriteLine("{0}, {1}", cnByMan[i - 1], getColorNumber(vhsv, limit));
            }
            sw.Close();
        }

        static ImageMatcher imageMatcher = new ImageMatcher();
        static void testGaborMultiThread()
        {
            Console.WriteLine("testGaborMultiThread begin");
            
            string s = @"F:\jspsp\20387.jpg";
            string s2 = @"F:\jspsp\20390.jpg";
            Console.WriteLine("init finish");

            ParameterizedThreadStart pts = new ParameterizedThreadStart(gaborThread);
            Thread t1 = new Thread(pts);
            Thread t2 = new Thread(pts);
            t1.Start(s);
            t2.Start(s2);
            t1.Join();
            t2.Join();

            Console.WriteLine("testGabor end");
        }

        static void gaborThread(Object obj)
        {
            String s = (String)obj;
            Console.WriteLine(s);
            float[] v = imageMatcher.ExtractGaborVector(s);
            foreach (float i in v)
            {
                Console.Write("{0} ", i);
            }
            Console.Write(s + "end");
        }

        static void Main(string[] args)
        {
            /*int r = 255;
            int g = 255;
            int b = 255;
            Console.WriteLine(((int)r) << 16);
            Console.WriteLine(((int)g) << 8);
            Console.WriteLine(b);
            Console.WriteLine((((int)r) << 16) + (((int)g) << 8) + b);*/
            //testClothSearch();
            
            //DateTime beg = DateTime.Now;
            //testCooccurrence();
            //testGabor();
            //DateTime end = DateTime.Now;
            //Console.WriteLine(end - beg);
            //testClothSearch2();

            //testExtractFeatures();

            //testExtractRGBColor();

            //Program pg = new Program();
            //pg.testRef();
            //testCalcColorNumber();
            //testHSVAsynColorNumberSuccess();
            //testColorNumberSuccess();
            //testCalcHsvAsynNumber();
            testGaborMultiThread();
        }

        private int stateFlag = 0;
        public int StateFlag
        {
            get { lock (this) {
                Console.WriteLine(stateFlag);
                return stateFlag; 
            } }
            set { lock (this) { stateFlag = value;
            Console.WriteLine(stateFlag);
            }
            }
        }

        
    }
}
