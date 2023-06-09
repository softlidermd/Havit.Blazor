using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Blazor.SoftLider;

public class UISettings
{
	public static IServiceCollection Initialize(IServiceCollection services)
	{
		services.AddHxServices();
		services.AddHxMessenger(forceAsSingleton: true);
		services.AddHxMessageBoxHost();
		SetDefaults();
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
		Color = ThemeColor.Light,
		Icon = BootstrapIcon.ArrowClockwise
	};

	private static void SetDefaults()
	{
		HxAutosuggest.Defaults.MinimumLength = 1;
		HxSearchBox.Defaults.MinimumLength = 1;

		HxButton.Defaults.Size = ButtonSize.Small;
		HxButton.Defaults.Spinner = false;

		HxInputText.Defaults.InputSize = InputSize.Small;
		HxInputNumber.Defaults.InputSize = InputSize.Small;
		HxInputDate.Defaults.InputSize = InputSize.Small;
		HxInputDateRange.Defaults.InputSize = InputSize.Small;
		HxInputFile.Defaults.InputSize = InputSize.Small;
		HxSelect.Defaults.InputSize = InputSize.Small;
		HxMultiSelect.Defaults.InputSize = InputSize.Small;
		HxInputTags.Defaults.InputSize = InputSize.Small;
		HxSearchBox.Defaults.InputSize = InputSize.Small;
		HxFormValue.Defaults.InputSize = InputSize.Small;
		HxAutosuggest.Defaults.InputSize = InputSize.Small;

		HxModal.Defaults.HeaderCssClass = "gap-2";
		HxModal.Defaults.Centered = true;

		HxPlaceholderContainer.Defaults.CssClass = "text-start";
		HxPlaceholder.Defaults.Color = ThemeColor.Light;
		HxPlaceholder.Defaults.CssClass = "rounded";

		HxGrid.Defaults.Responsive = true;
		HxGrid.Defaults.ContentNavigationMode = GridContentNavigationMode.InfiniteScroll;
		HxGrid.Defaults.OverscanCount = 10;
		HxGrid.Defaults.TableCssClass = "flex-grow-1 table-layout-fixed position-absolute";
		HxGrid.Defaults.TableContainerCssClass = "flex-grow-1 d-flex flex-column position-relative";

		HxInputBase.Defaults.ValidationMessageMode = ValidationMessageMode.Regular;

		HxFormValue.Defaults.ValueCssClass = "grayed";

		HxListLayout.Defaults.CssClass = "flex-grow-1 d-flex flex-column";
		HxListLayout.Defaults.CardSettings.CssClass = "flex-grow-1";
		HxListLayout.Defaults.CardSettings.BodyCssClass = "flex-grow-0";

		HxContextMenu.Defaults.DropdownCssClass = "position-static"; // avoid dropdown clipping inside table-responsive class.
	}
}
