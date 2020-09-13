using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Extensions;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Lecturer
{
    public partial class LecturerComponent
    {
        const int CODE_EXPIRES_MINUTES = 30;
        [Inject]
        protected ILectureService _lectureService { get; set; }
        public User User { get; set; }
        protected List<Models.UserLecture> UserLectures { get; set; }
        protected List<Models.UserLecture> FutureLectures { get; private set; }
        protected List<Models.UserLecture> ActiveLectures { get; private set; }
        protected List<Models.UserLecture> PastLectures { get; private set; }
        public bool LoadedLectures { get; set; }
        public LectureTab SelectedTab { get; set; }
        public bool DeleteConfVisible { get; protected set; }
        public bool SetPresentVisible { get; protected set; }
        public bool SetMarkVisible { get; protected set; }
        public bool SetOpinionVisible { get; set; }
        public string ConfirmationCode { get; set; }
        public Models.UserLecture SelectedLecture { get; set; }
        public bool ShowCode { get; set; }
        public int ExpirationMinutes { get; set; } = 30;
        public bool CodeLoaded { get; set; }
        public bool CodeIsValid { get; set; }
        public DateTime CodeValidTo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var token = await LocalStorage.GetItemAsync<JsonWebToken>("token");

            if (token == null || !token.Role.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/konto");
                return;
            }
            await base.OnInitializedAsync();
#if Debug
            Console.WriteLine("Navigated to me"); 
#endif
            await LoadUserDataAsync();
            await LoadUseLectures();
            this.StateHasChanged();
        }

        private async Task LoadUserDataAsync()
        {
            if (Http == null)
            {
#if Debug
                Console.WriteLine("Http null"); 
#endif
                return;
            }
#if Debug
            Console.WriteLine("AuthorizatioN: " + Http.DefaultRequestHeaders.Authorization); 
#endif
            var response = await Http.GetAsync(@"/api/me");
#if Debug
            Console.WriteLine("request sent"); 
#endif

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
#if Debug
                Console.WriteLine("Load user OK"); 
#endif
                string jsonContent = await response.Content.ReadAsStringAsync();
                User = JsonSerializer.Deserialize<User>(jsonContent, AppCtx.JsonOptions);
                AppCtx.CurrentUser = User;
            }

            else if (response == null)
            {
#if Debug
                Console.WriteLine("No response"); 
#endif
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
#if Debug
                Console.WriteLine("Unauthorized"); 
#endif
                UriHelper.NavigateTo("/");
            }
        }

        private async Task LoadUseLectures()
        {
            try
            {
                UserLectures = (await _lectureService.GetMyLectures()).ToList();
#if Debug
                Console.WriteLine("Futere lectures " + UserLectures.Count); 
#endif
            }
            catch (Exception ex)
            {
#if Debug
                Console.WriteLine(ex.Message); 
#endif
            }

            ActiveLectures = UserLectures.Where(x => x.Date <= DateTime.Now && x.Date.AddDays(30) >= DateTime.Now && !x.Present).ToList();
            FutureLectures = UserLectures.Where(x => x.Date > DateTime.Now).ToList();
            PastLectures = UserLectures.Where(x => x.Date < DateTime.Now && !ActiveLectures.Any(l => l.Id == x.Id)).ToList();
            LoadedLectures = true;
            StateHasChanged();
        }

        protected void ShowDeleteConfirmation(Models.UserLecture lecture)
        {
            SelectedLecture = lecture;
            DeleteConfVisible = true;
            StateHasChanged();
        }

        protected async Task DeleteLecture()
        {
            try
            {
                await _lectureService.DeleteLecture(SelectedLecture.Id);
                DeleteConfVisible = false;
                await LoadUseLectures();
                StateHasChanged();
            }
            catch (Exception ex)
            {
#if Debug
                Console.WriteLine(ex.Message); 
#endif
                FutureLectures.Remove(SelectedLecture);
            }
        }

        protected async Task CheckAbsence(UserLecture lecture)
        {
            ExpirationMinutes = CODE_EXPIRES_MINUTES;
            SelectedLecture = lecture;
            CodeLoaded = false;
            ShowCode = true;
            await GetActiveCode();
            CodeLoaded = true;
        }

        protected async Task GetCode()
        {
            if (ExpirationMinutes < 10)
            {
                ExpirationMinutes = 10;
                StateHasChanged();
            }
            else if (ExpirationMinutes > 300)
            {
                ExpirationMinutes = 300;
                StateHasChanged();
            }
            CodeValidTo = DateTime.Now.AddMinutes(ExpirationMinutes);
            StateHasChanged();
            var code = new VerificationCode()
            {
                LectureId = SelectedLecture.Id,
                ValidTo = DateTime.Now.ToLocalDateTime().AddMinutes(ExpirationMinutes)
            };
            var content = new StringContent(JsonSerializer.Serialize(code), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var result = await Http.PostAsync("/api/presence/code", content);
                var jsonResult = await result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    var c = JsonSerializer.Deserialize<VerificationCode>(jsonResult, AppCtx.JsonOptions);
                    SelectedLecture.Code = c.Code;
                    CodeValidTo = c.ValidTo;
                    CodeIsValid = CodeValidTo > DateTime.Now.ToLocalDateTime();
                    StateHasChanged();
                }
#if Debug
                Console.WriteLine(jsonResult); 
#endif
            }
            catch (Exception ex)
            {
#if Debug
                Console.WriteLine(ex.Message); 
#endif
            }
        }

        protected async Task GetActiveCode()
        {
            var resullt = await Http.GetAsync($"/api/presence/code/{this.SelectedLecture.Id}");
            if (resullt != null && resullt.IsSuccessStatusCode)
            {
                try
                {
                    var response = await resullt.Content.ReadAsStringAsync();
                    var activeCode = JsonSerializer.Deserialize<VerificationCode>(response, AppCtx.JsonOptions);
                    SelectedLecture.Code = activeCode.Code;
                    CodeValidTo = activeCode.ValidTo;
                    CodeIsValid = CodeValidTo > DateTime.Now.ToLocalDateTime();
                    ConfirmationCode = activeCode.Code;
                    CodeLoaded = true;
                    StateHasChanged();
                }
                catch (Exception ex)
                {
#if Debug
                    Console.WriteLine(ex.Message); 
#endif
                }
            }
        }

        protected void Edit(UserLecture lecture)
        {
            this.UriHelper.NavigateTo($"/zajecia/edytuj/{lecture.Id}");
        }

        protected void ShowOpinions(Models.UserLecture lecture)
        {
            UriHelper.NavigateTo($"/opinie/{lecture.Id}");
        }
    }
}
