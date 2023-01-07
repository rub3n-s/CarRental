using A.Models;

namespace A.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string RoleId { get; set; }        
        public string RoleName { get; set; }
        public bool Selected { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
    }
}
