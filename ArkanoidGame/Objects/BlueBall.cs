using ArkanoidGame.Framework;
using ArkanoidGame.Geometry;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArkanoidGame.Objects
{
    public class BlueBall : IGameObject
    {
        private static long IDCounter;
        private static readonly object lockCounter;

        static BlueBall()
        {
            IDCounter = long.MinValue;
            lockCounter = new object();
        }

        private long ballID;

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
            lock (lockCounter)
            {
                ballID = IDCounter++;
            }

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
            Health = 1000;
        }

        public double Health { get; private set; }

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

            this.Position += Velocity;

            IKeyState leftMouseKey = KeyStateInfo.GetAsyncKeyState(Keys.LButton);
            if (this.Velocity.Magnitude() < 0.001 && playerPaddle != null && leftMouseKey.IsPressed)
            {
                //најди ја средната точка на paddle
                Vector2D middlePoint = playerPaddle.Position + new Vector2D(playerPaddle.ObjectWidth / 2, 0)
                    + new Vector2D(0, playerPaddle.ObjectHeight / 2);

                //стартувај под агол кој ќе зависи од средната точка на paddle
                Vector2D velocityAngle = this.Position - middlePoint;
                velocityAngle /= velocityAngle.Magnitude();

                this.Velocity = velocityAngle * 18;

                collisionDetectorSkipFrames = 3;
            }

            if (this.Position.Y < this.Radius + 5 || this.Position.Y > GameArkanoid.GetInstance().VirtualGameHeight - Radius - 5)
            {
                this.Velocity.Y = -this.Velocity.Y;

                if (this.Position.Y > playerPaddle.PositionDL.Y)
                {
                    this.Health = 0;
                }

                //ако е надвор од прозорецот врати го назад
                if (this.Position.Y < this.Radius + 5)
                    this.Position.Y = Radius + 5;
                else if (this.Position.Y > GameArkanoid.GetInstance().VirtualGameHeight - Radius - 5)
                    this.Position.Y = GameArkanoid.GetInstance().VirtualGameHeight - Radius - 5;
            }
            else if (this.Position.X < this.Radius + 5 || this.Position.X > GameArkanoid.GetInstance().VirtualGameWidth - Radius - 5)
            {
                this.Velocity.X = -this.Velocity.X;

                if (this.Position.X < Radius + 5)
                    this.Position.X = Radius + 5;
                else if (this.Position.X > GameArkanoid.GetInstance().VirtualGameWidth - Radius - 5)
                    this.Position.X = GameArkanoid.GetInstance().VirtualGameWidth - Radius - 5;
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

            //ротација на текстурата (анимација дека се движи топчето и околу себе)
            //ротација за +5 степени
            textureRotator.RotateAroundPointDeg(this.Position, 5);

            ObjectTextures[0].PositionUL = textureRotator.PositionUL;
            ObjectTextures[0].PositionDL = textureRotator.PositionDL;
            ObjectTextures[0].PositionUR = textureRotator.PositionUR;

            collisionDetectorSkipFrames = Math.Max(collisionDetectorSkipFrames - 1, 0);
        }

        private IGameObject playerPaddle;

        private int collisionDetectorSkipFrames;

        public void OnCollisionDetected(IDictionary<IGameObject, IList<Geometry.Vector2D>> collisionArguments)
        {

            foreach (KeyValuePair<IGameObject, IList<Vector2D>> args in collisionArguments)
            {
                IGameObject collidingObject = args.Key;

                //Земи ја референцата од PlayerPaddle објектот
                if (args.Key.ObjectType == GameObjectType.PlayerPaddle)
                {
                    playerPaddle = args.Key;
                }

                bool collisionWithSingleObject = collisionArguments.Count == 1;

                if (collisionDetectorSkipFrames > 0)
                    return;

                if (this.Velocity.Magnitude() > 0 && (args.Key.ObjectType == GameObjectType.Brick || GameObjectType.PlayerPaddle
                    == args.Key.ObjectType))
                {
                    collidingObject = args.Key;

                    //темињата се специјален случај    

                    /*Vector2D argPositionDR = args.Key.PositionDL + (args.Key.PositionUR - args.Key.PositionUL);

                    //одбиј се во спротивна насока од дијагоналата
                    if (args.Value.Count == 1 && args.Value[0] == args.Key.PositionUL && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = argPositionDR - args.Key.PositionUL;

                        //ми треба единечен вектор само за правецот и насоката, должината е брзината на топчето
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = -newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    if (args.Value.Count == 1 && args.Value[0] == args.Key.PositionDL && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = args.Key.PositionUR - args.Key.PositionDL;
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = -newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    if (args.Value.Count == 1 && args.Value[0] == args.Key.PositionUR && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = args.Key.PositionUR - args.Key.PositionDL;
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    if (args.Value.Count == 1 && args.Value[0] == argPositionDR && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = argPositionDR - args.Key.PositionUL;
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = newVelocity * this.Velocity.Magnitude();
                        break;
                    }*/

                    /* http://www.3dkingdoms.com/weekly/weekly.php?a=2 */

                    //најди вектор што ги поврзува првата и последната точка од судирот
                    Vector2D temp = new Vector2D(0, 0);
                    Vector2D lastPoint = null;

                    foreach (KeyValuePair<IGameObject, IList<Vector2D>> pair in collisionArguments)
                    {
                        foreach (Vector2D vec in pair.Value)
                        {
                            if (lastPoint != null)
                            {
                                temp += (vec - lastPoint);
                            }

                            lastPoint = vec;
                        }
                    }

                    //најди го нормалниот вектор на temp
                    //нормалите се (-dy, dx) и (dy, -dx)
                    Vector2D normal = new Vector2D(-temp.Y, temp.X);

                    //единечен вектор на normal
                    if (normal.Magnitude() != 0)
                        normal /= normal.Magnitude();
                    else
                    {
                        temp = args.Value[0] - args.Key.PositionUL;
                        if (temp.Magnitude() == 0)
                        {
                            temp = args.Key.PositionDL - args.Key.PositionUR;
                        }

                        normal = new Vector2D(-temp.Y, temp.X);
                        normal /= normal.Magnitude();
                    }

                    //новата брзина е
                    this.Velocity = -2 * (Velocity * normal) * normal + Velocity;

                    //овозможи играчот да ја менува насоката на топчето со тоа што ќе го удри
                    //во моментот кога paddle се движи
                    if (args.Key.ObjectType == GameObjectType.PlayerPaddle)
                    {
                        double velocityMagnitude = this.Velocity.Magnitude();
                        double x = this.Velocity.X;
                        double y = this.Velocity.Y;

                        //Единечен вектор на брзината
                        this.Velocity /= this.Velocity.Magnitude();

                        double factorSpeedX = Math.Max(0.35, Math.Abs(collidingObject.Velocity.X) / velocityMagnitude);
                        factorSpeedX *= Math.Sign(collidingObject.Velocity.X); //знакот заради насоката на брзината
                        x += velocityMagnitude * factorSpeedX;

                        //Максимална брзина по x оската да не биде поголема од 90% од вкупната брзина
                        if (Math.Abs(x) > velocityMagnitude * 0.9)
                            x = Math.Sign(x) * velocityMagnitude * 0.9;

                        y = Math.Sign(Velocity.Y) * Math.Sqrt(velocityMagnitude * velocityMagnitude - x * x);
                        Velocity.X = x;
                        Velocity.Y = y;

                        this.collisionDetectorSkipFrames = 7;
                    }
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

        public Vector2D PositionUL
        {
            get { return Position - new Vector2D(Radius, Radius); }
        }

        public Vector2D PositionUR
        {
            get { return Position + new Vector2D(Radius, -Radius); }
        }

        public Vector2D PositionDL
        {
            get { return Position + new Vector2D(-Radius, -Radius); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return ((BlueBall)obj).ballID == this.ballID;
        }

        public override int GetHashCode()
        {
            return ballID.GetHashCode();
        }
    }
}
