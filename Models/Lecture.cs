using System;

namespace ZPP_Blazor.Models
{
    public class Lecture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public int LecturerId { get; set; }
        public string LecturerName { get; set; }
        public string LecturerSurname { get; set; }
        public string LecturerFullName
            => $"{LecturerName} {LecturerSurname}";
        public int NumberOfParticipants {get; set;}
    }

}