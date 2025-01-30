namespace backend_negosud.Enums;

public enum StatutCommande
{
    Panier = 0,   // coomande en cours (panier temporaire)
    EnCours = 1,  // commande validée mais pas encore traitée
    Expédiée = 2, // commande en cours de livraison
    Livrée = 3,   // commande livrée au client
    Annulée = 4   // commande annulée
}