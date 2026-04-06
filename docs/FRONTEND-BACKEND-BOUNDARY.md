# Frontend / Backend Boundary

---

## Frontend Owns

| Responsibility | Details |
|----------------|---------|
| **Layout** | Page structure, split-view panels, grid columns, responsive breakpoints |
| **Styling** | All CSS — tokens, typography, spacing, borders, backgrounds |
| **Component composition** | Choosing `SsField`, `SsSection`, `TelerikGrid`, native HTML |
| **UI states** | Loading skeletons, empty states ("—"), error alerts, flash animations |
| **Interaction** | Hover, focus, active states, tab switching, section expand/collapse |
| **Responsiveness** | Breakpoints at 1100px, 900px, 520px |
| **Client-side routing** | Blazor `@page` routes, tab state, section selection |
| **Field formatting** | Display formatting (currency symbols, date display, tabular-nums) |

## Backend Owns

| Responsibility | Details |
|----------------|---------|
| **APIs** | Endpoint design, versioning, authentication |
| **Data contracts** | JSON shape, field names, types, nullability |
| **Validation** | Business rules, required fields, value constraints |
| **Business logic** | Status calculations, eligibility rules, workflow state |
| **Permissions** | Who sees what, field-level visibility, role-based access |
| **Integrations** | External systems, data sync, event handling |
| **Data loading** | Pagination, filtering, sorting at the data layer |

---

## API Consumption Rules

### Request Pattern

Frontend calls backend via `HttpClient` (injected). All API calls happen in
`OnAfterRenderAsync(firstRender)` or on explicit user action.

```csharp
@inject HttpClient Http

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (!firstRender) return;
    try
    {
        _data = await Http.GetFromJsonAsync<MyDto>("api/endpoint");
    }
    catch (Exception ex)
    {
        _loadError = $"Could not load data: {ex.Message}";
    }
    finally
    {
        _isReady = true;
        await InvokeAsync(StateHasChanged);
    }
}
```

### Current State

The Work Orders Overview page currently loads data from an embedded JSON file
via `PropertyMdmCatalog.LoadRoot()`. This is a temporary pattern for
prototyping. Future screens should consume real APIs.

### Response Shape Expectations

Frontend expects:

| Requirement | Detail |
|-------------|--------|
| **Consistent field names** | PascalCase C# properties, camelCase JSON |
| **Null for missing values** | `null`, not empty string or `"N/A"` |
| **Flat field structure** | Label/value pairs grouped by category and subsection |
| **Typed values** | Dates as ISO 8601 strings, currency as decimals, booleans as `true`/`false` |
| **Category metadata** | Each category has: id, title, status (delayed/healthy), subsections |
| **Subsection metadata** | Each subsection has: key, title, ordered list of fields |
| **Field metadata** | Each field has: id, label, value, kind (currency/date/boolean/phone/email/textarea/text), visibility |

### Visibility Contract

| Visibility value | Frontend behavior |
|------------------|-------------------|
| `"default"` | Always show |
| `"clientSpecific"` | Show only when "Client fields" toggle is on |
| `"conditional"` | Show only when value is non-empty |

### Error Handling

| Scenario | Frontend behavior |
|----------|-------------------|
| API returns 200 + data | Render normally |
| API returns 200 + empty array | Show empty state (no fields, no error) |
| API returns 4xx/5xx | Show error alert with message |
| Network failure | Show error alert with generic message |
| Slow response | Show loading skeleton until resolved |

---

## What Frontend Does NOT Do

- Does not calculate derived values (e.g., "Days Delinquent" from dates)
- Does not determine field visibility rules (backend sets `visibility` per field)
- Does not enforce business validation (backend returns validated data)
- Does not decide status (delayed/healthy) — backend provides `IsDelayed`
- Does not store data persistently — all state is in-memory per session

## What Backend Does NOT Do

- Does not specify font sizes, colors, or spacing
- Does not dictate component choices (grid vs. cards vs. fields)
- Does not control layout or responsive behavior
- Does not embed HTML or CSS in API responses
- Does not return pre-formatted display strings (frontend formats raw values)
