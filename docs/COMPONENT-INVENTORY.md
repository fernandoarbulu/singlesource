# Component Inventory

> Components live in `Components/`. Registered globally via
> `@using SinglesourceApp.Components` in `_Imports.razor`.

---

## SsField

**Status:** Validated in production-like usage (Work Orders Overview — **Financials** and **Ownership & Loan** subsections).

**Purpose:** Displays a single label + value pair with design-system-compliant
typography and automatic empty-state handling.

**Files:** `Components/SsField.razor`, `Components/SsField.razor.css`

### Props

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `Label` | `string` | Yes | `""` | Uppercase label text |
| `Value` | `string?` | No | `null` | Display value |
| `IsEmpty` | `bool?` | No | `null` | Force empty state. Auto-detects if omitted. |

### Behavior

- If `Value` is null, empty, or whitespace: renders `—` in muted style
- If `IsEmpty` is explicitly set, it overrides auto-detection
- No business logic, no formatting, no Telerik dependency

### Rendered HTML

```html
<div class="ss-field">
    <span class="ss-field-label">LOAN STATUS</span>
    <span class="ss-field-value">In Progress</span>
</div>
```

Empty:

```html
<div class="ss-field">
    <span class="ss-field-label">UPB</span>
    <span class="ss-field-value ss-field-value--empty">—</span>
</div>
```

### Usage

```razor
<SsField Label="Loan Status" Value="@property.LoanStatus" />
<SsField Label="UPB" Value="" />
<SsField Label="Notes" Value="@notes" IsEmpty="@(notes == "N/A")" />
```

### Rules

- Do not override font-size, font-weight, or color on the value span
- Do not add Telerik components inside SsField
- Do not use for editable inputs — this is display-only
- **MDM inspector / `<details>` subsections:** use `SsField` **only** when the parent `<details>` is explicitly marked with `pd-mdm-subcard--ss-field` and the subsection is opted in in code (see `PropertyWorkOrderDetails.razor` — `SsFieldSubsectionTitles`). Scoped `::deep(.ss-field…)` rules in page CSS apply under that marker; do not assume global styling.
- For layouts without that marker, use native `pd-mdm-k` / `pd-mdm-v` (or other approved patterns) until the subsection is validated for `SsField`.

### Where to Use

- Property detail overview fields where the subsection uses the `pd-mdm-subcard--ss-field` marker and shared page CSS for `SsField`
- Static grids and hero-style metadata that match design tokens (see `HANDOFF-WorkOrdersOverview.md`)
- Future screens after the same subsection-level validation pattern

---

## SsSection

**Status:** Reserved for future screens or non-`<details>` layouts. **Not** used inside MDM `<details>` subsections on Work Orders Overview (native summary + grid already cover that pattern).

**Purpose:** Groups fields under a section header with standardized
height, background, and grid layout.

**Files:** `Components/SsSection.razor`, `Components/SsSection.razor.css`

### Props

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `Title` | `string` | Yes | `""` | Section header text (normal case) |
| `FieldCount` | `int?` | No | `null` | Shown as "N fields" on the right |
| `ChildContent` | `RenderFragment?` | No | `null` | Body content (typically `<SsField>` components) |

### Behavior

- Header: 36px height, subtle background, title on left, optional meta on right
- Body: 2-column grid with standard spacing (32px col-gap, 12px row-gap)
- No collapse logic — structure only
- Sections separated by `border-bottom: 1px solid var(--color-border-default)`
- Last section in a container removes its bottom border

### Rendered HTML

```html
<div class="ss-section">
    <div class="ss-section-header">
        <span class="ss-section-title">Financials</span>
        <span class="ss-section-meta">6 fields</span>
    </div>
    <div class="ss-section-body">
        <!-- SsField components here -->
    </div>
</div>
```

### Usage

```razor
<SsSection Title="Financials" FieldCount="6">
    <SsField Label="Prior P&P" Value="Sample value 806" />
    <SsField Label="Total Spent Client" Value="Sample value 393" />
    <SsField Label="Total Expenses" Value="$709,572" />
</SsSection>
```

### Rules

- Do not change the grid to more than 2 columns (use responsive media queries if needed)
- Do not add padding or margin around `<SsSection>` — it manages its own spacing
- Do not use uppercase for section titles (only labels are uppercase)
- Do not nest `<SsSection>` inside another `<SsSection>`

### Where to Use

- Future property or admin screens that need a **static** titled block + grid **without** wrapping each group in `<details>`
- Tab or panel content where `SsSection`’s header/body split matches the design (not the collapsible MDM subsection list)
- Do **not** nest `SsSection` inside Work Orders Overview–style `<details>` subsections — use the native subcard + optional `SsField` + `pd-mdm-subcard--ss-field` pattern instead
