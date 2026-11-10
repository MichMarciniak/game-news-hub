# Tailwind CSS classes from tailwindcss-primeui (OpenVue / PrimeVue)

Source: `node_modules/tailwindcss-primeui/v4/`
Package version: see `package.json`

---

## Color theme variables (`theme/colors.css`)

These are registered via `@theme inline` and map to PrimeVue CSS variables.

| Class | CSS variable |
|-------|-------------|
| `primary-*` (50-950) | `--p-primary-*` |
| `primary-emphasis` | `--p-primary-hover-color` |
| `primary-emphasis-alt` | `--p-primary-active-color` |
| `primary-contrast` | `--p-primary-contrast-color` |
| `surface-0` | `--p-surface-0` |
| `surface-50` | `--p-surface-50` |
| `surface-100` | `--p-surface-100` |
| `surface-200` | `--p-surface-200` |
| `surface-300` | `--p-surface-300` |
| `surface-400` | `--p-surface-400` |
| `surface-500` | `--p-surface-500` |
| `surface-600` | `--p-surface-600` |
| `surface-700` | `--p-surface-700` |
| `surface-800` | `--p-surface-800` |
| `surface-900` | `--p-surface-900` |
| `surface-950` | `--p-surface-950` |

> Example usage: `bg-primary-500`, `text-primary-emphasis`, `bg-surface-100`

---

## Utility classes (`utils/preset.css`)

| Class | Description |
|-------|-------------|
| `border-surface` | Border color from `--p-content-border-color` |
| `bg-emphasis` | Hover background + hover text color |
| `bg-highlight` | Highlight background + highlight text color |
| `bg-highlight-emphasis` | Focus background + focus text color |
| `rounded-border` | Border radius from `--p-content-border-radius` |
| `text-color` | Text color from `--p-text-color` |
| `text-color-emphasis` | Text hover color from `--p-text-hover-color` |
| `text-muted-color` | Muted text color from `--p-text-muted-color` |
| `text-muted-color-emphasis` | Muted hover text color from `--p-text-hover-muted-color` |

**Used in codebase:** `text-muted-color` (20x), `bg-highlight` (9x), `border-surface` (7x), `text-color` (3x), `text-muted-color-emphasis` (1x), `bg-emphasis` (1x), `rounded-border` (1x), `surface-400` (1x)

---

## Custom variants (`utils/variants.css`)

All match on `[data-p~="..."]` attributes set by PrimeVue components.

