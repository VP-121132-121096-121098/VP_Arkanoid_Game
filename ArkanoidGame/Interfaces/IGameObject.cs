using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameObject : IHasRectangle
    {

        /// <summary>
        /// Враќа поедноставена геометриска репрезентација на соодветниот објект.
        /// Пример за топчето оваа функција враќа круг со радиус r.
        /// </summary>
        IList<IGeometricShape> GetGeometricShape();

        /// <summary>
        /// Се повикува 60 пати во секунда (60 FPS). Притоа elapsedTime е број на поминати периоди
        /// во играта.
        /// </summary>
        /// <param name="gameElapsedTime"></param>
        void OnUpdate(long gameElapsedTime);

        /// <summary>
        /// Позиција на објектот во виртуелни координати
        /// </summary>
        Vector2D Position { get; set; }

        /// <summary>
        /// Должина на објектот во виртуелна единица
        /// </summary>
        double ObjectWidth { get; }

        /// <summary>
        /// Ширина на објектот во виртуелна единица
        /// </summary>
        double ObjectHeight { get; }

        /// <summary>
        /// Текстурите на објектите подредени по редоследот по кој треба да се цртаат
        /// </summary>
        IList<GameBitmap> ObjectTextures { get; }

        /// <summary>
        /// Моментална брзина на објектот изразена како векторска величина
        /// </summary>
        Vector2D Velocity { get; }

        /// <summary>
        /// Дали објектот е топчето кое ги крши циглите? Пример paddle-от треба да
        /// го игнорира секој објект освен топчето.
        /// </summary>
        bool IsBall { get; }

        bool IsPlayerPaddle { get; }
    }
}
