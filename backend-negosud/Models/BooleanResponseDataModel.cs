namespace backend_negosud.Models;

public class BooleanResponseDataModel : IResponseModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public bool Data { get; set; }
}
