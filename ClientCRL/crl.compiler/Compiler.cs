using Irony;
using Irony.Parsing;
using System;

namespace crl.Compiler
{
    /// <summary>
    /// Compilador de CRL
    /// </summary>
    public class Compiler
    {
        //No terminales a utilizar
        private const string MEM_PROGRAM = "MIEMBRO_PROGRAMA";
        private const string MEM_IMPORT = "IMPORTAR";
        private const string MEM_DEFINE = "DEFINIR";
        private const string MEM_NEW_VAR = "VARIABLE";
        private const string MEM_FUNC_DEC = "FUNCION";

        internal String Code { get; set; }
        internal ParseTree Ast { get; set; }
        internal LogMessageList AstLogs => Ast?.ParserMessages;

        /// <summary>
        /// Crea un compilador de CRL
        /// </summary>
        /// <param name="code">Codigo a compilar</param>
        public Compiler(String code)
        {
            Code = code;
        }

        /// <summary>
        /// Compila el codigo crl y verifica si hubieron errores de compilación
        /// </summary>
        /// <returns></returns>       
        public bool Compile()
        {
            CRLGrammar grammar = new CRLGrammar();
            LanguageData language_data = new LanguageData(grammar);
            Parser parser = new Parser(language_data);

            Ast = parser.Parse(Code);

            Logger.Logs = AstLogs;

            //Start compiling
            if (Ast.Root != null && AstLogs.Count == 0)
            {
                Program program = new Program();

                ReadMembers(Ast.Root, program);

                //Empezar a compilar desde Principal
                return program.Start();  
            }           

            return false;
        }
        
        /// <summary>
        /// Lee los miembros del programa y los guarda en el programa enviado como parametro (Util al momento de realizar IMPORTS)
        /// </summary>
        /// <param name="program_root">Contenedor de los miembros</param>
        /// <param name="program">Programa al que agregar las cosas</param>
        private void ReadMembers(ParseTreeNode program_root, Program program)
        {
            if (program_root != null)
            {
                foreach (ParseTreeNode member in program_root.ChildNodes)
                {
                    if (member.Term.Name == MEM_PROGRAM && member.ChildNodes.Count != 0)
                    {
                        ParseTreeNode mem = member.ChildNodes[0];
                        String mem_type = mem.Term.Name;

                        switch (mem_type)
                        {
                            //save import
                            case MEM_IMPORT:
                                break;
                            //save define
                            case MEM_DEFINE:
                                if (mem.ChildNodes.Count == 2)
                                {
                                    var val = mem.ChildNodes[1].FindTokenAndGetText();

                                    if (double.TryParse(val, out double uncertainty))
                                    {
                                        Program.Uncertainty = uncertainty;
                                    }
                                    if (val is string)
                                    {
                                        Program.ImagePath = val;
                                    }
                                    else
                                    {
                                        Logger.AddError(member, "El valor de DEFINIR no es String o Numerico.");
                                    }
                                }
                                break;
                            //save global var
                            case MEM_NEW_VAR:
                                program.AddVar(mem);
                                break;
                            //save function
                            case MEM_FUNC_DEC:
                                program.AddFunction(mem);
                                break;
                        }
                    }
                }
            }
        }
    }
}