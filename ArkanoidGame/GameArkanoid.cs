using ArkanoidGame.Framework;
using ArkanoidGame.Interfaces;
using ArkanoidGame.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ArkanoidGame
{
    public class ArkanoidStateMainMenu : IGameState
    {
        private IDictionary<char, Bitmap> orangeAlphabet;
        private IDictionary<char, Bitmap> blueAlphabet;

        private IDictionary<string, Bitmap> menuOptions; //опции во менито
        private IDictionary<string, Bitmap> readyStrings; //стрингови што се чуваат во меморија

        private Point quitGamePosition;
        private Point startGamePosition;
        //спремни за исцртување

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            if (MenuBackground != null)
                graphics.DrawImage(MenuBackground, 0, 0, frameWidth, frameHeight);

            Bitmap tempBitmap = null;
            if (menuOptions != null)
            {
                lock (this)
                {
                    if (menuOptions.TryGetValue("start game", out tempBitmap))
                    {
                        startGamePosition = new Point((frameWidth - tempBitmap.Width) / 2,
                            ((frameHeight - 8 * tempBitmap.Height) / 2));
                        graphics.DrawImage(tempBitmap, (float)startGamePosition.X, (float)startGamePosition.Y);
                    }
                }

                lock (this)
                {
                    if (menuOptions.TryGetValue("quit game", out tempBitmap))
                    {
                        quitGamePosition = new Point((frameWidth - tempBitmap.Width) / 2,
                            ((frameHeight - 5 * tempBitmap.Height) / 2));
                        graphics.DrawImage(tempBitmap, (float)quitGamePosition.X, (float)quitGamePosition.Y);
                    }
                }
            }
        }

        public int OnUpdate(IEnumerable<IGameObject> gameObjects, long gameElapsedTime)
        {
            Point cursorPosition = Game.CursorRelativeToPanel;

            //синхронизација бидејќи позицијата зависи од местото каде е исцртана опцијата,
            //а цртањето се прави на посебен Thread.
            lock (this)
            {

                //ако курсорот е над опцијата обој ја плаво, во спротивно портокалово
                if (cursorPosition.X >= startGamePosition.X
                    && cursorPosition.X <= startGamePosition.X + menuOptions["start game"].Width
                    && cursorPosition.Y >= startGamePosition.Y
                    && cursorPosition.Y <= startGamePosition.Y + menuOptions["start game"].Height)
                {
                    menuOptions["start game"] = readyStrings["start game hover"];
                }
                else
                {
                    menuOptions["start game"] = readyStrings["start game"];
                }
            }

            lock (this)
            {
                //ако курсорот е над опцијата обој ја плаво, во спротивно портокалово
                if (cursorPosition.X >= quitGamePosition.X
                    && cursorPosition.X <= quitGamePosition.X + menuOptions["quit game"].Width
                    && cursorPosition.Y >= quitGamePosition.Y
                    && cursorPosition.Y <= quitGamePosition.Y + menuOptions["quit game"].Height)
                {
                    menuOptions["quit game"] = readyStrings["quit game hover"];
                    if (KeyStateInfo.GetAsyncKeyState(Keys.LButton).WasPressedAfterPreviousCall
                        && KeyStateInfo.GetAsyncKeyState(Keys.LButton).IsPressed)
                    {
                        //ако е притиснат левиот клик, тогаш избрана е опцијата quit
                        return 0; //извести го framework-от да го прекине loop-ot
                    }
                }
                else
                {
                    menuOptions["quit game"] = readyStrings["quit game"];
                }
            }

            return 100;
        }

        /// <summary>
        /// Слика која ќе се прикажува како позадина на менито
        /// </summary>
        public Bitmap MenuBackground { get; private set; }

        public IGame Game { get; set; }

        public ArkanoidStateMainMenu(IGame game)
        {
            MenuBackground = null;
            this.Game = game;
            this.InitializeAlphabet();
            menuOptions = new Dictionary<string, Bitmap>();
            readyStrings = new Dictionary<string, Bitmap>();
            readyStrings.Add("start game", DrawOrangeString("start game"));
            readyStrings.Add("quit game", DrawOrangeString("quit game"));
            readyStrings.Add("start game hover", DrawBlueString("start game"));
            readyStrings.Add("quit game hover", DrawBlueString("quit game"));
            menuOptions.Add("start game", DrawOrangeString("start game"));
            menuOptions.Add("quit game", DrawOrangeString("quit game"));
            quitGamePosition = startGamePosition = new Point(0, 0);
        }

        public bool IsTimesynchronizationImportant
        {
            get { return false; }
        }


        public void OnResolutionChanged(int newWidth, int newHeight)
        {
            if (MenuBackground == null)
            {
                BitmapExtensionMethods.LoadBitmapIntoMainMemory("\\Resources\\Images\\background.jpg",
                    newWidth, newHeight, "MenuBackground");
                MenuBackground = BitmapExtensionMethods.GetBitmapFromMainMemory("MenuBackground");
            }
            else if (MenuBackground.Height != newHeight || MenuBackground.Width != newWidth)
            {
                MenuBackground = BitmapExtensionMethods.GetBitmapFromMainMemory("MenuBackground", newWidth, newHeight);
            }
        }

        private void InitializeAlphabet()
        {
            blueAlphabet = new Dictionary<char, Bitmap>();
            orangeAlphabet = new Dictionary<char, Bitmap>();

            Bitmap bitmapBlueAlphabet = BitmapExtensionMethods.GetBitmapFromFile("\\Resources\\Images\\alphabet_blue.png",
                480, 25);
            Bitmap bitmapOrangeAlphabet = BitmapExtensionMethods.GetBitmapFromFile("\\Resources\\Images\\alphabet_orange.png",
                480, 25);

            int offsetIncrement = bitmapBlueAlphabet.Width / 26;
            int offset = 0;
            for (char i = 'A'; i <= 'Z'; i++)
            {
                orangeAlphabet.Add(i, bitmapOrangeAlphabet.Clone(new Rectangle(offset + 1, 0, offsetIncrement - 2,
                    bitmapOrangeAlphabet.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb));
                blueAlphabet.Add(i, bitmapBlueAlphabet.Clone(new Rectangle(offset + 1, 0, offsetIncrement - 2,
                    bitmapBlueAlphabet.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb));
                offset += offsetIncrement;
            }
        }

        private Bitmap DrawOrangeString(string str)
        {
            int charWidth = orangeAlphabet['A'].Width;
            int charHeight = orangeAlphabet['A'].Height;
            Bitmap bitmapString = new Bitmap(charWidth * str.Length, charHeight);

            Graphics g = Graphics.FromImage(bitmapString);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            int offset = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    offset += charWidth;
                }
                else
                {
                    Bitmap temp = orangeAlphabet[char.ToUpper(str[i])];
                    g.DrawImage(temp, offset, 0);
                    offset += charWidth;
                }
            }

            return bitmapString;
        }

        private Bitmap DrawBlueString(string str)
        {
            int charWidth = blueAlphabet['A'].Width;
            int charHeight = blueAlphabet['A'].Height;
            Bitmap bitmapString = new Bitmap(charWidth * str.Length, charHeight);

            Graphics g = Graphics.FromImage(bitmapString);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            int offset = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    offset += charWidth;
                }
                else
                {
                    Bitmap temp = blueAlphabet[char.ToUpper(str[i])];
                    g.DrawImage(temp, offset, 0);
                    offset += charWidth;
                }
            }

            return bitmapString;
        }
    }

    public class GameArkanoid : IGame
    {
        //Стартна позиција на играчот (1750, 2010);

        public string Name { get; private set; }

        /// <summary>
        /// Позиција на курсорот релативно во однос на панелот.
        /// Координатите се реалните од екранот, не оние од играта.
        /// </summary>
        public Point CursorRelativeToPanel { get; private set; }

        public IGameState GameState { get; set; }

        public void OnDraw(Graphics graphics, int frameWidth, int frameHeight)
        {
            this.GameState.OnDraw(graphics, frameWidth, frameHeight);
        }

        //во милисекунди
        private int gameUpdatePeriod;

        /// <summary>
        /// Креира нова игра и ја поставува во почетна состојба initialState.
        /// gameUpdatePeriod е во милисекунди и означува на колкав период
        /// се повикува методот update()
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="gameUpdatePeriod"></param>
        public GameArkanoid(IGameState initialState, int gameUpdatePeriod)
        {
            CursorRelativeToPanel = Cursor.Position;
            this.gameUpdatePeriod = gameUpdatePeriod;
            GameState = initialState;
            Name = "Arkanoid";
            VirtualGameWidth = 3840;
            VirtualGameHeight = 2160;
        }

        public int OnUpdate(Point cursorPanelCoordinates)
        {
            CursorRelativeToPanel = cursorPanelCoordinates;
            ElapsedTime++; //поминал еден период
            return GameState.OnUpdate(null, ElapsedTime);
        }

        /// <summary>
        /// Овде се менуваат сите слики што се претходно биле 
        /// вчитани во главната меморија, но во друга резолуција.
        /// Бидејќи прозорецот има друга резолуција, мора и сликите
        /// да се вчитаат во друга резолуција.
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        public void OnResolutionChanged(int newWidth, int newHeight)
        {
            this.GameState.OnResolutionChanged(newWidth, newHeight);
        }

        public long ElapsedTime { get; set; }


        /* Играта ќе има посебни единици за должина, посебни просторот за цртање.
         * Играта е правоаголник со димензии 3840 x 2160 кои се преведуваат во 
         * координати за прикажување на екран во зависност од димензиите на прозорецот.
         */
        public int VirtualGameWidth { get; private set; }

        public int VirtualGameHeight { get; private set; }

        /// <summary>
        /// Пример во главното мени не е важно дали ќе задоцни времето во играта. Пример корисникот
        /// отворил нова форма од главното мени, но формата е отворена од методот update, па 
        /// целото време во кое што е отворена формата ќе се смета за еден период. Со ова
        /// property му кажуваме на главниот loop дека нема потреба да повикува update 10000 
        /// пати (ќе има голем лаг во овој случај) бидејќи во спротивно сликата ќе биде
        /// замрзната подолго време. Од друга страна додека се игра многу е важно
        /// времето да биде синхронизирано, инаку ќе дојде до различна брзина на анимациите
        /// на различни компјутери. Ако хардверот не може да ги изврши сите пресметки
        /// во дадениот период тогаш не се рендерира и ќе дојде до секцање во играта,
        /// но бројот на поминати периоди најверојатно ќе остане ист.
        /// </summary>
        public bool IsTimesynchronizationImportant
        {
            get
            {
                return GameState.IsTimesynchronizationImportant;
            }
        }
    }
}
