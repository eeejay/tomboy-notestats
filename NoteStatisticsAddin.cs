
using System;
using System.Collections;
using System.Text.RegularExpressions;

using Mono.Unix;

using Tomboy;

namespace Tomboy.NoteStatistics
{
	public class NoteStatisticsAddin : NoteAddin
	{
		Gtk.MenuItem menu_item;

		public override void Initialize ()
		{
		}

		public override void OnNoteOpened ()
		{
			// Add the menu item when the window is created.
			menu_item = new Gtk.MenuItem (
				Catalog.GetString ("Note Statistics"));
			
			menu_item.Activated += OnMenuItemActivated;
			
			menu_item.Show ();
			AddPluginMenuItem (menu_item);
		}

		public override void Shutdown ()
		{
			if (menu_item != null)
				menu_item.Activated -= OnMenuItemActivated;
		}

		// Return the word count of an arbitrary multi-line text string.
		
		void OnMenuItemActivated (object sender, EventArgs args)
		{
			NoteStatsDialog dialog = new NoteStatsDialog(Note);
			dialog.ShowAll();
			
		}
	}
}
