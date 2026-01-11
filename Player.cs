using System;

namespace RH_STAFF
{
    public class Player
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string RealName { get; set; }
        public int Age { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Notes { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastAccess { get; set; }
        public string Telefone { get; internal set; }
        public DateTime DataCadastro { get; internal set; }
        public string Nome { get; internal set; }

        public Player()
        {
            Nickname = "";
            RealName = "";
            Country = "";
            Status = "Ativo";
            Email = "";
            Phone = "";
            Notes = "";
            RegistrationDate = DateTime.Now;
            LastAccess = DateTime.Now;
        }
    }
}