# Screen Build Checklist

---

## Implemented Screens

### Work Orders Overview

| Item | Status |
|------|--------|
| **Screen** | Property Work Order Details |
| **Route** | `/properties/work-orders/123-main-st`, `/property-details` |
| **Layout** | `PropertyShellLayout` |
| **Status** | Implemented |
| **Components used** | `SsField` in **Financials** and **Ownership & Loan** (parent `<details>` marked `pd-mdm-subcard--ss-field`); all other MDM subsections use native `pd-mdm-k` / `pd-mdm-v`. `SsSection` exists but is not used in `<details>` subsections on this screen. |
| **New patterns** | None — this is the reference implementation |
| **API dependencies** | `PropertyMdmCatalog.LoadRoot()` (embedded JSON, no HTTP) |
| **Ready for frontend** | Yes |
| **Blocked** | No |

---

## Field rendering: `pd-mdm-*` vs `SsField`

Use this on any screen that follows the MDM inspector / subsection pattern.

| Situation | Use |
|-----------|-----|
| **Default / not yet migrated** | Raw `pd-mdm-k` / `pd-mdm-v` (and related `pd-mdm-*` wrappers) inside the subsection grid. No `SsField` and no `pd-mdm-subcard--ss-field` on `<details>`. |
| **Opt-in `SsField` for a subsection** | Add the subsection title to the code allowlist (e.g. `SsFieldSubsectionTitles`), set `pd-mdm-subcard--ss-field` on that subsection’s `<details>`, render fields with `<SsField>` inside the existing `pd-mdm-sf-wrap` structure, and rely on the page’s scoped `::deep(.ss-field…)` rules — **do not** comma-merge those selectors with native `pd-mdm-*` rules in scoped CSS. |
| **Scaling to more subsections** | **Validate each subsection** (visual + CSS + edge cases) before adding it to the allowlist. Do not blanket-convert an entire page in one pass. |

---

## Planned Screens

### Tasks Tab

| Item | Status |
|------|--------|
| **Screen** | Tasks tab within property detail |
| **Status** | Not started |
| **Components** | `SsSection`, `SsField` for detail view; likely `TelerikGrid` for list |
| **New patterns** | Task list grid, status badges |
| **API dependencies** | Task list endpoint (TBD) |
| **Ready for frontend** | No — needs API contract |
| **Blocked** | API contract |

### Financials Tab

| Item | Status |
|------|--------|
| **Screen** | Financials tab within property detail |
| **Status** | Not started |
| **Components** | `SsSection`, `SsField` for summary; likely `TelerikGrid` for line items |
| **New patterns** | Currency formatting, totals row |
| **API dependencies** | Financials endpoint (TBD) |
| **Ready for frontend** | No — needs API contract |
| **Blocked** | API contract |

---

## Reusable Checklist Template

Copy this for each new screen:

```markdown
### [Screen Name]

| Item | Status |
|------|--------|
| **Screen** | [Description] |
| **Route** | [Blazor route] |
| **Layout** | [MainLayout / PropertyShellLayout] |
| **Status** | [Not started / In progress / Implemented] |
| **Components used** | [List SsField, SsSection, TelerikGrid, etc.] |
| **New patterns** | [Any patterns not yet in design system] |
| **API dependencies** | [Endpoint names or "embedded data"] |
| **Ready for frontend** | [Yes / No — reason] |
| **Blocked** | [No / Yes — what's blocking] |
```

---

## Pre-Build Verification

Before starting any new screen, verify:

- [ ] Design tokens are loaded (`branding.css` included in `index.html`)
- [ ] `_Imports.razor` includes `@using SinglesourceApp.Components`
- [ ] Layout is chosen (`PropertyShellLayout` for property pages, `MainLayout` for others)
- [ ] API contract is defined and agreed with backend
- [ ] Response shape matches the data models needed for display
- [ ] No new font sizes, weights, or colors are needed (if they are, update `branding.css` first)

## Post-Build Verification

After implementing a screen:

- [ ] MDM subsections using `SsField` have `pd-mdm-subcard--ss-field` on `<details>`, are allowlisted in code, and were validated before adding (see **Field rendering** above)
- [ ] All field labels use `SsField` or `.pd-mdm-k` pattern (11px / 600 / uppercase / label color)
- [ ] All field values use `SsField` or `.pd-mdm-v` pattern (14px / 500 / secondary)
- [ ] No hardcoded hex values for text, border, or background
- [ ] No `box-shadow` on any card
- [ ] No `font-weight: 700` on field values
- [ ] No font-size above 14px on field values
- [ ] Cards use standard base (white bg, default border, 8px radius, no shadow)
- [ ] Telerik components are wrapped with project-scoped classes
- [ ] `::deep()` rules (if any) are scoped to wrappers
- [ ] Responsive breakpoints tested at 1100px, 900px, 520px
