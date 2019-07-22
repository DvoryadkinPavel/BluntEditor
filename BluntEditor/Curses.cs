using System;
using System.Runtime.InteropServices;
using System.Configuration;

namespace BluntEditor
{
    public class Curses
    {
        /// <summary>
        /// файл ncurses
        /// </summary>
        const string NCurses = "libncursesw.so.5";
        /// <summary>
        /// указатель на окно терминала
        /// </summary>
        private IntPtr window;
        [DllImport(NCurses)]
        private extern static IntPtr initscr();
        /// <summary>
        /// конструктор класса библиотеки ncurses
        /// </summary>
        public Curses()
        {
            window = initscr();
        }
        [DllImport(NCurses)]
        private extern static int endwin();
        /// <summary>
        /// Деструктор класса библиотеки ncurses
        /// </summary>
        ~Curses()
        {
            int result = endwin();
        }
        [DllImport(NCurses)]
        private extern static int mvwprintw(IntPtr window, int y, int x, string message);
        /// <summary>
        /// Вывод сообщения по координатам
        /// </summary>
        public int Print(int x, int y, string message)
        {
            return mvwprintw(window, y, x, message);
        }
        [DllImport(NCurses)]
        private extern static int refresh(IntPtr window);
        /// <summary>
        /// Обновить экран
        /// </summary>
        public int Refresh()
        {
            return refresh(window);
        }
        [DllImport(NCurses)]
        private extern static int getchar();
        /// <summary>
        /// Возвращает нажатую клавишу
        /// </summary>
        public int GetChar()
        {
            return getchar();
        }
        [DllImport(NCurses)]
        private extern static int keypad(IntPtr window, bool bf);
        /// <summary>
        /// Включение/выключение функциональных клавиш
        /// </summary>
        public int KeyPad(bool bf)
        {
            return keypad(window,bf);
        }
        [DllImport(NCurses)]
        private extern static int noecho();
        /// <summary>
        /// Не выводить нажатые клавиши на экран
        /// </summary>
        public int NoEcho()
        {
            return noecho();
        }
        [DllImport(NCurses)]
        private extern static int move(int y, int x);
        /// <summary>
        /// Переместить курсор по координатам
        /// </summary>
        public int Move(int x, int y)
        {
            return move(y, x);
        }
        [DllImport(NCurses)]
        private extern static int insertln();
        /// <summary>
        /// Вставить строку
        /// </summary>
        public int InsertLine()
        {
            return insertln();
        }
        [DllImport(NCurses)]
        private extern static int deleteln();
        /// <summary>
        /// Удалить строку
        /// </summary>
        public int DeleteLine()
        {
            return deleteln();
        }
        [DllImport(NCurses)]
        private extern static int start_color();
        /// <summary>
        /// Включить цвета
        /// </summary>
        public int StartColor()
        {
            return start_color();
        }
        [DllImport(NCurses)]
        private extern static int init_pair(short index, short back, short fore);
        /// <summary>
        /// Инициализация пары цветов
        /// </summary>
        public int InitPair(short index, short fore, short back)
        {
            return init_pair(index, fore, back);
        }
        [DllImport(NCurses)]
        private extern static int COLOR_PAIR(int n);
        /// <summary>
        /// Возвращает пару цветов
        /// </summary>
        public int ColorPair(int n)
        {
            return COLOR_PAIR(n);
        }
        [DllImport(NCurses)]
        private extern static int attron(int n);
        /// <summary>
        /// Применить цветовую пару
        /// </summary>
        public int AttrOn(int n)
        {
            return attron(n);
        }
        [DllImport(NCurses)]
        private extern static int attroff(int n);
        /// <summary>
        /// Отменить цветовую пару
        /// </summary>
        public int AttrOff(int n)
        {
            return attroff(n);
        }
        /// <summary>
        /// Удалить первую строку экрана
        /// </summary>
        public void DeleteFirstLine()
        {
            Move(0, 0);
            DeleteLine();
        }
    }
}
