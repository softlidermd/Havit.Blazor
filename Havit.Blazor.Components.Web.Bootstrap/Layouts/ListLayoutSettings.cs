﻿namespace Havit.Blazor.Components.Web.Bootstrap;

/// <summary>
/// Settings for the <see cref="HxListLayout"/> and derived components.
/// </summary>
public record ListLayoutSettings
{
	/// <summary>
	/// Additional CSS classes for the wrapping <c>div</c>.
	/// </summary>
	public string CssClass { get; set; }

	/// <summary>
	/// Additional CSS classes for the header <c>div</c>.
	/// </summary>
	public string HeaderCssClass { get; set; }

	/// <summary>
	/// Settings for the wrapping <see cref="HxCard"/>.
	/// </summary>
	public CardSettings CardSettings { get; set; }

	/// <summary>
	/// Settings for the <see cref="HxButton"/> that opens the filtering offcanvas.
	/// </summary>
	public ButtonSettings FilterOpenButtonSettings { get; set; }

	/// <summary>
	/// Settings for the <see cref="HxButton"/> that submits the filter.
	/// </summary>
	public ButtonSettings FilterSubmitButtonSettings { get; set; }

	/// <summary>
	/// Settings for the <see cref="HxOffcanvas"/> with the filter.
	/// </summary>
	public OffcanvasSettings FilterOffcanvasSettings { get; set; }
}
