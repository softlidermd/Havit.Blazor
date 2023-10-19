﻿using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace Havit.Blazor.Components.Web.Bootstrap.Internal;

public partial class HxMultiSelectInternal<TValue, TItem> : IAsyncDisposable
{
	[Parameter] public string InputId { get; set; }

	[Parameter] public string InputCssClass { get; set; }

	[Parameter] public string InputText { get; set; }

	[Parameter] public bool EnabledEffective { get; set; }

	[Parameter] public List<TItem> ItemsToRender { get; set; }

	[Parameter] public List<TValue> SelectedValues { get; set; }

	[Parameter] public Func<TItem, string> TextSelector { get; set; }

	[Parameter] public Func<TItem, TValue> ValueSelector { get; set; }

	[Parameter] public string NullDataText { get; set; }

	[Parameter] public EventCallback<SelectionChangedArgs> ItemSelectionChanged { get; set; }

	[Parameter] public string InputGroupCssClass { get; set; }

	[Parameter] public string InputGroupStartText { get; set; }

	[Parameter] public RenderFragment InputGroupStartTemplate { get; set; }

	[Parameter] public string InputGroupEndText { get; set; }

	[Parameter] public RenderFragment InputGroupEndTemplate { get; set; }

	[Parameter] public bool AllowFiltering { get; set; }

	[Parameter] public Func<TItem, string, bool> FilterPredicate { get; set; }

	[Parameter] public bool ClearFilterOnHide { get; set; }

	[Parameter] public EventCallback<string> OnShown { get; set; }

	[Parameter] public EventCallback<string> OnHidden { get; set; }

	[Parameter] public RenderFragment FilterEmptyResultTemplate { get; set; }

	[Parameter] public string FilterEmptyResultText { get; set; }

	[Parameter] public bool AllowSelectAll { get; set; }

	[Parameter] public string SelectAllText { get; set; }

	[Parameter] public IconBase FilterSearchIcon { get; set; }

	[Parameter] public IconBase FilterClearIcon { get; set; }

	/// <summary>
	/// Additional attributes to be splatted onto an underlying HTML element.
	/// </summary>
	[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }

	[Inject] private IJSRuntime JSRuntime { get; set; }

	[Inject] private IStringLocalizer<HxMultiSelect> StringLocalizer { get; set; }

	protected bool HasInputGroupsEffective => !String.IsNullOrWhiteSpace(InputGroupStartText) || !String.IsNullOrWhiteSpace(InputGroupEndText) || (InputGroupStartTemplate is not null) || (InputGroupEndTemplate is not null);

	private IJSObjectReference jsModule;
	private readonly DotNetObjectReference<HxMultiSelectInternal<TValue, TItem>> dotnetObjectReference;
	private ElementReference elementReference;
	private ElementReference filterInputReference;
	private bool isShown;
	private string filterText = string.Empty;
	private bool selectAll;
	private bool disposed;

	public HxMultiSelectInternal()
	{
		dotnetObjectReference = DotNetObjectReference.Create(this);
	}

	/// <inheritdoc cref="ComponentBase.OnAfterRenderAsync(bool)" />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await EnsureJsModuleAsync();
			if (disposed)
			{
				return;
			}

			await jsModule.InvokeVoidAsync("initialize", elementReference, dotnetObjectReference);
		}
	}

	private async Task EnsureJsModuleAsync()
	{
		jsModule ??= await JSRuntime.ImportHavitBlazorBootstrapModuleAsync(nameof(HxMultiSelect));
	}

	private async Task HandleItemSelectionChangedAsync(bool newChecked, TItem item, bool triggerSelectAllEvent = true)
	{
		await ItemSelectionChanged.InvokeAsync(new SelectionChangedArgs
		{
			Checked = newChecked,
			Item = item
		});

		if (triggerSelectAllEvent)
		{
			selectAll = false;
		}
	}

	private async Task HandleSelectAllClickedAsync()
	{
		var filteredItems = GetFilteredItems();

		// If all items are already selected then they should be deselected, otherwise only records that aren't selected should be
		if (selectAll)
		{
			foreach (var item in filteredItems)
			{
				// We don't want to trigger select all triggers each time, an item is selected, just once at the end
				await HandleItemSelectionChangedAsync(false, item, false);
			}
		}
		else
		{
			foreach (var item in filteredItems)
			{
				var value = SelectorHelpers.GetValue<TItem, TValue>(ValueSelector, item);
				var itemSelected = DoSelectedValuesContainValue(value);

				// If the item is already selected we don't need to reselect it
				if (!itemSelected)
				{
					// We don't want to trigger select all triggers each time, an item is selected, just once at the end
					await HandleItemSelectionChangedAsync(!itemSelected, item, false);
				}
			}
		}

		selectAll = !selectAll;
	}

	private void HandleCrossClick()
	{
		filterText = string.Empty;
	}

	private bool DoSelectedValuesContainValue(TValue value)
	{
		return SelectedValues?.Contains(value) ?? false;
	}

	private void HandleFilterInputChanged(ChangeEventArgs e)
	{
		filterText = e.Value?.ToString() ?? string.Empty;
		selectAll = false;
	}

	private List<TItem> GetFilteredItems()
	{
		if (!AllowFiltering || string.IsNullOrEmpty(filterText))
		{
			return ItemsToRender;
		}

		var filterPredicate = FilterPredicate ?? DefaultFilterPredicate;
		return ItemsToRender.Where(x => filterPredicate(x, filterText)).ToList();

		bool DefaultFilterPredicate(TItem item, string filter)
		{
			return string.IsNullOrEmpty(filter) || TextSelector(item).Contains(filter, StringComparison.OrdinalIgnoreCase);
		}
	}

	private string GetSelectAllText()
	{
		if (SelectAllText is not null)
		{
			return SelectAllText;
		}

		return StringLocalizer["SelectAllDefaultText"];
	}

	private string GetFilterEmptyResultText()
	{
		if (FilterEmptyResultText is not null)
		{
			return FilterEmptyResultText;
		}

		return StringLocalizer["FilterEmptyResultDefaultText"];
	}

	public async ValueTask FocusAsync()
	{
		if (EqualityComparer<ElementReference>.Default.Equals(elementReference, default))
		{
			throw new InvalidOperationException($"Cannot focus {this.GetType()}. The method must be called after first render.");
		}
		await elementReference.FocusAsync();
	}

	/// <summary>
	/// Receives notification from JavaScript when item is hidden.
	/// </summary>
	[JSInvokable("HxMultiSelect_HandleJsHidden")]
	public Task HandleJsHidden()
	{
		isShown = false;

		if (ClearFilterOnHide && filterText != string.Empty)
		{
			filterText = string.Empty;
			StateHasChanged();
		}

		return Task.CompletedTask;
	}

	[JSInvokable("HxMultiSelect_HandleJsShown")]
	public async Task HandleJsShown()
	{
		isShown = true;
		await filterInputReference.FocusAsync();
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore();
	}

	protected virtual async ValueTask DisposeAsyncCore()
	{
		disposed = true;

		if (jsModule != null)
		{
			try
			{
				await jsModule.InvokeVoidAsync("dispose", InputId);
				await jsModule.DisposeAsync();
			}
			catch (JSDisconnectedException)
			{
				// NOOP
			}
		}

		dotnetObjectReference?.Dispose();
	}



	public class SelectionChangedArgs
	{
		public bool Checked { get; set; }
		public TItem Item { get; set; }
	}
}