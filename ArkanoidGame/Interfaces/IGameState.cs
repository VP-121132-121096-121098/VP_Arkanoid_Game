﻿using ArkanoidGame.Renderer;
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
        void OnDraw(Graphics graphics, int frameWidth, int frameHeight, bool lowSpec);

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
        int OnUpdate(IList<IGameObject> gameObjects);

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
        /// Сликите што ќе бидат рендерирани. Оваа листа содржи листи каде што секоја
        /// листа ги содржи сликите (текстурите) од еден објект. 
        /// Поинаку кажано се работи за матрица каде што редиците се објекти, 
        /// а колоните текстури од соодветниот објект. При исцртување
        /// листата ќе се третира како да е редица (FIFO). Ова може да се 
        /// искористи за однапред да се одреди редоследот на исцртување. На 
        /// пример позадината треба да се исцртува прва.
        /// На овој начин се знае и редоследот на исцртување на објектите и редоследот
        /// на исцртување на секоја слика од тој објект.
        /// </summary>
        IList<IList<GameBitmap>> BitmapsToRender { get; }
    }
}
