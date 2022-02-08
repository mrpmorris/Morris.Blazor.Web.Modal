using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Immutable;

namespace Morris.Blazor.Web.Modal
{
	public class ModalHost : ComponentBase
	{
		private ImmutableArray<Modal> VisibleModals = Array.Empty<Modal>().ToImmutableArray();

		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		[Parameter]
		public Type? DefaultModalLayout { get; set; }

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			RenderFragment? ownChildContent = ChildContent;
			// <CascadingValue>
			{
				builder.OpenComponent<CascadingValue<ModalHost>>(0);
				builder.SetKey(this);
				builder.AddAttribute(1, nameof(CascadingValue<ModalHost>.IsFixed), true);
				builder.AddAttribute(2, nameof(CascadingValue<ModalHost>.Value), this);
				builder.AddAttribute(3, nameof(CascadingValue<ModalHost>.ChildContent),
					new RenderFragment(builder =>
					{
						ImmutableArray<Modal> modals = VisibleModals;
						// <fieldset>
						{
							builder.OpenElement(4, "fieldset");
							if (VisibleModals.Any())
								builder.AddAttribute(5, "disabled");
							builder.AddContent(6, ownChildContent);
							RenderDisabledModals(7, modals, builder);
							builder.CloseElement();
						}
						// </fieldset>

						if (VisibleModals.Any())
						{
							// <div>
							{
								builder.OpenElement(8, "div");
								builder.AddAttribute(9, "class", "modal_screen-obscurer");
								builder.CloseElement();
							}
							// </div>
							RenderModal(builder, modals[^1]);
						}
					}));
				builder.CloseComponent();
			}
			// </CascadingValue>
		}

		private void RenderDisabledModals(int index, ImmutableArray<Modal> modals, RenderTreeBuilder builder)
		{
			if (modals.Length <= 1)
				return;

			for (int i = 0; i < modals.Length - 1; i++)
				RenderModal(builder, modals[i]);
		}

		private void RenderModal(RenderTreeBuilder builder, Modal modal)
		{
			// <CascadingValue>
			{
				builder.OpenComponent<CascadingValue<Modal>>(0);
				builder.SetKey(modal);
				builder.AddAttribute(1, nameof(CascadingValue<Modal>.Value), modal);
				builder.AddAttribute(2, nameof(CascadingValue<Modal>.IsFixed), true);
				builder.AddAttribute(3, nameof(CascadingValue<Modal>.ChildContent),
					new RenderFragment(b =>
					{
						// <div>
						{
							b.OpenElement(4, "div");
							b.SetKey(modal);
							b.AddAttribute(5, "class", $"modal_container {modal.CssClass}");
							// <LayoutView>
							{
								b.OpenComponent<LayoutView>(6);
								b.AddAttribute(7, nameof(LayoutView.ChildContent), modal.ChildContent);
								b.AddAttribute(8, nameof(LayoutView.Layout), modal.Layout ?? DefaultModalLayout);
								b.CloseComponent();
							}
							b.CloseElement();
						}
						// </div>
					}));
				builder.CloseComponent();
			}
			// </CascadingValue>
		}

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
	}
}
