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
    }
}
