using System.Text;
using Marten;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Default")!);
    opts.AutoCreateSchemaObjects = AutoCreate.All;
}).UseLightweightSessions();

var app = builder.Build();
app.MapCarter();
app.Run();

public class SqlQueryBuilder
{
    private string _tableName;
    private readonly List<string> _columns;
    private readonly Dictionary<string, string> _conditions;
    private readonly List<string> _orderBy;
    private int? _limit;

    public SqlQueryBuilder()
    {
        _columns = new List<string>();
        _conditions = new Dictionary<string, string>();
        _orderBy = new List<string>();
    }

    public SqlQueryBuilder Select(string tableName, params string[] columns)
    {
        _tableName = tableName;
        _columns.AddRange(columns);
        return this;
    }

    public SqlQueryBuilder Where(string column, string value)
    {
        _conditions.Add(column, value);
        return this;
    }

    public SqlQueryBuilder OrderBy(string column, bool ascending = true)
    {
        _orderBy.Add($"{column} {(ascending ? "ASC" : "DESC")}");
        return this;
    }

    public SqlQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    public string Build()
    {
        if (string.IsNullOrEmpty(_tableName))
        {
            throw new InvalidOperationException("Table name cannot be null or empty");
        }

        var query = new StringBuilder();
        query.Append("SELECT ");
        query.Append(_columns.Count > 0 ? string.Join(", ", _columns) : "*");
        query.Append(" FROM ");
        query.Append(_tableName);

        if (_conditions.Count > 0)
        {
            query.Append(" WHERE ");
            foreach (var (column, value) in _conditions)
            {
                query.Append($"{column} = '{value}' AND ");
            }
            query.Length -= 5;
        }

        if (_orderBy.Count > 0)
        {
            query.Append(" ORDER BY ");
            query.Append(string.Join(", ", _orderBy));
        }

        if (_limit.HasValue)
        {
            query.Append(" LIMIT ");
            query.Append(_limit.Value);
        }

        return query.ToString();
    }
}
