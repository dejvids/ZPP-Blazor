using System;
namespace ZPP_Blazor.Models
{
    public class UserLecture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public bool Present { get; set; }
        public bool Marked { get; set; }
    }
}