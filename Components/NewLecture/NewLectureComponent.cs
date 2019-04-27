﻿using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Extensions;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components.NewLecture
{
    public class NewLectureComponent : AppComponent
    {
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
            var token = await LocalStorage.GetItem<JsonWebToken>("token");

            if (token == null || !token.Role.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/");
                return;
            }

            await base.OnInitAsync();
            await OnAfterRenderAsync();
        }

        public async Task Save()
        {
            IsAlertVisible = false;
            if (!(await ValidateForm()))
            {
                IsAlertVisible = true;
                return;
            }
            StateHasChanged();
            var newLecture = new Models.Lecture()
            {
                Name = this.Name,
                Date = this.Date,
                Description = this.Description,
                Place = this.Place
            };

            Console.WriteLine(AppCtx.CurrentUser?.Id);

            var content = new StringContent(Json.Serialize(newLecture), System.Text.Encoding.UTF8, "application/json");
            Processing = true;
            StateHasChanged();
            try
            {
                var response = await Http.PostAsync("/api/lectures", content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ShowDialog = true;
                    StateHasChanged();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string message = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(message);
                    Message = message;
                    IsAlertVisible = true;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Adding lecture failed " + ex.Message);
            }
            finally
            {
                Processing = false;
                StateHasChanged();
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
            Console.WriteLine("Date is " + Date + "Today "+DateTime.Today);
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
            }
        }
    }
}