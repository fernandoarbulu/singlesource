namespace BlazorBusiness.Web.Components.SingleSourceGrid
{
    /// <summary>
    /// Represents the complete set of pending changes collected by a <see cref="SingleSourceGrid{TItem}"/>
    /// and delivered to the consumer via <see cref="SingleSourceGrid{TItem}.OnSaveClicked"/> when the
    /// built-in Save Changes button is clicked (or <see cref="SingleSourceGrid{TItem}.SaveCurrentEditAsync"/>
    /// is called externally).
    /// </summary>
    /// <typeparam name="TItem">The data item type used by the grid.</typeparam>
    public sealed class SingleSourceGridChangeSet<TItem>
    {
        /// <summary>
        /// Rows that were checked and inline-edited since the last save.
        /// These items are the live working-copy instances — property values reflect the user's edits.
        /// </summary>
        public IReadOnlyList<TItem> EditedItems { get; init; } = Array.Empty<TItem>();

        /// <summary>
        /// Rows that were added via the "Add New" toolbar button and filled in since the last save.
        /// These items have never been persisted to the underlying data source.
        /// </summary>
        public IReadOnlyList<TItem> CreatedItems { get; init; } = Array.Empty<TItem>();

        /// <summary>
        /// Rows that were removed via the Delete button since the last save.
        /// These items still exist in the underlying data source and should be deleted on persist.
        /// </summary>
        public IReadOnlyList<TItem> DeletedItems { get; init; } = Array.Empty<TItem>();

        /// <summary>
        /// Returns <c>true</c> when there is at least one edit, create, or delete pending.
        /// </summary>
        public bool HasChanges =>
            EditedItems.Count > 0 || CreatedItems.Count > 0 || DeletedItems.Count > 0;
    }
}
