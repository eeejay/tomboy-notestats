
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
