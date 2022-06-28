using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Morris.Blazor.Web.Modal
{
	public class Modal : ComponentBase, IAsyncDisposable
	{
		[Parameter] public string? CssClass { get; set; }
		[Parameter] public Type? Layout { get; set; }
		[Parameter] public bool Visible { get; set; }
		[Parameter] public RenderFragment? ChildContent { get; set; }
		[CascadingParameter] private ModalHost ModalHost { get; set; } = null!;
		[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);

		private readonly string _id = $"Id_{Guid.NewGuid()}";
		public string Id => _id;
		private bool HasRendered;

		public T? GetAttributeOrDefault<T>(string name)
		{
			ArgumentNullException.ThrowIfNull(name);
			return (T?)AdditionalAttributes.GetValueOrDefault(name);
		}

		protected override bool ShouldRender()
		{
			ModalHost.ModalShouldRender(this);
			return false;
		}

		protected override void OnInitialized()
		{
			if (ModalHost is null)
				throw new NullReferenceException(nameof(ModalHost));

			base.OnInitialized();
		}

		public override async Task SetParametersAsync(ParameterView parameters)
		{
			bool wasVisible = Visible;
			await base.SetParametersAsync(parameters);
			if (!HasRendered)
				return;
			if (!wasVisible && Visible)
				await ShowAsync();
			if (wasVisible && !Visible)
				await HideAsync();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
			if (!HasRendered)
			{
				HasRendered = true;
				if (Visible)
				{
					await Task.Yield();
					await ModalHost.ShowAsync(this);
				}
			}
		}

		private ValueTask ShowAsync()
		{
			return ModalHost.ShowAsync(this);
		}

		private ValueTask HideAsync()
		{
			return ModalHost.HideAsync(this);
		}

		ValueTask IAsyncDisposable.DisposeAsync()
		{
			return ModalHost.HideAsync(this);
		}
	}
}