| Variant | Matches |
|---------|---------|
| `p-invalid` | `[data-p~="invalid"]` |
| `p-small` | `[data-p~="small"]` |
| `p-large` | `[data-p~="large"]` |
| `p-xlarge` | `[data-p~="xlarge"]` |
| `p-fluid` | `[data-p~="fluid"]` |
| `p-filled` | `[data-p~="filled"]` |
| `p-horizontal` | `[data-p~="horizontal"]` |
| `p-vertical` | `[data-p~="vertical"]` |
| `p-stacked` | `[data-p~="stacked"]` |
| `p-checked` | `[data-p~="checked"]` |
| `p-disabled` | `[data-p~="disabled"]` or `[data-p-disabled="true"]` |
| `p-enabled` | `:not([data-p~="disabled"])` |
| `p-selected` | `[data-p~="selected"]` or `[data-p-selected="true"]` |
| `p-selectable` | `[data-p~="selectable"]` / `[data-p-selectable="true"]` / `[data-p-selectable-row="true"]` |
| `p-left` | `[data-p~="left"]` / `[data-p-left="true"]` |
| `p-right` | `[data-p~="right"]` / `[data-p-right="true"]` |
| `p-top` | `[data-p~="top"]` / `[data-p-top="true"]` |
| `p-bottom` | `[data-p~="bottom"]` / `[data-p-bottom="true"]` |
| `p-alternate` | `[data-p~="alternate"]` |
| `p-center` | `[data-p~="center"]` |
| `p-top-center` | `[data-p~="top-center"]` |
| `p-bottom-center` | `[data-p~="bottom-center"]` |
| `p-active` | `[data-p~="active"]` / `[data-p-active="true"]` |
| `p-focus` | `[data-p~="focus"]` / `[data-p-focused="true"]` |
| `p-focus-visible` | `[data-p~="focus-visible"]` |
| `p-readonly` | `[data-p~="readonly"]` |
| `p-removable` | `[data-p~="removable"]` |
| `p-circle` | `[data-p~="circle"]` |
| `p-empty` | `[data-p~="empty"]` |
| `p-determinate` | `[data-p~="determinate"]` |
| `p-indeterminate` | `[data-p~="indeterminate"]` |
| `p-icon-only` | `[data-p~="icon-only"]` |
| `p-rounded` | `[data-p~="rounded"]` |
| `p-raised` | `[data-p~="raised"]` |
| `p-toggleable` | `[data-p~="toggleable"]` |
| `p-solid` | `[data-p~="solid"]` |
| `p-dashed` | `[data-p~="dashed"]` |
| `p-dotted` | `[data-p~="dotted"]` |
| `p-secondary` | `[data-p~="secondary"]` |
| `p-contrast` | `[data-p~="contrast"]` |
| `p-success` | `[data-p~="success"]` |
| `p-info` | `[data-p~="info"]` |
| `p-warn` | `[data-p~="warn"]` |
| `p-danger` | `[data-p~="danger"]` |
| `p-error` | `[data-p~="error"]` |
| `p-custom` | `[data-p~="custom"]` |
| `p-outlined` | `[data-p~="outlined"]` |
| `p-text` | `[data-p~="text"]` |
| `p-simple` | `[data-p~="simple"]` |
| `p-scrollable` | `[data-p~="scrollable"]` |
| `p-maximized` | `[data-p~="maximized"]` |
| `p-modal` | `[data-p~="modal"]` |
| `p-flipped` | `[data-p-popover-flipped~="true"]` |
| `p-nested` | `[data-p~="nested"]` |
| `p-weak` | `[data-p~="weak"]` |
| `p-medium` | `[data-p~="medium"]` |
| `p-strong` | `[data-p~="strong"]` |
| `p-portal-body` | `[data-p~="portal-body"]` |
| `p-portal-self` | `[data-p~="portal-self"]` |
| `p-has-s-icon` | `[data-p-has-s-icon~="true"]` |
| `p-has-e-icon` | `[data-p-has-e-icon~="true"]` |
| `p-full-screen` | `[data-p~="full-screen"]` |
| `p-open` | `[data-p~="open"]` |
| `p-popup` | `[data-p~="popup"]` |
| `p-placeholder` | `::placeholder` / `[data-p~="placeholder"]` |
| `p-clearable` | `[data-p~="clearable"]` |
| `p-editable` | `[data-p~="editable"]` |
| `p-has-dropdown` | `[data-p-has-dropdown="true"]` |
| `p-has-chip` | `[data-p~="has-chip"]` |
| `p-inline` | `[data-p~="inline"]` |
| `p-today` | `[data-p~="today"]` |
| `p-other-month` | `[data-p~="other-month"]` |
| `p-time-only` | `[data-p~="time-only"]` |
| `p-completed` | `[data-p~="completed"]` |
| `p-loading` | `[data-p~="loading"]` |
| `p-leaf` | `[data-p~="leaf"]` / `[data-p-leaf="true"]` |
| `p-sortable` | `[data-p~="sortable"]` / `[data-p-sortable-column="true"]` / `[data-p-sortable="true"]` |
| `p-sorted` | `[data-p~="sorted"]` / `[data-p-sorted="true"]` |
| `p-resizable` | `[data-p~="resizable"]` / `[data-p-resizable-column="true"]` / `[data-p-resizable="true"]` |
| `p-hoverable` | `[data-p~="hoverable"]` |
| `p-flex-scrollable` | `[data-p~="flex-scrollable"]` / `[data-p-flex-scrollable="true"]` |
| `p-frozen` | `[data-p~="frozen"]` / `[data-p-frozen="true"]` / `[data-p-frozen-column="true"]` |

**Used in codebase:** `p-text` (2x)

---

## Animation utilities (`utils/animation.css`)

