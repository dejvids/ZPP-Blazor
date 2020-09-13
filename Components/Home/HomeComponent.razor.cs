using System.Collections.Generic;
using Microsoft.JSInterop;
using System.Linq;
using System;
using System.Threading.Tasks;
using ZPP_Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ZPP_Blazor.Components.Home
{
    public partial class HomeComponent
    {
        [Inject]
        protected ILectureService _lectureService { get; set; }
        public List<Models.Lecture> Lectures { get; set; }
        public IEnumerable<Models.Lecture> PromotingLectures { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        public Models.Lecture FirstPromoting
            => PromotingLectures?.ElementAtOrDefault(0);
        public Models.Lecture SecondPromoting
           => PromotingLectures?.ElementAtOrDefault(1);
        public Models.Lecture ThirdPromoting
           => PromotingLectures?.ElementAtOrDefault(2);
        public List<Models.Lecture> SearchedLectures { get; set; }
        public bool DataLoaded { get; set; }
        public bool Searched { get; set; }
        public string Phrase { get; set; }

        public HomeComponent()
        { }

        protected override async Task OnInitializedAsync()
        {
            PromotingLectures = new List<Models.Lecture> { new Models.Lecture(), new Models.Lecture(), new Models.Lecture() };
            SearchedLectures = new List<Models.Lecture>();
            await base.OnInitializedAsync();
#if Debug
            Console.WriteLine("OnInit Home component"); 
#endif
            if (Http == null)
            {
#if Deubug
                Console.WriteLine("Http is null"); 
#endif
            }
            try
            {
                PromotingLectures = await _lectureService.GetPromotingLectures();
                DataLoaded = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
#if Deubug
                Console.WriteLine(ex.Message); 
#endif
            }
        }

        public async Task Search()
        {
#if Debug
            Console.WriteLine("Submit " + Phrase); 
#endif
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
#if Debug
                Console.WriteLine(ex.Message); 
#endif
            }
        }

        public async void OnKeyPressed(KeyboardEventArgs e)
        {
            Phrase = await JSRuntime.InvokeAsync<string>("getSearchValue");
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
                Phrase = await JSRuntime.InvokeAsync<string>("getSearchValue");
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