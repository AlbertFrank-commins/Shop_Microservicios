using System.ComponentModel.DataAnnotations;

namespace Shop_Microservicios.Models.ViewModels.Checkout
{
    public class CheckoutAddressViewModel
    {
        [Required]
        public long OrderId { get; set; }

        [Required(ErrorMessage = "Nombre requerido")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Dirección requerida")]
        public string AddressLine1 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }
    }
}
