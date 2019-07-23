/*
 * Проект консольного текстового редактора
 * с минимальным функционалом
 * System.Console поддерживается не полностью, поэтому реализована обертка для вызова методов Си-билиотеки ncurses
 * Предусловие: нужно установить ncurses ( команда : "sudo apt-get install libncurses5-dev" )
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace BluntEditor
{
    class MainClass
    {
        static string _fileName = "";
        /// <summary>
        /// Выводимое на экран имя файла
        /// </summary>
        static string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(_fileName)) return "НОВЫЙ";
                else return _fileName;
            }
        }
        /// <summary>
        /// Объект curses для управления выводом на экран
        /// </summary>
        static Curses Curses;
        /// <summary>
        /// Координата X курсора
        /// </summary>
        static int posX = 0;
        /// <summary>
        /// координата Y курсора
        /// </summary>
        static int posY = 1;
        /// <summary>
        /// Содержимое файла
        /// </summary>
        static ArrayList Content = new ArrayList();
        /// <summary>
        /// максимальное значение Y
        /// </summary>
        static int maxY = 0;
        /// <summary>
        /// максимальное занчение X
        /// </summary>
        static int maxX = 0;
        /// <summary>
        /// текущая строка
        /// </summary>
        static string currentString = "";
        /// <summary>
        /// индекс текущей строки
        /// </summary>
        static int stringIndex = 0;
        /// <summary>
        /// количество предшествующих строк не отображаемых на экране
        /// </summary>
        static int previousLinesCount = 0;
        /// <summary>
        /// количество предшествующих столбцов не отображаемых на экране
        /// </summary>
        static int previousColumnCount = 0;
        /// <summary>
        /// флаг выхода из программы
        /// </summary>
        static bool exit = false;

        /// <summary>
        /// перевод строки в список
        /// </summary>
        public static List<char> ToCharList(string str)
        {
            var arr = currentString.ToCharArray();
            var list = new List<char>();
            foreach (var c in arr) list.Add(c);
            return list;
        }
        /// <summary>
        /// перевод списка в строку
        /// </summary>
        public static string CharListToString(List<char> list)
        {
            var str = "";
            foreach (var c in list) str+=c;
            return str;
        }
        /// <summary>
        /// пустая строка длиной в экран
        /// </summary>
        public static string EmptyString
        {
            get
            {
                var emptyStr = "";
                for (int i = 0; i <= maxX; i++) emptyStr += " ";
                return emptyStr;
            }
        }
        /// <summary>
        /// Обновление информации о положении курсора в тексте
        /// </summary>
        public static void ShowInfo()
        {
            var symbol = posX + previousColumnCount;
            Curses.StartColor();
            Curses.InitPair(1, (short)ConsoleColor.Green, (short)ConsoleColor.Black);
            Curses.AttrOn(Curses.ColorPair(1));
            Curses.Print(0, 0, EmptyString);
            Curses.Print(0, maxY - 1, EmptyString);
            Curses.Print(0, 0, $"Файл : {FileName}");
            Curses.Print(0, maxY - 1, $"Строка : {stringIndex} Символ : {symbol}");
            Curses.AttrOff(Curses.ColorPair(1));
        }
        /// <summary>
        /// Обновление текущей строки
        /// </summary>
        public static void RefreshLine()
        {
            Curses.Print(0, posY, EmptyString);
            if(currentString.Substring(previousColumnCount).Length > maxX)
            {
                var l = currentString.Substring(previousColumnCount).Length;
                var max = maxX;
                if(l != max+1)
                {
                    Curses.Print(0, posY, currentString.Substring(previousColumnCount).Remove(maxX + 1));
                }
                else
                {
                    Curses.Print(0, posY, currentString.Substring(previousColumnCount));
                }
            }
            else
            {
                Curses.Print(0, posY, currentString.Substring(previousColumnCount));
            }
            Curses.Move(posX, posY);
            Curses.Refresh();
            SaveLine(stringIndex);
        }
        /// <summary>
        /// Обновить текст на экране с учетом положения курсора
        /// </summary>
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
                    var item = "";
                    if (Content.Count >=(index + previousLinesCount))
                    {
                        item = (string)Content[index + previousLinesCount - 1];
                    }

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
        /// <summary>
        /// Добавление символа в текущую строку
        /// </summary>
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
                posX++;
            }
            RefreshLine();
        }
        /// <summary>
        /// true если курсор на начале строки
        /// </summary>
        public static bool IsLineStart
        {
            get
            {
                return (posX + previousColumnCount) == 0;
            }
        }
        /// <summary>
        /// true если курсор на конце строки
        /// </summary>
        public static bool IsLineOver
        {
            get
            {
                return posX >= maxX;
            }
        }
        /// <summary>
        /// Обновить границы экрана
        /// </summary>
        public static void UpdateSizes()
        {
            maxY = Console.WindowHeight;
            maxX = Console.WindowWidth-1;
        }
        /// <summary>
        /// Получение содержимого файла
        /// </summary>
        public static void ReadFile()
        {
            try
            {
                using (StreamReader reader = File.OpenText(_fileName))
                {
                    string content = reader.ReadToEnd();
                    var strings = content.Split(new string[] { "\n","\0" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var str in strings)
                    {
                        var current = str.Replace("\r", String.Empty);
                        Content.Add(current);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Файл не найден");
            }
        }
        /// <summary>
        /// Сохранение файла
        /// </summary>
        public static void SaveFile()
        {
            var text = "";
            foreach (var str in Content)
            {
                if (String.IsNullOrEmpty((string)str)) text += "\r\n";
                text += str + "\r\n";
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
        #region обработчики нажатия клавиш
        /// <summary>
        /// переход на следующую строку
        /// </summary>
        public static void GoToNextLine()
        {
            posY++;
            var tile = "";
            if ((posX + previousColumnCount) < currentString.Length)
            {
                var index = posX + previousColumnCount;
                tile = currentString.Substring(posX + previousColumnCount);
                currentString = currentString.Remove(posX + previousColumnCount);
            }
            SaveLine(stringIndex);
            stringIndex++;
            currentString = tile;
            if (stringIndex <= Content.Count) { Content.Insert(stringIndex, currentString); }
            posX = 0;
            previousColumnCount = 0;
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
        /// <summary>
        /// Обработчик нажатия Backspace
        /// </summary>
        public static void BackspaceEvent()
        {
            if (!IsLineStart)
            {
                if (posX != 0)
                {
                    posX--;
                    Curses.Print(posX, posY, " ");
                    Curses.Move(posX, posY);
                    var list = ToCharList(currentString);
                    list.RemoveAt(posX + previousColumnCount);
                    currentString = CharListToString(list);
                }
                else
                {
                    previousColumnCount--;
                    RefreshLines(previousColumnCount);
                    var list = ToCharList(currentString);
                    list.RemoveAt(posX + previousColumnCount);
                    currentString = CharListToString(list);
                }
                RefreshLine();
                ShowInfo();
                Curses.Move(posX, posY);
                Curses.Refresh();
            }
            else
            {
                if (stringIndex != 0)
                {
                    UpKeyEvent();
                    EndKeyEvent();
                    DeleteEvent();
                }
            }
        }
        /// <summary>
        /// Обработчик нажатия Delete
        /// </summary>
        public static void DeleteEvent()
        {
            if (Content.Count != 0)
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
                    if (!(posX + previousColumnCount >= currentString.Length))
                    {
                        var list = ToCharList(currentString);
                        list.RemoveAt(posX + previousColumnCount);
                        currentString = CharListToString(list);
                        RefreshLine();
                    }
                    else
                    {
                        if (Content.Count > stringIndex + 1)
                        {
                            currentString += Content[stringIndex + 1];
                            Content.RemoveAt(stringIndex + 1);
                            SaveLine(stringIndex);
                            RefreshLines(previousColumnCount);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Обработчик нажатия Escape
        /// </summary>
        public static void EscapeEvent()
        {
            exit = true;
        }
        public static void SaveLine(int index)
        {
            if (index >= Content.Count)
            {
                Content.Add(currentString);
            }
            else Content[index] = currentString;
        }
        /// <summary>
        /// Обработчик нажатия End
        /// </summary>
        public static void EndKeyEvent()
        {
            if (currentString.Length > maxX)
            {
                previousColumnCount = currentString.Length - maxX;
                RefreshLines(previousColumnCount);
                posX = maxX;
            }
            else
            {
                posX = currentString.Length;
            }
            ShowInfo();
            Curses.Move(posX, posY);
            Curses.Refresh();
        }
        /// <summary>
        /// Обработчик нажатия стрелки вправо
        /// </summary>
        public static void RightArrowKeyEvent()
        {
            if (posX + previousColumnCount < currentString.Length)
            {
                if (posX < maxX)
                {
                    posX++;
                }
                else
                {
                    previousColumnCount++;
                    RefreshLines(previousColumnCount);
                }
                ShowInfo();
                Curses.Move(posX, posY);
                Curses.Refresh();
            }
        }
        /// <summary>
        /// Обработчик нажатия стрелки влево
        /// </summary>
        public static void LeftArrowKeyEvent()
        {
            if (posX > 0)
            {
                posX--;
            }
            else if (previousColumnCount > 0)
            {
                previousColumnCount--;
                RefreshLines(previousColumnCount);
            }
            ShowInfo();
            Curses.Move(posX, posY);
            Curses.Refresh();
        }
        /// <summary>
        /// Обработчик нажатия стрелки вниз
        /// </summary>
        public static void DownKeyEvent()
        {
            if (posY < Content.Count - previousLinesCount)
            {
                SaveLine(stringIndex);
                stringIndex++;
                if (posY < maxY - 2)
                {
                    posY++;
                }
                else
                {
                    previousLinesCount++;
                }
                currentString = (string)Content[stringIndex];
                RefreshLines();
                if (posX + previousColumnCount > currentString.Length) EndKeyEvent();
                ShowInfo();
                Curses.Move(posX, posY);
                Curses.Refresh();
            }
        }
        /// <summary>
        /// Обработчик нажатия стрелки вверх
        /// </summary>
        public static void UpKeyEvent()
        {
            SaveLine(stringIndex);
            if (posY != 1)
            {
                posY--; stringIndex--;
            }
            else
            {
                if (stringIndex != 0)
                {

                    stringIndex--;
                    previousLinesCount--;
                }
            }
            currentString = (string)Content[stringIndex];
            RefreshLines();
            if (posX + previousColumnCount > currentString.Length) EndKeyEvent();
            ShowInfo();
            Curses.Move(posX, posY);
            Curses.Refresh();
        }
        /// <summary>
        /// Обработчик нажатия Home
        /// </summary>
        public static void HomeKeyEvent()
        {
            Curses.Move(0, posY);
            posX = 0;
            previousColumnCount = 0;
            RefreshLines();
            ShowInfo();
        }
        /// <summary>
        /// Обработчик нажатия PageDown
        /// </summary>
        public static void PageDownEvent()
        {
            for(int index=0;index<maxY;index++)
            {
                DownKeyEvent();
            }
        }
        /// <summary>
        /// Обработчик нажатия PageUp
        /// </summary>
        public static void PageUpEvent()
        {
            for (int index = 0; index < maxY; index++)
            {
                UpKeyEvent();
            }
        }
        /// <summary>
        /// Обработчик нажатия текстовых клавиш
        /// </summary>
        public static void AddCharToCurrentLineEvent(ConsoleKeyInfo ck)
        {
            if (!IsLineOver)
            {
                AddCharToCurrentLine(ck.KeyChar);
            }
            else
            {
                AddCharToCurrentLine(ck.KeyChar, false);
                previousColumnCount++;
                RefreshLines(previousColumnCount);
            }
            ShowInfo();
            Curses.Move(posX, posY);
            Curses.Refresh();
        }
        public static void TabEvent()
        {
            for(int count=0;count<4;count++)
            {
                AddCharToCurrentLineEvent(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false));
            }
        }
        /// <summary>
        /// Переключатель обработчиков нажатия клавиш
        /// </summary>
        public static void OperateKey(ConsoleKeyInfo ck)
        {
            switch (ck.Key)
            {
                case ConsoleKey.Tab:
                    TabEvent();
                    break;
                case ConsoleKey.PageUp:
                    PageUpEvent();
                    break;
                case ConsoleKey.PageDown:
                    PageDownEvent();
                    break;
                case ConsoleKey.Insert:
                    break;
                case ConsoleKey.Home:
                    HomeKeyEvent();
                    break;
                case ConsoleKey.End:
                    EndKeyEvent();
                    break;
                case ConsoleKey.UpArrow:
                    UpKeyEvent();
                    break;
                case ConsoleKey.DownArrow:
                    DownKeyEvent();
                    break;
                case ConsoleKey.LeftArrow:
                    LeftArrowKeyEvent();
                    break;
                case ConsoleKey.RightArrow:
                    RightArrowKeyEvent();
                    break;
                case ConsoleKey.Escape:
                    EscapeEvent();
                    break;
                case ConsoleKey.Enter:
                    GoToNextLine();
                    break;
                case ConsoleKey.Backspace:
                    BackspaceEvent();
                    break;
                case ConsoleKey.Delete:
                    DeleteEvent();
                    break;
                default:
                    AddCharToCurrentLineEvent(ck);
                    break;
            }
        }
        #endregion
        /// <summary>
        /// Инициализация и настройка экрана
        /// </summary>
        public static void StartUp()
        {
            Curses = new Curses();
            Curses.NoEcho();
            Curses.KeyPad(true);
            UpdateSizes();
            RefreshLines();
            Curses.Move(0, 1);
            Curses.Refresh();
        }
        public static int Main(string[] args)
        {
            if(args.Length!=0)
            {
                _fileName = args[0];
                if(_fileName == "--version")
                {
                    Console.WriteLine("Blunt editor by Dvoryadkin Pavel");
                    Console.WriteLine("version 1.0 2019");
                    return 0;
                }
                else if(_fileName == "--help")
                {
                    Console.WriteLine("Blunt editor by Dvoryadkin Pavel");
                    Console.WriteLine("version 1.0 2019");
                    Console.WriteLine("'bedit' to create new file");
                    Console.WriteLine("'bedit filename.txt' to open file");
                    return 0;
                }
                ReadFile();
                currentString = (string)Content[0];
            }
            StartUp();
            while (!exit)
            {
                var ck = Console.ReadKey(true);
                UpdateSizes();
                OperateKey(ck);
            }
            Curses = null;
            SaveFile();
            return 1;
        }
    }
}
