﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ARPA20250320.AppWebMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ARPA20250320.AppWebMVC.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class UserController : Controller
    {
        private readonly Test20250320DbContext _context;

        public UserController(Test20250320DbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index(User usuario, int topRegistro = 10)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(usuario.Username))
                query = query.Where(s => s.Username.Contains(usuario.Username));
            if (!string.IsNullOrWhiteSpace(usuario.Role))
                query = query.Where(s => s.Role.Contains(usuario.Role));
            if (topRegistro > 0)
                query = query.Take(topRegistro);


            return View(await query.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,PasswordHash,Role,Estatus")] User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = CalcularHashMD5(user.PasswordHash);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        private string CalcularHashMD5(string passwordHash)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(passwordHash);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // "x2" convierte el byte en una cadena hexadecimal de dos caracteres.
                }
                return sb.ToString();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSession()
        {
            // Hola mundo
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(User usuario)
        {
            usuario.PasswordHash = CalcularHashMD5(usuario.PasswordHash);
            var usuarioAuth = await _context.
                Users.
                FirstOrDefaultAsync(s => s.Email == usuario.Email && s.PasswordHash == usuario.PasswordHash);
            if (usuarioAuth != null && usuarioAuth.UserId > 0 && usuarioAuth.Email == usuario.Email)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Name, usuarioAuth.Email),
                    new Claim("UserId", usuarioAuth.UserId.ToString()),
                     new Claim("Username", usuarioAuth.Username),
                    new Claim(ClaimTypes.Role, usuarioAuth.Role)
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "El email o contraseña estan incorrectos");
                return View();
            }
        }


        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,PasswordHash,Role,Estatus")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            var usuarioUpdate = await _context.Users
                 .FirstOrDefaultAsync(m => m.UserId == user.UserId);
            try
            {
                usuarioUpdate.Username = user.Username;
                usuarioUpdate.Email = user.Email;
                usuarioUpdate.Estatus = user.Estatus;
                usuarioUpdate.Role = user.Role;
                _context.Update(usuarioUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    return View(user);
                }
            }
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Profile()
        {

            var idStr = User.FindFirst("UserId")?.Value;
            int id = int.Parse(idStr);
            var usuario = await _context.Users.FindAsync(id);
            return View(usuario);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(int id, [Bind("UserId,Username,Email,Estatus,Role")] User usuario)
        {
            if (id != usuario.UserId)
            {
                return NotFound();
            }
            var usuarioUpdate = await _context.Users
                 .FirstOrDefaultAsync(m => m.UserId == usuario.UserId);
            try
            {
                usuarioUpdate.Username = usuario.Username;
                usuarioUpdate.Email = usuario.Email;
                usuarioUpdate.Estatus = usuario.Estatus;
                usuarioUpdate.Role = usuario.Role;
                _context.Update(usuarioUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(usuario.UserId))
                {
                    return NotFound();
                }
                else
                {
                    return View(usuario);
                }
            }
        }


    }
}
