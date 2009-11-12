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
		private bool needs_refresh;
		
		private struct StringStatistics
		{
			public int Lines;
			public int Words;
			public int CharsWithSpaces;
			public int CharsWithoutSpaces;
			
			public StringStatistics (int lines, int words, int chars_with_spaces, int chars_without_spaces)
			{
				Lines = lines;
				Words = words;
				CharsWithSpaces = chars_with_spaces;
				CharsWithoutSpaces = chars_without_spaces;
			}
			
			public static StringStatistics operator -(StringStatistics stats1, StringStatistics stats2)
			{
				return new StringStatistics (stats1.Lines - stats2.Lines, 
				                             stats1.Words - stats2.Words,
				                             stats1.CharsWithSpaces - stats2.CharsWithSpaces,
				                             stats1.CharsWithoutSpaces - stats2.CharsWithoutSpaces);
			}
		}
		
		protected virtual void OnResponse (object o, Gtk.ResponseArgs args)
		{
			Destroy ();
		}
		
		private void Refresh ()
		{	
			StringStatistics stats = GetStatistics (countStrikeout.Active);
			
			noteLines.Text = stats.Lines.ToString();
			noteWords.Text = stats.Words.ToString();
			noteCharsSpaces.Text = stats.CharsWithSpaces.ToString();
			noteCharsNoSpaces.Text = stats.CharsWithoutSpaces.ToString();
			
			if (note.Buffer.Selection != null)
			{	
				TextIter start;
				TextIter end;
				note.Buffer.GetSelectionBounds(out start, out end);
				
				stats = GetStatistics (start, end, countStrikeout.Active);
				
				selLines.Text = stats.Lines.ToString();
				selWords.Text = stats.Words.ToString();
				selCharsSpaces.Text = stats.CharsWithSpaces.ToString();
				selCharsNoSpaces.Text = stats.CharsWithoutSpaces.ToString();
				
				SetSelectionColumnSensitive (true);
			} else {
				selLines.Text = "0";
				selWords.Text = "0";
				selCharsSpaces.Text = "0";
				selCharsNoSpaces.Text = "0";
				
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
		
		private StringStatistics GetStatistics (TextIter start, TextIter end, bool include_strikethrough)
		{	
			string str = "";
						
			if (include_strikethrough)
			{
				str = note.Buffer.GetText(start, end, false);
			} else
			{
				TextTagEnumerator enumerator =
					new TextTagEnumerator (note.Buffer, "strikethrough");
				
				TextIter splice_start = start;
				TextIter splice_end = end;
				string splice = "";
					
				foreach (TextRange range in enumerator) {
					if (range.Start.Offset > end.Offset)
						break;
					
					if (range.End.Offset < start.Offset)
						continue;
					
					str += splice;
					
					if (range.Start.InRange (splice_start, splice_end))
					    splice = splice_start.GetVisibleText (range.Start);
					
					
					if (!range.End.InRange (splice_start, splice_end))
						splice_start = splice_end;
					else
						splice_start = range.End;
				}				
				str += splice;
				str += splice_start.GetVisibleText (splice_end);
			}
			return GetTextStatistics (str);
		}
		
		private StringStatistics GetStatistics (bool include_strikethrough)
		{
			return GetStatistics (note.Buffer.StartIter, note.Buffer.EndIter, include_strikethrough);
		}
		
		private StringStatistics GetTextStatistics (string s)
		{
			StringStatistics stats = new StringStatistics();
			
			stats.Words = Regex.Matches(s, @"[^\s•]+").Count;
			stats.Lines = Regex.Matches(s, @"^.+", RegexOptions.Multiline).Count;
			stats.CharsWithoutSpaces = Regex.Matches(s, @"[^\s•]").Count;
			stats.CharsWithSpaces = Regex.Matches(s, @"[^•]").Count;
			
			return stats;
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
			
			needs_refresh = false;
			
			note.Buffer.Changed += OnTextChanged;
			note.Buffer.MarkSet += OnMarkSet;
			note.Renamed += OnRenamed;
			
			GLib.Timeout.Add (500, OnAutoRefresh);
		}
		
		bool OnAutoRefresh ()
		{
			if (needs_refresh)
				Refresh();
			
			needs_refresh = false;
			
			return IsRealized;
		}

		void OnRenamed(Note sender, string old_title)
		{
			SetTitle();
		}

		void OnMarkSet(object o, MarkSetArgs args)
		{
			if (args.Mark.Name == "insert" || 
			    args.Mark.Name == "selection_bound")
				needs_refresh = true;
		}


		void OnTextChanged(object sender, EventArgs e)
		{
			needs_refresh = true;
		}

		protected virtual void OnStrikeoutToggled (object sender, System.EventArgs e)
		{
			Refresh();
		}
	}
}
