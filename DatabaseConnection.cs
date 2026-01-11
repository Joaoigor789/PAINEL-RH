using MySql.Data.MySqlClient;
 // ou using MySql.Data.MySqlClient;
using System;

namespace RH_STAFF
{
    public class DatabaseConnection
    {
        private string connectionString;

        public DatabaseConnection()
        {
            // Decodificar a string de conexão (remover URL encoding)
            connectionString = "server=sp-11.magnohost.com.br;" +
                               "port=3306;" +
                               "database=s3966_universe;" +
                               "uid=u3966_8SWvyRBqnr;" +
                               "password=wX@j^M=Mgf6BYc=.CJ7pdj3R;" +
                               "SslMode=Preferred;"; // ou Required/None dependendo do servidor
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}