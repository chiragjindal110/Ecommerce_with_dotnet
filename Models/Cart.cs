using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public partial class Cart
{
    
    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual User? User { get; set; }
}
