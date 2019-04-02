using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Lectures
{
    public class LecturesComponent : BaseComponent
    {
        [Parameter]
        protected string Phrase { get; set; }

        [Inject]
        protected ILectureService _lectureService { get; set; }
        public LecturesComponent()
        {
        }
        protected async override Task OnInitAsync()
        {
            await base.OnInitAsync();
            var uri = new Uri(UriHelper.GetAbsoluteUri());
            Phrase = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("wyszukaj", out Microsoft.Extensions.Primitives.StringValues phrase) ? phrase.First() : "";
            this.StateHasChanged();
            if (_lectureService != null)
                await LoadData();
        }

        private async Task LoadData()
        {
            IEnumerable<Models.Lecture> lectures = Enumerable.Empty<Models.Lecture>();
            try
            {
                lectures = await _lectureService.GetLectures(1, Phrase, Enums.OrderOption.date);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            foreach (var lecture in lectures)
            {
                Console.WriteLine(lecture.Name);
            }
        }
    }
}
