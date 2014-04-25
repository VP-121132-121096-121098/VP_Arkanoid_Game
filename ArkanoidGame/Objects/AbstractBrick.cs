using ArkanoidGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public abstract class AbstractBrick : IGameObject
    {
        public void OnUpdate(long gameElapsedTime)
        {
            throw new NotImplementedException();
        }

        public Framework.Vector2D Position
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

        public Framework.Vector2D Velocity
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

        public IGeometricShape GetGeometricShape()
        {
            throw new NotImplementedException();
        }

        public void OnCollisionDetected(IList<IGameObject> collidingObjects)
        {
            throw new NotImplementedException();
        }
    }
}
