namespace backend_negosud.DTOs.Commande_client;

public class CheckoutStripeInputDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public int CommandeId { get; set; }
}