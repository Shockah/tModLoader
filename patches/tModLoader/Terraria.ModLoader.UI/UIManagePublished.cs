using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System.Xml;
using System.Net;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Text;

namespace Terraria.ModLoader.UI
{
	internal class UIManagePublished : UIState
	{
		private UIList myPublishedMods;
		public UITextPanel<string> uITextPanel;

		public override void OnInitialize()
		{
			UIElement uIElement = new UIElement();
			uIElement.Width.Set(0f, 0.8f);
			uIElement.MaxWidth.Set(600f, 0f);
			uIElement.Top.Set(220f, 0f);
			uIElement.Height.Set(-220f, 1f);
			uIElement.HAlign = 0.5f;
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-110f, 1f);
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uIElement.Append(uIPanel);
			myPublishedMods = new UIList();
			myPublishedMods.Width.Set(-25f, 1f);
			myPublishedMods.Height.Set(0f, 1f);
			myPublishedMods.ListPadding = 5f;
			uIPanel.Append(myPublishedMods);
			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(100f, 1000f);
			uIScrollbar.Height.Set(0f, 1f);
			uIScrollbar.HAlign = 1f;
			uIPanel.Append(uIScrollbar);
			myPublishedMods.SetScrollbar(uIScrollbar);
			uITextPanel = new UITextPanel<string>("My Published Mods", 0.8f, true);
			uITextPanel.HAlign = 0.5f;
			uITextPanel.Top.Set(-35f, 0f);
			uITextPanel.SetPadding(15f);
			uITextPanel.BackgroundColor = new Color(73, 94, 171);
			uIElement.Append(uITextPanel);
			UITextPanel<string> button3 = new UITextPanel<string>("Back", 1f, false);
			button3.VAlign = 1f;
			button3.Height.Set(25f, 0f);
			button3.Width.Set(-10f, 1f / 2f);
			button3.Top.Set(-20f, 0f);
			button3.OnMouseOver += UICommon.FadedMouseOver;
			button3.OnMouseOut += UICommon.FadedMouseOut;
			button3.OnClick += BackClick;
			uIElement.Append(button3);
			base.Append(uIElement);
		}

		private static void BackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(11, -1, -1, 1);
			Main.menuMode = Interface.modSourcesID;
		}

		public override void OnActivate()
		{
			myPublishedMods.Clear();
			uITextPanel.SetText("My Published Mods", 0.8f, true);
			String xmlText = "";
			try
			{
				System.Net.ServicePointManager.Expect100Continue = false;
				string url = "http://javid.ddns.net/tModLoader/listmymods.php";
				IO.UploadFile[] files = new IO.UploadFile[0];
				var values = new NameValueCollection
				{
#if GOG
					{ "steamid64", ModLoader.steamID64 },
#else
					{ "steamid64", Steamworks.SteamUser.GetSteamID().ToString() },
#endif
					{ "modloaderversion", ModLoader.versionedName },
					{ "passphrase", ModLoader.modBrowserPassphrase },
				};
				byte[] result = IO.UploadFile.UploadFiles(url, files, values);
				xmlText = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.Timeout)
				{
					uITextPanel.SetText("Mod Browser OFFLINE (Busy)", 0.8f, true);
					return;
				}
				uITextPanel.SetText("Mod Browser OFFLINE.", 0.8f, true);
				return;
			}
			catch (Exception e)
			{
				ErrorLogger.LogModBrowserException(e);
				return;
			}
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(xmlText);
				foreach (XmlNode xmlNode in xmlDoc.DocumentElement)
				{
					string displayname = xmlNode.SelectSingleNode("displayname").InnerText;
					string name = xmlNode.SelectSingleNode("name").InnerText;
					string version = xmlNode.SelectSingleNode("version").InnerText;
					string author = xmlNode.SelectSingleNode("author").InnerText;
					string downloads = xmlNode.SelectSingleNode("downloads").InnerText;
					string downloadsversion = xmlNode.SelectSingleNode("downloadsversion").InnerText;
					string modloaderversion = xmlNode.SelectSingleNode("modloaderversion").InnerText;
					UIModManageItem modItem = new UIModManageItem(displayname, name, version, author, downloads, downloadsversion, modloaderversion);
					myPublishedMods.Add(modItem);
				}
			}
			catch (Exception e)
			{
				ErrorLogger.LogModBrowserException(e);
				return;
			}
		}
	}
}
