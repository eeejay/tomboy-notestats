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
using Tomboy;
using System.Text.RegularExpressions;
using Gtk;

namespace Tomboy.NoteStatistics
{
	public partial class NoteStatsDialog : Gtk.Dialog
	{
		private Note note;
		
		private struct StringStatistics
		{
			public int Lines;
			public int Words;
			public int CharsWithSpaces;
			public int CharsWithoutSpaces;
		}
		
		protected virtual void OnResponse (object o, Gtk.ResponseArgs args)
		{
			Destroy ();
		}
		
		private void Refresh ()
		{
			StringStatistics stats = GetStatistics ();
			
			noteLines.Text = stats.Lines.ToString();
			noteWords.Text = stats.Words.ToString();
			noteCharsSpaces.Text = stats.CharsWithSpaces.ToString();
			noteCharsNoSpaces.Text = stats.CharsWithoutSpaces.ToString();
			
			if (note.Buffer.Selection != null)
			{
				stats = GetStatistics (note.Buffer.Selection);
				
				selLines.Text = stats.Lines.ToString();
				selWords.Text = stats.Words.ToString();
				selCharsSpaces.Text = stats.CharsWithSpaces.ToString();
				selCharsNoSpaces.Text = stats.CharsWithoutSpaces.ToString();
				
				SetSelectionColumnSensitive (true);
			} else {
				SetSelectionColumnSensitive (false);
			}
				
		}
		
		private void SetSelectionColumnSensitive (bool sensitive)
		{
			selHeader.Sensitive = sensitive;
			selLines.Sensitive = sensitive;
			selWords.Sensitive = sensitive;
			selCharsSpaces.Sensitive = sensitive;
			selCharsNoSpaces.Sensitive = sensitive;
		}
		
		private StringStatistics GetStatistics (string s)
		{
			StringStatistics stats = new StringStatistics();
			
			stats.Words = Regex.Matches(s, @"[^\s•]+").Count;
			stats.Lines = Regex.Matches(s, @"^.+", RegexOptions.Multiline).Count;
			stats.CharsWithoutSpaces = Regex.Matches(s, @"[^\s•]").Count;
			stats.CharsWithSpaces = Regex.Matches(s, @"[^•]").Count;
			return stats;

		}
		
		private StringStatistics GetStatistics ()
		{
			return GetStatistics (note.Buffer.Text);
		}
		
		private void SetTitle ()
		{
			noteTitle.Markup = String.Format("<b>{0}</b>", note.Title);
		}
		
		public NoteStatsDialog(Note note)
		{
			this.note = note;
			
			this.Build();
			
			SetTitle();
			
			Refresh();
			
			note.Buffer.Changed += OnTextChanged;
			note.Buffer.MarkSet += OnMarkSet;
			note.Renamed += OnRenamed;
		}

		void OnRenamed(Note sender, string old_title)
		{
			SetTitle();
		}

		void OnMarkSet(object o, MarkSetArgs args)
		{
			if (args.Mark.Name == "insert" || 
			    args.Mark.Name == "selection_bound")
				Refresh ();
		}


		void OnTextChanged(object sender, EventArgs e)
		{
			Refresh ();
		}
	}
}
