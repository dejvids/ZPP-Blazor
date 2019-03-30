using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components.SignUp
{
    public class SignUpComponent : BaseComponent
    {
        public string Message { get; set; }
        public bool IsAlertVisible { get; set; }
        public bool SuccessfullyRegistered { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            if(IsSigned)
            {
                UriHelper.NavigateTo("/profil");
            }
        }

        public async Task SignUpAsync()
        {
            if (!ValidateForm())
            {
                IsAlertVisible = true;
                return;
            }

            var user = new SignUpModel { Login = Login, Email = Email, Surname = this.Surname, Name = this.Name, Password = this.Password };
            var content = new StringContent(Json.Serialize(user), System.Text.Encoding.UTF8, "application/json");
            var result = await Http.PostAsync(@"/api/sign-up", content);

            if (result == null)
            {
                Message = "Wystąpił błąd podczas rejestracji";
                return;
            }

            SignUpResult response;
            try
            {
                response = Json.Deserialize<SignUpResult>(await result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Message = "Wystąpił błąd podczas rejestracji";
                return;
            }
            if(!response.Success)
            {
                Message = response.Message;
                return;
            }
            SuccessfullyRegistered = true;
            Message = response.Message;
            IsAlertVisible = true;
            
           // UriHelper.NavigateTo("/logowanie");
        }

        public bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Message = "Wypełnij wszystkie wymagane pola formularza";
                return false;
            }
            if (!char.IsLetter(Login[0]))
            {
                Message = "Login musi rozpoczynać się od litery";
                return false;
            }
            if (!Password.Equals(RepeatedPassword))
            {
                Message = "Hasło i powórzone hasło muszą być takie same";
                return false;
            }
            if (Password.Length < 6)
            {
                Message = "Hasło musi zawierać co najmniej 6 znaków";
                return false;
            }
            if (!Password.Any(c => char.IsDigit(c)))
            {
                Message = "Hasło musi zawierać co najmniej jedną cyfrę";
                return false;
            }
            if (!Password.Any(c => char.IsLetter(c)))
            {
                Message = "Hasło musi zawierać co najmniej jedną literę";
                return false;
            }
            var mail = new System.Net.Mail.MailAddress(Email);
            if(mail == null)
            {
                Message = "Niepoprawny adres e-mail";
                return false;
            }
            return true;
        }
    }
}
