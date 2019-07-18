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
        static string Content = "";
        static int maxY = 0;
        static int maxX = 0;
        static string currentString = "";
        static int Backspace = 263;
        static int AltBackspace = 264;
        static int Escape = 27;
        public static void GoToNewLine(char ch)
        {
            currentString += '\r';
            currentString += '\n';
            posY++;
            posX = 0;
            Content += currentString;
            if ((maxY - 1) == posY)
            {
                Curses.DeleteFirstLine();
                posY--;
            }
            Curses.Move(posX, posY);
            //Curses.Print(posX, posY, ch.ToString());
            Curses.Refresh();
            currentString = "";
        }
        public static void AddCharToCurrentLine(char ch)
        {
            currentString += ch;
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
            maxX = Console.WindowWidth-1;
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
            bool exit = false;
            while (!exit)
            {
                int ch = Curses.GetChar();//код : 'Console.ReadKey(true).KeyChar;' в Linux частично не работает 
                UpdateSizes();
                if (ch != Escape)
                {
                    if (ch == 10)
                    {
                        GoToNewLine((char)ch);
                    }
                    else
                    {
                        if (ch == Backspace)
                        {
                            if ((!String.IsNullOrEmpty(Content)) || (!String.IsNullOrEmpty(currentString)))
                            {
                                if (IsNewLine)
                                {
                                    posY--;
                                    var strings = Content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                    currentString = strings[strings.Length - 1];
                                    Content = Content.Remove(Content.Length - currentString.Length - 2);
                                    var cur = currentString;
                                    var content = Content;
                                    posX = currentString.Length;
                                    Curses.Print(posX, posY, " ");
                                    Curses.Move(posX, posY);
                                    Curses.Refresh();
                                }
                                else
                                {
                                    //Curses.Print(posX - 1, posY, " ");
                                    posX--;
                                    Curses.Print(posX , posY, " ");
                                    Curses.Move(posX, posY);
                                    currentString = currentString.Remove(currentString.Length - 1);
                                    Curses.Refresh();
                                }
                            }
                        }
                        else
                        {
                            if(!IsLineOver)
                            {
                                AddCharToCurrentLine((char)ch);
                            }
                            else
                            {
                                GoToNewLine((char)ch);
                            }

                        }
                    }
                }
                else
                {//нажали Esc
                    if (posX != 0)
                    {
                        Content+=currentString;
                    }
                    exit = true;
                }
            }
            Curses = null;
            Console.Clear();
            Console.WriteLine("Result :\r");
            Console.Write(Content);
            Console.ReadLine();
        }
    }
}