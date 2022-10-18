namespace Domain.Entities
{
    public class Contact
    {
        public int ID { get; set; }
        public string mobile { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string? email { get; set; }
        public string? group { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public byte is_starred { get; set; }
        public byte is_deleted { get; set; }
    }
}
