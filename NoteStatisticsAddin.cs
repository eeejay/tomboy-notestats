//  
//  Copyright (C) 2009 Eitan Isaacson
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

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
