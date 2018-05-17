using crl.Compiler.AST;
using System.Windows.Media.Imaging;

namespace crl.Compiler
{
    class ProgramController
    {
        Compiler compiler;

        /// <summary>
        /// Crea un nuevo controlador del compilador
        /// </summary>
        /// <param name="code"></param>
        public ProgramController(string code)
        {
            compiler = new Compiler(code);
        }

        /// <summary>
        /// Obtiene la imagen que genera el AST en formato BitMap (Util para la GUI)
        /// </summary>
        /// <returns></returns>
        internal BitmapImage GetASTImage()
        {
            ASTImageCreator img_creator = new ASTImageCreator(compiler.Ast.Root);
            return img_creator.GetAsImage();
        }

        /// <summary>
        /// Compila el codigo CRL string en un programa
        /// </summary>
        /// <param name="code"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        internal bool Compile()
        {
            //Solve parse 
            return compiler.Compile();
        }

        internal string GetTextLogs()
        {
            return Logger.GetLogsAsText(compiler.AstLogs);
        }
    }
}
