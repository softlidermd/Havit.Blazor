using System.Runtime.InteropServices;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Blazor.SoftLider;

public class UISettings
{
	public static IServiceCollection Initialize(IServiceCollection services, UIOptions? options = null)
	{
		services.AddHxServices();
		services.AddHxMessenger(forceAsSingleton: !(options?.HxMessengerAsScopedService ?? false));
		services.AddHxMessageBoxHost();
		SetDefaults(options);
		return services;
	}

	public static ModalSettings FormSettings { get; } = new()
	{
		Backdrop = ModalBackdrop.Static,
		Scrollable = true,
		Size = ModalSize.Large,
		Fullscreen = ModalFullscreen.LargeDown,
		ContentCssClass = "vh-100"
	};

	public static ModalSettings FullscreenFormSettings { get; } = new()
	{
		Backdrop = ModalBackdrop.False,
		Scrollable = true,
		Fullscreen = ModalFullscreen.Always,
		ContentCssClass = "vh-100"
	};

	public static ButtonSettings AddButtonSettings { get; } = new()
	{
		Color = ThemeColor.Primary,
		Icon = BootstrapIcon.PlusLg,
	};

	public static ButtonSettings SelectButtonSettings { get; } = new()
	{
		Color = ThemeColor.Primary,
		Icon = BootstrapIcon.Check2,
	};

	public static ButtonSettings RefreshButtonSettings { get; } = new()
	{
		TextCssClass = "d-none d-sm-inline",
		CssClass = "bg-body-secondary",
		Icon = BootstrapIcon.ArrowClockwise
	};

	private static void SetDefaults(UIOptions? options = null)
	{
		var buttonSize = options?.ButtonSize ?? ButtonSize.Small;
		var inputSize = options?.InputSize ?? InputSize.Small;

		HxAutosuggest.Defaults.MinimumLength = 1;
		HxSearchBox.Defaults.MinimumLength = 1;

		HxButton.Defaults.Size = buttonSize;
		HxButton.Defaults.Spinner = false;

		HxInputText.Defaults.InputSize = inputSize;
		HxInputNumber.Defaults.InputSize = inputSize;
		HxInputDate.Defaults.InputSize = inputSize;
		HxInputDateRange.Defaults.InputSize = inputSize;
		HxInputFile.Defaults.InputSize = inputSize;
		HxSelect.Defaults.InputSize = inputSize;
		HxMultiSelect.Defaults.InputSize = inputSize;
		HxInputTags.Defaults.InputSize = inputSize;
		HxSearchBox.Defaults.InputSize = inputSize;
		HxFormValue.Defaults.InputSize = inputSize;
		HxAutosuggest.Defaults.InputSize = inputSize;

		HxModal.Defaults.HeaderCssClass = "gap-2";
		HxModal.Defaults.Centered = true;

		HxPlaceholderContainer.Defaults.CssClass = "text-start";
		HxPlaceholder.Defaults.Color = ThemeColor.None;
		HxPlaceholder.Defaults.CssClass = "bg-body-secondary rounded";

		HxGrid.Defaults.Responsive = true;
		HxGrid.Defaults.ContentNavigationMode = GridContentNavigationMode.InfiniteScroll;
		HxGrid.Defaults.OverscanCount = 10;
		HxGrid.Defaults.TableCssClass = "flex-grow-1 table-layout-fixed position-absolute";
		HxGrid.Defaults.TableContainerCssClass = "flex-grow-1 d-flex flex-column position-relative";

		HxInputBase.Defaults.ValidationMessageMode = ValidationMessageMode.Regular;

		HxFormValue.Defaults.ValueCssClass = "bg-body-tertiary";

		HxListLayout.Defaults.CssClass = "flex-grow-1 d-flex flex-column min-h-0";
		HxListLayout.Defaults.CardSettings.CssClass = "flex-grow-1 min-h-0";
		HxListLayout.Defaults.CardSettings.BodyCssClass = "flex-grow-0";
		HxListLayout.Defaults.FilterOpenButtonSettings.Color = ThemeColor.None;
		HxListLayout.Defaults.FilterOpenButtonSettings.CssClass = "bg-body-secondary";

		HxContextMenu.Defaults.DropdownCssClass = "position-static"; // avoid dropdown clipping inside table-responsive class.
	}

	public class UIOptions
	{
		public InputSize? InputSize { get; set; }
		public ButtonSize? ButtonSize { get; set; }
		public bool HxMessengerAsScopedService{ get; set; }
	}
}
