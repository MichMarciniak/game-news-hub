namespace GameNewsHub.External.Igdb.Client;

public class QueryBuilder
{
    private readonly List<string> _clauses = new List<string>();

    public QueryBuilder Fields(string fields)
    {
        _clauses.Add($"fields {fields};");
        return this;
    }

    public QueryBuilder Where(string condition)
    {
        _clauses.Add($"where {condition};");
        return this;
    }

    public QueryBuilder Limit(int limit)
    {
        _clauses.Add($"limit {limit};");
        return this;
    }

    public QueryBuilder Sort(string sortBy, string direction = "asc")
    {
        _clauses.Add($"sort {sortBy} {direction};");
        return this;
    }

    public override string ToString()
    {
        return string.Join(" ", _clauses);
    }
}