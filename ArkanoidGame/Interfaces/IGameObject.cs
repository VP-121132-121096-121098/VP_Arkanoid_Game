using ArkanoidGame.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameObject
    {
        void OnUpdate(IEnumerable<IGameObject> objects, long gameElapsedTime);

        void OnDraw(Graphics graphics, int frameWidth, int frameHeight);

        Vector2D Position { get; set; }

        /// <summary>
        /// Овде се менуваат сите слики што се претходно биле 
        /// вчитани во главната меморија, но во друга резолуција.
        /// Бидејќи прозорецот има друга резолуција, мора и сликите
        /// да се вчитаат во друга резолуција.
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        void OnResolutionChanged(int newWidth, int newHeight);
    }
}
