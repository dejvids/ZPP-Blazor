using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Lectures
{
    [Route("/zajecia")]
    public class LecturesEmptyComponent : BaseComponent
    {
        protected override void OnInitialized()
        {
            UriHelper.NavigateTo("/zajecia/strona/1");
        }
    }

    public partial class LecturesComponent : BaseComponent
    {
        protected string Phrase { get; set; }
        protected OrderOption Order { get; set; }

        public int CurrentPage { get; set; }

        public int Pages { get; set; }
        [Parameter]
        public string strona { get; set; }

        [Inject]
        protected ILectureService _lectureService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public bool IsDataLoaded { get; set; }

        public List<Models.Lecture> Lectures { get; private set; } = new List<Models.Lecture>();
        public LecturesComponent()
        {
        }
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Lectures = new List<Models.Lecture>() { new Models.Lecture() };

            var uri = new Uri(UriHelper.Uri);
            Phrase = QueryHelpers.ParseQuery(uri.Query).TryGetValue("wyszukaj", out StringValues phrase) ? phrase.First() : "";
            CurrentPage = int.TryParse(QueryHelpers.ParseQuery(uri.Query).TryGetValue("strona", out StringValues page) ? page.First() : "1", out int npage) ? npage : 1;
            if (CurrentPage < 1)
                CurrentPage = 1;
            this.StateHasChanged();
            if (_lectureService != null)
            {
                await LoadData();
            }
        }

        private async Task LoadData()
        {
#if Debug
            Console.WriteLine("Loading lectures");
            Console.WriteLine("Current page " + CurrentPage);
            Console.WriteLine("Order " + Order);
            Console.WriteLine("Phrase " + Phrase); 
#endif
            IsDataLoaded = false;
            StateHasChanged();
            try
            {
                Lectures = (await _lectureService.GetLectures(CurrentPage, Phrase, Order)).ToList();
                await GetNumberOfPages();
#if Debug
                Console.WriteLine("Pages " + Pages); 
#endif
                StateHasChanged();
#if Debug
                Console.WriteLine("Lectures loaded"); 
#endif
            }
            catch (Exception ex)
            {
#if Debug
                Console.WriteLine(ex.Message); 
#endif
            }
            IsDataLoaded = true;
            StateHasChanged();
        }

        private async Task GetNumberOfPages()
        {
            var result = await Http.GetAsync(AppCtx.BaseAddress + $"/api/lectures/results?phrase={Phrase}");
            if (result == null)
                return;
            int items = int.TryParse(await result.Content.ReadAsStringAsync(), out int numberOfitems) ? numberOfitems : 1;
            Pages = (int)Math.Ceiling(items / 6.0);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            CurrentPage = int.TryParse(strona, out int page) ? page : 1;
            StateHasChanged();
#if Debug
            Console.WriteLine("Current page " + CurrentPage); 
#endif
            await LoadData();
        }

        public async void OnSearch()
        {
            await LoadData();
        }

        public async Task OnKeyPressed(KeyboardEventArgs e)
        {
            if (e.Key.Equals("Enter", StringComparison.InvariantCultureIgnoreCase))
            {
                Phrase = await JSRuntime.InvokeAsync<string>("getSearchValue");
                await LoadData();
            }
        }

        public async Task SortByDate()
        {
            if (Order == OrderOption.date)
                return;
            await SortList(OrderOption.date);
        }

        public async Task SortByDateDesc()
        {
            if (Order == OrderOption.date_desc)
                return;
            await SortList(OrderOption.date_desc);
        }

        private async Task SortList(OrderOption order)
        {
            Order = order;
            CurrentPage = 1;
            await LoadData();
            strona = CurrentPage.ToString();
            StateHasChanged();
        }

        public async Task SortByName()
        {
            if (Order == OrderOption.name)
                return;
            await SortList(OrderOption.name);
        }

        public async Task SoryByNameDesc()
        {
            if (Order == OrderOption.name_desc)
                return;
            await SortList(OrderOption.name_desc);
        }
    }
}