using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Immutable;
using Morris.Blazor.Web.Modal.Internal;
using Microsoft.JSInterop;

namespace Morris.Blazor.Web.Modal
{
	public class ModalHost : ComponentBase
	{
		[Parameter] public RenderFragment? ChildContent { get; set; }
		[Parameter] public Type? DefaultModalLayout { get; set; }
		[Parameter] public bool AutoFocusActiveModal { get; set; } = true;

		[Inject] private IJSRuntime JSRuntime { get; set; } = null!;

		private readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);
		private Modal? ActiveModal => !VisibleModals.Any() ? null : VisibleModals[^1];

		private const string HolderElementId = "morris-blazor-web-modal_modal-holder";

		private Modal? PreviouslyVisibleModal;
		private ImmutableArray<Modal> VisibleModals = Array.Empty<Modal>().ToImmutableArray();
		private Dictionary<Modal, ModalContentRenderer> ModalToModalContentRendererLookup = new Dictionary<Modal, ModalContentRenderer>();

		public bool IsModalVisible { get; set; }

		internal async ValueTask ShowAsync(Modal modal)
		{
			await Lock.WaitAsync();
			try
			{
				VisibleModals = VisibleModals.Add(modal);
			}
			finally
			{
				Lock.Release();
			}
			StateHasChanged();
		}

		internal void ModalShouldRender(Modal modal)
		{
			if (!ModalToModalContentRendererLookup.TryGetValue(modal, out ModalContentRenderer? container))
				return;
			container.NotifyStateHasChanged();
		}

		internal async ValueTask HideAsync(Modal modal)
		{
			await Lock.WaitAsync();
			try
			{
				VisibleModals = VisibleModals.Remove(modal);
			}
			finally
			{
				Lock.Release();
			}
			StateHasChanged();
		}

		internal void RegisterContainerForModal(Modal modal, ModalContentRenderer modalContainer)
		{
			ModalToModalContentRendererLookup[modal] = modalContainer;
		}

		internal void UnregisterContainerForModal(Modal modal)
		{
			ModalToModalContentRendererLookup.Remove(modal);
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			RenderFragment? ownChildContent = ChildContent;
			// <CascadingValue>
			{
				builder.OpenComponent<CascadingValue<ModalHost>>(0);
				{
					builder.SetKey(this);
					builder.AddAttribute(1, nameof(CascadingValue<ModalHost>.IsFixed), true);
					builder.AddAttribute(2, nameof(CascadingValue<ModalHost>.Value), this);
					builder.AddAttribute(3, nameof(CascadingValue<ModalHost>.ChildContent),
						new RenderFragment(builder =>
						{
							ImmutableArray<Modal> modals = VisibleModals;

							builder.OpenElement(0, "section");
							{
								builder.AddAttribute(1, "id", HolderElementId);
								if (ActiveModal is not null)
									builder.AddAttribute(2, "disabled", true);
								builder.AddContent(3, ownChildContent);
								RenderDisabledModals(4, modals, builder);
								if (VisibleModals.Any())
									RenderModal(builder, modals[^1], isActive: true);
							}
							builder.CloseElement(); // section
						}));
				}
			}
			builder.CloseComponent(); // CascadingValue
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
			await Lock.WaitAsync();
			try
			{
				if (ActiveModal != PreviouslyVisibleModal)
				{
					PreviouslyVisibleModal = ActiveModal;
					if (ActiveModal is null)
						await JSRuntime.InvokeVoidAsync("MorrisBlazorWeb.restoreControlsById", HolderElementId);
					else
					{
						await JSRuntime.InvokeVoidAsync("MorrisBlazorWeb.disableControlsById", HolderElementId);
						await JSRuntime.InvokeVoidAsync("MorrisBlazorWeb.restoreControlsById", ActiveModal.Id);
						await JSRuntime.InvokeVoidAsync("MorrisBlazorWeb.focusFirstAvailableControlById", ActiveModal.Id);
					}
				}
			}
			finally
			{
				Lock.Release();
			}
		}

		private void RenderDisabledModals(int index, ImmutableArray<Modal> modals, RenderTreeBuilder builder)
		{
			for (int i = 0; i < modals.Length - 1; i++)
				RenderModal(builder, modals[i]);

			if (VisibleModals.Any())
			{
				builder.OpenElement(index++, "div");
				{
					builder.AddAttribute(index++, "class", "modal_screen-obscurer");
				}
				builder.CloseElement(); // div
			}
		}

		private void RenderModal(RenderTreeBuilder builder, Modal modal, bool isActive = false)
		{
			string modalIsActiveCssClass = isActive ? "modal-active" : "modal-inactive";
			builder.OpenComponent<CascadingValue<Modal>>(0);
			{
				builder.SetKey(modal);
				builder.AddAttribute(0, nameof(CascadingValue<Modal>.Value), modal);
				builder.AddAttribute(1, nameof(CascadingValue<Modal>.IsFixed), true);
				builder.AddAttribute(2, nameof(CascadingValue<Modal>.ChildContent),
					new RenderFragment(builder =>
					{
						builder.OpenComponent<ModalContentRenderer>(0);
						{
							builder.SetKey(modal.Id);
							builder.AddAttribute(2, nameof(ModalContentRenderer.IsActive), isActive);
							builder.AddAttribute(3, nameof(ModalContentRenderer.Modal), modal);
						}
						builder.CloseComponent(); // ModalContainer
					}));
			}
			builder.CloseComponent(); // CascadingValue
		}
	}
}
