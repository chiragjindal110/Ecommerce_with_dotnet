using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public partial class User
{
    
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public long Phone { get; set; }

    public string Password { get; set; } = null!;

    public string? ProfilePic { get; set; }

    public string? Address { get; set; }

    public long Token { get; set; }

    public string Verified { get; set; } = null!;
}
