using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace SQLReadingStuff
{
    internal class Program
    {
        public static string[] daysKey = new string[8]
            { "No Day", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        static void Main(string[] args) // complete.
        {
            SQLiteConnection conn = CreateConn();
            ConsoleKeyInfo autoChoice;
            do
            {
                Console.WriteLine("put in auto tasks? y/n");
                autoChoice = Console.ReadKey();
                Console.WriteLine();
            } while(autoChoice.Key != ConsoleKey.Y && autoChoice.Key != ConsoleKey.N); // auto fill database. this is just a filler really.
            if (autoChoice.Key == ConsoleKey.Y)
            {
                necessitiesTaskAutoSetup(conn);
            }
            mainMenu(conn);
        }

        static void mainMenu(SQLiteConnection conn) // complete; it's a simple main menu!
        {
            ConsoleKeyInfo choiceK;
            do
            {
                getMenuChoice();
                choiceK = Console.ReadKey();
                Console.WriteLine();
            } while (choiceK.Key != ConsoleKey.D1 && choiceK.Key != ConsoleKey.D2 && choiceK.Key != ConsoleKey.D3);
            Console.WriteLine();
            if (choiceK.Key == ConsoleKey.D1)
            {
                readTasks(conn);
            }
            else if (choiceK.Key == ConsoleKey.D2)
            {
                createTask(conn);
            }
            else if (choiceK.Key == ConsoleKey.D3)
            {
                deleteTasks(conn);
            }
        }
        static SQLiteConnection CreateConn() // complete; all this does is create and open a connection !
        {
            SQLiteConnection theConnection;
            theConnection = new SQLiteConnection("Data Source = TasksFilled.db; Version = 3; New = True; Compress = True;"); // MAKE SURE TO CHANGE NAME HERE. tasksfilled is a placeholder name
            theConnection.Open();
            return theConnection;
        }
        static void getMenuChoice() // complete, it's just a WL
        {
            Console.Clear();
            Console.WriteLine("[1] Read\n[2] New Task\n[3] Delete Task"); // just cwl
        }
        static void readTasks(SQLiteConnection conn) // complete, does as its asked.
        {
            // it's gotta loop like 7 times
            Console.WriteLine();
            for (int i = 0; i < 8; i++) // it's probably easier to just put in "if i == 1" output monday or something but an sqlite command would be easier.
            {
                dayNameGet(conn, i); // just outputs the day
                taskGetByDay(conn, i); // gets all the tasks of current day in loop
                Console.WriteLine();
            }
            Console.WriteLine("Back to main menu? Y/N"); // return to main menu, self explanatory.
            ConsoleKeyInfo mmChoice = Console.ReadKey();
            if (mmChoice.Key == ConsoleKey.Y)
            {
                mainMenu(conn);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        static void taskGetByDay(SQLiteConnection conn, int i) // need this function for both the reading one and the one that isn't reading (the create tasks one)
        {
            // this has to contain both group order, and also print all metadata essentially
            // concisely speaking, it must be expandable !
            for (int j = 1; j < 5; j++)
            {
                string taskCMD = "SELECT Name, TaskTimeHours, TaskName FROM TaskType, WeeklyAllocated WHERE WeeklyAllocated.TaskDay = " + i + " AND WeeklyAllocated.TaskGroup = " + j + " AND TaskType.ID = WeeklyAllocated.TaskGroup;";
                SQLiteCommand tCMD = new SQLiteCommand(taskCMD, conn);
                using (SQLiteDataReader reader = tCMD.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine();
                        Console.WriteLine("Category: {0}.\nTime Required: {1} hours.\nTask Name: {2}", reader.GetString(0), reader.GetInt32(1), reader.GetString(2));
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine("====== [END OF TASKS FOR THIS DAY] ======");
        }
        static void dayNameGet(SQLiteConnection conn, int i) // gets day name from sqlite table directly. that's it
        {
            string dayCMD = "SELECT DayName FROM DaySpecificKey WHERE DayID = " + i + ";";
            SQLiteCommand dayGetCMD = new SQLiteCommand(dayCMD, conn);
            using (SQLiteDataReader dayRead = dayGetCMD.ExecuteReader())
            {
                while (dayRead.Read())
                {
                    Console.WriteLine("====== {0} ======",dayRead.GetString(0).ToUpper());
                }
            }
        }
        static void createTask(SQLiteConnection conn)
        {
            // DayID PK in DaySpecificKey
            // TaskDay FK in WeeklyAllocated
            // 0 - no day
            // 7 is the highest (Friday)
            
            // TimeSpecific = Real in WeeklyAllocated
            // ID PK in TaskType
            // 1 - School, 2 - Hobbies, 3 - Cadets, 4 - Necessary
            // 1/3/4 is an example of grouping etc; it will be autofilled in with the subroutine
            Console.WriteLine("What day would you like to put a task in?\n[0] Not Time Specific\n[1] Monday\n[2] Tuesday\n[3] Wednesday\n[4] Thursday\n[5] Friday\n[6] Saturday\n[7] Sunday");
            int dayChoice = int.Parse(Console.ReadLine()); // yeah i don't care about making sure inputs work
            if (dayChoice is > 7 or < 0)
            {
                createTask(conn); // recursion !
            }
            Console.WriteLine();
            Console.WriteLine("Tasks For:");
            dayNameGet(conn,dayChoice);
            Console.WriteLine();
            int usableTime = 24 - taskDayDisplay(conn, dayChoice);
            if (usableTime == 0)
            {
                Console.WriteLine("Yeah, you're out of time :/ Returning to main menu on keypress...");
                Console.ReadKey();
                mainMenu(conn);
            }
            Console.WriteLine();
            int timeLengthChoice = 0;
            bool validChoice = false;
            while (validChoice == false)
            {
                Console.WriteLine("How much time would you like to allocate to this task?");
                if (dayChoice != 0) // these days have 24h limits of course.
                {
                    Console.WriteLine("You have {0} hour(s) usable for this day", usableTime);
                    Console.WriteLine("Also please use integers, you cannot use floats and decimals for this (yet)");
                    timeLengthChoice = int.Parse(Console.ReadLine());
                    if (timeLengthChoice > usableTime)
                    {
                        Console.WriteLine("Yeah that's too big");
                    }
                    else if (usableTime <= 0)
                    {
                        Console.WriteLine("?? You gotta spend SOME time on it! Come on...");
                    }
                    else
                    {
                        validChoice = true;
                    }
                }
                else // non day specific tasks. these have no time limits.
                {
                    timeLengthChoice = int.Parse(Console.ReadLine());
                    validChoice = true;
                }
            }
            
             
            int taskCat = 0; // TASK CATEGORY !
            bool valid2 = false;
            while (valid2 == false)
            {
                Console.WriteLine("What Type Of Task?\n[1] School\n[2] Hobbies\n[3] Cadets\n[4] Necessary");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();
                if (key.Key != ConsoleKey.D1 && key.Key != ConsoleKey.D2 && key.Key != ConsoleKey.D3 &&
                    key.Key != ConsoleKey.D4)
                {
                    Console.WriteLine("Please...");
                }
                else
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.D1:
                            taskCat = 1;
                            break;
                        case ConsoleKey.D2:
                            taskCat = 2;
                            break;
                        case ConsoleKey.D3:
                            taskCat = 3;
                            break;
                        case ConsoleKey.D4:
                            taskCat = 4;
                            break;
                    }

                    valid2 = true;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Enter a name for this task");
            string tName = Console.ReadLine(); // yeah that's it lol.

            ConsoleKeyInfo choooice;
            do
            {
                Console.WriteLine("Is it time specific? Y/N");
                choooice = Console.ReadKey();
                Console.WriteLine();
            } while (choooice.Key != ConsoleKey.Y && choooice.Key != ConsoleKey.N);

            bool tSpecific;
            if (choooice.Key == ConsoleKey.Y)
            {
                tSpecific = true;
            }
            else
            {
                tSpecific = false;
            }

            Console.WriteLine();
            Console.Write("So your day is {0}, you'll be spending {1} hour(s) on it, it's called {2} and ", daysKey[dayChoice], timeLengthChoice, tName);
            if (tSpecific == true)
            {
                Console.Write("it is time specific!");
            }
            else
            {
                Console.Write("it is not time specific!");
            }
            Console.WriteLine();
            Console.WriteLine("Is this right Y/N");
            Console.WriteLine();
            ConsoleKeyInfo isItRight = Console.ReadKey();
            Console.WriteLine();
            if (isItRight.Key == ConsoleKey.N)
            {
                createTask(conn);
            }

            string customTaskCreate =
                $"INSERT INTO WeeklyAllocated VALUES ({taskCat},'{tName}',{timeLengthChoice},{tSpecific},{dayChoice});";
            SQLiteCommand taskCreate = new SQLiteCommand(customTaskCreate, conn);
            taskCreate.ExecuteNonQuery();
            Console.WriteLine("Return to MM? Y/N");
            Console.WriteLine();
            ConsoleKeyInfo returnn = Console.ReadKey();
            if (returnn.Key == ConsoleKey.Y)
            {
                mainMenu(conn);
            }
            else
            {
                Environment.Exit(0);
            }
            // VARIABLE ZERO ! DAY CHOICE (dayChoice) DONE
            // VARIABLE ONE ! TIME LENGTH CHOICE (timeLengthChoice) DONE
            // VARIABLE TWO ! TASK NAME (tName) DONE
            // VARIABLE THREE ! TIME SPECIFIC (tSpecific)
        }
        
        static int taskDayDisplay(SQLiteConnection conn, int day)
        {
            // takes in day, displays that day's tasks
            // additionally returns the hours you can allocate
            string theCMD = "SELECT Name, TaskTimeHours, TaskName FROM TaskType, DaySpecificKey, WeeklyAllocated WHERE WeeklyAllocated.TaskDay = " + day + " AND DaySpecificKey.DayID = WeeklyAllocated.TaskDay AND  WeeklyAllocated.TaskGroup = TaskType.ID;";
            int totalTime = 0;
            SQLiteCommand dayDisplay = new SQLiteCommand(theCMD, conn);
            using (SQLiteDataReader reader = dayDisplay.ExecuteReader()) // i realise it'd make more sense using the older subroutine which gets all the tasks, and remove the iteration so the iteration is only used when necessary but idc enough ahhaha
            {
                while (reader.Read())
                {
                    Console.WriteLine("Task: {0}\nCategory: {1}\nTime: {2}", reader.GetString(2), reader.GetString(0), reader.GetInt32(1));
                    totalTime += reader.GetInt32(1);
                }
            }
            
            return totalTime;
        }
        
        static void deleteTasks(SQLiteConnection conn)
        {
            ConsoleKeyInfo choice;
            do // works
            {
                deleteTasksMenu();
                choice = Console.ReadKey();
                Console.WriteLine();
            } while (choice.Key != ConsoleKey.D1 && choice.Key != ConsoleKey.D2);

            switch (choice.Key)
            {
                case ConsoleKey.D1: // this one has to be specific with what it deletes... very complex.
                    

                    break;
                case ConsoleKey.D2: // delete everything !
                    ConsoleKeyInfo sureChoice;
                    do
                    {
                        Console.WriteLine("Are you SURE? Y/N");
                        sureChoice = Console.ReadKey();
                        Console.WriteLine();
                    } while (sureChoice.Key != ConsoleKey.Y && sureChoice.Key != ConsoleKey.N);
                    if (sureChoice.Key == ConsoleKey.N)
                    {
                        Console.WriteLine("Good choice.");
                        mainMenu(conn);
                    }
                    SQLiteCommand deleteACMD = new SQLiteCommand("DELETE FROM WeeklyAllocated;", conn);
                    deleteACMD.ExecuteNonQuery();
                    Console.WriteLine("Deleted everything.");
                    break;
                default: // this is impossible to get to i thought it'd be funny to have it please disregard it !
                    Console.WriteLine("no idea how you've done this lol."); // easter egg, impossible to get to though. lol.
                    Thread.Sleep(5000);
                    Console.WriteLine("you're still here? well... okay... bye bye");
                    Thread.Sleep(1000);
                    deleteTasks(conn);
                    break;
            }
            mainMenu(conn);
        }

        
        
        static void deleteTasksMenu() // complete.. it's just a menu lol
        {
            Console.WriteLine("[1] Delete Specific Tasks?");
            Console.WriteLine("[2] Delete Everything?");
        }
        
        static void necessitiesTaskAutoSetup(SQLiteConnection conn) // complete.
        {
            for(int i = 1; i < 8; i++)
            {
                string sleeping = "INSERT INTO WeeklyAllocated VALUES (4, 'Sleep', 8, FALSE," + i + ");";
                SQLiteCommand cmd = new SQLiteCommand(sleeping, conn);
                cmd.ExecuteNonQuery();
                if (i == 2 || i == 4)
                {
                    string cadets = "INSERT INTO WeeklyAllocated VALUES (3, 'Cadets', 2, TRUE, " + i + ");";
                    SQLiteCommand cmd1 = new SQLiteCommand(cadets, conn);
                    cmd1.ExecuteNonQuery();
                }
                string school = "INSERT INTO WeeklyAllocated VALUES (1, 'School', 7, TRUE," + i + ");";
                SQLiteCommand cmd2 = new SQLiteCommand(school, conn);
                cmd2.ExecuteNonQuery();
            }
        }
    }
}