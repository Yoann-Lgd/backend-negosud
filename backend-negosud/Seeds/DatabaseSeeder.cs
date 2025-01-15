using backend_negosud.entities;
using Bogus;
using Supabase.Postgrest.Attributes;

namespace backend_negosud.Seeds;

public class DatabaseSeeder
{
    private readonly PostgresContext _context;

    public DatabaseSeeder(PostgresContext context)
    {
        _context = context;
    }

    public void SeedDatabase()
    {
        if (!_context.Clients.Any())
        {
            // table : Client
        var clientFaker = new Faker<Client>("fr")
            .RuleFor(c => c.Nom, f => f.Name.LastName())
            .RuleFor(c => c.Prenom, f => f.Name.FirstName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.Tel, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.MotDePasse, f => f.Internet.Password(20))
            .RuleFor(c => c.EstValide, f => f.Random.Bool());

        var clients = clientFaker.Generate(50);
        _context.Clients.AddRange(clients);
        _context.SaveChanges();

            // table : Adresse
        // var adresseFaker = new Faker<Adresse>()
        //     .RuleFor(a => a.Numero, f => int.Parse(f.Address.BuildingNumber()))
        //     .RuleFor(a => a.Ville, f => f.Address.City())
        //     .RuleFor(a => a.CodePostal, f => int.Parse(f.Address.ZipCode()))
        //     .RuleFor(a => a.Departement, f => f.Address.County())
        //     .RuleFor(a => a.ClientId, f => f.PickRandom(clients).ClientId);
        //
        // var adresses = adresseFaker.Generate(100);
        // _context.Adresses.AddRange(adresses);
        // _context.SaveChanges();
        //
        //  // table :Pays
        // var paysFaker = new Faker<Pays>()
        //     .RuleFor(p => p.Nom, f => f.Address.Country());
        //
        // var pays = paysFaker.Generate(20);
        // _context.Pays.AddRange(pays);
        // _context.SaveChanges();
        //
        //  // table : Article
        // var articleFaker = new Faker<Article>()
        //     .RuleFor(a => a.Libelle, f => f.Commerce.ProductName())
        //     .RuleFor(a => a.Reference, f => f.Commerce.Ean13())
        //     .RuleFor(a => a.Prix, f => double.Parse(f.Commerce.Price()));
        //
        // var articles = articleFaker.Generate(30);
        // _context.Articles.AddRange(articles);
        // _context.SaveChanges();
        //
        //  // table : Fournisseur
        // var fournisseurFaker = new Faker<Fournisseur>()
        //     .RuleFor(f => f.Nom, f => f.Company.CompanyName())
        //     .RuleFor(f => f.RaisonSociale, f => f.Company.CompanySuffix())
        //     .RuleFor(f => f.Email, f => f.Internet.Email())
        //     .RuleFor(f => f.Tel, f => f.Phone.PhoneNumber());
        //
        // var fournisseurs = fournisseurFaker.Generate(20);
        // _context.Fournisseurs.AddRange(fournisseurs);
        // _context.SaveChanges();
        //
        //  // table : Image
        // var imageFaker = new Faker<Image>()
        //     .RuleFor(i => i.ArticleId, f => f.Random.Int(1, 100))
        //     .RuleFor(i => i.Format, f => f.Random.AlphaNumeric(10))
        //     .RuleFor(i => i.Libelle, f => f.Lorem.Sentence())
        //     .RuleFor(i => i.Slug, f => f.Lorem.Slug());
        //
        // var images = imageFaker.Generate(100);
        // _context.Images.AddRange(images);
        // _context.SaveChanges();
        //
        // // table : Inventorier
        // var inventorierFaker = new Faker<Inventorier>()
        //     .RuleFor(i => i.UtilisateurId, f => f.Random.Int(1, 50))
        //     .RuleFor(i => i.StockId, f => f.Random.Int(1, 100))
        //     .RuleFor(i => i.DateModification, f => f.Date.Past())
        //     .RuleFor(i => i.QuantitePostModification, f => f.Random.Int(0, 1000))
        //     .RuleFor(i => i.QuantitePrecedente, f => f.Random.Int(0, 1000))
        //     .RuleFor(i => i.TypeModification, f => f.Random.Word());
        //
        // var inventoriers = inventorierFaker.Generate(50);
        // _context.Inventoriers.AddRange(inventoriers);
        // _context.SaveChanges();
        //
        // // table : LigneBonCommande
        // var ligneBonCommandeFaker = new Faker<LigneBonCommande>()
        //     .RuleFor(l => l.ArticleId, f => f.Random.Int(1, 100))
        //     .RuleFor(l => l.BonCommandeId, f => f.Random.Int(1, 50))
        //     .RuleFor(l => l.LigneLivraisonId, f => f.Random.Int(1, 20))
        //     .RuleFor(l => l.PrixUnitaire, f => double.Parse(f.Commerce.Price()))
        //     .RuleFor(l => l.Quantite, f => f.Random.Int(1, 50));
        //
        // var ligneBonCommandes = ligneBonCommandeFaker.Generate(200);
        // _context.LigneBonCommandes.AddRange(ligneBonCommandes);
        // _context.SaveChanges();
        //
        // // table : Stock
        // var stockFaker = new Faker<Stock>()
        //     .RuleFor(s => s.ArticleId, f => f.Random.Int(1, 100))
        //     .RuleFor(s => s.Quantite, f => f.Random.Int(0, 1000))
        //     .RuleFor(s => s.RefLot, f => f.Random.AlphaNumeric(10))
        //     .RuleFor(s => s.SeuilMinimum, f => f.Random.Int(0, 50))
        //     .RuleFor(s => s.ReapprovisionnementAuto, f => f.Random.Bool());
        //
        // var stocks = stockFaker.Generate(100);
        // _context.Stocks.AddRange(stocks);
        // _context.SaveChanges();
        //
        // // table : TVA
        // var tva = new List<Tva>
        // {
        //     new Tva { Valeur = 20.0 },
        //     new Tva { Valeur = 10.0 },
        //     new Tva { Valeur = 5.5 },
        //     new Tva { Valeur = 2.1 }
        // };
        //
        // _context.Tvas.AddRange(tva);
        // _context.SaveChanges();
        }
    }
}