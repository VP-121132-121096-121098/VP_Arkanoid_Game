using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ArkanoidGame.Interfaces
{
    public interface IGameState
    {
        /// <summary>
        /// Функција која дефинира што се случува при повик на Draw()
        /// во состојбата во која моментално се наоѓа играта
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
        /// gameObjects е колекција од сите објекти во играта, корисно за алгоритми од типот 
        /// на детекција на судир.
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="gameElapsedTime"></param>
        /// <returns></returns>
        int OnUpdate(IEnumerable<IGameObject> gameObjects);

        /// <summary>
        /// Референца кон играта
        /// </summary>
        IGame Game { get; }

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
        /// Овде се менуваат сите слики што се претходно биле 
        /// вчитани во главната меморија, но во друга резолуција.
        /// Бидејќи прозорецот има друга резолуција, мора и сликите
        /// да се вчитаат во друга резолуција.
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        void OnResolutionChanged(int newWidth, int newHeight);
    }
}
