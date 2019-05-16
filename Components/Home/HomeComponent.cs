using ZPP_Blazor.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Home
{
    public class HomeComponent : BaseComponent
    {
        [Inject]
        protected ILectureService _lectureService { get; set; }
        public List<Models.Lecture> Lectures { get; set; }
        public IEnumerable<Models.Lecture> PromotingLectures { get; set; }

        public Models.Lecture FirstPromoting
            => PromotingLectures?.ElementAt<Models.Lecture>(0);
        public Models.Lecture SecondPromoting
           => PromotingLectures?.ElementAt<Models.Lecture>(1);
        public Models.Lecture ThirdPromoting
           => PromotingLectures?.ElementAt<Models.Lecture>(2);
        public List<Models.Lecture> SearchedLectures { get; set; }
        public bool DataLoaded { get; set; }
        public bool Searched { get; set; }
        public string Phrase { get; set; }

        public HomeComponent()
        { }

        protected override async Task OnInitAsync()
        {
            PromotingLectures = new List<Models.Lecture> { new Models.Lecture(), new Models.Lecture(), new Models.Lecture() };
            SearchedLectures = new List<Models.Lecture>();
            await base.OnInitAsync();
            Console.WriteLine("OnInit Home component");
            if (Http == null)
            {
                Console.WriteLine("Http is null");
            }
            try
            {
                PromotingLectures = await _lectureService.GetPromotingLectures();
                DataLoaded = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task Search()
        {
            Console.WriteLine("Submit " + Phrase);
            if (string.IsNullOrEmpty(Phrase) || Phrase.Length < 3)
            {
                Searched = false;
                return;
            }

            try
            {
                var lectures = await _lectureService.GetLectures(1, Phrase, Enums.OrderOption.name);
                if (lectures.Count() >= 1)
                {
                    SearchedLectures = lectures.Take(4).ToList();
                    Searched = true;
                    this.StateHasChanged();
                }
                else
                {
                    SearchedLectures.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void OnKeyPressed(UIKeyboardEventArgs e)
        {
            Phrase = await JSRuntime.Current.InvokeAsync<string>("getSearchValue");
            if (string.IsNullOrEmpty(Phrase) || Phrase.Count() < 4)
            {
                Searched = false;
                StateHasChanged();
            }
            if (e.Key.Equals("Enter", StringComparison.InvariantCultureIgnoreCase) && Phrase?.Count() >= 3)
            {
                await Search();
                Searched = true;
                StateHasChanged();
            }
            else
            {
                Phrase = await JSRuntime.Current.InvokeAsync<string>("getSearchValue");
                if (Phrase?.Count() >= 3)
                {
                    await Search();
                    Searched = true;
                    StateHasChanged();
                }

            }
        }
    }
}