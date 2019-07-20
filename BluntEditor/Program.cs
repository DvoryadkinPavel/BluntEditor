/*
 * Проект псевдо-графического текстового редактора
 * с минимальным функционалом
 * Предусловие: нужно установить ncurses ( команда : "sudo apt-get install libncurses5-dev" )
 * TODO:
 * прокрутка стрелками
 * Enter для положения курсура не в конце строки
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
        static string _fileName = "";
        static string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(_fileName)) return "НОВЫЙ";
                else return _fileName;
            }
        }
        static Curses Curses;
        static int posX = 0;
        static int posY = 1;
        static ArrayList Content = new ArrayList();
        static int maxY = 0;
        static int maxX = 0;
        static string currentString = "";
        static int stringIndex = 0;
        static int previousLinesCount = 0;
        static int previousColumnCount = 0;
        static bool exit = false;
        public static void GoToNewLine()
        {
            posY++;
            SaveLine(stringIndex);
            stringIndex++;
            currentString = "";
            posX = 0;
            if ((maxY - 1) == posY)
            {
                previousLinesCount++;
                Curses.DeleteFirstLine();
                posY--;
                ShowInfo();
                Curses.Print(posX, posY, EmptyString);
                Curses.Move(posX, posY);
                Curses.Refresh();
            }
            else
            {
                RefreshLines();
            }
        }
        public static void MoveColumns(int value = 1)
        {
            RefreshLines(value);
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
                for (int i = 0; i <= maxX; i++) emptyStr += " ";
                return emptyStr;
            }
        }
        public static string SeparatorString
        {
            get
            {
                var emptyStr = "";
                for (int i = 0; i < maxX; i++) emptyStr += "-";
                return emptyStr;
            }
        }
        public static void ShowInfo()
        {
            Curses.Print(0, 0, EmptyString);
            Curses.Print(0, maxY - 1, EmptyString);
            Curses.Print(0, 0, $"Файл : {FileName}");
            Curses.Print(0, maxY - 1, $"Строка : {stringIndex}");
        }
        public static void RefreshLine()
        {
            Curses.Print(0, posY, EmptyString);
            Curses.Print(0, posY, currentString);
            Curses.Move(posX, posY);
            Curses.Refresh();
            SaveLine(stringIndex);
        }
        public static void RefreshLines(int from = 0)
        {
            for (int index = 1; index < maxY - 1; index++)
            {
                if (index > Content.Count)
                {
                    Curses.Print(0, index, EmptyString);
                }
                else
                {
                    var item = (string)Content[index + previousLinesCount - 1];

                    if (String.IsNullOrEmpty(item))
                    {
                        Curses.Print(0, index, EmptyString);
                    }
                    else
                    {
                        if(item.Length <= from)
                        {
                            item = "";
                        }
                        else
                        {
                            item = item.Substring(from);
                        }
                        Curses.Print(0, index, EmptyString);
                        Curses.Print(0, index, item); 
                    }
                }
            }
            ShowInfo();
            Curses.Move(posX, posY);
            Curses.Refresh();
        }
        public static void AddCharToCurrentLine(char ch,bool withoutScroll = true)
        {
            if((posX+previousColumnCount) == currentString.Length)
            {
                currentString += ch;
                if(withoutScroll) Curses.Print(posX, posY, ch.ToString());
                if(withoutScroll) posX++;
            }
            else
            {
                var list = ToCharList(currentString);
                list.Insert(posX + previousColumnCount, ch);
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
                    if (Content.Count > stringIndex + 1)
                    {
                        Curses.DeleteLine();
                        Content.RemoveAt(stringIndex);
                        currentString = (string)Content[stringIndex];
                        RefreshLines();
                    }
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
        public static void ReadFile()
        {
            using (StreamReader reader = File.OpenText(_fileName))
            {
                string content = reader.ReadToEnd();
                var strings = content.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var str in strings)
                {
                    var current = str.Replace("\n", String.Empty);
                    Content.Add(current);
                }
            }
        }
        public static void SaveFile()
        {
            var text = "";
            foreach (var str in Content)
            {
                text += str + "\r";
            }
            if (String.IsNullOrEmpty(_fileName))
            {
                Console.Clear();
                Console.Write("Введите имя файла : ");
                _fileName = Console.ReadLine();
            }
            using (StreamWriter sw = new StreamWriter(_fileName, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(text);
            }
        }
        public static void Main(string[] args)
        {
            if(args.Length!=0)
            {
                _fileName = args[0];
                ReadFile();
                currentString = (string)Content[0];
            }
            _fileName = "test.txt";//для отладки, потом убрать
            ReadFile();//для отладки, потом убрать
            currentString = (string)Content[0];//для отладки, потом убрать
            Curses = new Curses();
            Curses.NoEcho();
            Curses.KeyPad(true);
            UpdateSizes();
            RefreshLines();
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
                            SaveLine(stringIndex);
                            posY--; stringIndex--;
                            ShowInfo();
                            currentString = (string)Content[stringIndex];
                            if (posX > currentString.Length) posX = currentString.Length;
                            Curses.Move(posX, posY);
                            Curses.Refresh();
                        }
                        else
                        {
                            if(stringIndex!=0)
                            {
                                SaveLine(stringIndex);
                                stringIndex--;
                                previousLinesCount--;
                                ShowInfo();
                                currentString = (string)Content[stringIndex];
                                RefreshLines();
                            }
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (posY < Content.Count - previousLinesCount)
                        {
                            if(posY < maxY - 2)
                            {
                                SaveLine(stringIndex);
                                posY++; stringIndex++;
                                ShowInfo();
                                currentString = (string)Content[stringIndex];
                                if (posX > currentString.Length) posX = currentString.Length;
                                Curses.Move(posX, posY);
                                Curses.Refresh();
                            }
                            else
                            {
                                SaveLine(stringIndex);
                                stringIndex++;
                                previousLinesCount++;
                                currentString = (string)Content[stringIndex];
                                if (posX > currentString.Length) posX = currentString.Length;
                                RefreshLines();
                            }
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
                            AddCharToCurrentLine(ck.KeyChar,false);
                            previousColumnCount++;
                            RefreshLines(previousColumnCount);
                        }
                        break;
                }
            }
            Curses = null;
            SaveFile();
        }
    }
}