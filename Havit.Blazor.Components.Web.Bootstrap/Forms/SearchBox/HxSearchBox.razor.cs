﻿using Havit.Blazor.Components.Web.Bootstrap.Internal;
using Microsoft.JSInterop;

namespace Havit.Blazor.Components.Web.Bootstrap;

/// <summary>
/// A search input component witch automatic suggestions, initial dropdown template and free-text queries support.<br />
/// Full documentation and demos: <see href="https://havit.blazor.eu/components/HxSearchBox">https://havit.blazor.eu/components/HxSearchBox</see>
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class HxSearchBox<TItem> : IAsyncDisposable
{
	/// <summary>
	/// Returns application-wide defaults for the component.
	/// Enables overriding defaults in descendants (use separate set of defaults).
	/// </summary>
	protected virtual SearchBoxSettings GetDefaults() => HxSearchBox.Defaults;

	/// <summary>
	/// Set of settings to be applied to the component instance (overrides <see cref="HxSearchBox.Defaults"/>, overridden by individual parameters).
	/// </summary>
	[Parameter] public SearchBoxSettings Settings { get; set; }

	/// <summary>
	/// Returns optional set of component settings.
	/// </summary>
	/// <remarks>
	/// Similar to <see cref="GetDefaults"/>, enables defining wider <see cref="Settings"/> in components descendants (by returning a derived settings class).
	/// </remarks>
	protected virtual SearchBoxSettings GetSettings() => this.Settings;

	/// <summary>
	/// Method (delegate) which provides data of the suggestions.
	/// </summary>
	[Parameter] public SearchBoxDataProviderDelegate<TItem> DataProvider { get; set; }

	/// <summary>
	/// Allows you to disable the input. Default is <c>true</c>.
	/// </summary>
	[Parameter] public bool Enabled { get; set; } = true;

	/// <summary>
	/// Text written by the user (input text).
	/// </summary>
	[Parameter]
	public string TextQuery { get; set; }
	[Parameter] public EventCallback<string> TextQueryChanged { get; set; }
	/// <summary>
	/// Triggers the <see cref="TextQueryChanged"/> event. Allows interception of the event in derived components.
	/// </summary>
	protected virtual Task InvokeTextQueryChangedAsync(string newTextQueryValue) => TextQueryChanged.InvokeAsync(newTextQueryValue);

	/// <summary>
	/// Raised, when the enter key is pressed or when the text-query item is selected in the dropdown menu.
	/// (Does not trigger when <see cref="AllowTextQuery"/> is <c>false</c>.)
	/// </summary>
	[Parameter] public EventCallback<string> OnTextQueryTriggered { get; set; }
	/// <summary>
	/// Triggers the <see cref="OnTextQueryTriggered"/> event. Allows interception of the event in derived components.
	/// </summary>
	protected virtual Task InvokeOnTextQueryTriggeredAsync(string textQuery) => OnTextQueryTriggered.InvokeAsync(textQuery);

	/// <summary>
	/// Occurs, when any of suggested items (other than plain text-query) is selected.
	/// </summary>
	[Parameter] public EventCallback<TItem> OnItemSelected { get; set; }
	/// <summary>
	/// Triggers the <see cref="OnItemSelected"/> event. Allows interception of the event in derived components.
	/// </summary>
	protected virtual Task InvokeOnItemSelectedAsync(TItem selectedItem) => OnItemSelected.InvokeAsync(selectedItem);

	/// <summary>
	/// Behavior when the item is selected.
	/// </summary>
	[Parameter] public SearchBoxItemSelectionBehavior? ItemSelectionBehavior { get; set; }
	protected SearchBoxItemSelectionBehavior ItemSelectionBehaviorEffective => this.ItemSelectionBehavior ?? this.GetSettings()?.ItemSelectionBehavior ?? GetDefaults().ItemSelectionBehavior ?? throw new InvalidOperationException(nameof(ItemSelectionBehavior) + " default for " + nameof(HxSearchBox) + " has to be set.");

	/// <summary>
	/// Placeholder text for the search input.
	/// </summary>
	[Parameter] public string Placeholder { get; set; }

	/// <summary>
	/// Selector to display item title from data item.
	/// </summary>
	[Parameter] public Func<TItem, string> ItemTitleSelector { get; set; }

	/// <summary>
	/// Selector to display item subtitle from data item.
	/// </summary>
	[Parameter] public Func<TItem, string> ItemSubtitleSelector { get; set; }

	/// <summary>
	/// Selector to display icon from data item.
	/// </summary>
	[Parameter] public Func<TItem, IconBase> ItemIconSelector { get; set; }

	/// <summary>
	/// Template for the item content.
	/// </summary>
	[Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }

	/// <summary>
	/// Template for the text-query item content (requires <c><see cref="AllowTextQuery"/>="true"</c>).
	/// </summary>
	[Parameter] public RenderFragment<string> TextQueryItemTemplate { get; set; }

	/// <summary>
	/// Rendered when the <see cref="DataProvider" /> doesn't return any data.
	/// </summary>
	[Parameter] public RenderFragment NotFoundTemplate { get; set; }

	/// <summary>
	/// Rendered when no input is entered (i.e. initial state).
	/// </summary>
	[Parameter] public RenderFragment DefaultContentTemplate { get; set; }

	/// <summary>
	/// Additional css classes for the dropdown.
	/// </summary>
	[Parameter] public string CssClass { get; set; }
	protected string CssClassEffective => this.CssClass ?? this.GetSettings()?.CssClass ?? GetDefaults().CssClass;

	/// <summary>
	/// Additional CSS classes for the items in the dropdown menu.
	/// </summary>
	[Parameter] public string ItemCssClass { get; set; }
	protected string ItemCssClassEffective => this.ItemCssClass ?? this.GetSettings()?.ItemCssClass ?? GetDefaults().ItemCssClass;

	/// <summary>
	/// Additional CSS classes for the search box input.
	/// </summary>
	[Parameter] public string InputCssClass { get; set; }
	protected string InputCssClassEffective => this.InputCssClass ?? this.GetSettings()?.InputCssClass ?? GetDefaults().InputCssClass;

	/// <summary>
	/// Custom CSS class to render with input-group span.
	/// </summary>
	[Parameter] public string InputGroupCssClass { get; set; }
	protected string InputGroupCssClassEffective => this.InputGroupCssClass ?? this.GetSettings()?.InputGroupCssClass ?? GetDefaults().InputGroupCssClass;

	/// <summary>
	/// Icon of the input, when no text is written.
	/// </summary>
	[Parameter] public IconBase SearchIcon { get; set; }
	protected IconBase SearchIconEffective => this.SearchIcon ?? this.GetSettings()?.SearchIcon ?? GetDefaults().SearchIcon;

	/// <summary>
	/// Icon of the input, when text is written allowing the user to clear the text.
	/// </summary>
	[Parameter] public IconBase ClearIcon { get; set; }
	protected IconBase ClearIconEffective => this.ClearIcon ?? this.GetSettings()?.ClearIcon ?? GetDefaults().ClearIcon;

	/// <summary>
	/// Offset between the dropdown and the input.
	/// <see href="https://popper.js.org/docs/v2/modifiers/offset/#options"/>
	/// </summary>
	[Parameter] public (int Skidding, int Distance) DropdownOffset { get; set; } = (0, 4);

	/// <summary>
	/// Label of the input field.
	/// </summary>
	[Parameter] public string Label { get; set; }

	/// <summary>
	/// Label type of the input field.
	/// </summary>
	[Parameter] public LabelType LabelType { get; set; }

	/// <summary>
	/// Input size of the input field.
	/// </summary>
	[Parameter] public InputSize? InputSize { get; set; }
	protected InputSize InputSizeEffective => this.InputSize ?? this.GetSettings()?.InputSize ?? this.GetDefaults()?.InputSize ?? throw new InvalidOperationException(nameof(InputSize) + " default for " + nameof(HxSearchBox) + " has to be set.");

	/// <summary>
	/// Minimum length to call the data provider (display any results). Default is <c>2</c>.
	/// </summary>
	[Parameter] public int? MinimumLength { get; set; }
	protected int MinimumLengthEffective => this.MinimumLength ?? this.GetSettings()?.MinimumLength ?? GetDefaults().MinimumLength ?? throw new InvalidOperationException(nameof(MinimumLength) + " default for " + nameof(HxSearchBox) + " has to be set.");

	/// <summary>
	/// Debounce delay in milliseconds. Default is <c>300</c> ms.
	/// </summary>
	[Parameter] public int? Delay { get; set; }
	protected int DelayEffective => this.Delay ?? this.GetSettings()?.Delay ?? GetDefaults().Delay ?? throw new InvalidOperationException(nameof(Delay) + " default for " + nameof(HxSearchBox) + " has to be set.");

	/// <summary>
	/// Indicates whether text-query mode is enabled (accepts free text in addition to suggested items).<br/>
	/// Default is <c>true</c>.
	/// </summary>
	[Parameter] public bool AllowTextQuery { get; set; } = true;

	/// <summary>
	/// Input-group at the beginning of the input.
	/// </summary>
	[Parameter] public string InputGroupStartText { get; set; }

	/// <summary>
	/// Input-group at the beginning of the input.
	/// </summary>
	[Parameter] public RenderFragment InputGroupStartTemplate { get; set; }

	/// <summary>
	/// Input-group at the end of the input.<br/>
	/// Hides the search icon when used!
	/// </summary>
	[Parameter] public string InputGroupEndText { get; set; }

	/// <summary>
	/// Input-group at the end of the input.<br/>
	/// Hides the search icon when used!
	/// </summary>
	[Parameter] public RenderFragment InputGroupEndTemplate { get; set; }

	[Inject] protected IJSRuntime JSRuntime { get; set; }

	internal ElementReference InputElement { get; set; }

	protected bool HasInputGroups => HasInputGroupStart || HasInputGroupEnd;
	private bool HasInputGroupStart => !String.IsNullOrWhiteSpace(InputGroupStartText) || (InputGroupStartTemplate is not null);
	private bool HasInputGroupEnd => !String.IsNullOrWhiteSpace(InputGroupEndText) || (InputGroupEndTemplate is not null);
	private bool HasClearButton => !HasInputGroupEnd
							&& !dataProviderInProgress
							&& !string.IsNullOrEmpty(TextQuery)
							&& (ClearIconEffective is not null);

	private string dropdownToggleElementId = "hx" + Guid.NewGuid().ToString("N");
	private string dropdownId = "hx" + Guid.NewGuid().ToString("N");
	private string inputId = "hx" + Guid.NewGuid().ToString("N");
	private List<TItem> searchResults = new();
	private HxDropdownToggleElement dropdownToggle;
	private bool dropdownMenuActive = false;
	private bool initialized = false;
	/// <summary>
	/// Indicates whether the <see cref="TextQuery"/> has been below minimum required length recently (before data provider loading is completed).
	/// </summary>
	private bool textQueryHasBeenBelowMinimumLength = true;
	private System.Timers.Timer timer;
	private CancellationTokenSource cancellationTokenSource;
	private bool dataProviderInProgress;
	private bool inputFormHasFocus;
	private bool scrollToFocusedItem;
	private IJSObjectReference jsModule;
	private DotNetObjectReference<HxSearchBox<TItem>> dotnetObjectReference;
	private bool clickIsComing;
	private bool disposed = false;

	public HxSearchBox()
	{
		dotnetObjectReference = DotNetObjectReference.Create(this);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			initialized = true;

			await EnsureJsModule();
			if (disposed)
			{
				return;
			}
			await jsModule.InvokeVoidAsync("initialize", inputId, dotnetObjectReference, new string[] { KeyCodes.ArrowUp, KeyCodes.ArrowDown });
		}

		if (scrollToFocusedItem)
		{
			scrollToFocusedItem = false;
			await jsModule.InvokeVoidAsync("scrollToFocusedItem");
		}
	}

	protected async Task EnsureJsModule()
	{
		jsModule ??= await JSRuntime.ImportHavitBlazorBootstrapModuleAsync(nameof(HxSearchBox));
	}

	protected async Task ClearInputAsync()
	{
		if (TextQuery != string.Empty)
		{
			TextQuery = string.Empty;
			await HandleTextQueryValueChanged(string.Empty);

			// #644 [HxSearchBox] Clear icon does not refresh data in typical usage scenarios (regression)
			// Although we can discuss whether the intention of the user was to trigger the text query, we invoke the callback to signalize the user submitted a new text query state.
			// Without this, the related UI won't be updated unless the TextQueryChanged callback is properly handled (which is not comfortable).
			await HandleTextQueryTriggered();
		}
	}

	protected async Task UpdateSuggestionsAsync()
	{
		if ((TextQuery?.Length ?? 0) < MinimumLengthEffective)
		{
			return;
		}

		// Cancelation is performed in HandleTextQueryValueChanged method
		cancellationTokenSource?.Dispose();

		cancellationTokenSource = new CancellationTokenSource();
		CancellationToken cancellationToken = cancellationTokenSource.Token;

		dataProviderInProgress = true;
		StateHasChanged();

		SearchBoxDataProviderRequest request = new()
		{
			UserInput = TextQuery,
			CancellationToken = cancellationToken
		};
		SearchBoxDataProviderResult<TItem> result = null;

		try
		{
			result = await DataProvider.Invoke(request);
		}
		catch (OperationCanceledException)
		{
			return;
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return;
		}

		dataProviderInProgress = false;

		// KeyboardNavigation
		if (this.AllowTextQuery)
		{
			focusedItemIndex = InputKeyboardNavigationIndex; // Move focus to the input, so that free-text can be easily confirmed with Enter.
		}
		else
		{
			focusedItemIndex = 0; // Move focus to the first item.
		}

		searchResults = result?.Data?.ToList() ?? new();

		textQueryHasBeenBelowMinimumLength = false;
		await ShowDropdownMenu();

		StateHasChanged();
	}

	private async Task HandleTextQueryValueChanged(string newTextQuery)
	{
		this.TextQuery = newTextQuery;

		CancelDataProviderAndDebounce();

		// start new time interval
		if ((TextQuery?.Length ?? 0) >= MinimumLengthEffective)
		{
			if (DelayEffective > 0)
			{
				if (timer == null)
				{
					timer = new System.Timers.Timer
					{
						AutoReset = false // just once
					};
					timer.Elapsed += HandleTimerElapsed;
				}
				timer.Interval = DelayEffective;
				timer.Start();
			}
			else
			{
				await UpdateSuggestionsAsync();
			}
		}
		else
		{
			textQueryHasBeenBelowMinimumLength = true;
		}

		if (ShouldDropdownMenuBeDisplayed())
		{
			await ShowDropdownMenu();
		}
		else if (dropdownMenuActive)
		{
			await HideDropdownMenu();
		}
		await InvokeTextQueryChangedAsync(newTextQuery);
	}

	#region KeyboardNavigation
	private int focusedItemIndex = -1;

	/// <summary>
	/// Input's index for the keyboard navigation. If this is the current index, then no item is selected.
	/// </summary>
	private const int InputKeyboardNavigationIndex = -1;

	private bool HasItemFocus(TItem item)
	{
		if ((focusedItemIndex > InputKeyboardNavigationIndex) && (focusedItemIndex < GetFreeTextItemIndex()))
		{
			TItem focusedItem = GetItemByIndex(focusedItemIndex);
			if ((focusedItem is not null) && (!focusedItem.Equals(default)))
			{
				return item.Equals(focusedItem);
			}
		}

		return false;
	}

	private bool HasFreeTextItemFocus()
	{
		return focusedItemIndex == GetFreeTextItemIndex();
	}

	[JSInvokable("HxSearchBox_HandleInputKeyDown")]
	public async Task HandleInputKeyDown(string keyCode)
	{
		// Confirm selection on the focused item if an item is focused and the enter key is pressed.
		TItem focusedItem = GetItemByIndex(focusedItemIndex);
		if ((keyCode == KeyCodes.Enter) || (keyCode == KeyCodes.NumpadEnter))
		{
			if ((focusedItem is not null) && (!focusedItem.Equals(default)))
			{
				await HandleItemSelected(focusedItem);
			}
			else if (focusedItemIndex == InputKeyboardNavigationIndex || focusedItemIndex == GetFreeTextItemIndex()) // Confirm free-text (text query) if the input or the free-text item is focused and the enter key is pressed.
			{
				await HandleTextQueryTriggered();
			}
		}

		// Move focus up or down.
		if (keyCode == KeyCodes.ArrowUp)
		{
			int previousItemIndex = focusedItemIndex - 1;
			int minimumIndex = AllowTextQuery ? InputKeyboardNavigationIndex : 0;

			if (previousItemIndex >= minimumIndex)
			{
				focusedItemIndex = previousItemIndex;
				scrollToFocusedItem = true;
				StateHasChanged();
			}
		}
		else if (keyCode == KeyCodes.ArrowDown)
		{
			int nextItemIndex = focusedItemIndex + 1;
			int maximumIndex = AllowTextQuery ? GetFreeTextItemIndex() : (searchResults?.Count ?? 0) - 1;

			if (nextItemIndex <= maximumIndex)
			{
				focusedItemIndex = nextItemIndex;
				scrollToFocusedItem = true;
				StateHasChanged();
			}
		}
	}

	[JSInvokable("HxSearchBox_HandleInputMouseDown")]
	public void HandleInputMouseDown()
	{
		clickIsComing = true;
	}

	[JSInvokable("HxSearchBox_HandleInputMouseUp")]
	public void HandleInputMouseUp()
	{
		clickIsComing = false;
	}

	[JSInvokable("HxSearchBox_HandleInputMouseLeave")]
	public void HandleInputMouseLeave()
	{
		clickIsComing = false;
	}

	private TItem GetItemByIndex(int index)
	{
		if ((index >= 0) && (index < searchResults.Count))
		{
			return searchResults[index];
		}
		else
		{
			return default;
		}
	}

	private int GetFreeTextItemIndex()
	{
		return searchResults.Count;
	}
	#endregion KeyboardNavigation

	private async void HandleTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
	{
		// when a time interval reached, update suggestions
		await InvokeAsync(async () =>
		{
			await UpdateSuggestionsAsync();
		});
	}

	private async Task HandleInputFocus()
	{
		inputFormHasFocus = true;

		// first focus when MinimumLength is 0 and we need to load initial suggestions
		if (((TextQuery?.Length ?? 0) == 0) && (MinimumLengthEffective == 0) && !searchResults.Any())
		{
			await UpdateSuggestionsAsync();
		}

		await ShowDropdownMenu();
	}

	private void HandleInputBlur()
	{
		inputFormHasFocus = false;

		if (!dropdownMenuActive)
		{
			ClearInputValueIfTextQueryDisabled();
		}
		CancelDataProviderAndDebounce();
	}

	private void ClearInputValueIfTextQueryDisabled()
	{
		if (!AllowTextQuery)
		{
			TextQuery = string.Empty;
		}
	}

	private void CancelDataProviderAndDebounce()
	{
		timer?.Stop(); // if waiting for an interval, stop it
		cancellationTokenSource?.Cancel(); // if waiting for an interval, stop it
		dataProviderInProgress = false; // data provider is no longer in progress
	}

	private async Task HandleTextQueryTriggered()
	{
		if (AllowTextQuery
			&& (((TextQuery?.Length ?? 0) >= MinimumLengthEffective) || ((TextQuery?.Length ?? 0) == 0)))
		{
			CancelDataProviderAndDebounce();

			await HideDropdownMenu();
			await InvokeOnTextQueryTriggeredAsync(this.TextQuery);
		}
	}

	private async Task HandleItemSelected(TItem item)
	{
		switch (this.ItemSelectionBehaviorEffective)
		{
			case SearchBoxItemSelectionBehavior.SelectAndClearTextQuery:
				this.TextQuery = String.Empty;
				break;
			case SearchBoxItemSelectionBehavior.SelectAndReplaceTextQueryWithItemTitle:
				this.TextQuery = ItemTitleSelector?.Invoke(item) ?? null;
				break;
			default:
				throw new InvalidOperationException($"Invalid {nameof(SearchBoxItemSelectionBehavior)} value: {this.ItemSelectionBehaviorEffective}");
		}

		await HideDropdownMenu();
		await InvokeTextQueryChangedAsync(this.TextQuery);
		await InvokeOnItemSelectedAsync(item);
	}

	private async Task HandleDropdownMenuShown()
	{
		dropdownMenuActive = true;

		if (!ShouldDropdownMenuBeDisplayed())
		{
			await HideDropdownMenu();
		}
	}

	private void HandleDropdownMenuHidden()
	{
		dropdownMenuActive = false;
		if (!inputFormHasFocus)
		{
			ClearInputValueIfTextQueryDisabled();
		}
	}

	private async Task ShowDropdownMenu()
	{
		if (!clickIsComing)
		{
			// clickIsComing logic fixes #572 - Initial suggestions disappear when the DataProvider response is quick
			// If click is coming, we do not want to show the dropdown as it will be toggled by the later click event (if we open it here, onfocus, click will hide it)
			await dropdownToggle.ShowAsync();
		}
	}

	private async Task HideDropdownMenu()
	{
		await dropdownToggle.HideAsync();
	}

	/// <summary>
	/// If the <see cref="DefaultContentTemplate"/> is empty, we don't want to display anything when nothing (or below the minimum amount of characters) is typed into the input.
	/// </summary>
	/// <returns></returns>
	private bool ShouldDropdownMenuBeDisplayed()
	{
		if (textQueryHasBeenBelowMinimumLength
			&& ((TextQuery?.Length ?? 0) >= MinimumLengthEffective))
		{
			return false;
		}

		if ((DefaultContentTemplate is null)
			&& ((TextQuery?.Length ?? 0) < MinimumLengthEffective))
		{
			return false;
		}

		return true;
	}

	public async ValueTask FocusAsync()
	{
		await InputElement.FocusAsync();
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore();
	}

	protected virtual async ValueTask DisposeAsyncCore()
	{
		disposed = true;

		timer?.Dispose();
		timer = null;

		cancellationTokenSource?.Dispose();
		cancellationTokenSource = null;

		if (jsModule != null)
		{
			try
			{
				await jsModule.InvokeVoidAsync("dispose", inputId);
				await dropdownToggle.DisposeAsync();
				await jsModule.DisposeAsync();
			}
			catch (JSDisconnectedException)
			{
				// NOOP
			}
			catch (TaskCanceledException)
			{
				// NOOP
			}
		}

		dotnetObjectReference?.Dispose();
	}
}
