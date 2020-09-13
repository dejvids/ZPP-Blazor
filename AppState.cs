using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP_Blazor
{
    public class AppState
    {
        public bool IsSignedIn { get; private set; }

        public event Action OnChange;

        public void SetSignInStatus(bool isSignedIn)
        {
            IsSignedIn = isSignedIn;
            RaiseStateChanged();
        }

        private void RaiseStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}
