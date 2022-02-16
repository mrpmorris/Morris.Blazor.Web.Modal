using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Morris.Blazor.Web.Modal
{
	public class ModalHost : ComponentBase
	{
		[Parameter] public RenderFragment? ChildContent { get; set; }
		[Parameter] public Type? DefaultModalLayout { get; set; }
		[Parameter] public bool AutoFocusActiveModal { get; set; } = true;

		[Inject] private IJSRuntime JSRuntime { get; set; } = null!;

		private Modal? ActiveModal => !VisibleModals.Any() ? null : VisibleModals[^1];

		private const string HolderElementId = "morris-blazor-web-modal_fieldset";
		private Modal? PreviouslyVisibleModal;
		private ImmutableArray<Modal> VisibleModals = Array.Empty<Modal>().ToImmutableArray();

		public bool IsModalVisible { get; set; }

		internal void Show(Modal modal)
		{
			VisibleModals = VisibleModals.Add(modal);
			StateHasChanged();
		}

		internal void Hide(Modal modal)
		{
			VisibleModals = VisibleModals.Remove(modal);
			StateHasChanged();
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

							builder.OpenElement(0, "fieldset");
							{
								builder.AddAttribute(1, "id", HolderElementId);
								builder.AddContent(2, ownChildContent);
								RenderDisabledModals(3, modals, builder);
								if (VisibleModals.Any())
									RenderModal(builder, modals[^1], isActive: true);
							}
							builder.CloseElement(); // fieldset

						}));
				}
			}
			builder.CloseComponent(); // CascadingValue
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
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
				builder.AddAttribute(1, nameof(CascadingValue<Modal>.Value), modal);
				builder.AddAttribute(2, nameof(CascadingValue<Modal>.IsFixed), true);
				builder.AddAttribute(3, nameof(CascadingValue<Modal>.ChildContent),
					new RenderFragment(builder =>
					{
						builder.OpenElement(4, "div");
						{
							builder.SetKey(modal.Id);
							builder.AddAttribute(5, "id", modal.Id);
							builder.AddAttribute(6, "class", $"modal_container {modal.CssClass} {modalIsActiveCssClass}");
							builder.AddAttribute(7, "aria-modal", "true");
							builder.AddAttribute(8, "role", "dialog");
							builder.AddMultipleAttributes(9, modal.AdditionalAttributes);

							builder.OpenComponent<LayoutView>(10);
							{
								builder.AddAttribute(0, nameof(LayoutView.ChildContent), modal.ChildContent);
								builder.AddAttribute(1, nameof(LayoutView.Layout), modal.Layout ?? DefaultModalLayout);
							}
							builder.CloseComponent(); // LayoutView
						}
						builder.CloseElement(); // div
					}));
			}
			builder.CloseComponent(); // CascadingValue
		}
	}
}
