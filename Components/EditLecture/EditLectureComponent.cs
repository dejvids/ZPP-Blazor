using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.EditLecture
{
    public class EditLectureComponent : AppComponent
    {
        [Inject]
        protected ILectureService _lectureService { get; set; }
        [Parameter]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public bool ShowDialog { get; set; }
        public bool IsAlertVisible { get; set; }
        public string Message { get; set; }
        public bool Processing { get; set; }

        protected async override Task OnInitAsync()
        {
            await base.OnInitAsync();

            var lecture = await _lectureService.GetLecture(int.Parse(Id));

            if (lecture != null)
            {
                Name = lecture.Name;
                Description = lecture.Description;
                Date = lecture.Date;
                Place = lecture.Place;
                StateHasChanged();
            }
        }

        protected async Task Save()
        {
            IsAlertVisible = false;
            if (!(await ValidateForm()))
            {
                IsAlertVisible = true;
                return;
            }
            StateHasChanged();
            var lecture = new Models.Lecture()
            {
                Id = int.Parse(Id),
                Name = this.Name,
                Description = this.Description,
                Date = this.Date,
                Place = this.Place
            };
            try
            {
                var result = await _lectureService.UpdateLecture(lecture);
                if(!string.IsNullOrEmpty(result))
                {
                    Message = result;
                }
                else
                {
                    UriHelper.NavigateTo("/wykladowca");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<bool> ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Message = "Nazwa jest wymagana";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                Message = "Opis jest wymagany";
                return false;
            }
            string selectedTimeAsString = await JSRuntime.Current.InvokeAsync<string>("lectures.getLectureTime");
            try
            {
                var time = TimeSpan.Parse(selectedTimeAsString);
                Date = new DateTime(Date.Year, Date.Month, Date.Day, time.Hours, time.Minutes, 0);
            }
            catch
            {
                Message = "Niepoprawna godzina zajęć";
                return false;
            }
            Console.WriteLine("Date is " + Date + "Today " + DateTime.Today);
            if (Date < DateTime.Today.AddDays(1))
            {
                Message = "Niepoprawna data zajęć";
                return false;
            }
            if (string.IsNullOrWhiteSpace(Place))
            {
                Message = "Miejsce jest wymagane";
                return false;
            }
            return true;
        }

        protected async Task SelectedDateChange()
        {
            string selectedDateAsString = await JSRuntime.Current.InvokeAsync<string>("lectures.getLectureDate");

            try
            {
                Date = Convert.ToDateTime(selectedDateAsString);
            }
            catch (Exception ex)
            {
                Date = DateTime.Now;
                Console.WriteLine("Date parse exception" + ex.Message);
            }
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync()
        {
            if (Date != DateTime.MinValue)
            {
                string selectedDateAsString = Date.ToString("yyyy-MM-dd");

                await JSRuntime.Current.InvokeAsync<string>("lectures.setLectureDate", selectedDateAsString);
                await JSRuntime.Current.InvokeAsync<string>("lectures.setLectureTime", Date.ToString("HH:mm:ss"));
            }
        }
    }
}
