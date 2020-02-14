using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Lecture
{
    public partial class LectureComponent
    {
        [Inject]
        protected ILectureService _lectureService { get; set; }
        public Models.Lecture CurrentLecture { get; set; } = new Models.Lecture();
        public bool HasJoined { get; set; }
        public string Message { get; set; }
        public bool HasError { get; private set; }
        [Parameter]
        public string Id { get; set; }
        public bool? UserAlreadyJoined { get; private set; }
        public bool Finished { get; set; }

        public LectureComponent()
        {
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Console.WriteLine("Id = " + Id);
            try
            {
                CurrentLecture = await _lectureService.GetLecture(int.Parse(Id));
                if(CurrentLecture.Date <= DateTime.Now)
                {
                    Finished = true;
                }
                if (IsSigned)
                {
                    Console.WriteLine("IsSigned");
                    var userLectures = await _lectureService.GetMyLectures();
                    foreach (var l in userLectures)
                    {
                        Console.WriteLine(l.Id);
                    }
                    if (userLectures.Any(x => x.Id == int.Parse(Id)))
                    {
                        Console.WriteLine("Is joined");
                        UserAlreadyJoined = true;
                    }
                    else
                        UserAlreadyJoined = false;
                }
                else
                    UserAlreadyJoined = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UriHelper.NavigateTo("/zajecia/strona/1");
            }
        }

        protected async Task Join()
        {
            var content = new StringContent(JsonSerializer.Serialize(new { LectureId = Id }), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await Http.PostAsync("/api/lectures/participants/add-me", content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("Joined to lecture ");
                    Message = "Właśnie zapisałeś się na zajęcia!";
                    HasJoined = true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Message = await response.Content.ReadAsStringAsync();
                    HasError = true;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    UriHelper.NavigateTo("/logowanie");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Message = "Wystąpił nieoczekiwany błąd serwera :(";
                HasError = true;
            }
            finally
            {
                StateHasChanged();
            }
        }
    }
}
