namespace SinglesourceApp.Models;

/// <summary>Optional holder for filter + query + search flag + rows when building list pages with SearchResultsPage&lt;T&gt;.</summary>
public sealed class SearchPageState<T>
{
    public string SelectedFilter { get; set; } = string.Empty;

    public string SearchText { get; set; } = string.Empty;

    public bool HasSearched { get; set; }

    public List<T> Results { get; set; } = [];
}
