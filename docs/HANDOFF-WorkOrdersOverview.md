# Developer Handoff: Work Orders Overview

> **Reference:** `Pages/PropertyWorkOrderDetails.razor` + `.razor.css`
> **Tokens:** `wwwroot/css/branding.css`
> **Components:** `Components/SsField.razor`, `Components/SsSection.razor`
> **Font:** Inter 400/500/600 via Google Fonts
> **Framework:** Telerik UI for Blazor 12.0 / .NET 10 WASM

---

## Frontend Implementation Status

- **This page is a reference implementation** for property Work Orders Overview: layout, tokens, inspector split-view, MDM-driven field grids, and secondary navigation are implemented end-to-end in Blazor WASM.
- **Field rendering is partially systemized and proven:** `SsField` is integrated only where explicitly opted in; all other subsections continue to use native `pd-mdm-k` / `pd-mdm-v` markup with shared page CSS.
- **The pattern is safe to expand incrementally:** add a subsection title to the allowlist in code, apply `pd-mdm-subcard--ss-field` on the corresponding `<details>`, and use the existing `::deep(.ss-field…)` rules in `PropertyWorkOrderDetails.razor.css` (do **not** comma-merge native `pd-mdm-*` selectors with `::deep` in scoped CSS).
- **No full-page refactor was required** to introduce `SsField`; conversion is subsection-by-subsection.

### SsField usage (current)

| Subsection        | Marker on `<details>`              | Rendering                          |
|-------------------|--------------------------------------|--------------------------------------|
| **Financials**    | `pd-mdm-subcard--ss-field`          | `<SsField>` inside `pd-mdm-sf-wrap` |
| **Ownership & Loan** | `pd-mdm-subcard--ss-field`      | Same                                 |

All **other** MDM subsections (e.g. Eviction & Occupancy, Location & Title, Property Summary, Foreclosure & Disposition, etc.) use **native** stacked `<span class="pd-mdm-k">` / `<span class="pd-mdm-v …">` only — no `SsField`.

### `SsSection` and `<details>`

- **`SsSection` exists** (`Components/SsSection.razor`) and is **intentionally not used** inside `<details>` / `<summary>` subsections: it would duplicate the collapsible header already provided by the summary row, and was not part of the Work Orders Overview design.
- **Reserve `SsSection`** for future screens or layouts that need a static titled block + grid **without** native `<details>` (see `COMPONENT-INVENTORY.md`).

---

## 1. Design Tokens

Declared in `branding.css` under `:root`.

### Text

| Token | Value | Usage |
|-------|-------|-------|
| `--color-text-primary` | `#111827` | Page titles, panel headings only |
| `--color-text-secondary` | `#4B5563` | All field values, nav titles, section headers, tab labels |
| `--color-text-muted` | `#6B7280` | Empty values, subtitle, status line, metric labels |
| `--color-text-label` | `#9CA3AF` | All field labels, group headings, search icon, chevron |

### Border

| Token | Value | Usage |
|-------|-------|-------|
| `--color-border-default` | `#E5E7EB` | Card borders, panel borders, group dividers |
| `--color-border-subtle` | `#F3F4F6` | Nav item borders, section header border |

### Background

| Token | Value | Usage |
|-------|-------|-------|
| `--color-bg-page` | `#F9FAFB` | Page wrapper |
| `--color-bg-card` | `#FFFFFF` | All cards, panels, topbar |
| `--color-bg-subtle` | `#F9FAFB` | Section headers, tabs bar, caret hover |
| `--color-bg-hover` | `#EEF2FF` | Active nav item, search result hover |

### Accent

| Token | Value | Usage |
|-------|-------|-------|
| `--color-accent-primary` | `#2563EB` | Active tab, active nav title, breadcrumb hover |
| `--color-accent-danger` | `#EF4444` | Attention dot |
| `--color-accent-warning` | `#C2410C` | "Needs review" status |

### Typography Scale

| Token | Value | Role |
|-------|-------|------|
| `--font-size-xs` | `11px` | Labels, group headings, status badge |
| `--font-size-sm` | `12px` | Section header titles, breadcrumbs, switch label |
| `--font-size-md` | `14px` | All field values, tab labels, nav titles |
| `--font-size-lg` | `16px` | Inspector panel title |
| `--font-size-xl` | `24px` | Reserved |

### Font Weight

