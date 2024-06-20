using System.Data.SQLite;
using System.Diagnostics;

namespace SQLReadingStuff
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* arrange for loop that goes through days:
             * DaySpecificKey.DayID = n <--- value to loop through 
             * DaySpecificKey.DayName <--- value to print
             * SELECT TaskName
             * (additionally, there's a lot more you can add.
             * FROM TaskType, DaySpecificKey, WeeklyAllocated
             * WHERE DaySpecificKey.DayID = n AND WeeklyAllocated = n
             * ORDER BY TaskType.ID asc
             *
             *
            */
            SQLiteConnection conn = CreateConn();
            ConsoleKeyInfo autoChoice;
            do
            {
                Console.WriteLine("put in auto tasks? y/n");
                autoChoice = Console.ReadKey();
                Console.WriteLine();
            } while(autoChoice.Key != ConsoleKey.Y || autoChoice.Key != ConsoleKey.N);
            if (autoChoice.Key == ConsoleKey.Y)
            {
                necessitiesTaskAutoSetup(conn);
            }
            mainMenu(conn);
        }

        static void mainMenu(SQLiteConnection conn)
        {
            ConsoleKeyInfo choiceK;
            do
            {
                getMenuChoice();
                choiceK = Console.ReadKey();
                Console.WriteLine();
            } while (choiceK.Key != ConsoleKey.D1 || choiceK.Key != ConsoleKey.D2 || choiceK.Key != ConsoleKey.D3);
        }
        static SQLiteConnection CreateConn()
        {
            SQLiteConnection theConnection;
            theConnection = new SQLiteConnection("Data Source = TasksFilled.db; Version = 3; New = True; Compress = True;"); // MAKE SURE TO CHANGE NAME HERE. tasksfilled is a placeholder name
            theConnection.Open();
            return theConnection;
        }
        static void getMenuChoice()
        {
            Console.Clear();
            Console.WriteLine("[1] Read\n[2] New Task\n[3] Delete Task");
        }
        static void readTasks(SQLiteConnection conn)
        {

        }
        static void createTask(SQLiteConnection conn)
        {

        }
        static void deleteTasks(SQLiteConnection conn)
        {
            ConsoleKeyInfo choice;
            do
            {
                deleteTasksMenu();
                choice = Console.ReadKey();
                Console.WriteLine();
            } while (choice.Key != ConsoleKey.D1 || choice.Key != ConsoleKey.D2);

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
                    } while (sureChoice.Key != ConsoleKey.Y || sureChoice.Key != ConsoleKey.N);
                    if (sureChoice.Key == ConsoleKey.N)
                    {
                        Console.WriteLine("Good choice.");
                        mainMenu(conn);
                    }
                    SQLiteCommand deleteACMD = new SQLiteCommand("DELETE FROM WeeklyAllocated;");
                    deleteACMD.ExecuteNonQuery();
                    Console.WriteLine("Delete everything.");
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
        static void necessitiesTaskAutoSetup(SQLiteConnection conn)
        {
            for(int i = 1; i < 8; i++)
            {
                string theCMD = "INSERT INTO WeeklyAllocated VALUES (4, 'Sleep', 8, FALSE," + i + ");";
                SQLiteCommand cmd = new SQLiteCommand(theCMD, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}