using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameObject
    {
        void OnUpdate();

        void OnDraw(Graphics graphics, int frameWidth, int frameHeight);

        public int PositionX {get; set;}
        public int PositionY { get; set; }
    }
}
