using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public class LectureService : ILectureService
    {
        public HttpClient _http;
        public LectureService(HttpClient http)
        {
            this._http = http;
        }

        public async Task<Lecture> GetLecture(int id)
        {
            // string endpoint = $"{AppCtx.BaseAddress}/api/lectures/{Id}";
            try
            {
                var result = await _http.GetAsync($"/api/lectures/{id}");
                if (result == null)
                    throw new Exception("Błąd serwera");
                string content = await result.Content.ReadAsStringAsync();
                return Json.Deserialize<Models.Lecture>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Błąd podczas pobierania danych");
            }
        }

        public async Task<IEnumerable<Lecture>> GetLectures(int page, string phrase, OrderOption order)
        {
            var response = await _http.GetAsync($"/api/lectures?page={page.ToString()}&phrase={phrase}&order={order.ToString()}");
            if (response == null)
            {
                throw new Exception("Błąd odpowiedzi serwera");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var lectures = Json.Deserialize<List<Models.Lecture>>(await response.Content.ReadAsStringAsync());
                    return lectures;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<Lecture>();
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            return new List<Lecture>();
        }

        public async Task<IEnumerable<Lecture>> GetPromotingLectures()
        {
            var response = await _http.GetAsync($"/api/lectures/promoting");
            if (response == null)
            {
                throw new Exception("Błąd odpowiedzi serwera");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var lectures = Json.Deserialize<List<Models.Lecture>>(await response.Content.ReadAsStringAsync());
                    return lectures;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new List<Lecture>();
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            return new List<Lecture>();
        }

        public async Task<IEnumerable<UserLecture>> GetMyLectures()
        {
            var response = await _http.GetAsync("/api/lectures/mine");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    return Json.Deserialize<List<UserLecture>>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            return new List<UserLecture>();
        }

        public async Task DeleteLecture(int lectureId)
        {
            try
            {
                var reponse = await _http.DeleteAsync($"/api/lectures/{lectureId}");
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateLecture(Lecture lecture)
        {
            var content = new StringContent(Json.Serialize(lecture), System.Text.Encoding.UTF8, "application/json");
            try
            {
                var response = await _http.PutAsync($"/api/lectures/{lecture.Id}", content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return string.Empty;
                }
                else
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
