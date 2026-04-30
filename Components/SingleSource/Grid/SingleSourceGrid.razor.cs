using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Telerik.Blazor;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.SingleSourceGrid
{
    /// <summary>
    /// Controls the visual presentation preset applied to a <see cref="SingleSourceGrid{TItem}"/>.
    /// </summary>
    public enum SsgGridContext
    {
        /// <summary>Standard Telerik grid appearance (default).</summary>
        Default,
        /// <summary>Task-style appearance: white rows, pointer-cursor hover, no heavy chrome.</summary>
        Task,
    }


    /// <summary>
    /// A reusable, generic grid component that dynamically generates columns for a given data model (<typeparamref name="TItem"/>)
    /// using reflection and optional <see cref="SingleSourceGridAttribute"/> decorations on model properties.
    /// </summary>
    /// <remarks>
    /// This component is designed to reduce repetitive grid configuration by automatically building columns based on the
    /// public properties of <typeparamref name="TItem"/>. Column behavior such as display name, width, sorting, grouping,
    /// filtering, and ordering can be customized via the <see cref="SingleSourceGridAttribute"/> applied to each property.
    ///
    /// If no attribute is provided on a property, sensible defaults are used:
    /// <list type="bullet">
    /// <item><description>The property will be rendered as a column.</description></item>
    /// <item><description>The column title will default to the property name.</description></item>
    /// <item><description>The column will be groupable, sortable, and filterable.</description></item>
    /// </list>
    ///
    /// Custom rendering for specific columns can be provided via <see cref="ColumnTemplates"/>, allowing consumers to override
    /// the default cell rendering logic for individual properties.
    ///
    /// Pagination is supported with configurable page size options, including a "show all" option represented by <c>null</c>.
    ///
    /// This component assumes integration with Telerik UI for Blazor and relies on compatible features such as filtering,
    /// grouping, and templating.
    /// </remarks>
    /// <typeparam name="TItem">
    /// The type of the data item represented in each row of the grid. Properties of this type are used to dynamically
    /// generate grid columns.
    /// </typeparam>
    public partial class SingleSourceGrid<TItem>
    {
        /// <summary>
        /// Represents a single top-level renderable unit in the grid — either a standalone
        /// (ungrouped) column or a visual column header group containing one or more sub-columns.
        /// </summary>
        protected sealed record GridColumnItem
        {
            /// <summary>
            /// The property backing a standalone (ungrouped) column.
            /// <c>null</c> when this item represents a column header group.
            /// </summary>
            public PropertyInfo? Property { get; init; }

            /// <summary>
            /// The group attribute backing a column header group.
            /// <c>null</c> when this item represents a standalone column.
            /// </summary>
            public SingleSourceGridGroupAttribute? Group { get; init; }

            /// <summary>
            /// The ordered list of properties belonging to this column header group.
            /// <c>null</c> when this item represents a standalone column.
            /// </summary>
            public IReadOnlyList<PropertyInfo>? GroupedProperties { get; init; }

            /// <summary>
            /// The effective ordering value used to position this item among all other top-level
            /// items (both ungrouped columns and groups). Sourced from
            /// <see cref="SingleSourceGridAttribute.Order"/> for standalone columns and
            /// <see cref="SingleSourceGridGroupAttribute.Order"/> for groups.
            /// </summary>
            public int EffectiveOrder { get; init; }
        }

        /// <summary>
        /// Optional CSS height for the grid (e.g. <c>"500px"</c>, <c>"60vh"</c>,
        /// <c>"calc(100vh - 200px)"</c>). When set, the grid renders as a fixed-height
        /// scrollable container: the header row is always visible at the top and only the
        /// data rows scroll. When <c>null</c> (the default) the grid expands to fit its
        /// content and no internal scroll is applied.
        /// </summary>
        [Parameter]
        public string? Height { get; set; }

        /// <summary>
        /// Visual presentation preset for the grid. Defaults to <see cref="SsgGridContext.Default"/>
        /// (standard Telerik appearance). Use <see cref="SsgGridContext.Task"/> to apply the task-style
        /// preset (white rows, hover highlight, pointer cursor) shared by task-oriented screens.
        /// </summary>
        [Parameter]
        public SsgGridContext Context { get; set; } = SsgGridContext.Default;

        /// <summary>
        /// When <c>true</c>, hides the Telerik drag-to-group panel that appears above the header row.
        /// Useful when row grouping is applied programmatically (via <see cref="InitialGroupDescriptors"/>)
        /// and the user should not be able to add or remove group fields interactively.
        /// Defaults to <c>false</c>.
        /// </summary>
        [Parameter]
        public bool HideGroupingBar { get; set; } = false;

        private string WrapperCssClass
        {
            get
            {
                var cls = "single-source-grid-wrapper";
                if (Context == SsgGridContext.Task)   cls += " ssg--ctx-task";
                if (HideGroupingBar)                  cls += " ssg--no-group-bar";
                return cls;
            }
        }

        /// <summary>
        /// Optional row grouping descriptors applied when the grid initialises its state.
        /// Each <see cref="GroupDescriptor"/> specifies a <c>Member</c> (property name) to
        /// group rows by. Groups are applied in the order provided and are expanded by default.
        /// When <c>null</c> or empty, no initial row grouping is applied.
        /// </summary>
        /// <example>
        /// <code>
        /// private readonly IEnumerable&lt;Telerik.DataSource.GroupDescriptor&gt; _groups = new[]
        /// {
        ///     new Telerik.DataSource.GroupDescriptor { Member = nameof(MyModel.Category), MemberType = typeof(string) }
        /// };
        ///
        /// &lt;SingleSourceGrid TItem="MyModel" Data="@_data" InitialGroupDescriptors="@_groups" /&gt;
        /// </code>
        /// </example>
        [Parameter]
        public IEnumerable<Telerik.DataSource.GroupDescriptor>? InitialGroupDescriptors { get; set; }

        /// <summary>
        /// The data source for the grid.
        /// </summary>
        /// <remarks>
        /// Each item in the collection represents a row in the grid. The public properties of <typeparamref name="TItem"/>
        /// are inspected to generate columns dynamically.
        /// </remarks>
        [Parameter]
        public IEnumerable<TItem> Data { get; set; } = Enumerable.Empty<TItem>();

        /// <summary>
        /// A collection of column templates used to override the default rendering for specific properties.
        /// </summary>
        /// <remarks>
        /// The key should match the property name of <typeparamref name="TItem"/>. If a template is provided for a property,
        /// it will be used instead of the default rendering for that column.
        /// </remarks>
        [Parameter]
        public Dictionary<string, RenderFragment<TItem>> ColumnTemplates { get; set; } = new();

        /// <summary>
        /// A collection of edit templates used to override the default inline editor for specific properties.
        /// </summary>
        /// <remarks>
        /// The key should match the property name of <typeparamref name="TItem"/>. When provided for a property,
        /// the template is rendered inside the cell in place of the auto-generated editor when the row is in edit mode.
        /// Only has an effect when <see cref="AllowEdit"/> is <c>true</c> and the row is selected.
        /// The template receives the live <typeparamref name="TItem"/> instance; any mutations made inside
        /// the template are reflected immediately on the item and committed when <see cref="SaveCurrentEditAsync"/> is called.
        /// </remarks>
        [Parameter]
        public Dictionary<string, RenderFragment<TItem>> EditTemplates { get; set; } = new();

        /// <summary>
        /// Optional label for the Save Changes button. Defaults to <c>"Save Changes"</c> when not set.
        /// </summary>
        [Parameter]
        public string? SaveButtonLabel { get; set; }

        /// <summary>
        /// Optional hex color applied to the Save Changes button background (e.g. <c>"#e65c00"</c>).
        /// When provided, overrides the default primary theme color. The button text is forced to white
        /// and the border is matched to the same hex value.
        /// When <c>null</c> or empty the button renders with the standard Telerik primary theme color.
        /// </summary>
        [Parameter]
        public string? SaveButtonColor { get; set; }

        /// <summary>
        /// Enables click-to-edit multi-row inline editing. When <c>true</c>, clicking any data cell in a
        /// row activates inline editors for all editable columns on that row simultaneously. Rows remain
        /// in edit mode until the user clicks Save Changes or Reset.
        /// </summary>
        /// <remarks>
        /// Properties decorated with <see cref="SingleSourceGridAttribute.Editable"/> set to <c>false</c>
        /// remain read-only even when the row is in edit mode. Auto-generated editors are provided for
        /// common .NET types (string, numeric, bool, DateTime, DateOnly, enum). Provide
        /// <see cref="EditTemplates"/> for properties that require custom editor controls.
        ///
        /// Changes are buffered in the working copy and do not fire <see cref="OnUpdate"/> until
        /// <see cref="SaveCurrentEditAsync"/> is called. Clicking Reset reverts all buffered edits.
        ///
        /// When <see cref="AllowSelect"/> is also <c>true</c>, the checkbox column performs pure row
        /// selection only — checking a row does <em>not</em> enter edit mode.
        /// </remarks>
        [Parameter]
        public bool AllowEdit { get; set; } = false;

        /// <summary>
        /// Enables inline row creation. When <c>true</c>, an "Add New" button appears in the grid toolbar
        /// that opens a new blank row for the user to fill in.
        /// </summary>
        /// <remarks>
        /// Wire up <see cref="OnCreate"/> to persist the new item when the user saves.
        /// <typeparamref name="TItem"/> must have a public parameterless constructor for the grid
        /// to instantiate the new row correctly.
        /// </remarks>
        [Parameter]
        public bool AllowCreate { get; set; } = false;

        /// <summary>
        /// Enables row deletion. When <c>true</c>, each row displays a Delete button in the command column.
        /// </summary>
        /// <remarks>
        /// Wire up <see cref="OnDelete"/> to remove the item from the underlying data source when the user confirms deletion.
        /// </remarks>
        [Parameter]
        public bool AllowDelete { get; set; } = false;

        /// <summary>
        /// Raised by the Telerik grid's inline create form when a new row is saved.
        /// Also fires for individual row edits when using Telerik's built-in edit mode.
        /// </summary>
        /// <remarks>
        /// For checkbox-driven bulk editing (<see cref="AllowEdit"/>), prefer
        /// <see cref="OnBatchUpdate"/> which delivers all edited rows in one callback.
        /// The grid does not modify the data source automatically.
        /// </remarks>
        [Parameter]
        public EventCallback<TItem> OnUpdate { get; set; }

        /// <summary>
        /// Raised when the built-in toolbar Save button is clicked or
        /// <see cref="SaveCurrentEditAsync"/> is called externally. Delivers a
        /// <see cref="SingleSourceGridChangeSet{TItem}"/> containing every pending edit,
        /// create, and delete in a single callback — use this as the one-stop handler for
        /// all grid mutations rather than wiring <see cref="OnBatchUpdate"/>,
        /// <see cref="OnCreate"/>, and <see cref="OnDelete"/> separately.
        /// </summary>
        /// <example>
        /// <code>
        /// &lt;SingleSourceGrid AllowEdit="true" AllowCreate="true" AllowDelete="true"
        ///                   OnSaveClicked="@HandleSaveClicked" ... /&gt;
        ///
        /// private async Task HandleSaveClicked(SingleSourceGridChangeSet&lt;MyModel&gt; changes)
        /// {
        ///     await _service.BulkUpdateAsync(changes.EditedItems);
        ///     await _service.BulkCreateAsync(changes.CreatedItems);
        ///     await _service.BulkDeleteAsync(changes.DeletedItems);
        /// }
        /// </code>
        /// </example>
        [Parameter]
        public EventCallback<SingleSourceGridChangeSet<TItem>> OnSaveClicked { get; set; }

        /// <summary>
        /// Raised once when <see cref="SaveCurrentEditAsync"/> is called, delivering all
        /// currently checked and edited rows as a single collection.
        /// </summary>
        /// <remarks>
        /// Use this instead of <see cref="OnUpdate"/> when <see cref="AllowEdit"/> is active
        /// so the parent receives the complete set of changes in one call and can issue a
        /// single batch persist operation (e.g. a bulk API endpoint).
        /// The grid does not modify the data source automatically; the consumer is responsible
        /// for persisting the changes and refreshing <see cref="Data"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// &lt;SingleSourceGrid @ref="_grid" AllowEdit="true" OnBatchUpdate="@HandleBatchUpdate" ... /&gt;
        ///
        /// private async Task HandleBatchUpdate(IEnumerable&lt;MyModel&gt; updatedItems)
        /// {
        ///     await _service.BulkUpdateAsync(updatedItems.ToList());
        /// }
        /// </code>
        /// </example>
        [Parameter]
        public EventCallback<IEnumerable<TItem>> OnBatchUpdate { get; set; }

        /// <summary>
        /// Raised when the user saves a newly created row. The argument is the populated <typeparamref name="TItem"/> instance.
        /// </summary>
        /// <remarks>
        /// The grid does not add the item to the data source automatically; the consumer is responsible for
        /// persisting and refreshing <see cref="Data"/>.
        /// </remarks>
        [Parameter]
        public EventCallback<TItem> OnCreate { get; set; }

        /// <summary>
        /// Raised when the user clicks the Delete button on a row. The argument is the <typeparamref name="TItem"/> to be deleted.
        /// </summary>
        /// <remarks>
        /// The grid does not remove the item from the data source automatically; the consumer is responsible for
        /// deleting and refreshing <see cref="Data"/>.
        /// </remarks>
        [Parameter]
        public EventCallback<TItem> OnDelete { get; set; }

        /// <summary>
        /// Enables multi-row checkbox selection. When <c>true</c>, a checkbox column is prepended to
        /// the grid. Checking rows adds them to the active selection; unchecking removes them.
        /// The current selection is emitted to the parent via <see cref="SelectedItemsChanged"/> each
        /// time it changes.
        /// </summary>
        /// <remarks>
        /// No editing occurs in this mode — it is intended purely for communicating which rows the user
        /// has chosen, so the parent component can act on them (e.g. bulk-action buttons).
        /// </remarks>
        [Parameter]
        public bool AllowSelect { get; set; } = false;

        /// <summary>
        /// Raised whenever the checkbox selection changes while <see cref="AllowSelect"/> is <c>true</c>.
        /// The argument is the current set of selected <typeparamref name="TItem"/> instances.
        /// </summary>
        [Parameter]
        public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }

        /// <summary>
        /// Optional toolbar content rendered on the right-hand side of the toolbar whenever
        /// <see cref="AllowSelect"/> is <c>true</c>. The context passed to the fragment is the
        /// current set of checkbox-selected <typeparamref name="TItem"/> instances, allowing
        /// command buttons to act directly on the selection without any additional wiring.
        /// </summary>
        /// <remarks>
        /// When <see cref="AllowSelect"/> is <c>false</c> this fragment is ignored entirely.
        /// <code>
        /// &lt;SingleSourceGrid TItem="MyModel" AllowSelect="true" Data="@_data"&gt;
        ///     &lt;SelectionCommands Context="selected"&gt;
        ///         &lt;TelerikButton Enabled="@selected.Any()"
        ///                        OnClick="@(() =&gt; ExportToPdf(selected))"&gt;
        ///             Export to PDF
        ///         &lt;/TelerikButton&gt;
        ///     &lt;/SelectionCommands&gt;
        /// &lt;/SingleSourceGrid&gt;
        /// </code>
        /// </remarks>
        [Parameter]
        public RenderFragment<IEnumerable<TItem>>? SelectionCommands { get; set; }

        /// <summary>
        /// Optional toolbar content rendered on the right-hand side of the toolbar whenever
        /// <see cref="AllowEdit"/> is <c>true</c>. The context passed to the fragment is the
        /// current set of rows in inline edit mode, allowing command buttons to act directly
        /// on in-progress edits without any additional wiring or a component <c>@ref</c>.
        /// </summary>
        /// <remarks>
        /// When <see cref="AllowEdit"/> is <c>false</c> this fragment is ignored entirely.
        /// <code>
        /// &lt;SingleSourceGrid TItem="MyModel" AllowEdit="true" Data="@_data"&gt;
        ///     &lt;EditingCommands Context="editing"&gt;
        ///         &lt;TelerikButton Enabled="@editing.Any()"
        ///                        OnClick="@(() =&gt; Preview(editing))"&gt;
        ///             Preview Changes
        ///         &lt;/TelerikButton&gt;
        ///     &lt;/EditingCommands&gt;
        /// &lt;/SingleSourceGrid&gt;
        /// </code>
        /// </remarks>
        [Parameter]
        public RenderFragment<IEnumerable<TItem>>? EditingCommands { get; set; }

        /// <summary>
        /// Snapshot of the <see cref="Data"/> reference from the last parameter set, used to detect
        /// when the data source has been replaced so that <see cref="_workingData"/> can be rebuilt.
        /// </summary>
        private IEnumerable<TItem> _lastData = Enumerable.Empty<TItem>();

        /// <summary>
        /// Internal working copy of <see cref="Data"/> that the grid is bound to.
        /// When <see cref="AllowEdit"/> is active, in-cell edits are applied here so that
        /// the consumer's original collection is never mutated until an explicit save.
        /// </summary>
        private List<TItem> _workingData = new();

        /// <summary>
        /// The set of items currently checked in the grid's checkbox column.
        /// Bound to Telerik's <c>SelectedItems</c> parameter. Only used when
        /// <see cref="AllowSelect"/> is <c>true</c>.
        /// </summary>
        private IEnumerable<TItem> _selectedItems = Enumerable.Empty<TItem>();

        /// <summary>
        /// The set of rows currently in inline edit mode. A row enters edit mode when the user
        /// clicks any data cell in that row (when <see cref="AllowEdit"/> is <c>true</c>).
        /// Distinct from checkbox selection — editing is driven by cell click, not the checkbox.
        /// </summary>
        private List<TItem> _editingItems = new();

        /// <summary>
        /// The rows currently in inline edit mode. Readable externally via a component
        /// <c>@ref</c> to allow parent components to act on in-progress edits — for example,
        /// inside a <see cref="SelectionCommands"/> fragment or a custom toolbar button.
        /// </summary>
        public IReadOnlyList<TItem> EditingItems => _editingItems.AsReadOnly();

        /// <summary>
        /// The rows currently checked in the checkbox column. Readable externally via a
        /// component <c>@ref</c> to allow parent components to intersect the selection with
        /// <see cref="EditingItems"/> or drive their own command logic.
        /// Only populated when <see cref="AllowSelect"/> is <c>true</c>.
        /// </summary>
        public IReadOnlyList<TItem> SelectedItems => _selectedItems.ToList().AsReadOnly();

        /// <summary>
        /// Pairs each row currently in edit mode with a deep clone of the values it held when
        /// editing began. Used by <see cref="ResetChanges"/> to revert uncommitted in-cell edits.
        /// </summary>
        private readonly List<(TItem Item, TItem Original)> _editingWithOriginals = new();

        /// <summary>
        /// Items added via the "Add New" button that have not yet been persisted.
        /// Flushed to <see cref="SingleSourceGridChangeSet{TItem}.CreatedItems"/> on save.
        /// </summary>
        private readonly List<TItem> _pendingCreates = new();

        /// <summary>
        /// Items removed via the Delete button that have not yet been removed from the
        /// underlying data source. Flushed to
        /// <see cref="SingleSourceGridChangeSet{TItem}.DeletedItems"/> on save.
        /// </summary>
        private readonly List<TItem> _pendingDeletes = new();

        /// <summary>
        /// Unique identifier for this component instance, used to scope the dynamically
        /// injected Save button CSS class so multiple grids on the same page don't conflict.
        /// </summary>
        private readonly string _instanceId = Guid.NewGuid().ToString("N")[..8];

        /// <summary>
        /// Incremented each time a column group is toggled. Used as <c>@key</c> on the
        /// <c>TelerikGrid</c> so Blazor tears down and recreates the grid component, forcing
        /// Telerik to re-render all <c>HeaderTemplate</c> fragments with current state.
        /// </summary>
        private int _gridVersion;

        /// <summary>
        /// Reference to the underlying <see cref="TelerikGrid{TItem}"/> instance, used to
        /// read and apply grid state when toggling collapsible column groups.
        /// </summary>
        private TelerikGrid<TItem> GridRef { get; set; } = default!;

        /// <summary>
        /// Tracks the keys of column groups that are currently collapsed.
        /// </summary>
        private readonly HashSet<string> _collapsedGroups = new(StringComparer.Ordinal);

        /// <summary>
        /// The current page size used for grid pagination.
        /// </summary>
        /// <remarks>
        /// This value determines how many rows are displayed per page. It can be changed by selecting a value from
        /// <see cref="PageSizeOptions"/>.
        /// </remarks>
        protected int PageSize { get; set; } = 10;

        /// <summary>
        /// The available page size options for the grid.
        /// </summary>
        /// <remarks>
        /// Includes predefined page sizes as well as a <c>null</c> option, which typically represents "show all" behavior.
        /// </remarks>
        protected List<int?> PageSizeOptions
        {
            get
            {
                return new List<int?> { 10, 25, 50, null };
            }
        }

        /// <summary>
        /// Synchronises <see cref="_workingData"/> from <see cref="Data"/> whenever the data source
        /// reference changes. Selection state is also cleared at that point because the prior
        /// originals are no longer valid against the new collection.
        /// </summary>
        protected override void OnParametersSet()
        {
            if (!ReferenceEquals(_lastData, Data))
            {
                _lastData = Data;
                _workingData = new List<TItem>(Data);
                _selectedItems = Enumerable.Empty<TItem>();
                _editingItems.Clear();
                _editingWithOriginals.Clear();
                _pendingCreates.Clear();
                _pendingDeletes.Clear();
            }
        }

        /// <summary>
        /// Gets the list of properties from <typeparamref name="TItem"/> that should be rendered as grid columns.
        /// </summary>
        /// <remarks>
        /// Properties are filtered based on <see cref="SingleSourceGridAttribute.AutoGenerate"/> and ordered using
        /// <see cref="SingleSourceGridAttribute.Order"/> followed by the property name.
        /// </remarks>
        protected List<PropertyInfo> GridProperties =>
            typeof(TItem)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(ShouldRenderProperty)
                .OrderBy(GetColumnOrder)
                .ThenBy(x => x.Name)
                .ToList();

        /// <summary>
        /// Gets the ordered list of top-level grid column items used to drive the grid template.
        /// </summary>
        /// <remarks>
        /// Each item is either a standalone (ungrouped) column or a column header group with sub-columns.
        /// Both types share the same ordering pool: a standalone column's <see cref="SingleSourceGridAttribute.Order"/>
        /// and a group's <see cref="SingleSourceGridGroupAttribute.Order"/> are compared directly so that columns
        /// and groups interleave naturally.
        ///
        /// Properties whose <see cref="SingleSourceGridAttribute.GroupKey"/> references a key that does not match
        /// any <see cref="SingleSourceGridGroupAttribute"/> declared on <typeparamref name="TItem"/> fall back to
        /// being rendered as standalone columns.
        /// </remarks>
        protected IReadOnlyList<GridColumnItem> GridColumnItems => BuildGridColumnItems();

        /// <summary>
        /// Builds the ordered list of top-level grid column items from the properties and group
        /// definitions of <typeparamref name="TItem"/>.
        /// </summary>
        private IReadOnlyList<GridColumnItem> BuildGridColumnItems()
        {
            List<PropertyInfo> allProperties = typeof(TItem)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(ShouldRenderProperty)
                .ToList();

            // Build a lookup of declared group definitions keyed by their Key.
            Dictionary<string, SingleSourceGridGroupAttribute> groupDefinitions = typeof(TItem)
                .GetCustomAttributes<SingleSourceGridGroupAttribute>()
                .ToDictionary(g => g.Key, g => g);

            // Bucket properties into their resolved group key (or null for ungrouped / unresolved).
            var propertyBuckets = new Dictionary<string, List<PropertyInfo>>(StringComparer.Ordinal);
            var ungroupedProperties = new List<PropertyInfo>();

            foreach (PropertyInfo prop in allProperties)
            {
                string? groupKey = prop.GetCustomAttribute<SingleSourceGridAttribute>()?.GroupKey;

                if (!string.IsNullOrEmpty(groupKey) && groupDefinitions.ContainsKey(groupKey))
                {
                    if (!propertyBuckets.TryGetValue(groupKey, out List<PropertyInfo>? bucket))
                    {
                        bucket = new List<PropertyInfo>();
                        propertyBuckets[groupKey] = bucket;
                    }

                    bucket.Add(prop);
                }
                else
                {
                    // No GroupKey, or GroupKey does not resolve to a declared group — treat as ungrouped.
                    ungroupedProperties.Add(prop);
                }
            }

            var items = new List<GridColumnItem>();

            // Add standalone (ungrouped) column items.
            foreach (PropertyInfo prop in ungroupedProperties)
            {
                items.Add(new GridColumnItem
                {
                    Property = prop,
                    EffectiveOrder = GetColumnOrder(prop)
                });
            }

            // Add column header group items.
            foreach (KeyValuePair<string, SingleSourceGridGroupAttribute> groupEntry in groupDefinitions)
            {
                if (!propertyBuckets.TryGetValue(groupEntry.Key, out List<PropertyInfo>? groupedProps) || groupedProps.Count == 0)
                {
                    continue;
                }

                IReadOnlyList<PropertyInfo> orderedGroupedProps = groupedProps
                    .OrderBy(GetColumnOrder)
                    .ThenBy(p => p.Name)
                    .ToList();

                items.Add(new GridColumnItem
                {
                    Group = groupEntry.Value,
                    GroupedProperties = orderedGroupedProps,
                    EffectiveOrder = groupEntry.Value.Order
                });
            }

            return items
                .OrderBy(i => i.EffectiveOrder)
                .ThenBy(i => i.Property?.Name ?? i.Group?.Key ?? string.Empty)
                .ToList();
        }

        /// <summary>
        /// Determines whether a property should be rendered as a column.
        /// </summary>
        /// <param name="propertyInfo">The property being evaluated.</param>
        /// <returns>
        /// <c>true</c> if the property should be included in the grid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If the property has a <see cref="SingleSourceGridAttribute"/>, its <see cref="SingleSourceGridAttribute.AutoGenerate"/>
        /// value is used. Otherwise, the property is included by default.
        /// </remarks>
        private static bool ShouldRenderProperty(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();

            if (columnAttribute is not null)
            {
                return columnAttribute.AutoGenerate;
            }

            return true;
        }

        /// <summary>
        /// Resolves the display title for a column based on the provided property.
        /// </summary>
        /// <param name="propertyInfo">The property for which the title is being determined.</param>
        /// <returns>The resolved column title.</returns>
        /// <remarks>
        /// Priority order:
        /// <list type="number">
        /// <item><description><see cref="SingleSourceGridAttribute.DisplayName"/></description></item>
        /// <item><description><see cref="DisplayAttribute"/> name</description></item>
        /// <item><description>Property name</description></item>
        /// </list>
        /// </remarks>
        private static string GetColumnTitle(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            DisplayAttribute? displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();

            if (!string.IsNullOrWhiteSpace(columnAttribute?.DisplayName))
            {
                return columnAttribute.DisplayName;
            }

            if (!string.IsNullOrWhiteSpace(displayAttribute?.GetName()))
            {
                return displayAttribute.GetName()!;
            }

            return propertyInfo.Name;
        }

        /// <summary>
        /// Gets the configured width for a column.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns>The column width if specified; otherwise, <c>null</c>.</returns>
        /// <remarks>
        /// The width is derived from <see cref="SingleSourceGridAttribute.Width"/>.
        /// </remarks>
        private static string? GetColumnWidth(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.Width;
        }

        /// <summary>
        /// Determines whether a column can be grouped.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns><c>true</c> if grouping is enabled; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Defaults to <c>true</c> when no <see cref="SingleSourceGridAttribute"/> is present.
        /// </remarks>
        private static bool IsGroupable(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.Groupable ?? true;
        }

        /// <summary>
        /// Determines whether a column can be sorted.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns><c>true</c> if sorting is enabled; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Defaults to <c>true</c> when no <see cref="SingleSourceGridAttribute"/> is present.
        /// </remarks>
        private static bool IsSortable(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.Sortable ?? true;
        }

        /// <summary>
        /// Determines whether a column can be filtered.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns><c>true</c> if filtering is enabled; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Defaults to <c>true</c> when no <see cref="SingleSourceGridAttribute"/> is present.
        /// </remarks>
        private static bool IsFilterable(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.Filterable ?? true;
        }

        /// <summary>
        /// Gets the filter menu type for a column.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns>
        /// The configured <see cref="FilterMenuType"/> if specified; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// The value is derived from <see cref="SingleSourceGridAttribute.FilterMenuType"/>.
        /// </remarks>
        private static FilterMenuType? GetFilterMenuType(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.FilterMenuType;
        }

        /// <summary>
        /// Gets the ordering value for a column.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns>The order value used for sorting columns.</returns>
        /// <remarks>
        /// Lower values are rendered first. Defaults to <c>0</c> when no <see cref="SingleSourceGridAttribute"/> is present.
        /// </remarks>
        private static Int32 GetColumnOrder(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.Order ?? 0;
        }

        /// <summary>
        /// Determines whether a column's cell is editable when the row is in inline edit mode.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns><c>true</c> if the column can be edited; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Defaults to <c>true</c> when no <see cref="SingleSourceGridAttribute"/> is present.
        /// </remarks>
        private static bool IsEditable(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            return columnAttribute?.Editable ?? true;
        }

        /// <summary>
        /// Gets the locked (frozen) state for a column.
        /// </summary>
        /// <param name="propertyInfo">The property associated with the column.</param>
        /// <returns>
        /// <c>true</c> if the column (or its containing group) should be frozen during
        /// horizontal scroll; <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// If the property belongs to a column group (via <see cref="SingleSourceGridAttribute.GroupKey"/>),
        /// the group's <see cref="SingleSourceGridGroupAttribute.Locked"/> value is used instead of
        /// the property-level value. This mirrors how Telerik handles grouped frozen columns — only the
        /// parent group header should carry <c>Locked</c>; sub-columns inherit it automatically.
        /// For ungrouped columns, <see cref="SingleSourceGridAttribute.Locked"/> is used directly.
        /// </remarks>
        private static bool GetColumnLocked(PropertyInfo propertyInfo)
        {
            SingleSourceGridAttribute? columnAttribute = propertyInfo.GetCustomAttribute<SingleSourceGridAttribute>();
            string? groupKey = columnAttribute?.GroupKey;

            if (!string.IsNullOrEmpty(groupKey))
            {
                // For grouped columns, Locked is set on the parent group GridColumn only.
                // Telerik propagates the locked state to all sub-columns from the parent.
                // Setting it on children as well breaks the group header freezing.
                return false;
            }

            return columnAttribute?.Locked ?? false;
        }

        /// <summary>
        /// Returns <c>true</c> when there is at least one pending edit, create, or delete
        /// that has not yet been committed via <see cref="SaveCurrentEditAsync"/>.
        /// Used to drive the enabled state of the Save Changes and Reset toolbar buttons.
        /// </summary>
        private bool HasPendingChanges =>
            _editingItems.Any() || _pendingCreates.Any() || _pendingDeletes.Any();

        /// <summary>
        /// Discards all buffered edits, creates, and deletes and restores
        /// <see cref="_workingData"/> to a fresh copy of the original <see cref="Data"/>.
        /// Can also be called externally via a component <c>@ref</c>.
        /// </summary>
        public void ResetChanges()
        {
            _workingData = new List<TItem>(_lastData);
            _selectedItems = Enumerable.Empty<TItem>();
            _editingItems.Clear();
            _editingWithOriginals.Clear();
            _pendingCreates.Clear();
            _pendingDeletes.Clear();
            StateHasChanged();
        }

        /// <summary>
        /// Returns <c>true</c> when the given row is currently in inline edit mode.
        /// A row enters edit mode when the user clicks any data cell (when
        /// <see cref="AllowEdit"/> is <c>true</c>).
        /// </summary>
        private bool IsItemEditing(TItem item) =>
            _editingItems.Any(e => EqualityComparer<TItem>.Default.Equals(e, item));

        /// <summary>
        /// Called when the user clicks any data cell in a row. If the row is not already in
        /// edit mode, it is added to <see cref="_editingItems"/> and its current state is
        /// cloned into <see cref="_editingWithOriginals"/> so that <see cref="ResetChanges"/>
        /// can restore the pre-edit values. If the row is already editing, this is a no-op —
        /// the user commits via Save or reverts via Reset.
        /// </summary>
        private void MarkRowEditable(TItem item)
        {
            if (!AllowEdit || IsItemEditing(item)) return;

            _editingWithOriginals.Add((item, CloneItem(item)));
            _editingItems.Add(item);
            StateHasChanged();
        }

        /// <summary>
        /// Handles the Telerik grid's OnUpdate event.
        /// This is only reached when <see cref="AllowCreate"/> is active and Telerik fires an update
        /// through the inline create form. All other edit operations go through
        /// <see cref="SaveCurrentEditAsync"/> instead.
        /// </summary>
        private async Task HandleUpdate(GridCommandEventArgs args)
        {
            if (args.Item is TItem item)
            {
                await OnUpdate.InvokeAsync(item);
            }
        }

        /// <summary>
        /// Handles the Telerik grid's OnCreate event. The new item is buffered in
        /// <see cref="_pendingCreates"/> and appended to <see cref="_workingData"/> so it
        /// is immediately visible in the grid. It is not persisted until
        /// <see cref="SaveCurrentEditAsync"/> is called.
        /// </summary>
        private Task HandleCreate(GridCommandEventArgs args)
        {
            if (args.Item is TItem item)
            {
                _pendingCreates.Add(item);
                _workingData.Add(item);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the Telerik grid's OnDelete event. If the target row was a pending create
        /// it is simply discarded; otherwise it is buffered in <see cref="_pendingDeletes"/>.
        /// In both cases the row is removed from <see cref="_workingData"/> immediately.
        /// The deletion is not persisted until <see cref="SaveCurrentEditAsync"/> is called.
        /// </summary>
        private Task HandleDelete(GridCommandEventArgs args)
        {
            if (args.Item is TItem item)
            {
                // If the row was a pending create, discard it without adding to deletes.
                int createIdx = _pendingCreates
                    .FindIndex(c => EqualityComparer<TItem>.Default.Equals(c, item));

                if (createIdx >= 0)
                {
                    _pendingCreates.RemoveAt(createIdx);
                }
                else
                {
                    _pendingDeletes.Add(item);
                }

                // Clean up any in-progress selection or edit state for this row.
                _selectedItems = _selectedItems
                    .Where(s => !EqualityComparer<TItem>.Default.Equals(s, item))
                    .ToList();
                _editingItems
                    .RemoveAll(e => EqualityComparer<TItem>.Default.Equals(e, item));
                _editingWithOriginals
                    .RemoveAll(t => EqualityComparer<TItem>.Default.Equals(t.Item, item));

                _workingData.RemoveAll(w => EqualityComparer<TItem>.Default.Equals(w, item));
                StateHasChanged();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Responds to Telerik's <c>SelectedItemsChanged</c> event.
        /// <para>
        /// Selection is driven solely by <see cref="AllowSelect"/> — the checkbox column is only
        /// rendered when that flag is set. When <see cref="AllowEdit"/> is also active, checking a
        /// row does <em>not</em> enter edit mode; edit mode is entered exclusively by clicking a
        /// data cell (see <see cref="MarkRowEditable"/>).
        /// </para>
        /// </summary>
        private async Task OnSelectedItemsChanged(IEnumerable<TItem> newSelection)
        {
            _selectedItems = newSelection.ToList();

            if (AllowSelect)
            {
                await SelectedItemsChanged.InvokeAsync(_selectedItems);
            }
        }

        /// <summary>
        /// Commits all rows currently in edit mode and clears editing state.
        /// Fires <see cref="OnBatchUpdate"/> once with the complete collection of edited items,
        /// then fires <see cref="OnUpdate"/> once per item for consumers that need individual callbacks.
        /// Triggered by the built-in toolbar Save button (visible when <see cref="AllowEdit"/> is <c>true</c>)
        /// or called externally via a component <c>@ref</c>.
        /// </summary>
        /// <remarks>
        /// After all callbacks complete the edit tracking state is cleared. The grid does not refresh
        /// <see cref="Data"/> automatically — the consumer must do so after persisting.
        /// </remarks>
        /// <example>
        /// <code>
        /// &lt;TelerikButton OnClick="@(() => _grid.SaveCurrentEditAsync())"&gt;Save&lt;/TelerikButton&gt;
        /// &lt;SingleSourceGrid @ref="_grid" AllowEdit="true" OnBatchUpdate="@HandleBatchUpdate" ... /&gt;
        ///
        /// private async Task HandleBatchUpdate(IEnumerable&lt;MyModel&gt; items)
        /// {
        ///     await _service.BulkUpdateAsync(items.ToList());
        /// }
        /// </code>
        /// </example>
        /// <summary>
        /// Invoked by the internal toolbar Save button. Delegates directly to
        /// <see cref="SaveCurrentEditAsync"/> which builds the full
        /// <see cref="SingleSourceGridChangeSet{TItem}"/> and fires <see cref="OnSaveClicked"/>.
        /// </summary>
        private async Task HandleSaveButtonClick()
        {
            await SaveCurrentEditAsync();
        }

        public async Task SaveCurrentEditAsync()
        {
            List<TItem> editedItems = _editingItems.ToList();
            List<TItem> createdItems = _pendingCreates.ToList();
            List<TItem> deletedItems = _pendingDeletes.ToList();

            // Unified event — delivers all pending changes in one callback.
            SingleSourceGridChangeSet<TItem> changeSet = new()
            {
                EditedItems = editedItems,
                CreatedItems = createdItems,
                DeletedItems = deletedItems
            };
            await OnSaveClicked.InvokeAsync(changeSet);

            // Legacy per-type callbacks retained for backward compatibility.
            await OnBatchUpdate.InvokeAsync(editedItems);
            foreach (TItem item in editedItems)
            {
                await OnUpdate.InvokeAsync(item);
            }

            _selectedItems = Enumerable.Empty<TItem>();
            _editingItems.Clear();
            _editingWithOriginals.Clear();
            _pendingCreates.Clear();
            _pendingDeletes.Clear();
            StateHasChanged();
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="TItem"/> and copies all writable public
        /// property values from <paramref name="item"/> into it.
        /// </summary>
        private static TItem CloneItem(TItem item)
        {
            TItem clone = Activator.CreateInstance<TItem>();
            CopyProperties(item, clone);
            return clone;
        }

        /// <summary>
        /// Copies all writable public property values from <paramref name="source"/> into
        /// <paramref name="target"/>. Both instances must be of type <typeparamref name="TItem"/>.
        /// </summary>
        private static void CopyProperties(TItem source, TItem target)
        {
            foreach (PropertyInfo prop in typeof(TItem).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(target, prop.GetValue(source));
                }
            }
        }

        /// <summary>
        /// Handles the Telerik grid's <c>OnStateInit</c> event.
        /// Applies any <see cref="InitialGroupDescriptors"/> to the grid state so that row grouping
        /// is active from the first render without requiring a separate state set after load.
        /// Each descriptor is added only if an identical <c>Member</c> is not already present,
        /// making repeated calls (e.g. hot-reload) safe.
        /// </summary>
        private void OnGridStateInit(GridStateEventArgs<TItem> args)
        {
            if (InitialGroupDescriptors is null) return;

            foreach (Telerik.DataSource.GroupDescriptor descriptor in InitialGroupDescriptors)
            {
                bool alreadyPresent = args.GridState.GroupDescriptors
                    .Any(g => string.Equals(g.Member, descriptor.Member, StringComparison.Ordinal));

                if (!alreadyPresent)
                {
                    args.GridState.GroupDescriptors.Add(descriptor);
                }
            }
        }

        /// <summary>
        /// Toggles the collapsed state of a column group identified by <paramref name="groupKey"/>.
        /// <para>
        /// Visibility is driven declaratively via <c>GridColumn.Visible</c> in the Razor template:
        /// the first sub-column in the group is always visible (keeping the parent group header row
        /// present), while columns after the first are bound to <c>groupVisible</c>. This avoids
        /// the <c>GetState</c>/<c>SetStateAsync</c> round-trip, which cannot restore columns whose
        /// state entries are dropped by Telerik once they are hidden.
        /// </para>
        /// </summary>
        private void ToggleGroup(string groupKey)
        {
            if (_collapsedGroups.Contains(groupKey))
                _collapsedGroups.Remove(groupKey);
            else
                _collapsedGroups.Add(groupKey);

            _gridVersion++;
            StateHasChanged();
        }
    }
}