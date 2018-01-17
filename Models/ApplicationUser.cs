using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    //Define como a tabela deve ser criado no banco, Herda de IdentityUser que possui vários atributos que também são inseridos no banco
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}