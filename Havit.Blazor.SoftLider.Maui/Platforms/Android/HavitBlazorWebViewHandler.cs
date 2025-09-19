using System.Threading.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Havit.Blazor.SoftLider.Maui
{
	internal class HavitBlazorWebViewHandler : BlazorWebViewHandler
	{
		public HavitBlazorWebViewHandler()
		{

		}
		protected override void ConnectHandler(Android.Webkit.WebView platformView)
		{
			base.ConnectHandler(platformView);

			var manager = MauiContext?.Services.GetService<ModalManager>();
			if (manager != null)
			{
				platformView.SetOnKeyListener(new BlockBackKeyListener(manager));
			}
		}
		private class BlockBackKeyListener : Java.Lang.Object, Android.Views.View.IOnKeyListener
		{
			private readonly ModalManager _modalManager;

			public BlockBackKeyListener(ModalManager modalManager)
			{
				_modalManager = modalManager;
			}

			public bool OnKey(Android.Views.View? v, [GeneratedEnum] Keycode keyCode, KeyEvent? e)
			{
				if (keyCode == Keycode.Back && _modalManager.HasOpenModal)
				{
					if (e != null && e.Action == KeyEventActions.Up)
					{
						_ = _modalManager.CloseTopAsync();
					}

					return true;
				}

				return false;
			}
		}
	}
}
