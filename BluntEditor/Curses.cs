using System;
using System.Runtime.InteropServices;
using System.Configuration;

namespace BluntEditor
{
    public class Curses
    {
        /// <summary>
        /// указатель на окно терминала
        /// </summary>
        private IntPtr window;
        [DllImport("libpdcursesw.dll", CharSet = CharSet.Unicode)]
        private extern static IntPtr initscr();
        /// <summary>
        /// конструктор класса библиотеки pdcurses
        /// </summary>
        public Curses()
        {
            window = initscr();
            //Console.SetWindowPosition(0, 1);
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int endwin();
        /// <summary>
        /// Деструктор класса библиотеки pdcurses
        /// </summary>
        ~Curses()
        {
            int result = endwin();
        }
        /// <summary>
        /// Вывод сообщения по координатам
        /// </summary>
        public void Print(int x, int y, string message)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(message);            
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int refresh(IntPtr window);
        /// <summary>
        /// Обновить экран
        /// </summary>
        public int Refresh()
        {
            return refresh(window);
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int getchar();
        /// <summary>
        /// Возвращает нажатую клавишу
        /// </summary>
        public int GetChar()
        {
            return getchar();
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int keypad(IntPtr window, bool bf);
        /// <summary>
        /// Включение/выключение функциональных клавиш
        /// </summary>
        public int KeyPad(bool bf)
        {
            return keypad(window,bf);
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int noecho();
        /// <summary>
        /// Не выводить нажатые клавиши на экран
        /// </summary>
        public int NoEcho()
        {
            return noecho();
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int move(int y, int x);
        /// <summary>
        /// Переместить курсор по координатам
        /// </summary>
        public void Move(int x, int y)
        {
            move(y, x);//так как pdcurses проинициализирована нужно синхронизировать отображение курсора
            Console.SetCursorPosition(x, y+1);
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int insertln();
        /// <summary>
        /// Вставить строку
        /// </summary>
        public int InsertLine()
        {
            return insertln();
        }
        [DllImport("libpdcursesw.dll")]
        private extern static int deleteln();
        /// <summary>
        /// Удалить строку
        /// </summary>
        public int DeleteLine()
        {
            return deleteln();
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
