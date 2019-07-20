/*
 * Проект псевдо-графического текстового редактора
 * с минимальным функционалом
 * Предусловие: нужно установить ncurses ( команда : "sudo apt-get install libncurses5-dev" )
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Collections;


namespace BluntEditor
{
    class MainClass
    {
        static Curses Curses;
        static int posX = 0;
        static int posY = 1;
        static ArrayList Content = new ArrayList();
        static int maxY = 0;
        static int maxX = 0;
        static string currentString = "";
        static bool exit = false;
        public static void GoToNewLine()
        {
            posY++;
            currentString = "";
            posX = 0;
            Content.Insert(posY - 1, currentString);
            if ((maxY - 1) == posY)
            {
                Curses.DeleteFirstLine();
                posY--;
            }
            RefreshLines();
        }
        public static List<char> ToCharList(string str)
        {
            var arr = currentString.ToCharArray();
            var list = new List<char>();
            foreach (var c in arr) list.Add(c);
            return list;
        }
        public static string CharListToString(List<char> list)
        {
            var str = "";
            foreach (var c in list) str+=c;
            return str;
        }
        public static string EmptyString
        {
            get
            {
                var emptyStr = "";
                for (int i = 0; i < maxX; i++) emptyStr += " ";
                return emptyStr;
            }
        }
        public static void RefreshLine()
        {

            Curses.Print(0, posY, EmptyString);
            Curses.Print(0, posY, currentString);
            Curses.Move(posX, posY);
            Curses.Refresh();
            SaveLine(posY - 1);
        }
        public static void RefreshLines()
        { 
            for(int index=1;index < maxY;index++)
            {
                if(index > Content.Count)
                {
                    Curses.Print(0, index, EmptyString);
                }
                else
                {
                    if(String.IsNullOrEmpty((string)Content[index - 1]))
                    {
                        Curses.Print(0, index, EmptyString);
                    }
                    else Curses.Print(0, index, (string)Content[index - 1]);
                }
            }
            Curses.Move(posX, posY);
            Curses.Refresh();
        }
        public static void AddCharToCurrentLine(char ch)
        {
            if(posX == currentString.Length)
            {
                currentString += ch;
                Curses.Print(posX, posY, ch.ToString());
                posX++;
            }
            else
            {
                var list = ToCharList(currentString);
                list.Insert(posX, ch);
                currentString = CharListToString(list);

            }
            RefreshLine();
        }
        public static bool IsNewLine
        {
            get
            {
                return String.IsNullOrEmpty(currentString);
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
            maxX = Console.WindowWidth-1;
        }
        public static void BackspaceEvent()
        {
            if (!IsNewLine)
            {
                posX--;
                Curses.Print(posX, posY, " ");
                Curses.Move(posX, posY);
                var list = ToCharList(currentString);
                list.RemoveAt(posX);
                currentString = CharListToString(list);
                RefreshLine();
            }
        }
        public static void DeleteEvent()
        {
            if(Content.Count!=0)
            {
                if (String.IsNullOrEmpty(currentString))
                {
                    Curses.DeleteLine();
                    Content.RemoveAt(posY - 1);
                    currentString = (string)Content[posY - 1];
                    RefreshLines();
                }
                else
                {
                    if (!(posX >= currentString.Length))
                    {
                        var list = ToCharList(currentString);
                        list.RemoveAt(posX);
                        currentString = CharListToString(list);
                        RefreshLine();
                    }
                }
            }
        }
        public static void EscapeEvent()
        {
            exit = true;
        }
        public static void SaveLine(int index)
        {
            if(index >= Content.Count)
            {
                Content.Add(currentString);
            }
            else Content[index] = currentString;
        }
        public static void Main(/*string[] args*/)
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
            Curses.NoEcho();
            Curses.KeyPad(true);
            Curses.Print(0, 0, "Введите текст:");
            Curses.Move(0, 1);
            Curses.Refresh();
            while (!exit)
            {
                var ck = Console.ReadKey(true);
                UpdateSizes();
                switch(ck.Key)
                {
                    case ConsoleKey.PageUp:
                    case ConsoleKey.PageDown:
                    case ConsoleKey.Insert:
                        break;
                    case ConsoleKey.Home:
                        Curses.Move(0, posY);
                        posX = 0;
                        Curses.Refresh();
                        break;
                    case ConsoleKey.End:
                        posX = currentString.Length;
                        Curses.Move(posX, posY);
                        Curses.Refresh();
                        break;
                    case ConsoleKey.UpArrow:
                        if(posY!=1)
                        {
                            if (posY > Content.Count)
                            {
                                Content.Add(currentString);
                            }
                            else
                            {
                                SaveLine(posY - 1);
                            }
                            posY--;
                            currentString = (string)Content[posY - 1];
                            if (posX > currentString.Length) posX = currentString.Length;
                            Curses.Move(posX, posY);
                            Curses.Refresh();
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (posY < Content.Count)
                        {
                            SaveLine(posY - 1);
                            posY++;
                            currentString = (string)Content[posY - 1];
                            if (posX > currentString.Length) posX = currentString.Length;
                            Curses.Move(posX, posY);
                            Curses.Refresh();
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if(posX > 0)
                        {
                            posX--;
                            Curses.Move(posX, posY);
                            Curses.Refresh();
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if(posX < currentString.Length)
                        {
                            posX++;
                            Curses.Move(posX, posY);
                            Curses.Refresh();
                        }
                        break;
                    case ConsoleKey.Escape:
                        EscapeEvent();
                        break;
                    case ConsoleKey.Enter:
                        GoToNewLine();
                        break;
                    case ConsoleKey.Backspace:
                        BackspaceEvent();
                        break;
                    case ConsoleKey.Delete:
                        DeleteEvent();
                        break;
                    default:
                        if (!IsLineOver)
                        {
                            AddCharToCurrentLine(ck.KeyChar);
                        }
                        else
                        {
                            GoToNewLine();
                        }
                        break;
                }
            }
            Curses = null;
            Console.Clear();
            Console.WriteLine("Result :\r");
            foreach(var str in Content)
            {
                Console.WriteLine(str+"\r");
            }
            Console.ReadKey();
        }
    }
}