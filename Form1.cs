using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TXTextControl;
using TXTextControl.DocumentServer.Fields;

namespace tx_checkbox_sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textControl1.RulerBar = rulerBar2;
            textControl1.ButtonBar = buttonBar1;
            textControl1.StatusBar = statusBar1;
            textControl1.VerticalRulerBar = rulerBar1;
        }

        private string UNCHECKED = UnicodeHexToString("\\u2610");
        private string CHECKED = UnicodeHexToString("\\u2612");

        // u2611: Use this value to use a checkmark instead the default 'X'
        //private string CHECKED = UnicodeHexToString("\\u2611");

        private static string UnicodeHexToString(string text)
        {
            // returns the string representation
            return System.Text.Encoding.Unicode.GetString(BitConverter.GetBytes(short.Parse(text.Substring(2), System.Globalization.NumberStyles.HexNumber)));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            InsertCheckBox();
        }

        private void InsertCheckBox()
        {
            // create a new checkbox using the specific adapter field class
            FormCheckBox clsCheckBox = new TXTextControl.DocumentServer.Fields.FormCheckBox();
            clsCheckBox.Text = UNCHECKED;
            clsCheckBox.Enabled = true;
            clsCheckBox.ApplicationField.Editable = false;
            clsCheckBox.ApplicationField.DoubledInputPosition = true;
            
            // add the ApplicationField to the document
            textControl1.ApplicationFields.Add(clsCheckBox.ApplicationField);

            // select the inserted check box
            textControl1.Select(textControl1.Selection.Start - 1, 1);

            // we use an Unicode font to simulate the check box
            textControl1.Selection.FontName = "Arial Unicode MS";
            // remove the selection
            textControl1.Select(textControl1.Selection.Start + 1, 0);
        }

        public void SetFields()
        {
            byte[] binaryDocument = null;
            // LoadSettings must be adjusted to load the MS Word fields
            TXTextControl.LoadSettings ls = new LoadSettings();
            ls.ApplicationFieldFormat = ApplicationFieldFormat.MSWord;
            // save the document to the variable
            textControl1.Save(out binaryDocument, BinaryStreamType.InternalUnicodeFormat);
            // create a ServerTextControl instance to convert the checkboxes
            using( TXTextControl.ServerTextControl serverTextControl = new ServerTextControl() )
            {
                serverTextControl.Create();
            // load the document from the variable
                serverTextControl.Load(binaryDocument, TXTextControl.BinaryStreamType.InternalUnicodeFormat, ls);

            // loop through all fields to activate the checkbox fields
                foreach (TXTextControl.ApplicationField appfield in serverTextControl.ApplicationFields)
                {
                    if ((appfield.TypeName == "FORMCHECKBOX"))
                    {
                        // create a new adapter field
                        FormCheckBox ChkBoxField = new FormCheckBox(appfield);

                        // select the field to change the font name
                        serverTextControl.Select(ChkBoxField.Start - 1, ChkBoxField.Length);
                        serverTextControl.Selection.FontName = "Arial Unicode MS";

                        // set the text (state)
                        if (ChkBoxField.Checked == true)
                            ChkBoxField.Text = CHECKED;
                        else
                            ChkBoxField.Text = UNCHECKED;
                    }
                }
            // save the document back to the variable
                serverTextControl.Save(out binaryDocument, BinaryStreamType.InternalUnicodeFormat);
            // load the document back into the TextControl to show it to the user
                textControl1.Load(binaryDocument, BinaryStreamType.InternalUnicodeFormat, ls);
            }
        }

        private void textControl1_TextFieldClicked(object sender, TXTextControl.TextFieldEventArgs e)
        {
            // cast the field to an ApplicationField
            ApplicationField field = e.TextField as ApplicationField;

            if (field != null)
            {
                // check whether the field is a checkbox field
                if ((field.TypeName == "FORMCHECKBOX"))
                {
                    // create a new adapter field
                    FormCheckBox chkb = new FormCheckBox(field);

                    if (chkb.Enabled == false)
                        return;

                    // change the checked state
                    if (field.Text == UNCHECKED)
                    {
                        chkb.Checked = true;
                        chkb.Text = CHECKED;
                    }
                    else if (field.Text == CHECKED)
                    {
                        chkb.Checked = false;
                        chkb.Text = UNCHECKED;
                    }
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // load the document
            // LoadSettings must be adjusted to load the MS Word fields
            LoadSettings ls = new LoadSettings();
            ls.ApplicationFieldFormat = ApplicationFieldFormat.MSWord;
            textControl1.Load(StreamType.RichTextFormat | StreamType.MSWord | StreamType.WordprocessingML, ls);

            SetFields();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save the document
            SaveSettings ss = new SaveSettings();
            textControl1.Save(StreamType.RichTextFormat | StreamType.MSWord | StreamType.WordprocessingML, ss);
        }

        private void textControl1_TextFieldCreated(object sender, TextFieldEventArgs e)
        {
            int iStartPos = textControl1.Selection.Start;
            SetFields();
            textControl1.Selection.Start = iStartPos;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textControl1.Print("TX Text Control Document");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textControl1.Tables.GridLines = false;
        }
    }
}
