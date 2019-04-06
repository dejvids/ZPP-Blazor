using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Lecture
{
    public class LectureComponent : BaseComponent
    {
        [Inject]
        protected ILectureService _lectureService { get; set; }
        public Models.Lecture CurrentLecture { get; set; } = new Models.Lecture();

        [Parameter]
        protected string Id { get; set; }
        public LectureComponent()
        {
        }
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            Console.WriteLine("Id = " + Id);
            try
            {
                CurrentLecture = await _lectureService.GetLecture(int.Parse(Id));
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UriHelper.NavigateTo("/zajecia/strona/1");
            }
        }
    }
}
