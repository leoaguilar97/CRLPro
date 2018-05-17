using Irony.Parsing;
using System;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using WINGRAPHVIZLib;

namespace crl.Compiler.AST
{
    /// <summary>
    /// Clase que maneja la generacion de imagenes a partir de un nodo AST
    /// </summary>
    public class ASTImageCreator
    {
        private const String node_format = "node{0}";
        private const String label_format = @"{0}[label=""{1}""];";
        private const String connect_format = "{0} -> {1};";

        private ParseTreeNode root_node;
        private int childCount;

        private string folder;
        private string fname;

        private bool Function;

        /// <summary>
        /// Creador de imagenes a partir de un nodo AST
        /// </summary>
        /// <param name="root_node"></param>
        public ASTImageCreator(ParseTreeNode root_node) {

            this.root_node = root_node;

            string date = DateTime.Now.ToShortDateString();
            string folder_name = "crl_reports_" + date;

            folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), folder_name);

            Directory.CreateDirectory(folder);

            fname = "ast_" + date + ".png";

            childCount = 0;
        }

        /// <summary>
        /// Creador de imagenes a partir de un nodo AST
        /// </summary>
        /// <param name="root_node"></param>
        /// <param name="fname">Especifica la ruta a la cual guardar la imagen</param>
        /// <param name="Function">Especifica que la imagen sera de una funcion</param>
        public ASTImageCreator(ParseTreeNode root_node, string folder, string fname, bool Function) : this(root_node)
        {
            this.folder = folder;

            Directory.CreateDirectory(folder);

            this.fname = fname + ".png";
            this.Function = Function;
        }

        /// <summary>
        /// Obtiene la imagen del AST en formato BitMap para utilizarlo en la GUI.
        /// </summary>
        /// <returns></returns>
        public BitmapImage GetAsImage()
        {

            string image64 = GetAsImage64();

            byte[] imageBytes = Convert.FromBase64String(image64);

            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);

            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();

            return bitmapImage;            
        }

        /// <summary>
        /// Obtiene la informacion de una imagen en formato base64
        /// </summary>
        /// <returns></returns>
        public string GetAsImage64()
        {
            String sdot = GetASDOT();

            DOT dot = new DOT();

            BinaryImage img = dot.ToPNG(sdot);

            img.Save(Path.Combine(folder, fname));
            
            return img.ToBase64String();
        }

        /// <summary>
        /// Obtiene la vista de la imagen como un HTML utilizando base64
        /// </summary>
        /// <returns></returns>
        public string GetAsHTMLImage64()
        {
            return "<img src='data:image/png;base64," + GetAsImage64() + "' alt='Arbol Binario' />";
        }

        private String GetASDOT()
        {

            childCount = 0;

            StringBuilder sb = new StringBuilder();
            
            sb.Append("digraph TreeGraph{");

            sb.Append("node[shape=\"box\"];");

            sb.Append(string.Format(label_format, "node0", ScapeDotString(root_node.ToString())));

            sb.Append(GetInnerDot("node0", root_node));

            return sb.Append("}").ToString();
        }

        private String GetInnerDot(String parent_label, ParseTreeNode parent)
        {

            StringBuilder result = new StringBuilder();

            String child_node;

            foreach (ParseTreeNode child in parent.ChildNodes)
            {

                child_node = string.Format(node_format, ++childCount);

                result.Append(string.Format(label_format, child_node, ScapeDotString(child.ToString())));

                result.Append(string.Format(connect_format, parent_label, child_node));

                result.Append(GetInnerDot(child_node, child));
            }

            return result.ToString();
        }

        private String ScapeDotString(String dot_value)
        {
            dot_value = dot_value.Replace("\\", "");
            dot_value = dot_value.Replace("\"", "");
            return dot_value;
        }

    }
}