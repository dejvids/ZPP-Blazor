using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP_Blazor.Models
{
    public class Opinion
    {
        public int LectureId { get; set; }
        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"Ocena tematu: {SubjectMark}/5, Ocenac wykładowcy: {LecturerMark}/5 Szamsa ma polecenie innym: {RecommendationChance}/5, komentarz: {Comment}";
        }
    }
}
