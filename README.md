# Morris.Blazor.Web.Modal
Modal support or Blazor Web


## Releases

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
 * Control events are not triggering re-render in modal ([#7](https://github.com/mrpmorris/Morris.Blazor.Web.Modal/issues/7))

### 1.4
 * Existing modal controls are being recreated when secondary modals are opened/closed ([#4](https://github.com/mrpmorris/Morris.Blazor.Web.Modal/issues/4))
