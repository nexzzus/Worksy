using Worksy.Web.DTOs;

namespace Worksy.Web.ViewModels
{
    public class ProfileViewModel
    {
        public UpdateProfileDTO User { get; set; }
        public ChangePasswordViewModel ChangePassword { get; set; }
    }

}