using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using A.Models;
using A.ViewModels;
using A.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace A.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();

            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (ApplicationUser user in users)
            {
                var utilizadorVM = new UserRolesViewModel();
                utilizadorVM.UserId = user.Id;
                utilizadorVM.Email = user.Email;
                utilizadorVM.PrimeiroNome = user.PrimeiroNome;
                utilizadorVM.UltimoNome = user.UltimoNome;
                utilizadorVM.EmpresaId = user.EmpresaId;
                utilizadorVM.Empresa = user.Empresa;
                utilizadorVM.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(utilizadorVM);
            }
            return View(userRolesViewModel);
        }

        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> IndexGestor()
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var gestor = _context.Users.Where(x => x.Id == applicationUserId).First();
            var empresa = gestor.EmpresaId;

            var users = await _userManager.Users.Where(u => u.EmpresaId == empresa).ToListAsync();

            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (ApplicationUser user in users)
            {
                var utilizadorVM = new UserRolesViewModel();
                utilizadorVM.UserId = user.Id;
                utilizadorVM.Email = user.Email;
                utilizadorVM.PrimeiroNome = user.PrimeiroNome;
                utilizadorVM.UltimoNome = user.UltimoNome;
                utilizadorVM.EmpresaId = user.EmpresaId;
                utilizadorVM.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(utilizadorVM);
            }
            return View(userRolesViewModel);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
        
        [Authorize(Roles = "Admin,Gestor")]
        public async Task<IActionResult> Details(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                //return View("NotFound");
                return View();
            }

            ViewBag.UserName = user.UserName;

            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        
        [Authorize(Roles = "Admin,Gestor")]
        [HttpPost]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");


        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _userManager.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var utilizadorVM = new UserRolesViewModel();
            utilizadorVM.UserId = user.Id;
            utilizadorVM.Email = user.Email;
            utilizadorVM.PrimeiroNome = user.PrimeiroNome;
            utilizadorVM.UltimoNome = user.UltimoNome;


            return View(utilizadorVM);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,Email,PrimeiroNome,UltimoNome,EmpresaId,UserName,Roles")] UserRolesViewModel usersViewModel)
        {
            var user = await _userManager.FindByIdAsync(id);
            ModelState.Remove(nameof(user.Empresa));
            ModelState.Remove(nameof(user.EmpresaId));
            ModelState.Remove(nameof(user.UserName));
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            user.PrimeiroNome = usersViewModel.PrimeiroNome;
            user.UltimoNome = usersViewModel.UltimoNome;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Não foi possível alterar o registo deste utilizador!");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
   

