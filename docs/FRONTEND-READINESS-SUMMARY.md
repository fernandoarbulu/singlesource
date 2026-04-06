# Frontend Readiness Summary — Work Orders Overview

Short companion to `HANDOFF-WorkOrdersOverview.md`, `COMPONENT-INVENTORY.md`, and `SCREEN-BUILD-CHECKLIST.md`.

---

## What is complete

- **Work Orders Overview** (`PropertyWorkOrderDetails.razor`) as an end-to-end Blazor WASM screen: shell layout, inspector split-view, MDM-driven categories and fields, search, and Telerik usage per existing patterns.
- **Design tokens and typography** wired through shared CSS (`branding.css`, page-scoped rules).
- **Partial `SsField` integration** for two MDM subsections — **Financials** and **Ownership & Loan** — with the parent `<details>` marked `pd-mdm-subcard--ss-field` and scoped `::deep(.ss-field…)` styling in `PropertyWorkOrderDetails.razor.css`.

---

## What is proven

- **`SsField`** behaves correctly for label/value display and empty states when used inside the MDM grid with the subsection marker and page CSS (currency, date, bool, phone, email, textarea variants as implemented).
- **Incremental adoption** works: opt-in via an allowlist (`SsFieldSubsectionTitles`), one subsection at a time, without refactoring the whole page.
- **Scoped CSS safety**: native `pd-mdm-*` rules stay separate from `::deep(.ss-field…)` chains; avoiding comma-merged selectors prevents Blazor scoped-CSS regressions.

---

## What is intentionally not refactored

- **All other MDM subsections** still use **native** `pd-mdm-k` / `pd-mdm-v` rendering — no blanket migration to `SsField`.
- **`SsSection`** is **not** used inside `<details>` subsections on this screen; collapsible subcards already provide the header + body structure.
- **No full-page or cross-cutting component refactor** was required to ship the current behavior.

---

## What backend can rely on

- **Stable field identity and ordering** from the embedded MDM catalog (`PropertyMdmCatalog` / `PropertyMdmV4.json`) for Work Orders Overview–style screens until an API replaces embedded data.
- **Predictable UI structure**: subsection titles map to collapsible regions; fields map to rows in the grid with consistent DOM classes for automation or docs.
- **API contracts** for future screens should preserve **subsection + field id/key** semantics so the frontend can bind without ad-hoc reshaping.

---

## What next screens should follow

1. **Default to native `pd-mdm-*`** for new MDM subsections until explicitly validated.
2. **To adopt `SsField`:** add the subsection to the allowlist, apply `pd-mdm-subcard--ss-field` on the corresponding `<details>`, mirror the `pd-mdm-sf-wrap` + `SsField` markup, and extend page-scoped `::deep` rules only under that marker.
3. **Validate subsection-by-subsection** (layout, long values, empty states, specialized value types) before scaling the pattern.
4. Use **`SsSection`** only where a **non-`<details>`** layout needs a static section header + grid (see `COMPONENT-INVENTORY.md`).

For full detail, see **Frontend Implementation Status** in `HANDOFF-WorkOrdersOverview.md`.
