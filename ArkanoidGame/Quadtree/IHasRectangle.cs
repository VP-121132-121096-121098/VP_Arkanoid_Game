using System.Drawing;

namespace ArkanoidGame.Interfaces
{
    /// <summary>
    /// An interface that defines and object with a rectangle.
    /// </summary>
    public interface IHasRectangle
    {
        RectangleF Rectangle { get; }
    }

}
