namespace backend_negosud.Services;

public interface IHashMotDePasseService
{
    string HashMotDePasse(string motDePasse);
    
    bool VerifyMotDePasse(string motDePasse, string hashedmotDePasse);

    public string RandomMotDePasseTemporaire();
}