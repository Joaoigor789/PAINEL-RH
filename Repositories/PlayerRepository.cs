using MySql.Data.MySqlClient;
using RH_STAFF.Models;
using System;
using System.Collections.Generic;

namespace RH_STAFF.Repositories
{
    public class PlayerRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public PlayerRepository()
        {
            _dbConnection = new DatabaseConnection();
        }

        public List<Player> GetAllPlayers()
        {
            var players = new List<Player>();

            using (var conn = _dbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM players", conn); // ajuste o nome da tabela

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        players.Add(new Player
                        {
                            Id = reader.GetInt32("id"),
                            Nome = reader.GetString("nome"),
                            Email = reader.GetString("email"),
                            Telefone = reader.GetString("telefone"),
                            DataCadastro = reader.GetDateTime("data_cadastro"),
                            Status = reader.GetString("status")
                        });
                    }
                }
            }

            return players;
        }

        public bool AddPlayer(Player player)
        {
            try
            {
                using (var conn = _dbConnection.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO players (nome, email, telefone, data_cadastro, status) " +
                        "VALUES (@nome, @email, @telefone, @data_cadastro, @status)", conn);

                    cmd.Parameters.AddWithValue("@nome", player.Nome);
                    cmd.Parameters.AddWithValue("@email", player.Email);
                    cmd.Parameters.AddWithValue("@telefone", player.Telefone);
                    cmd.Parameters.AddWithValue("@data_cadastro", player.DataCadastro);
                    cmd.Parameters.AddWithValue("@status", player.Status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                // Log do erro
                return false;
            }
        }

        // Outros métodos: Update, Delete, GetById, etc.
    }
}