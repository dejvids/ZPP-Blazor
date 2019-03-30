using ZPP_Blazor.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components.Home
{
    public class HomeComponent : BaseComponent
    {
        public List<Models.Lecture> Lectures { get; set; }
        public List<Models.Lecture> PromotingLectures { get; set; }
        public bool DataLoaded { get; set; }

        public HomeComponent()
        { }

        protected override async Task OnInitAsync()
        {
            PromotingLectures = new List<Models.Lecture> { new Models.Lecture(), new Models.Lecture(), new Models.Lecture() };
            await base.OnInitAsync();
            Console.WriteLine("OnInit Home component");
            if (Http == null)
            {
                Console.WriteLine("Http is null");
            }
            Console.WriteLine(AppCtx.BaseAddress);
            var response = await Http.GetAsync("/api/lectures");
            DataLoaded = true;
            if (response == null)
            {
                Console.WriteLine("Result is null");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Lectures = Json.Deserialize<List<Models.Lecture>>(await response.Content.ReadAsStringAsync());
                Console.WriteLine("Pobranych wyk�ad�w " + Lectures.Count);
                if (Lectures.Count >= 3)
                {
                    PromotingLectures = Lectures.Take(3).ToList();
                }
            }
        }
    }
}