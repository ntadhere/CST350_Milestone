namespace CST350_Milestone.Services.DataAccess
{
    public class UserDAO
    {
        // Define the connection for MSQL
        // change userauth?
        static string conn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserAuth;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        // Define the connection string for MySQL
        static string serverName = "localhost";
        static string username = "root";
        static string password = "root";
        static string dbName = "userauth";
        static string port = "3306";    // get correct port


    }
}
