using ArkanoidGame.Framework;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameObject
    {

        /// <summary>
        /// Враќа поедноставена геометриска репрезентација на соодветниот објект.
        /// Пример за топчето оваа функција враќа Круг со радиус r.
        /// </summary>
        IGeometricShape GetGeometricShape();

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

        /// <summary>
        /// Ако објектот се судри со некој/и друг/и објекти, тогаш кај секој од тие објекти
        /// се повикува методот OnCollisionDetected и секој објект добива листа од објектите со кои
        /// се судрил (заради оптимизација во таа листа ќе го има и самиот објект). Ниту 
        /// еден објект не смее да ја менуваа таа листа!!!
        /// </summary>
        /// <param name="collidingObjects"></param>
        void OnCollisionDetected(IList<IGameObject> collidingObjects);
    }
}
