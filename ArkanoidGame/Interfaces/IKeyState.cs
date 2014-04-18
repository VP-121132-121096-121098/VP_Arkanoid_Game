using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArkanoidGame.Interfaces
{

    /// <summary>
    /// Содржи информација дали копчето е притиснато или не,
    /// дали е вклучено (пример Caps Lock али е ON или OFF), 
    /// или дали било притиснато после претходната проверка.
    /// </summary>
    public interface IKeyState
    {
        /// <summary>
        /// Копче за кое што се одредува статусот
        /// </summary>
        Keys Key { get; }

        /// <summary>
        /// Дали копчето е притиснато?
        /// </summary>
        bool IsPressed { get; }

        /// <summary>
        /// Дали копчето е вклучено? (пример Caps Lock, Num Lock и слични
        /// копчиња кои може да имаат статус ON/OFF)
        /// Забелешка: Ова својство е невалидно ако информацијата е добиена со повик
        /// кон GetAsyncKeyState
        /// </summary>
        bool IsToggled { get; }

        /// <summary>
        /// Дали копчето било притиснато после последната проверка? 
        /// Забелешка: Својството е невалидно ако информацијата не е добиена
        /// со повик кон GetAsyncKeyState
        /// </summary>
        bool WasPressedAfterPreviousCall { get; }
    }
}
