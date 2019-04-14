using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public interface ILectureService
    {
        Task<IEnumerable<Lecture>> GetPromotingLectures();
        Task<IEnumerable<Lecture>> GetLectures(int page, string phrase, OrderOption order);
        Task<Lecture> GetLecture(int id);
        Task<IEnumerable<UserLecture>> GetMyLectures();
    }
}
