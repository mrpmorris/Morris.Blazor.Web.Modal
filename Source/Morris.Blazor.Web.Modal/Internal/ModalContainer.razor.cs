using Microsoft.AspNetCore.Components;

namespace Morris.Blazor.Web.Modal.Internal;

public partial class ModalContainer
{
	[Parameter] public bool IsActive { get; set; }
	[Parameter] public Modal? Modal { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Modal is null)
			throw new ArgumentNullException(nameof(Modal));
	}

	protected override bool ShouldRender()
	{
		Modal?.NotifyStateHasChanged();
		return base.ShouldRender();
	}

	private string ActiveStatusCss => IsActive ? "modal-active" : "modal-inactive";
}
