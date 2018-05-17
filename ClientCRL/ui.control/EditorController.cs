using crl.Compiler;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientCRL.ui.control
{
    class EditorController
    {
        private List<Editor> editors;
        private ImageSource astImage;
        private TabControl container;
        private int newEditorId;

        internal RichTextBox ProgramConsole { get; set; }
        
        private Editor GetCurrentEditor()
        {
            return GetEditorAt(GetCurrentTabItem());
        }

        private FileInfo CreateFileInfo(string name)
        {
            string data_start_type = string.Format("!! Programa: \"{0}\".crl \n", name);

            string current_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return new FileInfo { name = name, data = data_start_type, path = current_path };
        }

        private Editor CreateEditor(string name)
        {
            Editor editor = new Editor(CreateFileInfo(name));

            return editor;
        }
        
        private TabItem GetCurrentTabItem()
        {
            return container.SelectedItem as TabItem;            
        }

        private Editor GetEditorAt(TabItem currentTabItem) {
            try
            {
                Grid currentGrid = currentTabItem.Content as Grid;

                if (currentGrid.Children.Count == 1)
                {   
                    return editors[container.SelectedIndex];
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error al seleccionar editor: " + ex.Message);
                Console.Out.WriteLine(ex.StackTrace);
            }

            return null;           
        }
        
        /// <summary>
        /// Creates a new editor controller
        /// </summary>
        /// <param name="container"></param>
        public EditorController(TabControl container)
        {
            editors = new List<Editor>();

            newEditorId = 0;

            this.container = container;
        }

        /// <summary>
        /// Add a code editor to the main tab container
        /// </summary>
        internal Editor AddEditor() {

            string name = string.Format("NewCRL_{0}", ++newEditorId);

            Editor editor = CreateEditor(name);
            
            editors.Add(editor);

            Grid content = new Grid();

            content.Children.Add(editor.UI);

            TabItem tab = new TabItem() {
                Name = "tab" + newEditorId,
                Header = name,
                Content = content
            };

            container.Items.Add(tab);

            container.SelectedItem = tab;

            editor.UI.Text = editor.File.data;
            
            return editor;
        }

        /// <summary>
        /// Close the current editor
        /// </summary>
        internal void CloseEditor() {

            if (container.Items.Count == 0) return ;

            var currentTabItem = GetCurrentTabItem();

            var currentEditor = GetEditorAt(currentTabItem);

            if (currentEditor != null)
            {
                //remove current editor
                editors.Remove(currentEditor);

                //remove current tab item
                container.Items.Remove(currentTabItem);                
            }
        }

        /// <summary>
        /// Opens a CRL file
        /// </summary>
        /// <returns></returns>
        internal bool OpenCRLFile()
        {
            FileInfo info = FileAdmin.OpenFile();

            if (info.name != "")
            {
                if (container.Items.Count == 0)
                {
                    AddEditor();
                }

                TabItem currentTab = GetCurrentTabItem();

                if (currentTab != null)
                {
                    currentTab.Header = info.name;

                    Editor currentEditor = GetEditorAt(currentTab);

                    if (currentEditor != null)
                    {
                        currentEditor.UI.Text = info.data;
                        currentEditor.File = info;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the current textbox to a .crl file
        /// </summary>
        /// <returns></returns>
        internal bool SaveCRLFile()
        {
            Editor currentEditor = GetCurrentEditor();

            if (currentEditor != null)
            {
                currentEditor.SaveData();
                return FileAdmin.SaveFile(currentEditor.File);
            }

            return false;
        }

        /// <summary>
        /// Compila el codigo del editor actual
        /// </summary>
        /// <returns></returns>
        internal bool Compile()
        {
            bool result = false;

            Editor currentEditor = GetCurrentEditor();

            if (currentEditor != null)
            {
                string code = currentEditor.UI.Text;

                ProgramController controller = new ProgramController(code);
                
                if (controller.Compile())
                {
                    astImage = controller.GetASTImage();
                    result = true;
                }

                ProgramConsole.Document.Blocks.Clear();
                ProgramConsole.AppendText(controller.GetTextLogs());
            }

            return result;
        }

        /// <summary>
        /// Dibuja el AST en una imagen
        /// </summary>
        /// <param name="imgAST"></param>
        internal void PrintAST(Image imgAST) => imgAST.Source = astImage;
    }
}
