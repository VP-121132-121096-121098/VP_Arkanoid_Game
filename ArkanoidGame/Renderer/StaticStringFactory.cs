using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Renderer
{
    public class StaticStringFactory
    {
        static StaticStringFactory()
        {
            LoadOrangeAlphabet();
            LoadBlueAlphabet();
        }

        private static IDictionary<char, Bitmap> blueAlphabet;
        private static IDictionary<char, Bitmap> orangeAlphabet;

        private static void LoadOrangeAlphabet()
        {
            orangeAlphabet = new Dictionary<char, Bitmap>();
            Bitmap orangeAlphabetImage = RendererCache.GetBitmapFromFile("\\Resources\\Images\\alphabet_orange.png");
            int offsetSize = orangeAlphabetImage.Width / 26;
            int offset = 0;
            for (char i = 'A'; i <= 'Z'; i++)
            {
                orangeAlphabet.Add(i, orangeAlphabetImage.Clone(new Rectangle(offset + 2, 0, offsetSize - 4,
                    orangeAlphabetImage.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb));
                offset += offsetSize;
            }

            Bitmap orangeNumbers = RendererCache.GetBitmapFromFile("\\Resources\\Images\\numbers_orange.png");

            offsetSize = orangeNumbers.Width / 10;
            offset = 0;
            for (char i = '0'; i <= '9'; i++)
            {
                orangeAlphabet.Add(i, orangeNumbers.Clone(new Rectangle(offset + 2, 0, offsetSize - 4,
                    orangeNumbers.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb));
                offset += offsetSize;
            }
        }

        public static Bitmap CreateOrangeString(String str)
        {
            int totalWidth = orangeAlphabet['A'].Width * str.Length;
            int totalHeight = orangeAlphabet['A'].Height;

            PrivateFontCollection fonts;
            FontFamily family = LoadFontFamily(System.Environment.CurrentDirectory + "\\Resources\\Fonts\\Forte.ttf",
                out fonts);
            Font theFont = new Font(family, orangeAlphabet['A'].Height - 2, FontStyle.Bold);

            Bitmap bitmap = new Bitmap(totalWidth, totalHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int offset = 0;
                int offsetSize = orangeAlphabet['A'].Width;
                for (int i = 0; i < str.Length; i++)
                {
                    Bitmap temp;
                    if (orangeAlphabet.TryGetValue(char.ToUpper(str[i]), out temp))
                    {
                        g.DrawImage(temp, offset, 0);
                    }
                    else
                    {
                        Brush brush = new LinearGradientBrush(new Point(offset, 0),
                            new Point(offset + offsetSize, totalHeight), ColorTranslator.FromHtml("#fdefc4"),
                            ColorTranslator.FromHtml("#DCB750"));
                        g.DrawString(char.ToUpper(str[i]).ToString(), theFont, brush, offset - 2, -9, StringFormat.GenericDefault);
                    }
                    offset += offsetSize;
                }
            }

            return bitmap;
        }

        public static Bitmap CreateBlueString(String str)
        {
            int totalWidth = blueAlphabet['A'].Width * str.Length;
            int totalHeight = blueAlphabet['A'].Height;

            PrivateFontCollection fonts;
            FontFamily family = LoadFontFamily(System.Environment.CurrentDirectory + "\\Resources\\Fonts\\Forte.ttf",
                out fonts);
            Font theFont = new Font(family, blueAlphabet['A'].Height - 2, FontStyle.Bold);

            Bitmap bitmap = new Bitmap(totalWidth, totalHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                int offset = 0;
                int offsetSize = blueAlphabet['A'].Width;
                for (int i = 0; i < str.Length; i++)
                {
                    Bitmap temp;
                    if (blueAlphabet.TryGetValue(char.ToUpper(str[i]), out temp))
                    {
                        g.DrawImage(temp, offset, 0);
                    }
                    else
                    {
                        Brush brush = new LinearGradientBrush(new Point(offset, 0),
                            new Point(offset + offsetSize, totalHeight), ColorTranslator.FromHtml("#5185c5"),
                            ColorTranslator.FromHtml("#022b5e"));
                        g.DrawString(char.ToUpper(str[i]).ToString(), theFont, brush, offset - 2, -9, StringFormat.GenericDefault);
                    }
                    offset += offsetSize;
                }
            }

            return bitmap;
        }

        private static FontFamily LoadFontFamily(string fileName, out PrivateFontCollection fontCollection)
        {
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(fileName);
            return fontCollection.Families[0];
        }

        private static void LoadBlueAlphabet()
        {
            blueAlphabet = new Dictionary<char, Bitmap>();
            Bitmap blueAlphabetImage = RendererCache.GetBitmapFromFile("\\Resources\\Images\\alphabet_blue.png");
            int offsetSize = blueAlphabetImage.Width / 26;
            int offset = 0;
            for (char i = 'A'; i <= 'Z'; i++)
            {
                blueAlphabet.Add(i, blueAlphabetImage.Clone(new Rectangle(offset + 2, 0, offsetSize - 4,
                    blueAlphabetImage.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb));
                offset += offsetSize;
            }

            Bitmap blueNumbers = RendererCache.GetBitmapFromFile("\\Resources\\Images\\numbers_blue.png");

            offsetSize = blueNumbers.Width / 10;
            offset = 0;
            for (char i = '0'; i <= '9'; i++)
            {
                blueAlphabet.Add(i, blueNumbers.Clone(new Rectangle(offset + 2, 0, offsetSize - 4,
                    blueNumbers.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb));
                offset += offsetSize;
            }
        }
    }
}
