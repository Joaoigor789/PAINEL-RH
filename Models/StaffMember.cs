namespace RH_STAFF.Models
{
    public class StaffMember
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string Usuario { get; set; }
        public string NivelAcesso { get; set; }
        public bool Ativo { get; set; }
    }
}