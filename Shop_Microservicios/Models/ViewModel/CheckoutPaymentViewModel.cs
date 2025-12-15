using System.ComponentModel.DataAnnotations;

public class CheckoutPaymentViewModel
{
    [Required]
    public long OrderId { get; set; }

    [Required(ErrorMessage = "Nombre del titular requerido")]
    public string CardHolder { get; set; } = "";

    [Required(ErrorMessage = "Número de tarjeta requerido")]
    public string CardNumber { get; set; } = "";

    [Required(ErrorMessage = "Vencimiento requerido")]
    public string Expiry { get; set; } = "";

    [Required(ErrorMessage = "CVV requerido")]
    public string Cvv { get; set; } = "";
}
