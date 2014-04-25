using ArkanoidGame.Framework;
using ArkanoidGame.Renderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGame
    {
        /// <summary>
        /// Позиција на курсорот изразена како координати од играта,
        /// не од прозорецот на играта.
        /// </summary>
        Point CursorIngameCoordinates { get; }

        /// <summary>
        /// Листа од сите објекти во играта.
        /// </summary>
        IList<IGameObject> GameObjects { get; }

        /// <summary>
        /// Име на играта
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Состојба во која се наоѓа играта
        /// </summary>
        IGameState GameState { get; set; }

        /// <summary>
        /// Ја повикува функцијата Draw кај секој објект од играта
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameHeight"></param>
        void OnDraw(Graphics graphics, int frameWidth, int frameHeight);
        
        /// <summary>
        /// Прави update на сите објекти во играта. Ако функцијата врати 0,
        /// тоа значи дека играта треба да се исклучи.
        /// Ако функцијата врати 100 тогаш играта не треба да се исклучи.
        /// Ако функцијата врати друг број или фрли исклучок значи настанала грешка означена
        /// со соодветниот број (код на грешка).
        /// </summary>
        /// <param name="cursorPanelCoordinates"></param>
        /// <returns></returns>
        int OnUpdate(Point cursorPanelCoordinates);

        int VirtualGameWidth { get; }
        int VirtualGameHeight { get; } //Играта има посебни единици за должина од прозорецот на кој е црта

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
        bool IsTimesynchronizationImportant { get; }

        /// <summary>
        /// Рендерер
        /// </summary>
        IGameRenderer Renderer { get; }

        /// <summary>
        /// во милисекунди
        /// </summary>
        int GameUpdatePeriod { get; set; }

        /// <summary>
        /// Дали paddle-от ќе се контролира со тастатурата или со глувчето
        /// </summary>
        bool IsControllerMouse { get; set; }

        /// <summary>
        /// Дали рендерерот е вклучен или исклучен
        /// </summary>
        bool IsRendererEnabled { get; set; }
    }
}
