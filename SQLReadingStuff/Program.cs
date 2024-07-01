using System.Data;
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
            // FILES REQUIRED: the database, the auto fill checker
            // git push test 01/07 does it work on here ??
            SQLiteConnection conn = CreateConn();

            // i can't be asking this every time. the database has its limits and it can be violated and abused through this !

            if (ReadTxt() == "false")
            {
                ConsoleKeyInfo autoChoice;
                do
                {
                    Console.WriteLine("put in auto tasks? y/n");
                    autoChoice = Console.ReadKey();
                    Console.WriteLine();
                } while (autoChoice.Key != ConsoleKey.Y && autoChoice.Key != ConsoleKey.N); // auto fill database. this is just a filler really.
                if (autoChoice.Key == ConsoleKey.Y)
                {
                    NecessitiesTaskAutoSetup(conn);
                    WriteTxt("true");
                }
            }
            else { }
            
            MainMenu(conn);
        }

        static void WriteTxt(string toWrite) // writes over text, used for true/false nothing else. complete
        {
            using (StreamWriter sw = new StreamWriter("autoF.txt", false))
            {
                sw.WriteLine(toWrite);
            }
        }

        static string ReadTxt() // reads file, return output. that's it // complete.
        {
            string line = "";
            using (StreamReader sr = new StreamReader("autoF.txt"))
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                }
            }
            return line;
        }
        static void MainMenu(SQLiteConnection conn) // complete; it's a simple main menu!
        {
            ConsoleKeyInfo choiceK;
            do
            {
                GetMenuChoice();
                choiceK = Console.ReadKey();
                Console.WriteLine();
            } while (choiceK.Key != ConsoleKey.D1 && choiceK.Key != ConsoleKey.D2 && choiceK.Key != ConsoleKey.D3);
            Console.WriteLine();
            if (choiceK.Key == ConsoleKey.D1)
            {
                ReadTasks(conn);
            }
            else if (choiceK.Key == ConsoleKey.D2)
            {
                CreateTask(conn);
            }
            else if (choiceK.Key == ConsoleKey.D3)
            {
                DeleteTasks(conn);
            }
        }
        static SQLiteConnection CreateConn() // complete; all this does is create and open a connection !
        {
            SQLiteConnection theConnection;
            theConnection = new SQLiteConnection("Data Source = TasksFilled.db; Version = 3; New = True; Compress = True;"); // MAKE SURE TO CHANGE NAME HERE. tasksfilled is a placeholder name
            theConnection.Open();
            return theConnection;
        }
        static void GetMenuChoice() // complete, it's just a WL
        {
            Console.Clear();
            Console.WriteLine("[1] Read\n[2] New Task\n[3] Delete Task"); // just cwl
        }
        static void ReadTasks(SQLiteConnection conn) // complete, does as its asked.
        {
            // it's gotta loop like 7 times
            Console.WriteLine();
            Console.WriteLine("All Days? Y/N");
            ConsoleKeyInfo daysChoiceYN = Console.ReadKey();
            if (daysChoiceYN.Key == ConsoleKey.Y)
            {
                for (int i = 0; i < 8; i++) // it's probably easier to just put in "if i == 1" output monday or something but an sqlite command would be easier.
                {
                    DayNameGet(conn, i); // just outputs the day
                    TaskGetAllDays(conn, i); // gets all the tasks of current day in loop
                    Console.WriteLine();
                }
            }
            else if (daysChoiceYN.Key == ConsoleKey.N)
            {
                Console.WriteLine();
                ConsoleKeyInfo choosingDay;
                do
                {
                    for (int i = 0; i < daysKey.Length; i++)
                    {
                        Console.WriteLine("[{0}] {1}", i, daysKey[i]);
                    }
                    Console.WriteLine("\nWhat Day?");
                    choosingDay = Console.ReadKey();
                    Console.WriteLine();
                } while (choosingDay.Key is not (ConsoleKey.D0 or ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6 or ConsoleKey.D7));
                switch (choosingDay.Key)
                {
                    case ConsoleKey.D0:
                        TaskIndividualDayDisplay(conn, 0); break;
                    case ConsoleKey.D1:
                        TaskIndividualDayDisplay(conn, 1); break;
                    case ConsoleKey.D2:
                        TaskIndividualDayDisplay(conn, 2); break;
                    case ConsoleKey.D3:
                        TaskIndividualDayDisplay(conn, 3); break;
                    case ConsoleKey.D4:
                        TaskIndividualDayDisplay(conn, 4); break;
                    case ConsoleKey.D5:
                        TaskIndividualDayDisplay(conn, 5); break;
                    case ConsoleKey.D6:
                        TaskIndividualDayDisplay(conn, 6); break;
                    case ConsoleKey.D7:
                        TaskIndividualDayDisplay(conn, 7); break;
                } // probably could stuff this into a subroutine, but it's not THAT necessary
            }
            else
            {
                ReadTasks(conn); // hell yeah recursion !
            }
            Console.WriteLine("Back to main menu? Y/N"); // return to main menu, self explanatory.
            ConsoleKeyInfo mmChoice = Console.ReadKey();
            if (mmChoice.Key == ConsoleKey.Y)
            {
                MainMenu(conn);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        static void TaskGetAllDays(SQLiteConnection conn, int i) // need this function for both the reading one and the one that isn't reading (the create tasks one)
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
        static void DayNameGet(SQLiteConnection conn, int i) // gets day name from sqlite table directly. that's it
        {
            string dayCMD = "SELECT DayName FROM DaySpecificKey WHERE DayID = " + i + ";";
            SQLiteCommand dayGetCMD = new SQLiteCommand(dayCMD, conn);
            using (SQLiteDataReader dayRead = dayGetCMD.ExecuteReader())
            {
                while (dayRead.Read())
                {
                    Console.WriteLine("====== {0} ======",dayRead.GetString(0).ToUpper()); // make it look fancy with those equals
                }
            }
        }
        static void CreateTask(SQLiteConnection conn)
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
            int dayChoice = DayChoice(); // implemented function, removed previous int.parse, cr and recursion. 
            Console.WriteLine();
            Console.WriteLine("Tasks For:");
            DayNameGet(conn,dayChoice);
            Console.WriteLine();
            int usableTime = 24 - TaskIndividualDayDisplay(conn, dayChoice);
            if (usableTime == 0)
            {
                Console.WriteLine("Yeah, you're out of time for this day :/ Returning to main menu on keypress...");
                Console.ReadKey();
                MainMenu(conn);
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
                    else if (timeLengthChoice <= 0)
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
                CreateTask(conn);
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
                MainMenu(conn);
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
        
        static int TaskIndividualDayDisplay(SQLiteConnection conn, int day)
        {
            // takes in day, displays that day's tasks
            // additionally returns the hours you can allocate
            string theCMD = "SELECT Name, TaskTimeHours, TaskName FROM TaskType, DaySpecificKey, WeeklyAllocated WHERE WeeklyAllocated.TaskDay = " + day + " AND DaySpecificKey.DayID = WeeklyAllocated.TaskDay AND  WeeklyAllocated.TaskGroup = TaskType.ID;";
            int totalTime = 0;
            SQLiteCommand dayDisplay = new SQLiteCommand(theCMD, conn);
            using (SQLiteDataReader reader = dayDisplay.ExecuteReader()) // i realise it'd make more sense using the older subroutine which gets all the tasks, and remove the iteration so the iteration is only used when necessary but i'm not refactoring right now.
            {
                while (reader.Read())
                {
                    Console.WriteLine("Task: {0}\nCategory: {1}\nTime: {2}", reader.GetString(2), reader.GetString(0), reader.GetInt32(1));
                    totalTime += reader.GetInt32(1);
                }
            }
            
            return totalTime;
        }

        public struct IDCollection
        {
            public string name; // TaskName in WeeklyAllocated
            public int hours; // get the task's hours
        }
        static void DeleteTasks(SQLiteConnection conn)
        {
            ConsoleKeyInfo choice;
            do // works
            {
                DeleteTasksMenu();
                choice = Console.ReadKey();
                Console.WriteLine();
            } while (choice.Key != ConsoleKey.D1 && choice.Key != ConsoleKey.D2);

            switch (choice.Key)
            {
                case ConsoleKey.D1: // this one has to be specific with what it deletes... very complex.
                    Console.WriteLine("\n=============\n");
                    Console.WriteLine("What day are you picking?");
                    for (int i = 0; i < daysKey.Length; i++)
                    {
                        Console.WriteLine("[{0}] {1}", i, daysKey[i]); // display days
                    }
                    Console.WriteLine("\n=============\n");
                    int currentDayChoice = DayChoice(); // get day choice
                    Console.WriteLine("You've chosen \nHere are the tasks for that day:");
                    // i can't use task individual day display because i need a bit more info than that. but i can take it's sql query.
                    int iteratorCounter = 0;
                    List<IDCollection> tasksToSelect = new List<IDCollection>();
                    string theHereCMD = "SELECT Name, TaskTimeHours, TaskName, TimeSpecific FROM TaskType, DaySpecificKey, WeeklyAllocated WHERE WeeklyAllocated.TaskDay = " + currentDayChoice + " AND DaySpecificKey.DayID = WeeklyAllocated.TaskDay AND  WeeklyAllocated.TaskGroup = TaskType.ID;";
                    SQLiteCommand dayIterator = new SQLiteCommand(theHereCMD, conn);
                    using (SQLiteDataReader reader = dayIterator.ExecuteReader()) // i realise it'd make more sense using the older subroutine which gets all the tasks, and remove the iteration so the iteration is only used when necessary but i'm not refactoring right now.
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("[SELECT THIS TASK WITH: {0}]", iteratorCounter); // iterator counter has no purpose but to just give everything an ID in this subcontext.
                            Console.WriteLine("Task: {0}\nCategory: {1}\nTime: {2}", reader.GetString(2), reader.GetString(0), reader.GetInt32(1));
                            Console.WriteLine();
                            IDCollection temp;
                            temp.name = reader.GetString(2);
                            temp.hours = reader.GetInt32(1);
                            tasksToSelect.Add(temp);
                            iteratorCounter++;
                        }
                    }

                    if (iteratorCounter == 0)
                    {
                        Console.WriteLine("No tasks to delete here");
                        Thread.Sleep(2500);
                        Console.WriteLine("Returning to main menu on keypress...");
                        Console.ReadKey();
                        MainMenu(conn);
                    }
                    
                    // i could probably break this down into loads of funcs and stuff but it is too late
                    int select;
                    do
                    {
                        Console.WriteLine("Choose your task from 0 to {0}", iteratorCounter - 1);
                        select = int.Parse(Console.ReadLine());
                    } while (select < 0 || select > iteratorCounter - 1);
                    Console.WriteLine();
                    Console.WriteLine("You're going to delete the task {0} which you've dedicated {1} hours to on {2}", tasksToSelect[select].name, tasksToSelect[select].hours, daysKey[select]);
                    string deleteSpecificTaskCMD = "DELETE FROM WeeklyAllocated WHERE WeeklyAllocated.TaskDay = " + currentDayChoice + " AND WeeklyAllocated.TaskName = '" + tasksToSelect[select].name + "' AND WeeklyAllocated.TaskTimeHours = " + tasksToSelect[select].hours + ";";
                    // full cmd here noted for error checking
                    SQLiteCommand deleter = new SQLiteCommand(deleteSpecificTaskCMD, conn);
                    
                    deleter.ExecuteNonQuery();
                    
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
                        MainMenu(conn);
                    }
                    SQLiteCommand deleteACMD = new SQLiteCommand("DELETE FROM WeeklyAllocated;", conn);
                    deleteACMD.ExecuteNonQuery();
                    WriteTxt("false");
                    Console.WriteLine("Deleted everything.");
                    break;
                default: // this is impossible to get to i thought it'd be funny to have it please disregard it !
                    Console.WriteLine("no idea how you've done this lol."); // easter egg, impossible to get to though. lol.
                    Thread.Sleep(5000);
                    Console.WriteLine("you're still here? well... okay... bye bye");
                    Thread.Sleep(1000);
                    DeleteTasks(conn);
                    break;
            }

            Console.WriteLine("Returning to Main Menu...");
            Thread.Sleep(6500);
            MainMenu(conn);
        }

        static int DayChoice() // Complete... just get's the day choice. might implement this into other things.
        {
            ConsoleKeyInfo theC;
            Console.WriteLine("Please Enter Your Choice:");
            Console.WriteLine();
            theC = Console.ReadKey();
            Console.WriteLine();
            if (theC.Key is not (ConsoleKey.D0 or ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4
                or ConsoleKey.D5
                or ConsoleKey.D6 or ConsoleKey.D7))
            {
                DayChoice();
            }
            switch (theC.Key)
            {
                case ConsoleKey.D0:
                    return 0;
                case ConsoleKey.D1:
                    return 1;
                case ConsoleKey.D2:
                    return 2;
                case ConsoleKey.D3:
                    return 3;
                case ConsoleKey.D4:
                    return 4;
                case ConsoleKey.D5:
                    return 5;
                case ConsoleKey.D6:
                    return 6;
                case ConsoleKey.D7:
                    return 7;
            }

            return 0;
        }
        
        static void DeleteTasksMenu() // it's just a menu lol
        {
            Console.WriteLine("[1] Delete Specific Tasks?");
            Console.WriteLine("[2] Delete Everything?");
        }
        
        static void NecessitiesTaskAutoSetup(SQLiteConnection conn) // complete.
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