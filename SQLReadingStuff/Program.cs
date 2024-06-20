using System.Data.SQLite;

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
            Console.WriteLine("Read [1]\nNew Task[2]\nDelete Task[3]");
        }
        static void readTasks(SQLiteConnection conn)
        {

        }
        static void createTask(SQLiteConnection conn)
        {

        }
        static void deleteTasks(SQLiteConnection conn)
        {

        }
    }
}