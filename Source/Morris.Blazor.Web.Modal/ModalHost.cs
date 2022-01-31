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
				builder.OpenComponent<CascadingValue<ModalHost>>(4);
				builder.AddAttribute(0, nameof(CascadingValue<ModalHost>.IsFixed), true);
				builder.AddAttribute(1, nameof(CascadingValue<ModalHost>.Value), this);
				builder.AddAttribute(2, nameof(CascadingValue<ModalHost>.ChildContent),
					new RenderFragment(b =>
					{
						b.AddContent(3, ownChildContent);
						RenderModals(4, b);
					}));
				builder.CloseComponent();
			}
			// </CascadingValue>
		}

		private void RenderModals(int index, RenderTreeBuilder builder)
		{
			var modals = VisibleModals;
			if (modals.Length == 0)
				return;

			// <fieldset>
			{
				builder.OpenElement(index++, "fieldset");
				builder.AddAttribute(index++, "disabled");
				for (int i = 0; i < modals.Length - 1; i++)
					RenderModal(index++, builder, modals[i]);

				// <div>
				{
					builder.OpenElement(index++, "div");
					builder.AddAttribute(0, "class", "modal_screen-obscurer");
					builder.CloseElement();
				}
				// </div>
				builder.CloseElement();
			}
			// </fieldset>

			RenderModal(index, builder, modals[^1]);
		}

		private void RenderModal(int index, RenderTreeBuilder builder, Modal modal)
		{
			// <CascadingValue>
			{
				builder.OpenComponent<CascadingValue<Modal>>(0);
				builder.AddAttribute(1, nameof(CascadingValue<Modal>.Value), modal);
				builder.AddAttribute(2, nameof(CascadingValue<Modal>.IsFixed), true);
				builder.AddAttribute(3, nameof(CascadingValue<Modal>.ChildContent),
					new RenderFragment(b =>
					{
						// <div>
						b.OpenElement(index++, "div");
						{
							b.SetKey(modal);
							b.AddAttribute(index++, "class", $"modal_container {modal.CssClass}");
							// <LayoutView>
							{
								b.OpenComponent<LayoutView>(0);
								b.AddAttribute(1, nameof(LayoutView.ChildContent), modal.ChildContent);
								b.AddAttribute(2, nameof(LayoutView.Layout), modal.Layout ?? DefaultModalLayout);
								b.CloseComponent();
							}
						}
						b.CloseElement();
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
