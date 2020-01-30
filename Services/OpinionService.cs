using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public class OpinionService: IOpinionService
    {
        private HttpClient _http;

        public OpinionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Opinion>> GetOpinions(int lectureId)
        {
            var list = new List<Opinion>();
            try
            {
                var result = await _http.GetAsync($"/api/opinions/lecture/{lectureId}");
                if(result != null)
                {
                    var content = await result.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<Opinion>>(content);
                }
                return list;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
