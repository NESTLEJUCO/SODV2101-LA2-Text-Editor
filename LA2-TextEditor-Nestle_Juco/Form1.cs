//Learning Activity 2: Text Editor
//SODV2101 Rapid Application Development 24SEPMNFS1
//Submitted By: Nestle Juco
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace LA2_TextEditor_Nestle_Juco
{
    public partial class Form1 : Form
    {
        string path;
        private PrintDocument printDocument = new PrintDocument();
        private int currentCharIndex = 0;
        private string[] lines;

        public Form1()
        {
            InitializeComponent();
            printDocument.PrintPage += PrintDocument_PrintPage;
            printDocument.BeginPrint += (sender, e) => BeginPrint();
        }

        // FILE TAB
        // New File
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 newForm = new Form1();
            newForm.Show();
        }

        // Open File
        private async void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Text Document | *.txt" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    path = ofd.FileName;
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        MainText.Text = await sr.ReadToEndAsync();
                    }
                }
            }
        }

        // Save File
        private async void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(path))
            {
                using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Text Document | *.txt" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            await sw.WriteAsync(MainText.Text);
                            path = sfd.FileName;
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    await sw.WriteAsync(MainText.Text);
                }
            }
        }

        // Save As
        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Text Document | *.txt" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        await sw.WriteAsync(MainText.Text);
                        path = sfd.FileName;
                    }
                }
            }
        }

        // Print
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PrintDialog pd = new PrintDialog())
            {
                pd.Document = printDocument;
                pd.AllowSomePages = true;
                pd.ShowHelp = true;
                pd.ShowNetwork = true;

                if (pd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        printDocument.Print();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while printing: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Print Preview
        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (PrintPreviewDialog previewDialog = new PrintPreviewDialog())
            {
                previewDialog.Document = printDocument;
                previewDialog.WindowState = FormWindowState.Maximized; 

                try
                {
                    previewDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while showing print preview: {ex.Message}", "Print Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Initialize pagination before printing
        private void BeginPrint()
        {
            lines = MainText.Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            currentCharIndex = 0;
        }

        // PrintPage event handler with pagination
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Define the font and brush
            using (Font printFont = new Font("Arial", 12))
            {
                Brush brush = Brushes.Black;

                // Define the area to print
                float linesPerPage = 0;
                float yPosition = 0;
                int count = 0;
                float leftMargin = e.MarginBounds.Left;
                float topMargin = e.MarginBounds.Top;
                string line = null;

                // Calculate the number of lines per page.
                linesPerPage = e.MarginBounds.Height / printFont.GetHeight(e.Graphics);

                // Print each line of the file.
                while (count < linesPerPage && currentCharIndex < lines.Length)
                {
                    line = lines[currentCharIndex];
                    yPosition = topMargin + (count * printFont.GetHeight(e.Graphics));
                    e.Graphics.DrawString(line, printFont, brush, leftMargin, yPosition, new StringFormat());
                    count++;
                    currentCharIndex++;
                }

                // If more lines exist, print another page.
                if (currentCharIndex < lines.Length)
                {
                    e.HasMorePages = true;
                }
                else
                {
                    e.HasMorePages = false;
                }
            }
        }

        // Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // EDIT TAB
        // Undo
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainText.Undo();
        }

        // Redo
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainText.Redo();
        }

        // Cut
        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MainText.SelectedText))
            {
                Clipboard.SetText(MainText.SelectedText);
                MainText.SelectedText = "";
            }
        }

        // Copy
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MainText.SelectedText))
            {
                Clipboard.SetText(MainText.SelectedText);
            }
        }

        // Paste
        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                int selectionIndex = MainText.SelectionStart;
                MainText.Text = MainText.Text.Insert(selectionIndex, Clipboard.GetText());
                MainText.SelectionStart = selectionIndex + Clipboard.GetText().Length;
            }
        }

        // Select All
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainText.SelectAll();
        }

        // About
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form2 aboutForm = new Form2())
            {
                aboutForm.ShowDialog(this);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
