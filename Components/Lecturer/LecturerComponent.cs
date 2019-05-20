using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Lecturer
{
    public class LecturerComponent : AppComponent
    {
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

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            Console.WriteLine("Navigated to me");
            await LoadUserDataAsync();
            await LoadUseLectures();
            this.StateHasChanged();
        }

        private async Task LoadUserDataAsync()
        {
            if (Http == null)
            {
                Console.WriteLine("Http null");
                return;
            }
            Console.WriteLine("AuthorizatioN: " + Http.DefaultRequestHeaders.Authorization);
            var response = await Http.GetAsync(@"/api/me");
            Console.WriteLine("request sent");

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Load user OK");
                string jsonContent = await response.Content.ReadAsStringAsync();
                User = Json.Deserialize<User>(jsonContent);
                AppCtx.CurrentUser = User;
            }

            else if (response == null)
            {
                Console.WriteLine("No response");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Unauthorized");
                UriHelper.NavigateTo("/");
            }
        }

        private async Task LoadUseLectures()
        {
            try
            {
                UserLectures = (await _lectureService.GetMyLectures()).ToList();
                Console.WriteLine("Futere lectures " + UserLectures.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                FutureLectures.Remove(SelectedLecture);
            }
        }

        protected async Task CheckAbsence(UserLecture lecture)
        {
            await SaveLectureCode(lecture);
            ShowCode = true;

        }

        private async Task SaveLectureCode(UserLecture lecture)
        {
            SelectedLecture = lecture;
            if(!string.IsNullOrEmpty(lecture.Code))
            {
                StateHasChanged();
                return;
            }
            var code = new VerificationCode()
            {
                LectureId = lecture.Id,
                ValidTo = lecture.Date.AddMinutes(100)
            };
            var content = new StringContent(Json.Serialize(code), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var result = await Http.PostAsync("/api/presence/code", content);
                var jsonResult = await result.Content.ReadAsStringAsync();
                if(!string.IsNullOrEmpty(jsonResult))
                {
                    var c = Json.Deserialize<VerificationCode>(jsonResult);
                    lecture.Code = c.Code;
                    StateHasChanged();
                }
                Console.WriteLine(jsonResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected void ShowOpinions(Models.UserLecture lecture)
        {

        }
    }
}
