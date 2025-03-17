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
    }
    
    public void SetPrimaryKeyColumn(string columnName)
    {
        this.PrimaryKey = new DataColumn[] { this.Columns[columnName] };
    }
}