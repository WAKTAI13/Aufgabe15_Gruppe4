using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace minesweeper_console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.Title = "Minesweeper";
            //Hauptmenü
            mainmenu();
        }

        public static void mainmenu()
        {
            Console.SetWindowSize(45, 7);
            Console.CursorVisible = true;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.WriteLine("Benutze die Pfeiltasten um zu navigieren.\nBestätige deine Auswahl mit der Leertaste.\n" +
                "\nPlay" +
                "\nHighscore" +
                "\nExit");

            //Menünavigation und Cursorposition auswerten
            int navi = menunavigation(3, 5);
            switch (navi)
            {
                case 3:
                    play();
                    break;
                case 4:
                    highscore();
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
            }
        }

        public static void play()
        {
            Console.Clear();
            Console.SetWindowSize(24, 6);
            //Schwerigkeitsgrad auswählen
            Console.WriteLine("Choose your Difficulty:\n" +
                "\nEasy" +
                "\nAdvanced" +
                "\nExpert");

            //Menünavigation
            int navi = menunavigation(2, 4);
            //Schwierigkeitseinstellungen aus Datei lesen
            string[] diff = File.ReadAllLines(@"difficulty.txt");
            //Je nach ausgewählter Schwierigkeit das Level in Textdatei und Array generieren
            string[,] level = new string[16, 30];	//geht eleganter
            int x_max = 0;
            int y_max = 0;
            int diffi = 0;
            switch (navi)
            {
                case 2:
                    level = generate_level(diff[0]);
                    y_max = 8;
                    x_max = 8;                          //Eigenkritik: doppelt gemoppelt gibs in generate auch schon
                    diffi = 1;
                    Console.SetWindowSize(10, 13);
                    break;
                case 3:
                    level = generate_level(diff[1]);
                    y_max = 15;
                    x_max = 15;
                    diffi = 2;
                    Console.SetWindowSize(17, 20);
                    break;
                case 4:
                    level = generate_level(diff[2]);
                    y_max = 29;
                    x_max = 15;
                    diffi = 3;
                    Console.SetWindowSize(17, 33);
                    break;
            }
            //ingame
            //Zeit
            Stopwatch time = new Stopwatch();
            time.Start();
            while (true)
            {

                //Navigation
                bool leertaste = false;
                ConsoleKeyInfo taste = new ConsoleKeyInfo();
                do
                {
                    //Auf Navigationseingabe warten
                    while (Console.KeyAvailable == false)
                    {
                        Thread.Sleep(100);
                        //Zeitausgabe
                        int ikks = Console.CursorLeft;
                        int üpsilonn = Console.CursorTop;
                        Console.SetCursorPosition(0, y_max + 2);
                        Console.Write(time.Elapsed.TotalSeconds.ToString());
                        Console.SetCursorPosition(ikks, üpsilonn);
                    }
                    //Navigationseingabe auswerten
                    taste = Console.ReadKey(true);
                    switch (taste.Key)
                    {
                        case ConsoleKey.DownArrow:
                            if (Console.CursorTop < y_max)
                            {
                                ++Console.CursorTop;
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (Console.CursorTop > 0)
                            {
                                --Console.CursorTop;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (Console.CursorLeft > 0)
                            {
                                --Console.CursorLeft;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (Console.CursorLeft < x_max)
                            {
                                ++Console.CursorLeft;
                            }
                            break;
                        case ConsoleKey.Spacebar:
                            leertaste = true;
                            break;
                        default:
                            break;
                    }
                } while (!leertaste);

                //Feld aufdecken
                int x = Console.CursorLeft;
                int y = Console.CursorTop;
                if (level[y, x] != "00")
                {
                    Console.Write(level[y, x]);
                    Console.SetCursorPosition(x, y);
                }
                //Falls 0, alle anliegenden Nullen und deren anliegenden Zahlen aufdecken
                if (level[y, x] == "0")
                {
                    nullchecker(x, y, level, x_max, y_max);	//^^
                    Console.SetCursorPosition(x, y);
                }
                //Mine aufgedeckt? WTFBOOOOOOOOOOOOOOOOOOOOM!
                if (level[y, x] == "x")
                {
                    splat(x, y, y_max + 1);
                }
                //Der Wert 00 steht für "bereits aufgedeckt"
                level[y, x] = "00";
                //wincheck
                bool win = true;
                for (int yy = 0; yy < y_max+1; yy++)
                {
                    for (int xx = 0; xx < x_max; xx++)
                    {
                        if (level[yy, xx] != "00" && level[yy, xx] != "x")
                        {
                            win = false;
                        }
                    }
                }
                //Debughelp
                //Console.SetCursorPosition(0, y_max + 3);
                //for (int yyy = 0; yyy <= y_max; yyy++)
                //{
                //    for (int xxx = 0; xxx <= x_max; xxx++)
                //    {
                //        Console.Write(level[yyy, xxx]);
                //    }
                //    Console.Write("\n");
                //}
                //Console.SetCursorPosition(x, y);
                //
                if (win)
                {
                    aufdeck();
                    Console.SetCursorPosition(0, y_max + 1);
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Geschafft.");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;
                    highscorecheck(time.Elapsed.TotalSeconds, diffi);
                }
            }
        }

        public static void nullchecker(int x, int y, string[,] level, int x_max, int y_max) //^^
        {
            if (y > 0 && level[y - 1, x] == "0")
            {
                Console.SetCursorPosition(x, y - 1);
                Console.Write(level[y - 1, x]);
                level[y - 1, x] = "00";
                nullchecker(x, y - 1, level, x_max, y_max);
            }
            else if (y > 0 && level[y - 1, x] != "00")
            {
                Console.SetCursorPosition(x, y - 1);
                Console.Write(level[y - 1, x]);
                level[y - 1, x] = "00";
            }
            if (x > 0 && level[y, x - 1] == "0")
            {
                Console.SetCursorPosition(x - 1, y);
                Console.Write(level[y, x - 1]);
                level[y, x - 1] = "00";
                nullchecker(x - 1, y, level, x_max, y_max);
            }
            else if (x > 0 && level[y, x - 1] != "00")
            {
                Console.SetCursorPosition(x - 1, y);
                Console.Write(level[y, x - 1]);
                level[y, x - 1] = "00";
            }
            if (x < x_max && level[y, x + 1] == "0")
            {
                Console.SetCursorPosition(x + 1, y);
                Console.Write(level[y, x + 1]);
                level[y, x + 1] = "00";
                nullchecker(x + 1, y, level, x_max, y_max);
            }
            else if (x < x_max && level[y, x + 1] != "00")
            {
                Console.SetCursorPosition(x + 1, y);
                Console.Write(level[y, x + 1]);
                level[y, x + 1] = "00";
            }
            if (y < y_max && level[y + 1, x] == "0")
            {
                Console.SetCursorPosition(x, y + 1);
                Console.Write(level[y + 1, x]);
                level[y + 1, x] = "00";
                nullchecker(x, y + 1, level, x_max, y_max);
            }
            else if (y < y_max && level[y + 1, x] != "00")
            {
                Console.SetCursorPosition(x, y + 1);
                Console.Write(level[y + 1, x]);
                level[y + 1, x] = "00";
            }
            if (y>0 && x>0 && level[y - 1, x-1] != "00")
            {
                Console.SetCursorPosition(x-1, y - 1);
                Console.Write(level[y - 1, x-1]);
                if (level[y - 1, x - 1] == "0")
                {
                    nullchecker(x - 1, y - 1, level, x_max, y_max);
                }
                level[y - 1, x-1] = "00";
            }
            if (y<y_max && x<x_max && level[y + 1, x + 1] != "00")
            {
                Console.SetCursorPosition(x + 1, y + 1);
                Console.Write(level[y + 1, x + 1]);
                if (level[y + 1, x + 1] == "0")
                {
                    nullchecker(x + 1, y + 1, level, x_max, y_max);
                }
                level[y + 1, x + 1] = "00";
            }
            if (y > 0 && x < x_max && level[y - 1, x + 1] != "00")
            {
                Console.SetCursorPosition(x + 1, y - 1);
                Console.Write(level[y - 1, x + 1]);
                if (level[y - 1, x + 1] == "0")
                {
                    nullchecker(x + 1, y - 1, level, x_max, y_max);
                }
                level[y - 1, x + 1] = "00";
            }
            if (y<y_max && x > 0 && level[y + 1, x - 1] != "00")
            {
                Console.SetCursorPosition(x - 1, y + 1);
                Console.Write(level[y + 1, x - 1]);
                if (level[y + 1, x - 1] == "0")
                {
                    nullchecker(x-1, y + 1, level, x_max, y_max);
                }
                level[y + 1, x - 1] = "00";
            }
        }

        public static void highscore()
        {
            Console.Clear();
            Console.SetWindowSize(18, 6);
            //Schwerigkeitsgrad auswählen
            Console.WriteLine("Which Scoreboard?\n" +
                "\nEasy" +
                "\nAdvanced" +
                "\nExpert");

            //Menünavigation
            int diff = menunavigation(2, 4)-1;
            //Highscore aus Datei
            Console.SetWindowSize(33, 13);
            Console.Clear();
            Console.WriteLine("Rank\tTime\t\tName\n");
            string[] inhalt = File.ReadAllLines(@"highscore_"+diff+".txt");
            int rank = 1;
            foreach (var item in inhalt)
            {
                string[] line = item.Split(';');
                Console.WriteLine(String.Format("{0}\t{1}\t{2}", rank, line[0], line[1]));
                rank++;
            }
            Console.ReadKey();
            mainmenu();
        }

        static int menunavigation(int mintop, int maxtop)
        {
            //Menünavigation
            //Startposition des Cursors festlegen
            Console.CursorTop = mintop;

            //Navigations-loop
            bool leertaste = false;
            ConsoleKeyInfo taste = new ConsoleKeyInfo();
            do
            {
                //Auf Navigationseingabe warten
                while (Console.KeyAvailable == false)
                {
                    System.Threading.Thread.Sleep(100);
                }
                //Navigationseingabe auswerten
                taste = Console.ReadKey(true);
                switch (taste.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (Console.CursorTop < maxtop)
                        {
                            ++Console.CursorTop;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (Console.CursorTop > mintop)
                        {
                            --Console.CursorTop;
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        leertaste = true;
                        break;
                    default:
                        break;
                }
            } while (!leertaste);

            //Cursorposition zurückgeben
            int pos = Console.CursorTop;
            return pos;
        }

        static string[,] generate_level(string setupstring)
        {
            //setupstring zerlegen
            string[] setup = setupstring.Split(',');
            //"Setupwerte" aus array lesen
            int minecount = Convert.ToInt32(setup[0]);
            int x_max = Convert.ToInt32(setup[1]);
            int y_max = Convert.ToInt32(setup[2]);
            //Level-Array generieren
            string[,] level = new string[y_max, x_max];
            //Minen setzen
            Random rand = new Random();
            for (int i = 0; i < minecount; i++)
            {
                int x = rand.Next(x_max - 1);
                int y = rand.Next(y_max - 1);
                if (level[y, x] == "x")
                {
                    i--;
                }
                else
                {
                    level[y, x] = "x";
                }
                ;
            }
            //Zahlen setzten
            for (int i = 0; i < y_max; i++)
            {
                for (int j = 0; j < x_max; j++)
                {
                    int num = 0;
                    if (level[i, j] == "x")
                    {
                        continue;
                    }
                    if (i > 0 && j > 0 && level[i - 1, j - 1] == "x")
                    {
                        num++;
                    }
                    if (i > 0 && level[i - 1, j] == "x")
                    {
                        num++;
                    }
                    if (j > 0 && level[i, j - 1] == "x")
                    {
                        num++;
                    }
                    if (i < y_max - 1 && j < x_max - 1 && level[i + 1, j + 1] == "x")
                    {
                        num++;
                    }
                    if (i < y_max - 1 && level[i + 1, j] == "x")
                    {
                        num++;
                    }
                    if (j < x_max - 1 && level[i, j + 1] == "x")
                    {
                        num++;
                    }
                    if (i < y_max - 1 && j > 0 && level[i + 1, j - 1] == "x")
                    {
                        num++;
                    }
                    if (i > 0 && j < x_max - 1 && level[i - 1, j + 1] == "x")
                    {
                        num++;
                    }

                    level[i, j] = Convert.ToString(num);
                }
            }
            //Level in txt-Datei schreiben
            StreamWriter generate = new StreamWriter(@"level.txt");
            int rowcheck = 0;
            foreach (var item in level)
            {
                generate.Write(item);
                rowcheck++;
                if (rowcheck == x_max)
                {
                    generate.Write("\n");
                    rowcheck = 0;
                }
            }
            generate.Close();
            //Zugedecktes Level darstellen
            Console.Clear();
            for (int i = 0; i < y_max; i++)
            {
                for (int j = 0; j < x_max; j++)
                {
                    Console.Write("#");
                }
                Console.Write("\n");
            }									
            Console.SetCursorPosition(0, 0);
            return level;
        }

        static void splat(int x, int y, int y_max)
        {
            aufdeck();
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("x");
            Console.SetCursorPosition(0, y_max);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("verkackt^^");
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nEnter...");
            bool enter = false;
            ConsoleKeyInfo taste = new ConsoleKeyInfo();
            do
            {
                taste = Console.ReadKey(true);
                if (taste.Key == ConsoleKey.Enter)
                {
                    enter = true;
                }
            } while (!enter);
            mainmenu();
        }

        static void aufdeck()
        {
            //aufgedecktes Level darstellen
            Console.Clear();
            StreamReader display = new StreamReader(@"level.txt");
            Console.WriteLine(display.ReadToEnd());
            display.Close();
        }

        static void highscorecheck(double score, int diff)
        {
            string[] scorearr = File.ReadAllLines(@"highscore_"+diff+".txt");
            for (int i = 0; i < 10; i++)
            {
                if (score < Convert.ToDouble(scorearr[i].Split(';')[0]) || scorearr[i].Split(';')[0] == "0")
                {
                    Console.WriteLine("\nName?:");
                    Thread.Sleep(100);
                    string name = Console.ReadLine();
                    //score nachrücken
                    for (int j = 9; j > i; j--)
                    {
                        scorearr[j] = scorearr[j-1];
                    }
                    scorearr[i] = String.Format("{0};{1}", score.ToString(), name);
                    File.WriteAllLines(@"highscore_"+diff+".txt", scorearr);
                    //Highscore aus Datei ausgeben
                    Console.Clear();
                    Console.SetWindowSize(33, 13);
                    Console.WriteLine("Rank\tTime\t\tName\n");
                    string[] inhalt = File.ReadAllLines(@"highscore_" + diff + ".txt");
                    int rank = 1;
                    foreach (var item in inhalt)
                    {
                        string[] line = item.Split(';');
                        Console.WriteLine(String.Format("{0}\t{1}\t{2}", rank, line[0], line[1]));
                        rank++;
                    }
                    Console.ReadKey();
                    mainmenu();
                }
            }
            mainmenu();     
        }
    }
}
