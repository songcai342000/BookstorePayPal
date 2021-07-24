using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayPalPaymentIntergration.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Name is required"), StringLength(100), Display(Name = "Name")]
        public string Name { get; set; }
       
        [Required(ErrorMessage = "Email Address is required")]
        [Display(Name="Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
        ErrorMessage = "Email is is not valid.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }
        [Required(ErrorMessage = "Postal Code is required"), Display(Name = "Postal Code")]
        [StringLength(40)]
        public string PostNumber { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Mobil Number is required")]
        [StringLength(24)]
        public string Mobil { get; set; }
        public virtual List<Order> Orders { get; set; }
    }
}
