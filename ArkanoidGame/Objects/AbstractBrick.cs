using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public abstract class AbstractBrick : IGameObject
    {
        public void OnUpdate(long gameElapsedTime, IList<IGameObject> allGameObjects)
        {
            throw new NotImplementedException();
        }

        public Vector2D Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double ObjectWidth
        {
            get { throw new NotImplementedException(); }
        }

        public double ObjectHeight
        {
            get { throw new NotImplementedException(); }
        }

        public IList<Renderer.GameBitmap> ObjectTextures
        {
            get { throw new NotImplementedException(); }
        }

        public Vector2D Velocity
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsBall
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPlayerPaddle
        {
            get { throw new NotImplementedException(); }
        }

        public IList<IGeometricShape> GetGeometricShape()
        {
            throw new NotImplementedException();
        }
    }
}