| Class | Animation |
|-------|-----------|
| `animate-fadein` | `fadein 0.15s linear` |
| `animate-fadeout` | `fadeout 0.15s linear` |
| `animate-slidedown` | `slidedown 0.45s ease-in-out` |
| `animate-slideup` | `slideup 0.45s cubic-bezier(0, 1, 0, 1)` |
| `animate-scalein` | `scalein 0.15s linear` |
| `animate-fadeinleft` | `fadeinleft 0.15s linear` |
| `animate-fadeoutleft` | `fadeoutleft 0.15s linear` |
| `animate-fadeinright` | `fadeinright 0.15s linear` |
| `animate-fadeoutright` | `fadeoutright 0.15s linear` |
| `animate-fadeinup` | `fadeinup 0.15s linear` |
| `animate-fadeoutup` | `fadeoutup 0.15s linear` |
| `animate-fadeindown` | `fadeindown 0.15s linear` |
| `animate-fadeoutdown` | `fadeoutdown 0.15s linear` |
| `animate-width` | `width 0.45s linear` |
| `animate-flip` | `flip 0.15s linear` |
| `animate-flipup` | `flipup 0.15s linear` |
| `animate-flipleft` | `flipleft 0.15s linear` |
| `animate-flipright` | `flipright 0.15s linear` |
| `animate-zoomin` | `zoomin 0.15s linear` |
| `animate-zoomindown` | `zoomindown 0.15s linear` |
| `animate-zoominleft` | `zoominleft 0.15s linear` |
| `animate-zoominright` | `zoominright 0.15s linear` |
| `animate-zoominup` | `zoominup 0.15s linear` |

---

## Enter/Leave transition utilities (`utils/enterleave.css`)

| Class | Description |
|-------|-------------|
| `animate-enter` | Sets enter animation name |
| `animate-leave` | Sets leave animation name |
| `fade-in-*` | `--p-enter-opacity` (integer percent) |
| `fade-out-*` | `--p-leave-opacity` (integer percent) |
| `zoom-in-*` | `--p-enter-scale` (integer percent) |
| `zoom-out-*` | `--p-leave-scale` (integer percent) |
| `spin-in-*` | `--p-enter-rotate` (degrees) |
| `spin-out-*` | `--p-leave-rotate` (degrees) |
| `slide-in-from-t-*` | Translate Y up |
| `slide-in-from-b-*` | Translate Y down |
| `slide-in-from-l-*` | Translate X left |
| `slide-in-from-r-*` | Translate X right |
| `slide-out-from-t-*` | Leave translate Y up |
| `slide-out-from-b-*` | Leave translate Y down |
| `slide-out-from-l-*` | Leave translate X left |
| `slide-out-from-r-*` | Leave translate X right |

---

## Animation timing / control utilities

| Class | Property |
|-------|----------|
| `animate-delay-none` / `animate-delay-*` | `animation-delay` |
| `animate-normal` / `animate-reverse` / `animate-alternate` / `animate-alternate-reverse` | `animation-direction` |
| `animate-duration-*` | `animation-duration` (ms) |
| `animate-fill-none` / `animate-fill-forwards` / `animate-fill-backwards` / `animate-fill-both` | `animation-fill-mode` |
| `animate-infinite` / `animate-once` / `animate-twice` | `animation-iteration-count` |
| `animate-running` / `animate-paused` | `animation-play-state` |
| `animate-ease-linear` / `animate-ease-in` / `animate-ease-out` / `animate-ease-in-out` | `animation-timing-function` |

---

## Backface utilities

| Class | Property |
|-------|----------|
| `backface-visible` | `backface-visibility: visible` |
| `backface-hidden` | `backface-visibility: hidden` |

---

## Usage notes

- All `p-*` variants are **custom variants** (not standard Tailwind utilities). They activate when PrimeVue sets `data-p="..."` attributes on its components.
- Color classes (`primary-*`, `surface-*`) work like standard Tailwind color utilities because they are registered via `@theme inline`.
- Utility classes (`text-muted-color`, `bg-highlight`, `border-surface`, etc.) work like standard Tailwind utilities.
- The project uses Tailwind CSS v4 (`@tailwindcss/vite`).

---

## Classes actually found in this codebase (`src/`)

| Class | Count | File(s) |
|-------|-------|---------|
| `primary` | 31 | Multiple (buttons, badges, links) |
| `text-muted-color` | 20 | Multiple (muted text) |
| `bg-highlight` | 9 | Multiple (highlighted rows/cards) |
| `border-surface` | 7 | Multiple (borders) |
| `text-color` | 3 | Multiple |
| `p-text` | 2 | Multiple (PrimeVue `variant="text"`) |
| `text-muted-color-emphasis` | 1 | — |
| `surface-400` | 1 | — |
| `rounded-border` | 1 | — |
| `bg-emphasis` | 1 | — |
