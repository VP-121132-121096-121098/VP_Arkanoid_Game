using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Framework
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

    public class BitmapExtensionMethods
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
        /// може да се добие со повик на методот GetBitmapFromMainMemory(string uniqueAlias)
        /// Локацијата на датотеката се задава како 
        /// релативна патека, пример: /Resources/Images/background.jpg
        /// Доколку сликата не е во бараната резолуција се прави скалирање, при што
        /// се користи System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic интерполација
        /// и се користи anti-aliasing (AA).
        /// Забелешка: Прекарот (alias) на сликата е case-sensitive.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="alias"></param>
        public static void LoadBitmapIntoMainMemory(string relativePath, int width, int height, string uniqueAlias)
        {
            Bitmap bitmap = GetBitmapFromFile(relativePath, width, height);

            lock (objectLock)
            {
                bitmapsInMemory.Add(uniqueAlias, bitmap);
                mapAliasRelativePath.Add(uniqueAlias, relativePath);
            }
        }

        /// <summary>
        /// Избриши ја сликата од виртуелната меморија.
        /// </summary>
        /// <param name="uniqueAlias"></param>
        public static void RemoveBitmapFromMainMemory(string uniqueAlias)
        {
            lock (objectLock)
            {
                if (!bitmapsInMemory.Remove(uniqueAlias))
                {
                    throw new UnknownAliasException();
                }
                mapAliasRelativePath.Remove(uniqueAlias);
            }
        }

        /// <summary>
        /// Ги брише сите слики кои се наоѓаат во виртуелната меморија
        /// </summary>
        public static void RemoveAllBitmapsFromMainMemory()
        {
            lock (objectLock)
            {
                Dictionary<string, Bitmap>.Enumerator it = bitmapsInMemory.GetEnumerator();
                while (it.MoveNext())
                {
                    it.Current.Value.Dispose();
                }
                bitmapsInMemory = new Dictionary<string, Bitmap>();
                mapAliasRelativePath = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Повторно ја вчитува сликата uniqueAlias во виртуелната меморија,
        /// но се скалира во друга резолуција. Потребно е веќе да постои слика
        /// со прекар (alias) uniqueAlias.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="newBitmapRelativePath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ResizeBitmap(string uniqueAlias, int width, int height)
        {
            string relativePath = null;
            mapAliasRelativePath.TryGetValue(uniqueAlias, out relativePath);
            RemoveBitmapFromMainMemory(uniqueAlias);
            LoadBitmapIntoMainMemory(relativePath, width, height, uniqueAlias);            
        }

        public static Bitmap ResizeBitmap(Bitmap bitmapToResize, int newWidth, int newHeight, string uniqueKey)
        {
            Bitmap newBitmap = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newBitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmapToResize, 0, 0, newWidth, newHeight);

            return newBitmap;
        }

        /// <summary>
        /// Враќа референца кон сликата со прекар (alias) uniqueAlias. 
        /// Сликата претходно треба да е вчитана во виртуелната меморија со повик 
        /// на методот LoadBitmapIntoMainMemory.
        /// </summary>
        /// <param name="uniqueAlias"></param>
        /// <returns></returns>
        /// <exception cref="UnknownAliasException"></exception>
        public static Bitmap GetBitmapFromMainMemory(string uniqueAlias)
        {
            Bitmap temp = null;
            lock (objectLock)
            {
                if (bitmapsInMemory.TryGetValue(uniqueAlias, out temp))
                {
                    return temp;
                }
            }

            throw new UnknownAliasException();
        }

        /// Враќа референца кон сликата со прекар (alias) uniqueAlias. 
        /// Ако е потребно скалирање, прво се повикува методот ChangeBitmapResolution().
        /// Сликата претходно треба да е вчитана во виртуелната меморија со повик 
        /// на методот LoadBitmapIntoMainMemory.
        public static Bitmap GetBitmapFromMainMemory(string uniqueAlias, int width, int height)
        {
            Bitmap temp = null;
            bitmapsInMemory.TryGetValue(uniqueAlias, out temp);
            if (temp.Width != width || temp.Height != height)
            {
                ResizeBitmap(uniqueAlias, width, height);
            }
            bitmapsInMemory.TryGetValue(uniqueAlias, out temp);
            return temp;
        }

        private static Dictionary<string, Bitmap> bitmapsInMemory; //map <uniqueAlias, Bitmap>
        private static Dictionary<string, string> mapAliasRelativePath; // <alias, relativePath>

        static BitmapExtensionMethods()
        {
            bitmapsInMemory = new Dictionary<string, Bitmap>();
            mapAliasRelativePath = new Dictionary<string, string>();
            objectLock = new object();
        }

        private static readonly object objectLock;
    }
}
