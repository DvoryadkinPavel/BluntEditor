using System;
using System.Runtime.InteropServices;

namespace BluntEditor
{
    public class Curses
    {
        const string NCurses = "libncursesw.so.5";
        private IntPtr window;
        /// <summary>
        /// Initscr this instance.
        /// </summary>
        /// <returns>The initscr.</returns>
        [DllImport(NCurses)]
        private extern static IntPtr initscr();
        public Curses()
        {
            window = initscr();
        }
        /// <summary>
        /// Endwin this instance.
        /// </summary>
        /// <returns>The endwin.</returns>
        [DllImport(NCurses)]
        private extern static int endwin();
        ~Curses()
        {
            int result = endwin();
        }
        /// <summary>
        /// Mvwprintw the specified window, y, x and message.
        /// </summary>
        /// <returns>The mvwprintw.</returns>
        /// <param name="window">Window.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="message">Message.</param>
        [DllImport(NCurses)]
        private extern static int mvwprintw(IntPtr window, int y, int x, string message);
        /// <summary>
        /// Print the specified x, y and message.
        /// </summary>
        /// <returns>The print.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="message">Message.</param>
        public int Print(int x, int y, string message)
        {
            return mvwprintw(window, y, x, message);
        }
        /// <summary>
        /// Refresh the specified window.
        /// </summary>
        /// <returns>The refresh.</returns>
        /// <param name="window">Window.</param>
        [DllImport(NCurses)]
        private extern static int refresh(IntPtr window);
        /// <summary>
        /// Refresh this instance.
        /// </summary>
        /// <returns>The refresh.</returns>
        public int Refresh()
        {
            return refresh(window);
        }
        /// <summary>
        /// Wgetch the specified window.
        /// </summary>
        /// <returns>The wgetch.</returns>
        /// <param name="window">Window.</param>
        [DllImport(NCurses)]
        private extern static int wgetch(IntPtr window);
        /// <summary>
        /// Gets the char.
        /// </summary>
        /// <returns>The char.</returns>
        public int GetChar()
        {
            return wgetch(window);
        }
        /// <summary>
        /// Keypad the specified window and bf.
        /// </summary>
        /// <returns>The keypad.</returns>
        /// <param name="window">Window.</param>
        /// <param name="bf">If set to <c>true</c> bf.</param>
        [DllImport(NCurses)]
        private extern static int keypad(IntPtr window, bool bf);
        /// <summary>
        /// Keies the pad.
        /// </summary>
        /// <returns>The pad.</returns>
        /// <param name="bf">If set to <c>true</c> bf.</param>
        public int KeyPad(bool bf)
        {
            return keypad(window,bf);
        }
        /// <summary>
        /// Noecho this instance.
        /// </summary>
        /// <returns>The noecho.</returns>
        [DllImport(NCurses)]
        private extern static int noecho();
        /// <summary>
        /// Nos the echo.
        /// </summary>
        /// <returns>The echo.</returns>
        public int NoEcho()
        {
            return noecho();
        }
        /// <summary>
        /// Move the specified y and x.
        /// </summary>
        /// <returns>The move.</returns>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        [DllImport(NCurses)]
        private extern static int move(int y, int x);
        /// <summary>
        /// Move the specified x and y.
        /// </summary>
        /// <returns>The move.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public int Move(int x, int y)
        {
            return move(y, x);
        }
        /// <summary>
        /// Insertln this instance.
        /// </summary>
        /// <returns>The insertln.</returns>
        [DllImport(NCurses)]
        private extern static int insertln();
        /// <summary>
        /// Inserts the line.
        /// </summary>
        /// <returns>The line.</returns>
        public int InsertLine()
        {
            return insertln();
        }
        /// <summary>
        /// Deleteln this instance.
        /// </summary>
        /// <returns>The deleteln.</returns>
        [DllImport(NCurses)]
        private extern static int deleteln();
        /// <summary>
        /// Deletes the line.
        /// </summary>
        /// <returns>The line.</returns>
        public int DeleteLine()
        {
            return deleteln();
        }

        public void DeleteFirstLine()
        {
            Move(0, 0);
            DeleteLine();
        }
    }
}
