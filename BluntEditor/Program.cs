/*
 * Проект псевдо-графического текстового редактора
 * с минимальным функционалом
 * Предусловие: нужно установить ncurses ( команда : "sudo apt-get install libncurses5-dev" )
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace BluntEditor
{
    class MainClass
    {
        static Curses Curses;
        static int posX = 0;
        static int posY = 1;
        static List<Dictionary<int, char>> Content = new List<Dictionary<int, char>>();
        static int maxY = 0;
        static int maxX = 0;
        static int currentLineIndex = 0;
        static Dictionary<int, char> currentString = new Dictionary<int, char>();
        static char Backspace = '\0';
        public static void GoToNewLine(char ch)
        {
            currentString.Add(posX, '\r');
            posX++;
            currentString.Add(posX, '\n');
            posY++;
            posX = 0;
            Content.Add(currentString);
            if ((maxY - 1) == posY)
            {
                Curses.DeleteFirstLine();
                posY--;
            }
            Curses.Print(posX, posY, ch.ToString());
            currentLineIndex++;
            Curses.Refresh();
        }
        public static void AddCharToCurrentLine(char ch)
        {
            currentString.Add(posX, ch);
            Curses.Print(posX, posY, ch.ToString());
            posX++;
            Curses.Refresh();
        }
        public static bool IsNewLine
        {
            get
            {
                return posX == 0;
            }
        }
        public static bool IsLineOver
        {
            get
            {
                return posX >= maxX;
            }
        }
        public static void UpdateSizes()
        {
            maxY = Console.WindowHeight;
            maxX = Console.WindowWidth;
        }
        public static void Main(string[] args)
        {

            //string filename = null;
            //try
            //{
            //    filename = args[0];
            //    using (StreamReader reader = File.OpenText(filename))
            //    {
            //        string content = reader.ReadToEnd();
            //        var strings = content.Split(new string[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries);
            //        foreach (var str in strings)
            //        {
            //            Console.WriteLine(str);
            //        }
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Console.Error.WriteLine(ex);
            //}
            Curses = new Curses();
            Curses.Print(0, 0, "Введите текст:");
            Curses.Move(0, 1);
            Curses.Refresh();
            bool exit = false;
            while (!exit)
            {
                char ch = Console.ReadKey(true).KeyChar;
                UpdateSizes();
                if (ch != (char)ConsoleKey.Escape)
                {
                    if (IsNewLine)
                    {
                        currentString = new Dictionary<int, char>();
                    }
                    if ((ch == (char)ConsoleKey.Enter) || (IsLineOver))
                    {
                        GoToNewLine(ch);
                    }

                    if (ch == Backspace)
                    {
                        if(IsNewLine)
                        {
                            posY--;
                            posX = maxX;
                            Curses.Move(posX, posY);
                            Curses.Print(posX, posY, " ");
                            currentString.Remove(currentString.Keys.Count - 1);
                            Curses.Refresh();
                        }
                        else
                        {
                            Curses.Print(posX - 1, posY, " ");
                            posX--;
                            Curses.Move(posX, posY);
                            currentString.Remove(currentString.Keys.Count - 1);
                            Curses.Refresh();
                        }
                    }
                    else
                    {
                        AddCharToCurrentLine(ch);
                    }
                }
                else
                {//нажали Esc
                    if (posX != 0)
                    {
                        Content.Add(currentString);
                    }
                    exit = true;
                }
            }
            Curses = null;
            Console.Clear();
            Console.WriteLine("Result :\r");
            foreach (var str in Content)
            {
                foreach(var ch in str)
                {
                    Console.Write(ch.Value);
                }
            }
            Console.ReadLine();
        }
    }
}