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
        void OnUpdate(IList<IGameObject> objects, long gameElapsedTime);

        /// <summary>
        /// Позиција на објектот во виртуелни координати
        /// </summary>
        Vector2D Position { get; set; }

        /// <summary>
        /// Должина на објектот во виртуелна единица
        /// </summary>
        int ObjectWidth { get; }

        /// <summary>
        /// Ширина на објектот во виртуелна единица
        /// </summary>
        int ObjectHeight { get; }

        /// <summary>
        /// Текстурите на објектите подредени по редоследот по кој треба да се црта
        /// </summary>
        IList<GameBitmap> ObjectTextures { get; }
    }
}
