using MySql.Data.MySqlClient;
using RH_STAFF.Models;
using System.Collections.Generic;

namespace RH_STAFF.Repositories
{
    public class StaffRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public StaffRepository()
        {
            _dbConnection = new DatabaseConnection();
        }

        public bool ValidateLogin(string username, string password)
        {
            using (var conn = _dbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM staff WHERE usuario = @usuario AND senha = @senha AND ativo = 1",
                    conn);

                cmd.Parameters.AddWithValue("@usuario", username);
                cmd.Parameters.AddWithValue("@senha", password); // Considere usar hash na prática!

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        public StaffMember GetStaffByUsername(string username)
        {
            using (var conn = _dbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT * FROM staff WHERE usuario = @usuario",
                    conn);

                cmd.Parameters.AddWithValue("@usuario", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new StaffMember
                        {
                            Id = reader.GetInt32("id"),
                            Nome = reader.GetString("nome"),
                            Cargo = reader.GetString("cargo"),
                            Usuario = reader.GetString("usuario"),
                            NivelAcesso = reader.GetString("nivel_acesso"),
                            Ativo = reader.GetBoolean("ativo")
                        };
                    }
                }
            }

            return null;
        }
    }
}