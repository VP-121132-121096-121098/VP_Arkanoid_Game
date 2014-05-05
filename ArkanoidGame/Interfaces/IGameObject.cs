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
    public enum GameObjectType
    {
        PlayerPaddle,
        Ball,
        Brick

    }

    public interface IGameObject
    {
        Vector2D PositionUL { get; }
        Vector2D PositionUR { get; }
        Vector2D PositionDL { get; }
        

        /// <summary>
        /// „Здравје“ на објектот. Кога ќе дојде на 0 објектот е уништен.
        /// </summary>
        double Health { get; }

        /// <summary>
        /// За колку ќе се намали Health на другиот објект со кој овој објект ќе се судри.
        /// </summary>
        double DamageEffect { get; }

        /// <summary>
        /// Правоаголник потребен за Quadtree
        /// </summary>
        RectangleF Rectangle { get; }

        /// <summary>
        /// Враќа поедноставена геометриска репрезентација на соодветниот објект.
        /// Пример за топчето оваа функција враќа круг со радиус r.
        /// </summary>
        IGeometricShape GetGeometricShape();

        /// <summary>
        /// Се повикува 60 пати во секунда (60 FPS). Притоа elapsedTime е број на поминати периоди
        /// во играта. Овде само се поместува објектот и се проверува дали излегол од границите
        /// на прозорецот. Ако излегол треба да се врати назад или да се постави Health = 0,
        /// за соодветно да се избрише од листата на објекти (дали ќе се врати назад во прозорецот или
        /// ќе „умре“ зависи од типот на објектот.
        /// </summary>
        /// <param name="gameElapsedTime"></param>
        void OnUpdate(long gameElapsedTime);

        /// <summary>
        /// За колку објектот се поместил од последниот update
        /// </summary>
        Vector2D PositionChange { get; }

        /// <summary>
        /// Со кој објект настанало судир и во кои точки. Дополнително има информација и со колку објекти
        /// </summary>
        /// <param name="collisionArguments"></param>
        void OnCollisionDetected(IDictionary<IGameObject, IList<Vector2D>> collisionArguments);

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
        Vector2D Velocity { get; set; }

        /// <summary>
        /// Тип на објектот
        /// </summary>
        GameObjectType ObjectType { get; }
    }
}