| Token | Value | Role |
|-------|-------|------|
| `--font-weight-regular` | `400` | Breadcrumb links, subtitle, search input |
| `--font-weight-medium` | `500` | All field values, tab labels, switch label |
| `--font-weight-semibold` | `600` | All labels, section headers, nav titles, panel title |

### Spacing

4px grid: `2, 4, 8, 12, 14, 16, 20, 24, 32, 40`.

| Value | Usage |
|-------|-------|
| `2px` | Label-to-value gap |
| `12px` | Split-view gap, nav padding, group divider margin |
| `16px` | Page padding, section header h-padding, field grid h-padding |
| `32px` | Field grid column gap |

### Status Colors

From `--ss-*` tokens. Each has `-bg`, `-text`, `-border`.

| Status | Bg | Text | Border |
|--------|-----|------|--------|
| `--ss-ok` | `#f0fdf4` | `#166534` | `#bbf7d0` |
| `--ss-error` | `#fef2f2` | `#b91c1c` | `#fca5a5` |
| `--ss-warn` | `#fffbeb` | `#92400e` | `#fde68a` |
| `--ss-info` | `#eff6ff` | `#1d4ed8` | `#bfdbfe` |
| `--ss-neutral` | `#f9fafb` | `#374151` | `#d1d5db` |

---

## 2. Typography Rules

### Font Stack

```
font-family: "Inter", ui-sans-serif, system-ui, -apple-system, sans-serif;
```

### Base

```
font-size: var(--font-size-md);  /* 14px */
line-height: 20px;
```

### Five Tiers

| Tier | Size | Weight | Color | Transform | Letter-spacing |
|------|------|--------|-------|-----------|----------------|
| Page title | `min(1.125rem, 24px)` | semibold | `--color-text-primary` | none | `-0.02em` |
| Panel title | `--font-size-lg` | semibold | `--color-text-primary` | none | `-0.01em` |
| Section header | `--font-size-sm` | semibold | `--color-text-secondary` | none | `0` |
| Field label | `--font-size-xs` | semibold | `--color-text-label` | uppercase | `0.04em` |
| Field value | `--font-size-md` | medium | `--color-text-secondary` | none | `normal` |

### Prohibited

- `font-weight: 700` on field values (only `.pd-metric-num`)
- `font-size` above `14px` on any field value
- Color darker than `#4B5563` on field values
- Tailwind utility classes

---

## 3. Card System

Three cards on page: hero (`.pd-hero`), nav panel (`.pd-nav-panel`), inspector (`.pd-inspector-right`).

### Base

```css
background:    var(--color-bg-card);
border:        1px solid var(--color-border-default);
border-radius: 8px;
box-shadow:    none;
```

No shadows anywhere.

### Variants

| Card | Additions |
|------|-----------|
| Hero | `overflow: hidden` |
| Nav panel | `padding: 12px`, `position: sticky`, `overflow-y: auto` |
| Inspector | `overflow: hidden`, flex column |
| Tabs bar | `background: var(--color-bg-subtle)` |

---

## 4. Section Header System

Rendered as `<details>` / `<summary>` with `.pd-mdm-subcard`.

### Header

```css
height: 36px;
padding: 0 16px;
background: var(--color-bg-subtle);
border-bottom: 1px solid var(--color-border-subtle);
display: flex;
align-items: center;
justify-content: space-between;
```

### Title

```css
font-size: var(--font-size-sm);         /* 12px */
font-weight: var(--font-weight-semibold);
color: var(--color-text-secondary);
text-transform: none;
letter-spacing: 0;
```

### Chevron

14px, `var(--color-text-label)`, rotates 90deg on open.

### Dividers

`border-bottom: 1px solid var(--color-border-default)` on `.pd-mdm-subcard`. Last child removes it.

---

## 5. Field Label/Value System

### Label

```css
font-size: var(--font-size-xs);         /* 11px */
font-weight: var(--font-weight-semibold);
color: var(--color-text-label);
text-transform: uppercase;
letter-spacing: 0.04em;
margin: 0 0 2px;
```

### Value

```css
font-size: var(--font-size-md);         /* 14px */
font-weight: var(--font-weight-medium);
color: var(--color-text-secondary);
line-height: 20px;
```

### Value Variants

| Class | Adds |
|-------|------|
| `--currency`, `--date`, `--phone`, `--email` | `font-variant-numeric: tabular-nums` |
| `--textarea` | `white-space: pre-wrap` |
| `--empty` | `color: var(--color-text-muted)` |

### Field Grid

