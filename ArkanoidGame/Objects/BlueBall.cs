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


                //најди средна точка на судир
                double avgX = 0;
                double avgY = 0;

                foreach (Vector2D vec in args.Value)
                {
                    avgX += vec.X;
                    avgY += vec.Y;
                }

                avgX /= args.Value.Count;
                avgY /= args.Value.Count;

                Vector2D averageVector = new Vector2D(avgX, avgY);

                if (this.Velocity.Magnitude() > 0 && args.Key.ObjectType == GameObjectType.Brick || GameObjectType.PlayerPaddle
                    == args.Key.ObjectType)
                {
                    collidingObject = args.Key;

                    //најди го аголот на судир
                    //има три можни начини на судир: од левата страна, од горе или од десно
                    //темињата се специјален случај                    

                    //дали судирот е со хоризонталните или со вертикалните страни од правоаголникот
                    //ако се еднакви ќе сметаме дека судирот е со некое теме
                    int numXEquals = 0, numYEquals = 0;
                    foreach(Vector2D point in args.Value) {
                        if(point.Y == args.Key.PositionDL.Y || point.Y == args.Key.PositionUL.Y) {
                            numYEquals++;
                        } else {
                            numXEquals++;
                        }
                    }

                    Vector2D argPositionDR = args.Key.PositionDL + (args.Key.PositionUR - args.Key.PositionUL);

                    //одбиј се во спротивна насока од дијагоналата
                    if (averageVector == args.Key.PositionUL && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = argPositionDR - args.Key.PositionUL;

                        //ми треба единечен вектор само за правецот и насоката, должината е брзината на топчето
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = -newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    if (averageVector == args.Key.PositionDL && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = args.Key.PositionUR - args.Key.PositionDL;
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = -newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    if (averageVector == args.Key.PositionUR && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = args.Key.PositionUR - args.Key.PositionDL;
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    if (averageVector == argPositionDR && collisionWithSingleObject)
                    {
                        Vector2D newVelocity = argPositionDR - args.Key.PositionUL;
                        newVelocity /= newVelocity.Magnitude();
                        this.Velocity = newVelocity * this.Velocity.Magnitude();
                        break;
                    }

                    //ако топчето удрило во теме векторот на брзината е спротивниот
                    if (numXEquals == numYEquals && numXEquals != 0)
                    {
                        this.Velocity = -this.Velocity;
                        break;
                    }

                    if (this.Velocity.Magnitude() > 0 && numYEquals > numXEquals) 
                    {
                        if (Math.Abs(collidingObject.Velocity.X) > 0)
                        {
                            double velocityMagnitude = this.Velocity.Magnitude();
                            double x = this.Velocity.X;
                            double y = this.Velocity.Y;

                            if (velocityMagnitude == 0)
                                velocityMagnitude = 1;
                            double factorSpeedX = Math.Max(0.5, Math.Abs(collidingObject.Velocity.X) / velocityMagnitude);
                            factorSpeedX *= Math.Sign(collidingObject.Velocity.X); //знакот заради насоката на брзината
                            x += velocityMagnitude * factorSpeedX;

                            //Максимална брзина по x оската да не биде поголема од 90% од вкупната брзина
                            if (Math.Abs(x) > velocityMagnitude * 0.9)
                                x = Math.Sign(x) * velocityMagnitude * 0.9;

                            //одбиј се по y нагоре
                            y = -Math.Sqrt(velocityMagnitude * velocityMagnitude - x * x);

                            this.Velocity.X = x;
                            this.Velocity.Y = y;
                        }
                        else
                        {
                            this.Velocity.Y = -this.Velocity.Y;
                        }

                        this.collisionDetectorSkipFrames = 2;
                        break;
                    } else if(this.Velocity.Magnitude() > 0 && numYEquals < numXEquals) {
                        
                        //истата логика само по y
                        if (Math.Abs(collidingObject.Velocity.Y) > 0)
                        {
                            double velocityMagnitude = this.Velocity.Magnitude();
                            double x = this.Velocity.X;
                            double y = this.Velocity.Y;

                            if (velocityMagnitude == 0)
                                velocityMagnitude = 1;
                            double factorSpeedY = Math.Max(0.5, Math.Abs(collidingObject.Velocity.Y) / velocityMagnitude);
                            factorSpeedY *= Math.Sign(collidingObject.Velocity.Y); //знакот заради насоката на брзината
                            y += velocityMagnitude * factorSpeedY;

                            //Максимална брзина по y оската да не биде поголема од 90% од вкупната брзина
                            if (Math.Abs(y) > velocityMagnitude * 0.9)
                                y = Math.Sign(y) * velocityMagnitude * 0.9;

                            //одбиј се по y нагоре
                            x = -Math.Sqrt(velocityMagnitude * velocityMagnitude - y * y);

                            this.Velocity.X = x;
                            this.Velocity.Y = y;
                        }
                        else
                        {
                            this.Velocity.X = -this.Velocity.X;
                        }
                        this.collisionDetectorSkipFrames = 2;
                        break;
                    }
                    else if(this.Velocity.Magnitude() > 0)
                    {
                        //најверојатно се заглавил објектот
                        if (this.Velocity.Y > this.Velocity.X)
                            this.Velocity.Y = -this.Velocity.Y;
                        else
                            this.Velocity.X = -this.Velocity.X;

                        /* да се одбие по поголемата брзина за побрзо да излезе
                         * од објектот и да се прескокне детекција во наредните 2 фрејма за секој случај
                         */

                        this.collisionDetectorSkipFrames = 2; //оваа + 1
                        break;
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
