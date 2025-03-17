using System.Data;

namespace backend_negosud.Models;

public class CodeEmailDataTable : BaseDataTable
{
    private static CodeEmailDataTable _instance;
    private static readonly object _lock = new object();

    public static CodeEmailDataTable Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new CodeEmailDataTable("TableTemporaireCode");
            }
        }
    }
    public CodeEmailDataTable(string tableName) : base(tableName)
    {
        // On n'ajoute pas ID car il est déjà dans BaseDataTable
        Columns.Add("Email", typeof(string));
        Columns.Add("CodeEmail", typeof(string));  // Attention à la casse !
        Columns.Add("DateCreation", typeof(DateTime)); 
    
        SetPrimaryKeyColumn("ID");
    }
    

    public void AddCodeEmail(string email, string codeEmail)
    {
        DataRow row = this.NewRow();
        row["ID"] = new Random().Next(); // Utilisation d'un entier aléatoire comme identifiant unique
        row["Email"] = email;
        row["CodeEmail"] = codeEmail;
        row["DateCreation"] = DateTime.Now;
        this.Rows.Add(row);
    }
    
    public async Task<string?> GetCodeEmail(string email)
    {
        // Vérifier si le code n'a pas expiré (15 minutes)
        var row = this.Rows.Cast<DataRow>()
            .Where(r => r["Email"].ToString() == email &&
                        (DateTime)r["DateCreation"] >= DateTime.Now.AddMinutes(-15))
            .OrderByDescending(r => r["DateCreation"])
            .FirstOrDefault();

        return row?["CodeEmail"].ToString();
    }

    public async Task DeleteCodeEmail(string email)
    {
        var rowsToDelete = this.Rows.Cast<DataRow>()
            .Where(r => r["Email"].ToString() == email)
            .ToList();

        foreach (var row in rowsToDelete)
        {
            row.Delete();
        }

        this.AcceptChanges();
    }
}