```css
grid-template-columns: repeat(2, minmax(0, 1fr));
column-gap: 32px;
row-gap: 12px;
padding: 14px 16px 16px 16px;
```

### Controlled Exceptions

| Context | What changes | Rule |
|---------|-------------|------|
| Page title (`.pd-hero-title`) | Size up to 24px, weight 600, color primary | Only hero property name |
| Panel title (`.pd-insp-header-title`) | 16px, 600, primary | Only inspector heading |
| Status text (`.pd-insp-status-line--risk`) | Color: `--color-accent-warning`, weight 600 | Accent colors on values, not labels |
| Metric counters (`.pd-metric-num`) | 24px, weight 700 | `.pd-metric-num` only |
| Dense tables (future) | 12px, weight 400 | Via wrapper class, never override `.pd-mdm-v` |

---

## 6. Secondary Sidebar System

### Container

```css
width: 280px; padding: 12px;
background: var(--color-bg-card);
border: 1px solid var(--color-border-default);
border-radius: 8px;
position: sticky; top: 0;
```

### Search

Height 36px, border `1px solid #D1D5DB`, radius 6px, font-size 12px, icon 14px at left 12px.

### Group Labels

11px / semibold / uppercase / `--color-text-label` / `letter-spacing: 0.04em` / padding `12px 12px 8px 12px`.

### Group Dividers

`border-top: 1px solid var(--color-border-default)`, margin/padding `12px 0`.

### Nav Items

Min-height 48px, padding `8px 12px`, border-bottom `var(--color-border-subtle)`, radius 6px.

Title: 14px / semibold / `--color-text-secondary`.

### Active: `background: var(--color-bg-hover)`, title color `--color-accent-primary`.

### Attention Dot: 6px, `--color-accent-danger`, absolute right 10px, centered.

---

## 7. Interaction States

### Transitions

- Navigation/buttons: `0.12s ease`
- Inputs: `0.15s ease`
- Always list specific properties, never `transition: all`

### Hover

| Element | Change |
|---------|--------|
| Breadcrumb link | color → `--color-accent-primary` |
| Tab | color → primary, bg → `rgba(17,28,46,0.04)` |
| Nav item | bg → `rgba(17,24,39,0.04)` |
| Search result | bg → `--color-bg-hover` |
| Caret | bg → subtle, color → secondary |

### Focus

Search input: `border-color: #93C5FD; box-shadow: 0 0 0 1px #93C5FD`.
Form controls: `box-shadow: 0 0 0 0.2rem var(--ss-focus)`.
Telerik inputs: `box-shadow: 0 0 0 2px rgba(30,111,217,0.18); border-color: #a3c0f0`.

### Active/Selected

Uses `.is-active` class (Blazor-toggled), not CSS `:active`.
- Tabs: `--color-accent-primary` text + border-bottom, semibold, card bg
- Nav: `--color-bg-hover` bg, `--color-accent-primary` title

### Disabled (future)

```css
pointer-events: none;
color: var(--color-text-muted);
background: var(--color-bg-subtle);
border-color: var(--color-border-subtle);
```

Never use opacity for disabled state.

---

## 8. Telerik-Safe Implementation Rules

### Do Not Replace Telerik Components

Wrap in purpose-built classes. Style via wrapper, not by replacing.

### `::deep()` — Last Resort Only

Use only when the component exposes no parameter, state hook, or wrapper-targetable surface.

| Allowed | Not allowed |
|---------|-------------|
| Restyling chrome (border, bg, shadow) | Restructuring Kendo DOM |
| Font adjustments to match design system | Overriding animations/popups |
| Removing box-shadow | Styling `.k-*` without wrapper scope |

### Wrapper Scoping (Mandatory)

```css
/* CORRECT */
::deep(.pd-dd.k-dropdownlist) { ... }

/* WRONG */
::deep(.k-dropdownlist) { ... }
```

### Override Priority Order

Exhaust each level before the next:

| Priority | Mechanism | When |
|----------|-----------|------|
| 1 | `--kendo-color-*` tokens | Brand-align all Telerik at once |
| 2 | Telerik component parameters | `Size`, `FillMode`, `Class` |
| 3 | Wrapper class | Layout, spacing reachable without piercing |
| 4 | `::deep()` scoped to wrapper | Last resort only |

Before writing `::deep()`: (1) token exists? (2) parameter exists? (3) wrapper reaches it? If all no, write `::deep()` with a CSS comment explaining why.

### `!important` Policy

