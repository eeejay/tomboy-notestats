// 
// NoteStatisticsAddin.cs
//  
// Author:
//       Eitan Isaacson <eitan@monotonous.org>
// 
// Copyright (c) 2009 Eitan Isaacson
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
