using ClientCRL.ui.control;
using System;
using System.Windows;
using Ui.SyntaxHighlightBox;

namespace ClientCRL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EditorController editorController;
        /// <summary>
        /// Generates a new Alert message in error format
        /// </summary>
        /// <param name="message"></param>
        private void AlertError(string message)
        {
            Console.Out.WriteLine("Error: " + message);
            MessageBox.Show(message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
        
        /// <summary>
        /// Generates a new Alert message in success format
        /// </summary>
        /// <param name="message"></param>
        private void AlertSuccess(string message)
        {
            Console.Out.WriteLine("Success: " + message);
            MessageBox.Show(message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        public MainWindow()
        {
            InitializeComponent();

            editorController = new EditorController(EditorContainer)
            {
                ProgramConsole = console
            };
        }

        private void btnNewEditor_Click(object sender, RoutedEventArgs e)
        {
            editorController.AddEditor();
        }

        private void btnCloseEditor_Click(object sender, RoutedEventArgs e)
        {
            editorController.CloseEditor();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            bool opened = editorController.OpenCRLFile();

            if (!opened)
            {
                AlertError("No se abrió ningún archivo.");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool saved = editorController.SaveCRLFile();

            if (saved)
                AlertSuccess("Archivo guardado exitosamente.");
            else
                AlertError("No se guardó el archivo.");
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (editorController.Compile())
            {
                editorController.PrintAST(astImage);
            }
            else
            {
                AlertError("El codigo ingresado no pudo ser compilado. Tiene uno o más errores.");
            }
        }
    }
}