Only inside `::deep()`, only for: `border-color`, `background`, `box-shadow`.

---

## 9. Component Usage Policy

### Use Native HTML

| Scenario | Element |
|----------|---------|
| Simple dropdown ≤20 options | `<select>` + `.pd-dd-native` |
| Boolean toggle | `<input type="checkbox">` + `.pd-switch-native` |
| Search input | `<input type="search">` + `.pd-nav-search-input` |
| Label/value display | `<SsField>` or `<span>` + `.pd-mdm-k` / `.pd-mdm-v` |
| Collapsible section | `<details>` / `<summary>` + `.pd-mdm-subcard` |
| Static section | `<SsSection>` |
| Tabs (static list) | `<button>` + `.pd-primary-tab` |

### Use Telerik

| Scenario | Component |
|----------|-----------|
| Dropdown with search/filter/remote data | `TelerikDropDownList` / `TelerikComboBox` |
| Data grid | `TelerikGrid` |
| Date picker | `TelerikDatePicker` |
| Numeric input | `TelerikNumericTextBox` |
| File upload | `TelerikUpload` |
| Modal/dialog | `TelerikDialog` / `TelerikWindow` |

### Adding a Telerik Component

1. Wrap in `<div class="pd-my-wrapper">`
2. Set sizing parameters (`Size="sm"` for 36px inputs)
3. Add `::deep()` rules scoped to wrapper if needed
4. Follow override priority order

---

## 10. Do-Not-Override Rules

### Absolute

| Rule | Exceptions |
|------|------------|
| Field values: `14px / 500 / var(--color-text-secondary)` | Dense tables (12px/400), metric counters (24px/700), status values (accent colors) |
| Field labels: `11px / 600 / uppercase / var(--color-text-label)` | None |
| No `box-shadow` on cards | Focus rings on inputs only |
| No `font-weight: 700` on field values | `.pd-metric-num` only |
| No color darker than `#4B5563` on field values | Titles only |
| Section headers: normal case | None |
| `!important` only in `::deep()` Telerik overrides | Only for border-color, background, box-shadow |

### Token Integrity

Never use raw hex for: text color, border color, background, font-size, font-weight. Exceptions: scrollbar, focus glow, skeleton gradient, `::deep()`.

### Layout Constraints

| Constraint | Value |
|------------|-------|
| Page max-width | `1440px` |
| Page padding | `16px` |
| Nav panel width | `280px` |
| Split-view gap | `12px` |
| Section header height | `36px` |
| Field grid columns | `2` |
| Field grid column-gap | `32px` |
| Field grid row-gap | `12px` |
| Nav item min-height | `48px` |
| Search input height | `36px` |
| Card border-radius | `8px` |

### Do Not

- Add shadows
- Use `rem` for token values
- Create font sizes outside the scale (11, 12, 14, 16, 24)
- Mix `--ss-text-*` with `--color-text-*` in the same component
- Override Telerik component structure
- Add `letter-spacing` to values or section headers
- Use `text-transform: uppercase` beyond labels and group headings

---

## Appendix: File Map

| File | Purpose |
|------|---------|
| `wwwroot/css/branding.css` | Global tokens, Bootstrap/Telerik overrides |
| `Pages/PropertyWorkOrderDetails.razor` | Page markup and code-behind |
| `Pages/PropertyWorkOrderDetails.razor.css` | Scoped CSS (`pd-*` classes) |
| `Components/SsField.razor` | Reusable field label/value component |
| `Components/SsField.razor.css` | SsField scoped styles |
| `Components/SsSection.razor` | Reusable section header/body component |
| `Components/SsSection.razor.css` | SsSection scoped styles |
| `Layout/PropertyShellLayout.razor` | Shell layout with sidebar (`psh-*`) |
| `Data/PropertyMdmCatalog.cs` | Category/field view model builder |
| `Data/PropertyMdmV4.json` | Embedded field schema |

### Subsection `SsField` marker (code)

- **Class:** `pd-mdm-subcard--ss-field` on `<details class="pd-mdm-subcard …">` when that subsection uses `SsField`.
- **Logic:** `SsFieldSubsectionTitles` in `PropertyWorkOrderDetails.razor` (currently `"Financials"`, `"Ownership & Loan"`).
- **CSS:** Native `.pd-mdm-*` rules are **not** comma-joined with `::deep(.ss-field…)` selectors; duplicate declarations under `.pd-mdm-subcard--ss-field` keep Blazor scoped CSS and native stacking correct.
