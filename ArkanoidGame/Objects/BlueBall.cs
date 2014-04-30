using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Objects
{
    public class BlueBall : IGameObject
    {
        /// <summary>
        /// Најди го центарот на сликата (местото каде што се сечат двете дијагонали на сликата)
        /// </summary>
        /// <returns></returns>
        private Vector2D GetTextureCenter()
        {

            //   \/ е стрелка надолу, -> стрелка надесно, C центарот на впишаната кружница

            /*
             *    UL              UR
             *    ------->|--------
             *    |       |       |
             *    |       \/      |
             *    --------C       |
             *    |               |
             *    |               |   
             *    -----------------
             *    DL              DR
             */

            //половина од горната хоризонтална страна од сликата
            Vector2D textureUL_UR = (ObjectTextures[0].PositionUR - ObjectTextures[0].PositionUL) / 2;

            //половина од левата вертикална страна од сликата
            Vector2D textureUL_DL = (ObjectTextures[0].PositionUL - ObjectTextures[0].PositionDL) / 2;

            //собери го радиусвекторот на горното лево теме (PositionUL) со овие два вектори
            Vector2D textureCenter = ObjectTextures[0].PositionUL + textureUL_DL + textureUL_UR;

            return textureCenter;
        }

       private GameRectangle textureRotator;

        public BlueBall(Vector2D position, double radius)
        {
            this.Velocity = new Vector2D(0, 0);
            ObjectTextures = new List<GameBitmap>();
            this.Position = position;
            this.Radius = radius;
            ObjectTextures.Add(new GameBitmap("\\Resources\\Images\\ballBlue.png", this.Position - new Vector2D(0, Radius)
                - new Vector2D(Radius, 0),
                this.Position - new Vector2D(0, Radius) + new Vector2D(Radius, 0), this.Position + new Vector2D(0, Radius)
                + new Vector2D(-Radius, 0)));
            ObjectWidth = ObjectHeight = 2 * radius;
            ObjectTextures[0].IsSquare = true;

            //квадратот ќе ги има истите темиња како и сликата
            textureRotator = new GameRectangle(ObjectTextures[0].PositionUL,
                ObjectTextures[0].PositionUR, ObjectTextures[0].PositionDL);

            playerPaddle = null;
        }

        public double Health { get { return 1000; } }

        public double DamageEffect { get { return 100; } }

        public RectangleF Rectangle
        {
            get { return new GameCircle(Position, ObjectWidth / 2).GetBoundingRectangle; }
        }

        public IGeometricShape GetGeometricShape()
        {
            return new GameCircle(Position, Radius);
        }

        public double Radius { get; private set; }

        public void OnUpdate(long gameElapsedTime)
        {

            Vector2D oldPosition = this.Position;

            //ако брзината е 0, тогаш топчето се наоѓа врз paddle и 
            //мора да се помести за да остане врз него
            if (this.Velocity.Magnitude() == 0 && playerPaddle != null)
            {
                this.Position += playerPaddle.PositionChange;
            }

            //Најди го векторот на поместување
            PositionChange = this.Position - oldPosition;

            //Направи транслација на темињата на сликата
            ObjectTextures[0].PositionUL += PositionChange;
            ObjectTextures[0].PositionDL += PositionChange;
            ObjectTextures[0].PositionUR += PositionChange;

            textureRotator.PositionUL = ObjectTextures[0].PositionUL;
            textureRotator.PositionDL = ObjectTextures[0].PositionDL;
            textureRotator.PositionUR = ObjectTextures[0].PositionUR;

            if (this.Velocity.Magnitude() > 0)
            {
                //ротација на текстурата (анимација дека се движи топчето и околу себе)
                //ротација за +5 степени
                textureRotator.RotateAroundPointDeg(this.Position, 5);

                ObjectTextures[0].PositionUL = textureRotator.PositionUL;
                ObjectTextures[0].PositionDL = textureRotator.PositionDL;
                ObjectTextures[0].PositionUR = textureRotator.PositionUR;
            }

            if (this.Velocity.Magnitude() == 0 && playerPaddle != null)
            {
                return;
            }

            //throw new NotImplementedException();
        }

        private IGameObject playerPaddle;

        public void OnCollisionDetected(IDictionary<IGameObject, IList<Geometry.Vector2D>> collisionArguments)
        {
            foreach (KeyValuePair<IGameObject, IList<Vector2D>> args in collisionArguments)
            {

                //Земи ја референцата од PlayerPaddle објектот
                if (args.Key.ObjectType == GameObjectType.PlayerPaddle)
                {
                    playerPaddle = args.Key;
                }
            }

            //throw new NotImplementedException();
        }

        public Vector2D Position { get; set; }

        public double ObjectWidth { get; private set; }

        public double ObjectHeight { get; private set; }

        public IList<GameBitmap> ObjectTextures { get; private set; }

        public Vector2D Velocity { get; private set; }

        public GameObjectType ObjectType
        {
            get { return GameObjectType.Ball; }
        }


        public Vector2D PositionChange { get; private set; }
    }
}
