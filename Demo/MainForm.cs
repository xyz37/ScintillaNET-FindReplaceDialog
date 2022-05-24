using ScintillaNET;
using ScintillaNET_FindReplaceDialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
	public partial class MainForm : Form
	{
		private FindReplace MyFindReplace;

		public MainForm()
		{
			InitializeComponent();
			scintilla1.Styles[Style.Default].Size = 10;

			MyFindReplace = new FindReplace();
			MyFindReplace.Scintilla = scintilla1;
			MyFindReplace.FindAllResults += MyFindReplace_FindAllResults;
			MyFindReplace.KeyPressed += MyFindReplace_KeyPressed;

			incrementalSearcher1.FindReplace = MyFindReplace;

			findAllResultsPanel1.Scintilla = scintilla1;
		}

		private void MyFindReplace_KeyPressed(object sender, KeyEventArgs e)
		{
			genericScintilla_KeyDown(sender, e);
		}

		private void MyFindReplace_FindAllResults(object sender, FindResultsEventArgs FindAllResults)
		{
			// Pass on find results
			findAllResultsPanel1.UpdateFindAllResults(FindAllResults.FindReplace, FindAllResults.FindAllResults);
		}

		private void GotoButton_Click(object sender, EventArgs e)
		{
			// Use the FindReplace Scintilla as this will change based on focus
			GoTo MyGoTo = new GoTo(MyFindReplace.Scintilla);
			MyGoTo.ShowGoToDialog();
		}

		/// <summary>
		/// Key down event for each Scintilla. Tie each Scintilla to this event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void genericScintilla_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.F)
			{
				MyFindReplace.ShowFind();
				e.SuppressKeyPress = true;
			}
			else if (e.Shift && e.KeyCode == Keys.F3)
			{
				MyFindReplace.Window.FindPrevious();
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.F3)
			{
				MyFindReplace.Window.FindNext();
				e.SuppressKeyPress = true;
			}
			else if (e.Control && e.KeyCode == Keys.H)
			{
				MyFindReplace.ShowReplace();
				e.SuppressKeyPress = true;
			}
			else if (e.Control && e.KeyCode == Keys.I)
			{
				MyFindReplace.ShowIncrementalSearch();
				e.SuppressKeyPress = true;
			}
			else if (e.Control && e.KeyCode == Keys.G)
			{
				GoTo MyGoTo = new GoTo((Scintilla)sender);
				MyGoTo.ShowGoToDialog();
				e.SuppressKeyPress = true;
			}
		}

		/// <summary>
		/// Enter event tied to each Scintilla that will share a FindReplace dialog.
		/// Tie each Scintilla to this event.
		/// </summary>
		/// <param name="sender">The Scintilla receiving focus</param>
		/// <param name="e"></param>
		private void genericScintilla1_Enter(object sender, EventArgs e)
		{
			MyFindReplace.Scintilla = (Scintilla)sender;
		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.Q | Keys.Control:
                        Close();

                        return true;
                    case Keys.D1 | Keys.Shift:
						scintilla1.Text = File.ReadAllText(Path.Combine(Application.StartupPath, @"..\..\..\MainForm.cs"));

                        return true;
                    case Keys.D2 | Keys.Shift:
						SetLexer(scintilla1);

                        return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetLexer(Scintilla scintilla)
        {
			//scintilla.Lexer = Lexer.Cpp;
			//scintilla.StyleResetDefault();
			//scintilla.Styles[Style.Default].Font = "D2Coding";
			//scintilla.Styles[Style.Default].Size = 10;
			//scintilla.StyleClearAll();
			Scintilla.SetModulePath(Path.Combine(Application.StartupPath, "SciLexer.dll")); // Notepadd++ 8.4.1 Lexer file

			var scintilla3 = new Scintilla();
			var version = scintilla3.GetVersionInfo();
		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
			scintilla1.TextChanged += (sender, e) => SetLineNumber(scintilla1);
			scintilla2.TextChanged += (sender, e) => SetLineNumber(scintilla2);
            scintilla1.ZoomChanged += (sender, e) => SetLineNumber(scintilla1);
            scintilla2.ZoomChanged += (sender, e) => SetLineNumber(scintilla2);
			SetLineNumber(scintilla1);
			SetLineNumber(scintilla2);
        }

        private void SetLineNumber(Scintilla scintilla)
        {
			// Did the number of characters in the line number display change?
			// i.e. nnn VS nn, or nnnn VS nn, etc...
			var maxLineNumberCharLength = scintilla.Lines.Count.ToString().Length;
            var padding = (int)Math.Max(1.2d * (double)scintilla.Zoom, 2d);

            //if (maxLineNumberCharLength == Convert.ToInt32(scintilla.Tag))
            //{
            //    return;
            //}

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
			scintilla.Margins[0].Width = scintilla.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
			scintilla.Tag = maxLineNumberCharLength;
		}
    }
}