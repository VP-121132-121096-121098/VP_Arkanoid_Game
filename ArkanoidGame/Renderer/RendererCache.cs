using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ArkanoidGame.Renderer
{
    public class UnknownAliasException : Exception
    {
        public UnknownAliasException() : base() { }

        public UnknownAliasException(string message) : base(message) { }
    }

    /**************************************************************************
     * Поради фактот што .NET Framework го користи GDI+ за рендерирање, било  *
     * какво скалирање на слика со голема резолуција е премногу бавно за      *
     * игра. Дури и 100 читања од хард диск на слика без скалирање е побрзо   *
     * од 15 скалирања на слика која само еднаш е прочитана од хард диск.     *
     * Значи ако еднаш ја скалираме сликата во потребната резолуција, истата  *
     * ќе треба да ја користиме што е можно повеќе пати. Ако е потребна       *
     * истата слика во друга резолуција, најдобро е да се направи скалирање   *
     * на оригинална слика што се наоѓа на хард диск бидејќи повеќе скалирања *
     * на иста слика води кон лош квалитет на истата. Моја идеја за решение   *
     * на овој проблем е штом еднаш ќе се скалира сликата, истата да се чува  *
     * во меморија се додека е можно истата да притреба повторно. Пример      *
     * позадината во играта се рендерира 60 пати во секунда. Ако истата се    *
     * скалираше на секое рендерирање, тогаш немаше да се рендерира 60 пати,  *
     * туку околу 10 пати. Ако истата се скалира еднаш и потоа се користи при *
     * секое рендерирање, на резолуција 1600 x 900 може да се постигнат околу *
     * 100 FPS (frames per second). И тоа толкава е разликата за само една    *
     * слика. Но игра како оваа има повеќе од една слика, а 10 FPS е          *
     * неприфатливо. Значи мора сите слики со голема резолуција скалирани да  *
     * се чуваат во меморија. Притоа може да се користи AA и висок квалитет   *
     * на интерполација бидејќи скалирањето се прави само еднаш. Не може да   *
     * се чуваат слики на хард дискот за секоја можна резолуција бидејќи      *
     * играчот може да ја менува големината на прозорецот, па не може         *
     * однапред да се знае колкава ќе биде неговата резолуција.               *
     *                                                                        *
     * Тестирањата се направени на компјутер со CPU Intel Core i5 2450M       *
     * 8 GB RAM DDR3-1333, GPU GeForce GT525M                                 *
     *                                                                        *
     * Петар Ќимов                                                            *
     *                                                                        *
     **************************************************************************/

    public class RendererCache
    {

        /// <summary>
        /// Вчитува слика од датотека. Локацијата на датотеката се задава како 
        /// релативна патека, пример: /Resources/Images/background.jpg
        /// Доколку сликата не е во бараната резолуција се прави скалирање, при што
        /// се користи System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic интерполација
        /// и се користи anti-aliasing (AA).
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromFile(string relativePath, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Image image = Image.FromFile(string.Format("{0}{1}",
                System.Environment.CurrentDirectory, relativePath));

            if (image.Width != width || image.Height != height)
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, width, height);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Вчитува слика од датотека. Сликата се чува во вирутелната меморија. Референца кон сликата
        /// може да се добие со повик на методот GetBitmapFromMainMemory(long uniqueID)
        /// Локацијата на датотеката се задава како 
        /// релативна патека, пример: \\Resources\\Images\\background.jpg
        /// Доколку сликата не е во бараната резолуција се прави скалирање, при што
        /// се користи System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic интерполација
        /// и се користи anti-aliasing (AA).
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="alias"></param>
        public static void LoadBitmapIntoMainMemory(string relativePath, int width, int height, long uniqueID)
        {
            Bitmap bitmap = GetBitmapFromFile(relativePath, width, height);

            lock (objectLock)
            {
                bitmapsInMemory.Add(uniqueID, bitmap);
                mapIDRelativePath.Add(uniqueID, relativePath);
            }
        }

        public static void LoadBitmapIntoMainMemory(string relativePath, long uniqueID)
        {
            Image image = Image.FromFile(string.Format("{0}{1}",
                System.Environment.CurrentDirectory, relativePath));
            Bitmap bitmap = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, image.Width, image.Height);
            }

            lock (objectLock)
            {
                bitmapsInMemory.Add(uniqueID, bitmap);
                mapIDRelativePath.Add(uniqueID, relativePath);
            }
        }

        /// <summary>
        /// Избриши ја сликата од виртуелната меморија.
        /// </summary>
        /// <param name="uniqueID"></param>
        public static void RemoveBitmapFromMainMemory(long uniqueID)
        {
            lock (objectLock)
            {
                if (!bitmapsInMemory.Remove(uniqueID))
                {
                    throw new UnknownAliasException();
                }
                mapIDRelativePath.Remove(uniqueID);
            }
        }

        /// <summary>
        /// Ги брише сите слики кои се наоѓаат во виртуелната меморија
        /// </summary>
        public static void RemoveAllBitmapsFromMainMemory()
        {
            lock (objectLock)
            {
                Dictionary<long, Bitmap>.Enumerator it = bitmapsInMemory.GetEnumerator();
                while (it.MoveNext())
                {
                    it.Current.Value.Dispose();
                }
                bitmapsInMemory = new Dictionary<long, Bitmap>();
                mapIDRelativePath = new Dictionary<long, string>();
            }
        }

        /// <summary>
        /// Повторно ја вчитува сликата со ID во виртуелната меморија,
        /// но се скалира во друга резолуција. Потребно е веќе да постои слика
        /// со тој ID.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="newBitmapRelativePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ResizeBitmap(long uniqueID, int width, int height)
        {
            string relativePath = null;
            lock (objectLock)
            {
                mapIDRelativePath.TryGetValue(uniqueID, out relativePath);
            }
            RemoveBitmapFromMainMemory(uniqueID);
            LoadBitmapIntoMainMemory(relativePath, width, height, uniqueID);
        }

        /// <summary>
        /// Ја менува големината на сликата користејќи Anti-aliasing и HighQualityBicubic интерполација.
        /// </summary>
        /// <param name="bitmapToResize"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap bitmapToResize, int newWidth, int newHeight)
        {
            Bitmap newBitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmapToResize, 0, 0, newWidth, newHeight);

            return newBitmap;
        }

        /// <summary>
        /// Сликата што се додава со повик на овој метод може да се добие со повик 
        /// на методот GetBitmapFromMainMemory().
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public static void SaveBitmap(long uniqueID, Bitmap bitmap)
        {
            lock (objectLock)
            {
                Bitmap temp;
                if (bitmapsInMemory.TryGetValue(uniqueID, out temp))
                {
                    return;
                }
            }

            bitmap.Save(string.Format("{0}\\Resources\\Cache\\{1}.bmp",
                System.Environment.CurrentDirectory,
                uniqueID), ImageFormat.Png);
            lock (objectLock)
            {
                bitmapsInMemory.Add(uniqueID, bitmap);
                mapIDRelativePath.Add(uniqueID, string.Format("\\Resources\\Cache\\{0}.bmp", uniqueID));
            }
        }

        /// <summary>
        /// Враќа референца кон сликата со дадениот ID.
        /// Сликата претходно треба да е вчитана во виртуелната меморија со повик 
        /// на методот LoadBitmapIntoMainMemory.
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromMainMemory(long uniqueID)
        {
            Bitmap temp = null;
            lock (objectLock)
            {
                if (bitmapsInMemory.TryGetValue(uniqueID, out temp))
                {
                    return temp;
                }
            }

            return null;
        }

        /// Враќа референца кон сликата со баранаото ID. 
        /// Ако е потребно скалирање, прво се повикува методот ChangeBitmapResolution().
        /// Сликата претходно треба да е вчитана во виртуелната меморија со повик 
        /// на методот LoadBitmapIntoMainMemory.
        public static Bitmap GetBitmapFromMainMemory(long uniqueID, int width, int height)
        {
            Bitmap temp = null;
            bitmapsInMemory.TryGetValue(uniqueID, out temp);
            if (temp == null)
                return null;

            if (temp.Width != width || temp.Height != height)
            {
                ResizeBitmap(uniqueID, width, height);
            }

            return temp;
        }

        private static Dictionary<long, Bitmap> bitmapsInMemory; //map <uniqueAlias, Bitmap>
        private static Dictionary<long, string> mapIDRelativePath; // <alias, relativePath>

        static RendererCache()
        {
            bitmapsInMemory = new Dictionary<long, Bitmap>();
            mapIDRelativePath = new Dictionary<long, string>();
            objectLock = new object();
        }

        private static readonly object objectLock;

        /// <summary>
        /// Вчитува слика од датотека. Локацијата на датотеката се задава како 
        /// релативна патека, пример: \\Resources\\Images\\background.jpg
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromFile(string relativePath)
        {
            Image bitmap = Image.FromFile(string.Format("{0}{1}",
                System.Environment.CurrentDirectory, relativePath));
            Bitmap tempBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImage(bitmap, 0, 0);
            }

            return tempBitmap;
        }
    }
}
