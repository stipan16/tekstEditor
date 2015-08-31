using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<string> lines;
        private Color colorSearch=Color.Yellow;
        private Color colorReplace=Color.Red;

        //klikom na New očistiti sadržaj velikog text boxa
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtVelikiTextBox.Clear();
        }

        //klikom na Exit zatvara se app
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //klikom na Save otvara se SaveFileDialog
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            //svojstva saveFileDialoga
            saveFileDialog.Title = "Save as";
            saveFileDialog.FileName = "";
            saveFileDialog.Filter = "Text files (*.rtf,*txt)|*.rtf;*txt" ;

            //ako je korisnik kliknia OK
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog.InitialDirectory + saveFileDialog.FileName;
             
                try
                {
                    //Stream writer čita liniju po liniju iz velikog text boxa i upisuje je u datoteku definiranu path-om 
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        foreach (var line in txtVelikiTextBox.Lines)
                        {
                            if(line!=null)
                                sw.WriteLine(line);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("nešto nije u redu sa putanjom");
                }
            
            }
        }
        
        
        //klikom na Open otvara se OpenFileDialog
       private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //svojstva openFileDialoga
            openFileDialog.Title = "Open";
            openFileDialog.FileName = "";
            openFileDialog.Filter = "Text files (*.rtf, *.txt)|*.rtf;*.txt";

            //ako je korisnik kliknia OK
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.InitialDirectory + openFileDialog.FileName;

                //Stream Reader čita liniju po liniju iz datoteke definirane path-om i zapisuje linije u listu  
                
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        lines = new List<string>();
                        while (sr.Peek() >= 0)
                        {
                            string line = sr.ReadLine();
                            lines.Add(line);
                        }
                        //svaku liniju zapisuje u veliki text box
                        foreach (var l in lines)
                        {
                            txtVelikiTextBox.Text += l.ToString() + "\n";
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("nešto nije u redu sa putanjom");
                }
            }
        }

        //klik na Cut
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //provjera da li je označen tekst
            if (txtVelikiTextBox.SelectedText != "")
            {
                //reže označeni tekst i sprema ga u ClipBoard
                txtVelikiTextBox.Cut();
            }

        }

        //klik na Copy
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //provjera da li je označen text
            if (txtVelikiTextBox.SelectionLength > 0)
            {
                //kopira označeni tekst u Clipboard
                txtVelikiTextBox.Copy();
            }
        }

        //klik na Paste
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //provjera da li postoji tekst u Clipboardu koji bi se zalijepio u text box
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) == true)
            {
                ///provjera da li je označen neki tekst unutar text boxa
                if (txtVelikiTextBox.SelectionLength > 0)
                {
                    //upit da li želimo novi tekst zalijepiti preko trenutno označenog
                    if (MessageBox.Show("Da li želiš zalijepiti preko označenog?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        //preskoći označeni tekst
                        txtVelikiTextBox.SelectionStart = txtVelikiTextBox.SelectionStart + txtVelikiTextBox.SelectionLength;
                    }
                }
                txtVelikiTextBox.Paste();
            }
            
        }

        //klik na Undo
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //provjera da li se zadnja operacija može poništiti
            if (txtVelikiTextBox.CanUndo == true)
            {
                //poništi zadnju operaciju
                txtVelikiTextBox.Undo();

                //očiti buffer kako se slučajno poništena akcija nebi opet izvela
                txtVelikiTextBox.ClearUndo();
            }
        }

        //klik na font
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                //font tekst
                txtVelikiTextBox.Font = fontDialog.Font;
            }
        }

        //klik na Background color
        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                //boja pozadine
                txtVelikiTextBox.BackColor = colorDialog.Color;
            }
        }

        //klik na text color
        private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                //boja slova
                txtVelikiTextBox.ForeColor = colorDialog.Color;
            }
        }

        //klik na botun search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            int index = 0;
            //praznimo i ponovno vračamo sadržaj velikog text boxa,kako bi se osigurali da nisu ostale oznake prethodne pretrage
            String temp = txtVelikiTextBox.Text;
            txtVelikiTextBox.Text = "";
            txtVelikiTextBox.Text = temp;
            while (index < txtVelikiTextBox.Text.LastIndexOf(txtInput.Text))
            {
                //traži tekst unutar zadanog raspona
                txtVelikiTextBox.Find(txtInput.Text, index, txtVelikiTextBox.TextLength, RichTextBoxFinds.None);
                //obojamo pozadinu pronađenog teksta
                txtVelikiTextBox.SelectionBackColor = colorSearch;
                //inklementiraj index kako bi se potraga nastavila
                index = txtVelikiTextBox.Text.IndexOf(txtInput.Text, index) + 1;
            }
        }

        //klik na botun replace
        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (txtInput.Text != "" && txtReplace.Text != "")
            {
                int index = 0;
                //praznimo i ponovno vračamo sadržaj velikog text boxa,kako bi se osigurali da nisu ostale oznake prethodne pretrage
                String temp = txtVelikiTextBox.Text;
                txtVelikiTextBox.Text = "";
                txtVelikiTextBox.Text = temp;
                while (index < txtVelikiTextBox.Text.LastIndexOf(txtInput.Text))
                {
                    //traži tekst unutar zadanog raspona
                    txtVelikiTextBox.Find(txtInput.Text, index, txtVelikiTextBox.TextLength, RichTextBoxFinds.None);
                    //obojamo pozadinu pronađenog teksta
                    txtVelikiTextBox.SelectionBackColor = colorReplace;
                    
                    txtVelikiTextBox.SelectedText = txtReplace.Text;
                    //inklementiraj index kako bi se potraga nastavila
                    index = txtVelikiTextBox.Text.IndexOf(txtReplace.Text, index) + 1;
                }
            }
            else
            {
                if(txtInput.Text=="")
                    MessageBox.Show("molim unesite tekst koji želite zamjeniti");
                
                else if(txtReplace.Text=="")
                    MessageBox.Show("molim unesite text kojim će se postojeći zamjeniti");
                
                else
                    MessageBox.Show("morate ispuniti oba textboxa");

            }
        }

        //klik na Search color
        private void searchColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                //boja pozadine teksta koji se pronađe
                colorSearch = colorDialog.Color; 
            }
        }

        //klik na Replace Color
        private void replaceColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                //boja pozadine teksta koji se zamijeni
                colorReplace = colorDialog.Color;
            }
        }

    }
}
