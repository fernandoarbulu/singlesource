using Telerik.Blazor;

namespace BlazorBusiness.Web.Components.SingleSourceGrid
{
    /// <summary>
    /// Provides metadata used by <c>SingleSourceGrid&lt;TItem&gt;</c> to control how a model property
    /// is rendered and behaves as a column within the grid.
    /// </summary>
    /// <remarks>
    /// This attribute is applied to properties of a data model (<typeparamref name="TItem"/>) to
    /// influence column generation when used with the <c>SingleSourceGrid</c> component.
    ///
    /// It allows developers to configure column-specific behavior such as display name, width,
    /// ordering, and whether the column supports grouping, sorting, and filtering.
    ///
    /// When this attribute is not applied to a property, default grid behavior is used:
    /// <list type="bullet">
    /// <item><description>The property is automatically rendered as a column.</description></item>
    /// <item><description>The column title defaults to the property name or <see cref="DisplayAttribute"/> if present.</description></item>
    /// <item><description>Grouping, sorting, and filtering are enabled.</description></item>
    /// </list>
    ///
    /// Setting <see cref="AutoGenerate"/> to <c>false</c> will exclude the property from being rendered
    /// as a column entirely.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SingleSourceGridAttribute : Attribute
    {
        /// <summary>
        /// The column header text.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Width of the column (e.g. "150px").
        /// </summary>
        public string? Width { get; set; } = "150px";

        /// <summary>
        /// Controls whether the column can be grouped.
        /// </summary>
        public bool Groupable { get; set; } = true;

        /// <summary>
        /// Controls whether the column can be sorted.
        /// </summary>
        public bool Sortable { get; set; } = true;

        /// <summary>
        /// Controls whether the column can be filtered.
        /// </summary>
        public bool Filterable { get; set; } = true;

        /// <summary>
        /// Sets the filter menu type (e.g. CheckBoxList).
        /// </summary>
        public FilterMenuType FilterMenuType { get; set; } = Telerik.Blazor.FilterMenuType.Menu;

        /// <summary>
        /// Controls column ordering.
        /// Lower numbers render first.
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Controls whether this column should be auto-generated.
        /// </summary>
        public bool AutoGenerate { get; set; } = true;

        /// <summary>
        /// Associates this column with a named header group defined by a
        /// <see cref="SingleSourceGridGroupAttribute"/> on the containing class.
        /// When set, the column is rendered as a sub-column beneath the matching group header.
        /// </summary>
        /// <remarks>
        /// The value must match the <see cref="SingleSourceGridGroupAttribute.Key"/> of one of the
        /// <see cref="SingleSourceGridGroupAttribute"/> declarations on the model class (case-sensitive).
        /// If no matching group is found, the column falls back to rendering as a top-level
        /// (ungrouped) column using its <see cref="Order"/> value for positioning.
        /// When <c>null</c> or empty, the column is always rendered as a top-level column.
        /// </remarks>
        public string? GroupKey { get; set; }

        /// <summary>
        /// Controls whether this column's cell can be edited when the grid is in inline edit mode.
        /// </summary>
        /// <remarks>
        /// Set to <c>false</c> to make a column read-only even when the row is in edit mode.
        /// This is useful for identifier or computed columns that should never be changed by the user.
        /// Defaults to <c>true</c>. Only has an effect when the <c>SingleSourceGrid</c> component
        /// is configured with <c>AllowEdit="true"</c>.
        /// </remarks>
        public bool Editable { get; set; } = true;

        /// <summary>
        /// Freezes (locks) this column during horizontal scrolling so it always remains visible.
        /// Only applies to columns that are <b>not</b> part of a column group.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the column is pinned at the left edge of the grid and does not scroll
        /// horizontally with the rest of the columns. Defaults to <c>false</c>.
        /// When using a locked column its <see cref="Width"/> must be specified in <c>px</c> units.
        /// For columns that belong to a group (via <see cref="GroupKey"/>), this value is ignored —
        /// set <see cref="SingleSourceGridGroupAttribute.Locked"/> on the group instead.
        /// </remarks>
        public bool Locked { get; set; } = false;

        /// <summary>
        /// When <c>true</c>, this sub-column remains visible even when its parent column group is
        /// collapsed. The first sub-column in a group is always visible regardless of this flag.
        /// Defaults to <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Use this to pin summary or identity columns inside a collapsible group so users can
        /// still read the essential values without expanding the group (e.g. keep "Description"
        /// visible while "WO Unit" and "WO Unit Cost" are hidden when the group is collapsed).
        /// </remarks>
        public bool VisibleWhenCollapsed { get; set; } = false;

        /// <summary>
        /// Marks this column as a calculated/derived value. When <c>true</c>, the cell renders
        /// with a distinct visual treatment (muted, italic) to help users distinguish computed
        /// fields from editable or plain read-only fields. Defaults to <c>false</c>.
        /// </summary>
        public bool IsCalculated { get; set; } = false;
    }
}
