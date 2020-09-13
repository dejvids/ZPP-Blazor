using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZPP_Blazor.Extensions;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Opinions
{
    public partial class OpinionsComponent
    { 
        [Inject]
        protected IOpinionService OpinionService { get; set; }
        [Inject]
        private ILectureService lectureService { get; set; }
        protected List<Opinion> Opinions { get; set; } = new List<Opinion>();
        [Parameter]
        public string Id { get; set; }
        private int lectureId;

        public Models.Lecture SelectedLecture { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var token = await LocalStorage.GetItemAsync<JsonWebToken>("token");

            if (token == null || token.Expires < DateTime.Now.ToTimestamp() || !token.Role.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/konto");
                return;
            }

            lectureId = int.TryParse(Id, out int id) ? id : -1;

            SelectedLecture = await lectureService.GetLecture(lectureId);
            StateHasChanged();
            try
            {
                Opinions = await OpinionService.GetOpinions(lectureId);
                StateHasChanged();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
