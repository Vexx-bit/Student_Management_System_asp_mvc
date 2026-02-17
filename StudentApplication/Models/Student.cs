namespace StudentApplication.Models
{
    public class Student
    {
        public int Id { get; set; } // The Primary Key
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Course { get; set; }
        public int Age { get; set; }
    }
}