using crl.Compiler.AST;
using crl.Compiler.SymbolTable;
using Irony.Parsing;
using System;
using System.Collections.Generic;

namespace crl.Compiler
{
    internal class ProgramBuiltIns
    {       
        /// <summary>
        /// Realiza la operacion MOSTRAR
        /// </summary>
        /// <param name="values"></param>
        /// <param name="scope"></param>
        internal static void Log(ParseTreeNode values, Scope scope, ParseTreeNode calling) {

            object format = scope.Solve(values.ChildNodes[0]);
            
            if (format == null)
            {
                Logger.AddLog("Valor Indefinido.", calling);
            }
            else
            {
                string strFormat = format.ToString();

                int numPars = values.ChildNodes.Count - 1;

                if (numPars > 0)
                {
                    List<string> pars = new List<string>();

                    int i = 0;

                    foreach (ParseTreeNode param in values.ChildNodes)
                    {
                        //Ignora el primer parametro que contiene el template
                        if (i != 0)
                        {
                            object par = scope.Solve(values.ChildNodes[i]);
                            pars.Add(par.ToString());
                        }

                        i++;
                    }
                    try
                    {
                        Logger.AddLog(string.Format(strFormat.ToString(), pars.ToArray()), calling);
                    }
                    catch (Exception ex)
                    {
                        Logger.AddWarning(ex.ToString() + " AL ESCRIBIR: " + strFormat.ToString());
                    }
                }
                else
                {
                    Logger.AddLog(strFormat.ToString(), calling);
                }
            }
        }

        internal static bool DrawAST(ParseTreeNode id)
        {
            if (id.ChildNodes.Count == 1)
            {
                string func_name = id.FindTokenAndGetText();

                List<ScopeMember> functs = Program.Global.GetFunctions(func_name);

                if (functs.Count != 0)
                {
                    foreach (var f in functs)
                    {
                        string fname = f.Name + f.GetHashCode();

                        ASTImageCreator imgCreator = new ASTImageCreator(f.Root, Program.ImagePath, fname, Function: true);

                        Logger.AddImg(imgCreator.GetAsImage64());
                    }
                    return true;
                }
                else
                {
                    Logger.AddError(id, "No existe ninguna función con ese nombre");
                }
            }
            return false;
        }

        internal static bool DrawEXP(ParseTreeNode pars)
        {
            throw new NotImplementedException();
        }
    }
}