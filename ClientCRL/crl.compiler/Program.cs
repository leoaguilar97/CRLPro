using Irony.Parsing;
using crl.Compiler.SymbolTable;
using ClientCRL.crl.compiler.symtbl;
using System;

namespace crl.Compiler
{
    /// <summary>
    /// Representa un programa de CRL
    /// </summary>
    public class Program
    {
        public static double Uncertainty { get; set; }
        internal static Scope Global;
        private ScopeMember Main { get; set; }
        public static string ImagePath { get; internal set; }

        /// <summary>
        /// Crea un nuevo programa de CRL
        /// </summary>
        public Program() {
            Uncertainty = 0.5;
            ImagePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Global = new Scope(ScopeType.GLOBAL);
        }
                          
        #region functions management
        /// <summary>
        /// Añade una funcion al programa
        /// </summary>
        /// <param name="member"></param>
        internal void AddFunction(ParseTreeNode member)
        {
            if (member.ChildNodes.Count == 4)
            {
                String type = member.ChildNodes[0].FindTokenAndGetText();
                String name = member.ChildNodes[1].FindTokenAndGetText();

                ParseTreeNode pars = member.ChildNodes[2];
                ParseTreeNode body = member.ChildNodes[3];

                ScopeMember funct = new ScopeMember(type, name, member)
                {
                    Uncertainty = Uncertainty,
                    ImagePath = ImagePath
                };

                if (funct.IsMain)
                {
                    Main = funct;
                }

                Global.AddFunct(funct);
            }
        }
        #endregion

        #region var management    
                        
        /// <summary>
        /// Añade una variable al ambito global
        /// /// </summary>
        /// <param name="node">Nodo de tipo VARIABLE</param>
        internal void AddVar(ParseTreeNode node)
        {
            Global.AddVar(node);
        }  

        #endregion
      
        /// <summary>
        /// Inicia un programa desde el método PRINCIPAL
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            if (Main != null )
            {
                Main.Raise();
                return true;
            }

            Logger.AddWarning("No se encontró la función PRINCIPAL. No se ejecutó el código.");
            return false;
        }
    }
}