using System.Web;
using System.Web.Optimization;

namespace ShareGrid
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/js/bootstrap").Include(
				"~/Content/bootstrap/js/bootstrap-button.js",
				"~/Content/bootstrap/js/bootstrap-modal.js",
				"~/Content/bootstrap/js/bootstrap-transition.js",
				"~/Content/bootstrap/js/bootstrap-dropdown.js",
				"~/Content/bootstrap/js/bootstrap-alert.js",
				"~/Content/bootstrap/js/bootstrap-tab.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/js/jqueryPlugins").Include(
				"~/Content/js/libs/jquery.masonry.js",
				"~/Content/js/libs/jquery.cookie.js",
				"~/Content/js/textHelpers.js",
				"~/Content/js/libs/knockout-2.1.0.js",
				"~/Content/js/libs/knockout-mapping-2.2.0.js",
				"~/Content/js/libs/sammy-0.7.1.min.js",
				"~/Content/js/libs/jquery.autosize.js",
				"~/Content/js/libs/load-image.min.js",
				"~/Content/js/libs/jquery.resize.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/js/jqueryFileUpload").Include(
				"~/Content/js/libs/jquery-file-upload/vendor/jquery.ui.widget.js",
				"~/Content/js/libs/jquery-file-upload/jquery.iframe-transport.js",
				"~/Content/js/libs/jquery-file-upload/jquery.fileupload.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/js/knockout").Include(
				"~/Content/js/knockoutExtenders.js",
				"~/Content/js/knockoutBindings.js",

				"~/Content/js/apiroutes.js",
	
				"~/Content/js/viewmodels/RegisterPanel.js",
				"~/Content/js/viewmodels/LoginPanel.js",
				"~/Content/js/viewmodels/UserEmailPanel.js",
				"~/Content/js/viewmodels/UserPasswordPanel.js",
				"~/Content/js/viewmodels/RegisterChannelPanel.js",
				"~/Content/js/viewmodels/SubscribeChannelPanel.js",
				"~/Content/js/viewmodels/Channels/ChannelEntity.js",
				"~/Content/js/viewmodels/Channels/ChannelView.js",
				"~/Content/js/viewmodels/Channels/UploadEntityPanel.js",
				"~/Content/js/viewmodels/main.js"
			));
			/*
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-1.*"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
						"~/Content/themes/base/jquery.ui.core.css",
						"~/Content/themes/base/jquery.ui.resizable.css",
						"~/Content/themes/base/jquery.ui.selectable.css",
						"~/Content/themes/base/jquery.ui.accordion.css",
						"~/Content/themes/base/jquery.ui.autocomplete.css",
						"~/Content/themes/base/jquery.ui.button.css",
						"~/Content/themes/base/jquery.ui.dialog.css",
						"~/Content/themes/base/jquery.ui.slider.css",
						"~/Content/themes/base/jquery.ui.tabs.css",
						"~/Content/themes/base/jquery.ui.datepicker.css",
						"~/Content/themes/base/jquery.ui.progressbar.css",
						"~/Content/themes/base/jquery.ui.theme.css"));
			*/
		}
	}
}