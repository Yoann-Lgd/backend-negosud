using System.Data;

namespace backend_negosud.Models;

public class BaseDataTable : DataTable
{

    public BaseDataTable(string tableName) : base(tableName)
    {
        SetupColumns();
    }
    protected virtual void SetupColumns()
    {
        this.Columns.Add("ID", typeof(int));
        this.Columns.Add("codeEmail", typeof(string));
    }
    
    public void SetPrimaryKeyColumn(string columnName)
    {
        this.PrimaryKey = new DataColumn[] { this.Columns[columnName] };
    }
}