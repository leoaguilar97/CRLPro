using System;
using System.Windows.Media;
using Ui.SyntaxHighlightBox;

namespace ClientCRL.ui.control
{
    class Editor
    {
        private FileInfo file;
        public SyntaxHighlightBox UI { get; set; }
        public FileInfo File { get { return file; } set { file = value; } }

        public Editor(FileInfo file)
        {
            File = file;

            UI = new SyntaxHighlightBox
            {
                IsLineNumbersMarginVisible = true,

                FontFamily = new FontFamily("Consolas"),

                CurrentHighlighter = HighlighterManager.Instance.Highlighters["CRL"]
            };
        }

        internal void SaveData()
        {
            file.data = UI.Text;
        }
    }
}
