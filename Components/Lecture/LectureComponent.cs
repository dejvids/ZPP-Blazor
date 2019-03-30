using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components.Lecture
{
    public class LectureComponent : BaseComponent
    {
        public Models.Lecture CurrentLecture { get; set; } = new Models.Lecture();

        [Parameter]
        public string Id { get; set; }
        public LectureComponent()
        {
        }
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            Console.WriteLine("Id = " + Id);
            string endpoint = $"{AppCtx.BaseAddress}/api/lectures/{Id}";
            Console.WriteLine(endpoint);
            var result = await Http.GetAsync(endpoint);
            string content = await result.Content.ReadAsStringAsync();
            CurrentLecture = Json.Deserialize<Models.Lecture>(content);
        }
    }
}
