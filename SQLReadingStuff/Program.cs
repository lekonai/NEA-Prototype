using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Diagnostics;

namespace SQLReadingStuff
{
    internal class Program
    {
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
        static SQLiteConnection CreateConn() // all this does is create and open a connection !
        {
            SQLiteConnection theConnection;
            theConnection = new SQLiteConnection("Data Source = TasksFilled.db; Version = 3; New = True; Compress = True;"); // MAKE SURE TO CHANGE NAME HERE. tasksfilled is a placeholder name
            theConnection.Open();
            return theConnection;
        }
        static void getMenuChoice()
        {
            Console.Clear();
            Console.WriteLine("[1] Read\n[2] New Task\n[3] Delete Task"); // just cwl
        }
        static void readTasks(SQLiteConnection conn)
        {
            // it's gotta loop like 7 times
            Console.WriteLine();
            for (int i = 0; i < 8; i++) // it's probably easier to just put in "if i == 1" output monday or something but an sqlite command would be easier.
            {
                dayGet(conn, i); // just outputs the day
                taskGetByDay(conn, i); // gets all the tasks of current day in loop
                Console.WriteLine();
            }
        }

        static void taskGetByDay(SQLiteConnection conn, int i) // need this function for both the reading one and the one that isn't reading (the create tasks one)
        {
            // this has to contain both group order, and also print all metadata essentially
            // concisely speaking, it must be expandable !
            for (int j = 1; j < 5; j++)
            {
                string taskCMD =
                    "SELECT Name, TaskTimeHours, TaskName FROM TaskType, WeeklyAllocated WHERE WeeklyAllocated.TaskDay = " +
                    i + " AND WeeklyAllocated.TaskGroup = " + j + " AND TaskType.ID = WeeklyAllocated.TaskGroup;";
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
        static string dayGet(SQLiteConnection conn, int i) // gets day name from sqlite table directly. that's it
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

            return null;
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
            Console.WriteLine("What day would you like to put a task in?");
        }
        static void deleteTasks(SQLiteConnection conn)
        {
            ConsoleKeyInfo choice;
            do
            {
                deleteTasksMenu();
                choice = Console.ReadKey();
                Console.WriteLine();
            } while (choice.Key != ConsoleKey.D1 && choice.Key != ConsoleKey.D2);

            switch (choice.Key)
            {
                case ConsoleKey.D1:


                    break;
                case ConsoleKey.D2:
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
                    SQLiteCommand deleteACMD = new SQLiteCommand("DELETE FROM WeeklyAllocated;");
                    deleteACMD.ExecuteNonQuery();
                    Console.WriteLine("Deleted everything.");
                    break;
                default:
                    Console.WriteLine("no idea how you've done this lol."); // easter egg, impossible to get to though. lol.
                    Thread.Sleep(5000);
                    Console.WriteLine("you're still here? well... okay... bye bye");
                    Thread.Sleep(1000);
                    deleteTasks(conn);
                    break;
            }
            mainMenu(conn);
        }

        static void deleteTasksMenu()
        {
            Console.WriteLine("[1] Delete Specific Tasks?");
            Console.WriteLine("(Not Functional Yet)");
            Console.WriteLine("[2] Delete Everything?");
        }

        static string daySelector()
        {
            
            return null;
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