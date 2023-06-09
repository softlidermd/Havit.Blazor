namespace Havit.Blazor.Components.Web.Bootstrap;

/// <summary>
/// Settings for the <see cref="HxButton"/> and derived components.
/// </summary>
public record ButtonSettings
{
	/// <summary>
	/// <see href="https://getbootstrap.com/docs/5.3/components/buttons/#sizes">Bootstrap button size</see>.
	/// </summary>
	public ButtonSize? Size { get; set; }

	/// <summary>
	/// CSS class to be rendered with the button.
	/// </summary>
	public string CssClass { get; set; }

	/// <summary>
	/// CSS class to be rendered with the button icon.
	/// </summary>
	public string IconCssClass { get; set; }
	
	/// <summary>
	/// CSS class to be rendered with the button's text.
	/// </summary>
	public string TextCssClass { get; set; }

	/// <summary>
	/// Icon to be rendered with the button.
	/// </summary>
	public IconBase Icon { get; set; }

	/// <summary>
	/// Position of the icon within the button.
	/// </summary>
	public ButtonIconPlacement? IconPlacement { get; set; }

	/// <summary>
	/// Bootstrap button color (style).
	/// </summary>
	public ThemeColor? Color { get; set; }

	/// <summary>
	/// <see href="https://getbootstrap.com/docs/5.3/components/buttons/#outline-buttons">Bootstrap outline button style</see>.
	/// </summary>
	public bool? Outline { get; set; } = false;

	/// <summary>
	/// Set state of the embedded <see cref="HxSpinner"/>.
	/// Leave <c>null</c> if you want automated spinner when any of the <see cref="HxButton.OnClick"/> handlers is running.
	/// You can set an explicit <c>false</c> constant to disable (override) the spinner automation.
	/// </summary>
	public bool? Spinner { get; set; }
}
