using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameState
    {
        /// <summary>
        /// Функција која дефинира што се случува при повик на Draw()
        /// во состојбата во која моментално се наоѓа играта
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        void OnDraw(Graphics graphics, int frameWidth, int frameHeight);

        /// <summary>
        /// Функција која дефинира што се случува со објектите во играта при 
        /// повик на Update() во состојбата во која моментално се наоѓа играта
        /// </summary>
        /// <param name="gameObjects"></param>
        void OnUpdate(IEnumerable<IGameObject> gameObjects);

        /// <summary>
        /// Референца кон играта
        /// </summary>
        IGame Game { get; }
    }
}
