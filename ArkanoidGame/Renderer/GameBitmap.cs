using ArkanoidGame.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Renderer
{
    public class GameBitmapKey
    {
        private Vector2D positionUR;
        private Vector2D positionUL;
        private Vector2D positionDL;

        private string uniqueName;

        public GameBitmapKey(string uniqueName, Vector2D positionUL, Vector2D positionUR, Vector2D positionDL)
        {
            this.positionUL = positionUL;
            this.positionUR = positionUR;
            this.positionDL = positionDL;

            this.uniqueName = uniqueName;
        }



        public override int GetHashCode()
        {
            Vector2D HU = positionUR - positionUL; //горна хоризонтала
            Vector2D VL = positionDL - positionUL; //лева вертикала
            Vector2D HD = (positionDL + (positionUR - positionUL)) - positionDL; //долна хоризонатала
            int temp = (uniqueName.GetHashCode() + HU.GetHashCode()) % int.MaxValue;
            temp = (temp + VL.GetHashCode()) % int.MaxValue;
            temp = (temp + HD.GetHashCode()) % int.MaxValue;
            return temp;
        }
    }

    public class GameBitmap
    {
        private static readonly object lockObject;

        static GameBitmap()
        {
            lockObject = new object();
        }

        /// <summary>
        /// Ширина на сликата во единици од играта
        /// </summary>
        public double WidthInGameUnits { get; set; }

        /// <summary>
        /// Висина на сликата во единици од играта
        /// </summary>
        public double HeightInGameUnits { get; set; }

        /// <summary>
        /// Позиција на горниот лев агол од сликата во единици од играта
        /// </summary>
        public Vector2D PositionUL { get; set; }

        /// <summary>
        /// Позиција на горниот десен агол од сликата
        /// </summary>
        public Vector2D PositionUR { get; set; }

        /// <summary>
        /// Позиција на долниот лев агол од сликата
        /// </summary>
        public Vector2D PositionDL { get; set; }

        /// <summary>
        /// Позиција на долниот десен агол од сликата
        /// </summary>
        public Vector2D PositionDR
        {
            get
            {
                return GetPositionVectorDR();
            }
        }

        /// <summary>
        /// Позиција на долниот десен агол од сликата
        /// </summary>
        /// <returns></returns>
        private Vector2D GetPositionVectorDR()
        {
            return (PositionDL + (PositionUR - PositionUL));
        }

        /// <summary>
        /// Клуч за hash и tree мапа.
        /// </summary>
        /// <returns></returns>
        public GameBitmapKey GetKey()
        {
            return new GameBitmapKey(UniqueName, PositionUR, PositionUL, PositionDL);
        }

        public virtual string UniqueName { get; protected set; }

        public GameBitmap(string relativePath, double x, double y, double widthInGameUnits,
            double heightInGameUnits, string uniqueName)
        {
            RendererCache.LoadBitmapIntoMainMemory(relativePath, uniqueName);
            this.WidthInGameUnits = widthInGameUnits;
            this.HeightInGameUnits = heightInGameUnits;
            this.PositionUL = new Vector2D(x, y);
            this.PositionUR = this.PositionUL + new Vector2D(widthInGameUnits, 0);
            this.PositionDL = this.PositionUL + new Vector2D(0, heightInGameUnits);
            this.UniqueName = uniqueName;
        }

        private void SetDimensions()
        {
            this.WidthInGameUnits = (this.PositionUR - this.PositionUL).Magnitude();
            this.HeightInGameUnits = (this.PositionDL - this.PositionUL).Magnitude();
        }

        public GameBitmap(string relativePath, Vector2D positionUL, Vector2D positionUR, Vector2D positionDL,
            string uniqueName)
        {
            RendererCache.LoadBitmapIntoMainMemory(relativePath, uniqueName);
            this.PositionUL = positionUL;
            this.PositionUR = positionUR;
            this.PositionDL = positionDL;
            this.UniqueName = uniqueName;
            this.SetDimensions();
        }

        public GameBitmap(Bitmap bitmap, double x,
            double y, double widthInGameUnits, double heightInGameUnits, string uniqueName)
        {
            RendererCache.SaveBitmap(uniqueName, bitmap);
            this.WidthInGameUnits = widthInGameUnits;
            this.HeightInGameUnits = heightInGameUnits;
            this.PositionUL = new Vector2D(x, y);
            this.PositionUR = this.PositionUL + new Vector2D(widthInGameUnits, 0);
            this.PositionDL = this.PositionUL + new Vector2D(0, heightInGameUnits);
            this.UniqueName = uniqueName;
        }

        public GameBitmap(Bitmap bitmap, Vector2D positionUL, Vector2D positionUR, Vector2D positionDL,
            string uniqueName)
        {
            RendererCache.SaveBitmap(uniqueName, bitmap);
            this.UniqueName = uniqueName;
            this.PositionUL = positionUL;
            this.PositionUR = positionUR;
            this.PositionDL = positionDL;
            this.SetDimensions();
            this.UniqueName = uniqueName;
        }
    }
}
