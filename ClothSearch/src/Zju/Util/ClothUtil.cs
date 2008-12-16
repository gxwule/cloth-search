using System;
using System.Collections.Generic;
using Zju.Domain;
using System.IO;
using Zju.Image;

namespace Zju.Util
{
    public sealed class ClothUtil
    {
        private static ImageMatcher imageMatcher;

        private static StreamWriter log;
        private static String logfile = @"E:\projects\ClothSearch\codes\trunk\data\clothlog.txt";

        static ClothUtil()
        {
            imageMatcher = new ImageMatcher();
            imageMatcher.LuvInit(SearchConstants.LuvFileName);
        }

        public static ImageMatcher ImageMatcherInst
        {
            get { return imageMatcher; }
        }

        public static StreamWriter Log
        {
            get
            {
                if (log == null)
                {
                    if (File.Exists(logfile))
                    {
                        log = File.AppendText(logfile);
                    }
                    else
                    {
                        log = File.CreateText(logfile);
                    }
                    log.AutoFlush = true;
                }
                return log;
            }
        }

        public static float CalcManhattanDistance(int[] v1, int[] v2)
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

        public static float CalcManhattanDistance(float[] v1, float[] v2)
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

        public static float CalcGaborDistance(float[] v1, float[] v2)
        {
            if (v1 == null || v2 == null || v1.Length != v2.Length)
            {
                return float.MaxValue;
            }

            float total = 0.0f;
            int n = v1.Length;
            for (int i=1; i<n; i+=2)
            {
                float t1 = v1[i - 1] - v2[i - 1];
                float t2 = (float)(Math.Sqrt(v1[i]) - Math.Sqrt(v2[i]));
                total += (float)Math.Sqrt(t1 * t1 + t2 * t2);
            }

            return total;
        }

        /// <summary>
        /// Each cloth list in <code>clothLists</code> should not contain duplicate element, or the algorithm will be acted correctly.
        /// </summary>
        /// <param name="clothLists"></param>
        /// <returns></returns>
        public static List<Cloth> IntersectClothLists(List<List<Cloth>> clothLists)
        {
            if (clothLists.Count == 0)
            {
                return new List<Cloth>();
            }
            if (clothLists.Count == 1)
            {
                return clothLists[0];
            }

            Dictionary<Cloth, int> tc = new Dictionary<Cloth, int>();
            foreach (List<Cloth> clothList in clothLists)
            {
                foreach (Cloth cloth in clothList)
                {
                    if (!tc.ContainsKey(cloth))
                    {
                        tc[cloth] = 1;
                    }
                    else
                    {
                        tc[cloth]++;
                    }
                }
            }

            int nLists = clothLists.Count;
            List<Cloth> result = new List<Cloth>();
            foreach (KeyValuePair<Cloth, int> kvp in tc)
            {
                if (kvp.Value == nLists)
                {
                    result.Add(kvp.Key);
                }
            }

            return result;
        }

        public static List<Cloth> UnionClothLists(List<List<Cloth>> clothLists)
        {
            if (clothLists.Count == 0)
            {
                return new List<Cloth>();
            }
            if (clothLists.Count == 1)
            {
                return clothLists[0];
            }

            HashSet<Cloth> hs = new HashSet<Cloth>();
            foreach (List<Cloth> clothList in clothLists)
            {
                foreach (Cloth cloth in clothList)
                {
                    hs.Add(cloth);
                }
            }

            List<Cloth> result = new List<Cloth>();
            foreach (Cloth cloth in hs)
            {
                result.Add(cloth);
            }

            return result;
        }

        /// <summary>
        /// Extract color and texture features for the cloth picture.
        /// And save the features back into the <code>cloth</code> objects.
        /// </summary>
        /// <param name="cloth"></param>
        public static void ExtractFeatures(Cloth cloth)
        {
            if (String.IsNullOrEmpty(cloth.Path))
            {
                return;
            }

            cloth.ColorVector = ImageMatcherInst.ExtractColorVector(cloth.Path, 8, SearchConstants.IgnoreColors);
            cloth.TextureVector = ImageMatcherInst.ExtractTextureVector(cloth.Path);
            cloth.GaborVector = ImageMatcherInst.ExtractGaborVector(cloth.Path);
            cloth.CooccurrenceVector = ImageMatcherInst.ExtractCooccurrenceVector(cloth.Path);
        }

        /// <summary>
        /// Extract pattern string from the picture name. I.e.
        /// C;\a\bcd.jpg -> bcd
        /// </summary>
        /// <param name="picName"></param>
        /// <returns></returns>
        public static string ExtractPattern(string picName)
        {
            if (string.IsNullOrEmpty(picName))
            {
                return null;
            }

            int i = picName.LastIndexOf('.');
            int j = picName.LastIndexOfAny(new char[] { '/', '\\' });

            j = j == -1 ? 0 : j;
            i = i == -1 ? picName.Length : i;

            return i - j - 1 > 0 ? picName.Substring(j + 1, i - j - 1) : null;
        }

        public static Cloth GenerateClothObject(string picName)
        {
            Cloth cloth = new Cloth();

            cloth.Path = picName;
            cloth.Pattern = ClothUtil.ExtractPattern(cloth.Path);
            cloth.Name = cloth.Pattern;

            //ClothUtil.Log.WriteLine("begin ExtractFeatures");
            ClothUtil.ExtractFeatures(cloth);

            return cloth;
        }
    }
}
