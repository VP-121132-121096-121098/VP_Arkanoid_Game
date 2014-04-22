using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Framework
{
    public class GameUnitConversion
    {
        //VirtualWidth / Height се должини на играта
        //RealWidth / Height се должини на прозорецот

        /// <summary>
        /// virtualX: Виртуелна координата на објектот од играта
        /// virtualY: Виртуелна координата на објектот од играта
        /// realX: Координата изразена во мерна единица која ја користи панелот на кој се црта
        /// realY: Координата изразена во мерна единица која иа користи панелот на кој се црта
        /// virtualWidth: Должина на панелот изразена во единици од играта (пример 3840, 
        /// кога димензиите се 3840 x 2160
        /// virtualHeight: Ширина на панелот изразена во единици од играта (пример 2160,
        /// кога димензиите се 3840 х 2160
        /// realWidth: Вистинската должина на панелот што се содржи во Panel.Width
        /// realHeight: Вистинската ширина на панелот што се содржи во Panel.Height
        /// </summary>
        /// <param name="virtualX"></param>
        /// <param name="virtualY"></param>
        /// <param name="realX"></param>
        /// <param name="realY"></param>
        /// <param name="virtualWidth"></param>
        /// <param name="virtualHeight"></param>
        /// <param name="realWidth"></param>
        /// <param name="realHeight"></param>
        public static void ConvertGameUnits(int virtualX, int virtualY, out int realX, out int realY,
            int virtualWidth, int virtualHeight, int realWidth, int realHeight)
        {
            realX = (int)Math.Round((double)realWidth / virtualWidth * virtualX);
            realY = (int)Math.Round((double)realHeight / virtualHeight * virtualY);
        }

        public static void ConvertGameUnits(double virtualX, double virtualY, out double realX, out double realY,
            int virtualWidth, int virtualHeight, int realWidth, int realHeight)
        {
            realX = (double)realWidth / virtualWidth * virtualX;
            realY = (double)realHeight / virtualHeight * virtualY;
        }
    }
}
