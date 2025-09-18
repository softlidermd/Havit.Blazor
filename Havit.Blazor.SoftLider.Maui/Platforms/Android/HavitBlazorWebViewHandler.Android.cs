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
			platformView.SetOnKeyListener(new BlockBackKeyListener(MauiContext.Services.GetService<ModalManager>()));
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
				if (keyCode == Android.Views.Keycode.Back)
				{
					if (_modalManager?.HasOpenModal == true)
					{
						_ = _modalManager.CloseTopAsync();
						return true;
					}
				}

				return false;
			}
		}
	}
}
