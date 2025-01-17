namespace backend_negosud.Models;

public class ResponseModel : IResponseModel
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? TokenJWT { get; set; }
    
    public int StatusCode { get; set; }
}
public class ResponseDataModel<T> : ResponseModel, IResponseDataModel<T> where T : class
{
    public T Data { get; set; } = null!;
}