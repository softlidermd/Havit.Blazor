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
			platformView.SetOnKeyListener(new BlockBackKeyListener());
		}
		private class BlockBackKeyListener : Java.Lang.Object, Android.Views.View.IOnKeyListener
		{
			private readonly ModalManager _modalManager;

			public BlockBackKeyListener()
			{
				_modalManager = ModalManager.GetInstance();
			}

			public bool OnKey(Android.Views.View? v, [GeneratedEnum] Keycode keyCode, KeyEvent? e)
			{
				if (keyCode == Android.Views.Keycode.Back)
				{
					if (_modalManager?.HasOpenModal == true)
					{
						_modalManager.CloseTop();
						return true;
					}
				}

				return false;
			}
		}
	}
}
