using Microsoft.AspNetCore.Components;

namespace Morris.Blazor.Web.Modal.Internal;

public partial class ModalContentRenderer : IDisposable
{
	[Parameter] public bool IsActive { get; set; }
	[Parameter] public Modal? Modal { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[CascadingParameter] public ModalHost? ModalHost { get; set; }

	void IDisposable.Dispose()
	{
		if (Modal is not null)
			ModalHost?.UnregisterContainerForModal(Modal);
	}

	internal void NotifyStateHasChanged()
	{
		StateHasChanged();
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (ModalHost is null)
			throw new ArgumentNullException(nameof(ModalHost));
		if (Modal is null)
			throw new ArgumentNullException(nameof(Modal));
		ModalHost.RegisterContainerForModal(Modal, this);
	}

	private string ActiveStatusCss => IsActive ? "modal-active" : "modal-inactive";

}
