using Microsoft.AspNetCore.Components;

namespace Morris.Blazor.Web.Modal;

public partial class ModalInput<T>
{
	[Parameter] public string? CssClass { get; set; }
	[Parameter] public Type? Layout { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

	private bool IsModalVisible => TaskCompletionSource is not null;
	private TaskCompletionSource<T>? TaskCompletionSource { get; set; }



}
