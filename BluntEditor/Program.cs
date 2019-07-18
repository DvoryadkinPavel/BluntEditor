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

        public static void Main(string[] args)
        {
            int posX = 0;
            int posY = 1;
            var Content = new List<Dictionary<int, char>>();
            int maxY = 0;
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
            Curses Curses = new Curses();
            Curses.Print(0, 0, "Введите текст:");
            Curses.Move(0, 1);
            Curses.Refresh();
            var currentString = new Dictionary<int, char>();
            bool exit = false;
            while (!exit)
            {
                char ch = Console.ReadKey(true).KeyChar;
                if(ch != (char)ConsoleKey.Escape)
                {
                    if(posX ==0)
                    {
                        currentString = new Dictionary<int,char>();
                    }
                    if (ch == (char)ConsoleKey.Enter)
                    {
                        currentString.Add(posX,'\r');
                        posX++;
                        currentString.Add(posX,'\n');
                        posY++;
                        posX = 0;
                        Content.Add(currentString);
                        maxY = Console.WindowHeight;
                        if ((maxY-1) == posY)
                        {
                            Curses.DeleteFirstLine();
                            posY--;
                        }
                        Curses.Print(posX, posY, ch.ToString());
                    }
                    else
                    {
                        currentString.Add(posX, ch);
                        Curses.Print(posX, posY, ch.ToString());
                        posX++;
                    }
                    Curses.Refresh();
                }
                else
                {
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