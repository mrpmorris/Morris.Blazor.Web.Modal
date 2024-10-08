# Morris.Blazor.Web.Modal
Modal support or Blazor Web


## Releases

### 3.0
- Support .NET 8
- Use `<dialog>` element to revent keyboard navigation to controls outside of modal

### 2.0
- Support .NET 7
- Remove frameworks no longer supported by Microsoft

### 1.9
 * Fixed bug where some controls wouldn't re-enable when multiple modals were closed at the same time ([#13](https://github.com/mrpmorris/Morris.Blazor.Web.Modal/issues/13))

### 1.8
 * Fixed re-entrancy bug when closing a modal ([#12](https://github.com/mrpmorris/Morris.Blazor.Web.Modal/issues/12))

### 1.7
 * Stopped relying on known dialog sizes for vertical/horizontal positioning

### 1.6
 * Ensured obscurer fills all content when page scrolls vertically
 * Ensure underlying elements are disabled if Modal is rendered visible by default

### 1.5
 * Ensure events trigger re-render in modal ([#7](https://github.com/mrpmorris/Morris.Blazor.Web.Modal/issues/7))

### 1.4
 * Ensure modals are not destroyed / recreated when secondary modals are displayed on top of them ([#4](https://github.com/mrpmorris/Morris.Blazor.Web.Modal/issues/4))

### 1.3.2
 * Use JavaScript to disable obscured controls

### 1.2
 * Ensure screen obscurer covers all disabled controls so screen readers do not read them out

### 1.1
 * Ensure all content other than the active modal is disabled when a modal is visible

### 1.0
 * Initial release