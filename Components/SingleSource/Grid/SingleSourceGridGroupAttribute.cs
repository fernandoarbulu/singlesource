namespace BlazorBusiness.Web.Components.SingleSourceGrid
{
    /// <summary>
    /// Defines a visual column header group for use with <c>SingleSourceGrid&lt;TItem&gt;</c>.
    /// </summary>
    /// <remarks>
    /// Apply this attribute at the class level on your display model to declare named column groups.
    /// Individual properties decorated with <see cref="SingleSourceGridAttribute"/> can be assigned
    /// to a group via <see cref="SingleSourceGridAttribute.GroupKey"/>.
    ///
    /// Groups are rendered as multi-column headers in the grid. Sub-columns within a group are
    /// ordered by their individual <see cref="SingleSourceGridAttribute.Order"/> values. Groups
    /// themselves are positioned among ungrouped columns using <see cref="Order"/>, so that both
    /// grouped and ungrouped items share the same ordering pool.
    ///
    /// Multiple instances of this attribute may be applied to the same class to define multiple groups.
    ///
    /// Example:
    /// <code>
    /// [SingleSourceGridGroup(DisplayName = "Status", Key = "Status", Order = 1)]
    /// [SingleSourceGridGroup(DisplayName = "Unit Price / Total Fee", Key = "UnitPrice", Order = 2)]
    /// public class MyDisplayModel { ... }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SingleSourceGridGroupAttribute : Attribute
    {
        /// <summary>
        /// The text displayed in the group header cell spanning its sub-columns.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// A unique identifier for this group. Properties reference this value via
        /// <see cref="SingleSourceGridAttribute.GroupKey"/> to declare membership in the group.
        /// Matching is case-sensitive.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Controls the position of this column group relative to other groups and ungrouped columns.
        /// Lower numbers render first. Shares the same ordering pool as ungrouped column
        /// <see cref="SingleSourceGridAttribute.Order"/> values.
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Controls whether the column group header can be collapsed by the user to hide its sub-columns.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the group header renders a collapse/expand toggle that allows users to
        /// hide or show all sub-columns within the group without removing them from the grid.
        /// Defaults to <c>false</c>.
        /// </remarks>
        public bool Collapsible { get; set; } = false;

        /// <summary>
        /// Freezes (locks) this entire column group during horizontal scrolling so all of its
        /// sub-columns always remain visible.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the group header and all of its sub-columns are pinned and do not
        /// scroll horizontally with the rest of the grid. When <c>null</c> (the default), this
        /// setting is ignored and the group scrolls normally. All sub-columns inside a locked
        /// group must have their Width specified in px units.
        /// </remarks>
        public bool Locked { get; set; } = false;

        /// <summary>
        /// Optional CSS class applied to the group header <c>&lt;th&gt;</c> cell.
        /// Use to apply visual differentiation (e.g. subtle tinting) to individual group headers
        /// without affecting sub-column cells. When <c>null</c> no extra class is added.
        /// </summary>
        public string? HeaderClass { get; set; }
    }
}
