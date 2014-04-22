using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Renderer
{
    public class GameBitmap
    {
        private static long idCounter;

        private static readonly object lockObject;

        static GameBitmap()
        {
            idCounter = long.MinValue;
            lockObject = new object();
        }

        public long PictureID { get; private set; }

        /// <summary>
        /// Ширина на сликата во единици од играта
        /// </summary>
        public double WidthInGameUnits { get; set; }

        /// <summary>
        /// Висина на сликата во единици од играта
        /// </summary>
        public double HeightInGameUnits { get; set; }

        /// <summary>
        /// Координати на позиција на сликата во единици од играта
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координати на позиција на сликата во единици од играта
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Креира нова слика од веќе постоечка. Бидејќи сликата се додава само еднаш,
        /// најдобро е да се прати како аргумент слика во најголема можна резолуција.
        /// Квалитетот на скалираните слики ќе зависи од сликата што се праќа како аргумент.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="widthInGameUnits"></param>
        /// <param name="heightInGameUnits"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public GameBitmap(Bitmap bitmap, double x, 
            double y, double widthInGameUnits, double heightInGameUnits)
        {
            lock (lockObject)
            {
                PictureID = idCounter++;
            }
            RendererCache.SaveBitmap(PictureID, bitmap);
            this.WidthInGameUnits = widthInGameUnits;
            this.HeightInGameUnits = heightInGameUnits;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Сликата што се праќа како аргумент се копира во виртуелната меморија
        /// и се прават скалирани верзии кога тоа е потребно. Сликата што се наоѓа на
        /// хард дискот не се менува!!! При секое скалирање се вчитува повторно од диск
        /// со цел да се минимизира губењето на квалитет на истата.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="widthInGameUnits"></param>
        /// <param name="heightInGameUnits"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public GameBitmap(string relativePath, double x, double y, double widthInGameUnits,
            double heightInGameUnits)
        {
            lock (lockObject)
            {
                PictureID = idCounter++;
            }

            RendererCache.LoadBitmapIntoMainMemory(relativePath, PictureID);
            this.WidthInGameUnits = widthInGameUnits;
            this.HeightInGameUnits = heightInGameUnits;
            this.X = x;
            this.Y = y;
        }

        public GameBitmap(long uniqueID, double x, double y, double widthInGameUnits,
            double heightInGameUnits)
        {
            this.X = x;
            this.Y = y;
            this.WidthInGameUnits = widthInGameUnits;
            this.HeightInGameUnits = heightInGameUnits;
            this.PictureID = uniqueID;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            GameBitmap bmp = (GameBitmap)obj;
            return this.PictureID == bmp.PictureID;
        }

        public override int GetHashCode()
        {
            return PictureID.GetHashCode();
        }
    }
}
