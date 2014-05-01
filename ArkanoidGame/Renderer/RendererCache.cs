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
        private static readonly object objectLock;
        private static IDictionary<long, Bitmap> bitmapsInMemory;
        private static IDictionary<long, string> mapBitmapRelativePath;

        static RendererCache()
        {
            objectLock = new object();
            bitmapsInMemory = new Dictionary<long, Bitmap>();
            mapBitmapRelativePath = new Dictionary<long, string>();
        }

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
        /// Повторно ја вчитува сликата со единствен клуч во виртуелната меморија,
        /// но се скалира во друга резолуција. Потребно е веќе да постои слика
        /// со тој клуч.
        /// </summary>
        private static void ResizeBitmap(long uniqueKey, int width, int height)
        {
            string relativePath = null;
            lock (objectLock)
            {
                mapBitmapRelativePath.TryGetValue(uniqueKey, out relativePath);
            }
            RemoveBitmapFromMainMemory(uniqueKey);
            LoadBitmapIntoMainMemory(relativePath, width, height, uniqueKey);
        }

        /// <summary>
        /// Вчитува слика од датотека. Сликата се чува во вирутелната меморија. Референца кон сликата
        /// може да се добие со повик на методот GetBitmapFromMainMemory(string uniqueKey)
        /// Локацијата на датотеката се задава како 
        /// релативна патека, пример: \\Resources\\Images\\background.jpg
        /// Доколку сликата не е во бараната резолуција се прави скалирање, при што
        /// се користи System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic интерполација
        /// и се користи anti-aliasing (AA).
        /// </summary>
        private static void LoadBitmapIntoMainMemory(string relativePath, int width, int height, long uniqueKey)
        {
            Bitmap bitmap = GetBitmapFromFile(relativePath, width, height);

            lock (objectLock)
            {
                bitmapsInMemory.Add(uniqueKey, bitmap);
                mapBitmapRelativePath.Add(uniqueKey, relativePath);
            }
        }

        public static void LoadBitmapIntoMainMemory(string relativePath, long key)
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
                bitmapsInMemory.Add(key, bitmap);
                mapBitmapRelativePath.Add(key, relativePath);
            }
        }

        /// <summary>
        /// Избриши ја сликата од виртуелната меморија.
        /// </summary>
        public static void RemoveBitmapFromMainMemory(long uniqueKey)
        {
            lock (objectLock)
            {
                if (!bitmapsInMemory.Remove(uniqueKey))
                {
                    throw new UnknownAliasException();
                }
                mapBitmapRelativePath.Remove(uniqueKey);
            }
        }

        /// <summary>
        /// Ги брише сите слики кои се наоѓаат во виртуелната меморија
        /// </summary>
        public static void RemoveAllBitmapsFromMainMemory()
        {
            lock (objectLock)
            {
                bitmapsInMemory = new Dictionary<long, Bitmap>();
                mapBitmapRelativePath = new Dictionary<long, string>();
            }
        }

        /// <summary>
        /// Враќа референца кон сликата со дадениот клуч.
        /// Сликата претходно треба да е вчитана во виртуелната меморија со повик 
        /// на методот LoadBitmapIntoMainMemory.
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public static Bitmap GetBitmapFromMainMemory(long uniqueKey)
        {
            Bitmap temp = null;
            //lock (objectLock)
            //{
            if (bitmapsInMemory.TryGetValue(uniqueKey, out temp))
            {
                return temp;
            }
            //}

            return null;
        }

        /// Враќа референца кон сликата со бараниот клуч. 
        /// Ако е потребно скалирање, прво се повикува методот ChangeBitmapResolution().
        /// Сликата претходно треба да е вчитана во виртуелната меморија со повик 
        /// на методот LoadBitmapIntoMainMemory.
        public static Bitmap GetBitmapFromMainMemory(long uniqueKey, int width, int height)
        {
            Bitmap temp = null;
            bitmapsInMemory.TryGetValue(uniqueKey, out temp);
            if (temp == null)
                return null;

            if (temp.Width != width || temp.Height != height)
            {
                ResizeBitmap(uniqueKey, width, height);
            }

            return temp;
        }

        /// <summary>
        /// Сликата што се додава со повик на овој метод може да се добие со повик 
        /// на методот GetBitmapFromMainMemory().
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public static void SaveBitmap(long uniqueKey, Bitmap bitmap)
        {
            lock (objectLock)
            {
                Bitmap temp;
                if (bitmapsInMemory.TryGetValue(uniqueKey, out temp))
                {
                    return;
                }
            }

            bitmap.Save(string.Format("{0}\\Resources\\Cache\\{1}.bmp",
                System.Environment.CurrentDirectory,
                uniqueKey), ImageFormat.Png);
            lock (objectLock)
            {
                bitmapsInMemory.Add(uniqueKey, bitmap);
                mapBitmapRelativePath.Add(uniqueKey, string.Format("\\Resources\\Cache\\{0}.bmp", uniqueKey));
            }
        }
    }
}
