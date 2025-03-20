using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ARPA20250320.AppWebMVC.Models;

public partial class User
{
    [Display(Name = "UsuarioId")]
    public int UserId { get; set; }
    [Display(Name = "UsuarioNombre")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Username { get; set; } = null!;
    [Display(Name = "UsuarioEmail")]
    [Required(ErrorMessage = "El Email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string Email { get; set; } = null!;
    [Display(Name = "UsuarioContraseña")]
    [Required(ErrorMessage = "El password es obligatorio.")]
    [DataType(DataType.Password)]
    [StringLength(40, MinimumLength = 5, ErrorMessage = "El passowrd debe tener entre 5 y 50 caracteres.")]
    public string PasswordHash { get; set; } = null!;
    [Display(Name = "UsuarioRol")]
    public string Role { get; set; } = null!;
    [Display(Name = "UsuarioEstado")]
    public byte? Estatus { get; set; }
    private string strEstatus;
    [NotMapped]
    public string StrEstatus
    {
        get
        {
            if (Estatus == 1)
                strEstatus = "Activo";
            else if (Estatus == 2)
                strEstatus = "Inactivo";
            return strEstatus;
        }
        set { strEstatus = value; }
    }
    [NotMapped]
    [StringLength(40, MinimumLength = 5, ErrorMessage = "El password debe tener entre 5 y 50 caracteres.")]
    [Display(Name = "Confirmar Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmarPassword { get; set; } = null!;
}
