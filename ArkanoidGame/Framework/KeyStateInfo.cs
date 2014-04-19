using ArkanoidGame.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArkanoidGame.Framework
{
    public class KeyStateInfo
    {
        static KeyStateInfo()
        {
            mostSignificantBit = -127;
            leastSignificantBit = 1;
        }

        private static readonly short mostSignificantBit; //за логичко И
        private static readonly short leastSignificantBit; //за логичко И

        /// <summary>
        /// Microsoft documentation:
        /// Retrieves the status of the specified virtual key.
        /// The status specifies whether the key is up, down, or toggled 
        /// (on, off—alternating each time the key is pressed).
        ///
        /// The return value specifies the status of the specified virtual key, as follows:
        /// If the high-order bit is 1, the key is down; otherwise, it is up.
        /// If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key,
        /// is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0.
        /// A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled,
        /// and off when the key is untoggled. 
        /// </summary>
        /// <param name="key">A virtual key.</param>
        /// <returns>The return value specifies the status of the specified virtual key.</returns>
        public static IKeyState GetKeyState(Keys key)
        {
            short bits = Win32KeyState.GetKeyState((int)key);
            return new KeyState(key, (bits & mostSignificantBit) != 0,
                (bits & leastSignificantBit) != 0);
        }

        /// <summary>
        /// Microsoft documentation:
        /// Determines whether a key is up or down at the 
        /// time the function is called, and whether the key was
        /// pressed after a previous call to GetAsyncKeyState.
        /// If the function succeeds, the return value specifies whether the key was pressed since 
        /// the last call to GetAsyncKeyState, and whether the key is currently up or down. 
        /// If the most significant bit is set, the key is down, and if the least significant 
        /// bit is set, the key was pressed after the previous call to GetAsyncKeyState.
        /// 
        /// Although the least significant bit of the return value indicates whether 
        /// the key has been pressed since the last query, due to the pre-emptive multitasking 
        /// nature of Windows, another application can call GetAsyncKeyState and receive the 
        /// "recently pressed" bit instead of your application. The behavior of the least 
        /// significant bit of the return value is retained strictly for compatibility with 
        /// 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        /// The return value is zero for the following cases:
        /// The current desktop is not the active desktop
        /// The foreground thread belongs to another process and the 
        /// desktop does not allow the hook or the journal record.
        /// </summary>
        /// <param name="key">The virtual-key code.</param>
        public static IKeyState GetAsyncKeyState(Keys key)
        {
            short bits = Win32KeyState.GetAsyncKeyState((int)key);
            return new AsyncKeyState(key, (bits & mostSignificantBit) != 0,
                (bits & leastSignificantBit) != 0);
        }

        private static class Win32KeyState
        {
            /*---------------------------------- Win32 API ------------------------------------------*/

            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(int vKey);

            [DllImport("user32.dll")]
            public static extern short GetKeyState(int nVirtKey);
        }

        /// <summary>
        /// Содржи информација дали копчето е притиснато или не,
        /// или дали е вклучено (пример Caps Lock али е ON или OFF).
        /// </summary>
        private class KeyState : IKeyState
        {
            public KeyState(Keys key, bool isPressed, bool isToggled)
            {
                this.Key = key;
                this.IsPressed = isPressed;
                this.IsToggled = isToggled;
            }

            /// <summary>
            /// Копче за кое што се одредува статусот
            /// </summary>
            public Keys Key { get; private set; }

            /// <summary>
            /// Дали копчето е притиснато?
            /// </summary>
            public bool IsPressed { get; private set; }

            /// <summary>
            /// Дали копчето е вклучено? (пример Caps Lock, Num Lock и слични
            /// копчиња кои може да имаат статус ON/OFF)
            /// </summary>
            public bool IsToggled { get; private set; }

            /// <summary>
            /// Својството е невалидно за оваа класа
            /// </summary>
            public bool WasPressedAfterPreviousCall
            {
                get { throw new InvalidOperationException(); }
            }

            public override string ToString()
            {
                return string.Format("Key: {0}, IsPressed: {1}, IsToggled {2}",
                    Key, IsPressed, IsToggled);
            }
        }

        /// <summary>
        /// Содржи информација дали некое копче е притиснато или не,
        /// и дали било притиснато после претходниот повик на GetAsyncKeyState
        /// </summary>
        private class AsyncKeyState : IKeyState
        {
            public AsyncKeyState(Keys key, bool isPressed, bool wasPressedAfterLastCall)
            {
                this.Key = key;
                this.IsPressed = isPressed;
                this.WasPressedAfterPreviousCall = wasPressedAfterLastCall;
            }

            /// <summary>
            /// Копче за кое што се одредува статусот
            /// </summary>
            public Keys Key { get; private set; }

            /// <summary>
            /// Дали копчето е притиснато?
            /// </summary>
            public bool IsPressed { get; private set; }

            /// <summary>
            /// Својството е невалидно за оваа класа
            /// </summary>
            public bool IsToggled
            {
                get { throw new InvalidOperationException(); }
            }

            /// <summary>
            /// Дали копчето било притиснато после последната проверка?
            /// Забелешка: Точноста на оваа информација зависи од функцијата
            /// GetAsyncKeyState од Win32 API-то.
            /// </summary>
            public bool WasPressedAfterPreviousCall { get; private set; }

            public override string ToString()
            {
                return string.Format("Key: {0}, IsPressed: {1}, WasPressedAfterPreviousCall {2}",
                    Key, IsPressed, WasPressedAfterPreviousCall);
            }
        }
    }
}
