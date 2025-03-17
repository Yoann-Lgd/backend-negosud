namespace backend_negosud.Models;

public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
}