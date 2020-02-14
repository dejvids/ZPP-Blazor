using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Student
{

    public partial class StudentComponent
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
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }
        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!AppCtx.CurrentUser.Role.Equals("student", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/konto");
            }
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
                User = JsonSerializer.Deserialize<User>(jsonContent, AppCtx.JsonOptions);
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
            PastLectures = UserLectures.Where(x => x.Date < DateTime.Now && ! ActiveLectures.Any(l=>l.Id == x.Id)).ToList();
            LoadedLectures = true;
            StateHasChanged();
        }

        public async Task QuitLecture()
        {
            Console.WriteLine("Quit " + SelectedLecture);
            var content = new StringContent(SelectedLecture?.Id.ToString(), System.Text.Encoding.UTF8, "application/json");
            try
            {
                var response = await Http.PutAsync("/api/lecture/quit", content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    FutureLectures.Remove(FutureLectures.First(x => x.Id == SelectedLecture?.Id));
                    StateHasChanged();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DeleteConfVisible = false;
                StateHasChanged();
            }
        }

        protected void ShowDeleteConfirmation(Models.UserLecture lecture)
        {
            SelectedLecture = lecture;
            DeleteConfVisible = true;
            StateHasChanged();
        }

        protected void ShowSetPresentDialog(UserLecture lecture)
        {
            Console.WriteLine("Potwierdzenie obecnoœci");
            SelectedLecture = lecture;
            SetPresentVisible = true;
            StateHasChanged();
        }

        protected void ShowSetOpinionDialog(UserLecture lecture)
        {
            Console.WriteLine("");
            SelectedLecture = lecture;
            SetOpinionVisible = true;
            StateHasChanged();
        }

        public async Task SetPresent()
        {
            var code = new VerificationCode();
            code.LectureId = SelectedLecture.Id;
            code.Code = ConfirmationCode;

            var content = new StringContent(JsonSerializer.Serialize(code), System.Text.Encoding.UTF8, "application/json");
           var result =  await Http.PostAsync("api/presence", content);
            if(result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ErrorMessage = await result.Content.ReadAsStringAsync();
                return;
            }
            SetPresentVisible = false;
            await LoadUseLectures();
        }

        public async Task SaveOpinion()
        {
            Console.WriteLine("Opinion set");

            this.SubjectMark = await JSRuntime.InvokeAsync<int>("opinions.getLectureOpinion");
            this.LecturerMark = await JSRuntime.InvokeAsync<int>("opinions.getLecturerOpinion");
            this.RecommendationChance = await JSRuntime.InvokeAsync<int>("opinions.getRecommendationChance");
            

            var opinion = new Opinion()
            {
                LectureId = SelectedLecture.Id,
                SubjectMark = this.SubjectMark,
                LecturerMark = this.LecturerMark,
                RecommendationChance = this.RecommendationChance,
                Comment = this.Comment
            };

            var content = new StringContent(JsonSerializer.Serialize(opinion), System.Text.Encoding.UTF8, "application/json");
            try
            {
                await Http.PostAsync("/api/opinions", content);
                Console.WriteLine("set opinion success");
                await LoadUseLectures();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Error");
            }
            finally
            {
                SetOpinionVisible = false;
                StateHasChanged();
            }
        }
    }
}