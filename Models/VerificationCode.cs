using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP_Blazor.Models
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public int LectureId { get; set; }
        public DateTime ValidTo { get; set; }
        public string Code { get; set; }
    }
}
