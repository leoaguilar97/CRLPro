using ClientCRL.crl.compiler.calc;
using ClientCRL.crl.compiler.symtbl;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using static crl.Compiler.SymbolTable.Parameter;

namespace crl.Compiler.SymbolTable
{
    class Scope
    {
        Calculator Calc;

        List<ScopeMember> Vars;
        List<ScopeMember> Functions;

        public Scope Child;
        public Scope Parent;

        public ScopeType Type { get; set; }
        
        /// <summary>
        /// Obtiene el tipo de valor de una expresion (String, Char, Int, Double, Bool)
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        internal static ParameterType GetObjectType(object par)
        {
            if (par is string) return ParameterType.String;
            if (par is double || par is int) return ParameterType.Numeric;
            if (par is char) return ParameterType.Char;
            if (par is bool) return ParameterType.Bool;

            return ParameterType.Empty;
        }

        /// <summary>
        /// Crea un nuevo ambito
        /// </summary>
        public Scope(ScopeType type)
        {
            Vars = new List<ScopeMember>();
            Calc = new Calculator();
            Type = type;
            
            Functions = new List<ScopeMember>();
        }

        /// <summary>
        /// Crea un nuevo ámbito hijo del ambito anterior
        /// </summary>
        /// <param name="program"></param>
        /// <param name="parent"></param>
        public Scope(ScopeType type, Scope parent): this(type)
        {
            Parent = parent;
            Parent.Child = this;

            Functions = new List<ScopeMember>();
        }
        
        private object Resolve(object o1, string op, object o2, ParseTreeNode node)
        {
            //Obtener valor si es una referencia a variable
            if (o1 is ScopeMember)
            {
                o1 = ((ScopeMember)o1).Value;
            }

            if (o2 is ScopeMember)
            {
                o2 = ((ScopeMember)o2).Value;
            }

            if (o1 != null && o2 != null)
            {
                switch (op)
                {
                    //Aritmethics
                    case "+": return Calc.Add(o1, o2);
                    case "-": return Calc.Sub(o1, o2);
                    case "*": return Calc.Mul(o1, o2);
                    case "/": return Calc.Div(o1, o2);
                    case "%": return Calc.Mod(o1, o2);
                    case "^": return Calc.Pot(o1, o2);

                    case "~": return Calc.Compare(o1, o2);
                    case ">": return Calc.Greater(o1, o2);
                    case "<": return Calc.Less(o1, o2);

                    case ">=": return Calc.GreaterEql(o1, o2);
                    case "<=": return Calc.LessEql(o1, o2);
                    case "==": return Calc.Equal(o1, o2);
                    case "!=": return Calc.Different(o1, o2);

                    case "&&": return Calc.And(o1, o2);
                    case "||": return Calc.Or(o1, o2);
                    case "|&": return Calc.Xor(o1, o2);
                }
            }

            Logger.AddError(node, "[Expresion binaria] El operando no tiene un valor definido. No se puede operar.");
            return null;
        }

        private object Resolve(string op, object o1, ParseTreeNode node)
        {

            if (o1 is ScopeMember)
            {
                o1 = ((ScopeMember)o1).Value;
            }

            if (o1 != null)
            {
                switch (op)
                {
                    case "!": return Calc.Not(o1);
                    case "-": return Calc.Sub(o1);
                }
            }

            Logger.AddError(node, "[Expresion unaria] El operando no tiene un valor definido. No se puede operar.");

            return null;
        }

        private object GetExpValue(ParseTreeNode node)
        {
            if (node == null) return null;

            string curr_content = node.FindTokenAndGetText();

            switch (node.Term.Name)
            {
                case "IDENTIFICADOR": return GetVar(curr_content)?.Value;

                case "NUMERO":
                    bool accepted = double.TryParse(curr_content, out double cont);

                    return accepted ? (object)cont : null;

                case "CADENA": return curr_content.Replace("\"", "");

                case "LOGICO_TRUE": return true;

                case "LOGICO_FALSE": return false;

                case "LLAMADA_FUNCION": return Raise(node, this);

                case "EXPRESION":
                case "OPERANDO":
                case "OPERACION":
                    {
                        int childCount = node.ChildNodes.Count;

                        switch (childCount)
                        {
                            //Operando unico
                            case 1: return GetExpValue(node.ChildNodes[0]);
                            //Operando unario (-, !)
                            case 2: return Resolve(node.ChildNodes[0].FindTokenAndGetText(), GetExpValue(node.ChildNodes[1]), node);
                            //Operando binario (+, -, *, /, %, ^, ~, >, <, >=, <=, ==, !=, &&, ||, |&)
                            case 3: return Resolve(GetExpValue(node.ChildNodes[0]), node.ChildNodes[1].FindTokenAndGetText(), GetExpValue(node.ChildNodes[2]), node);
                        }
                    }
                    break;
            }

            return null;
        }

        private object RaiseFromGlobal(ParseTreeNode node)
        {
            return Program.Global.Raise(node, new Scope(ScopeType.FUNCTION));
        }
        
        private List<Parameter> CreateParams(ParseTreeNode node)
        {
            if (node.ChildNodes.Count > 0)
            {
                List<Parameter> result = new List<Parameter>();

                foreach(var par in node.ChildNodes)
                {
                    //obtener el valor del parametro
                    object val = Solve(par);

                    result.Add(new Parameter { Type = GetObjectType(val), Data = val });
                }

                return result;
            }
            return null;
        }
        
        /// <summary>
        /// Agrega una funcion al scope
        /// </summary>
        /// <param name="funct"></param>
        internal void AddFunct(ScopeMember funct)
        {
            Functions.Add(funct);
        }

        /// <summary>
        /// Devuelve una funcion en base a su nombre
        /// </summary>
        /// <param name="funct"></param>
        /// <returns></returns>
        internal ScopeMember GetFunction(string funct, List<Parameter> parameters)
        {
            foreach (ScopeMember f in Program.Global.Functions)
            {
                if (funct == f.Name)
                {
                    bool returnValue = true;
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (parameters[i].Type == f.Parameters[i].Type)
                        {
                            var param = f.Parameters[i];

                            param.Data = parameters[i].Data;
                        }
                        else
                        {
                            //Los parametros no se cumplen
                            returnValue = false;
                            break;
                        }
                    }
                    if (returnValue) return f;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene una lista de funciones que cumplan con un nombre
        /// </summary>
        /// <param name="func_name"></param>
        /// <returns></returns>
        internal List<ScopeMember> GetFunctions(string func_name)
        {
            List<ScopeMember> result = new List<ScopeMember>();
            foreach (var f in Program.Global.Functions)
            {
                if (f.Name.Equals(func_name))
                {
                    result.Add(f);
                }
            }
            return result;
        }

        /// <summary>
        /// Corre una funcion y retorna su valor
        /// </summary>
        /// <param name="fnct"></param>
        /// <returns></returns>
        internal object Raise(ParseTreeNode fnct, Scope param_scope)
        {
            object result = null;

            //func_name( param_list ) ;
            if (fnct.ChildNodes.Count == 2)
            {
                String func_name = fnct.ChildNodes[0].FindTokenAndGetText();
                ParseTreeNode pars = fnct.ChildNodes[1];
                int parSize = pars.ChildNodes.Count;

                if (func_name.Equals("DibujarAST") && parSize == 1 && ProgramBuiltIns.DrawAST(pars))
                {
                    return null;
                }

                if (func_name.Equals("DibujarEXP") && parSize == 1 && ProgramBuiltIns.DrawEXP(pars))
                {
                    return null;
                }

                List<Parameter> parameters = CreateParams(pars);
                
                if (func_name.Equals("Mostrar") && parSize >= 1 && parameters[0].Type == ParameterType.String)
                {
                    ProgramBuiltIns.Log(pars, param_scope, fnct);
                }
                else
                {
                    //Si se encuentra la funcion, los parametros son agregados a ella adentro de la funcion
                    ScopeMember funct = GetFunction(func_name, parameters);

                    if (funct != null)
                    {
                        Program.Uncertainty = funct.Uncertainty;
                        Program.ImagePath = funct.ImagePath;

                        result = funct.Raise();

                        //check result validity
                        if (result == null && funct.Type != "Vacio")
                        {
                            Logger.AddError(fnct, "No se devolvio ningun valor de la funcion [" + funct.Name + "] de tipo [" + funct.Type + "].");
                        }
                    }

                    else
                    {
                        Logger.AddError(fnct, string.Format("No se encontró la función [{0}].", func_name));
                    }
                }
            }

            return result;
        }
                                
        /// <summary>
        /// Obtiene una variable en base a su nombre
        /// </summary>
        /// <param name="var_name"></param>
        /// <returns></returns>
        internal ScopeMember GetVarInCurrentScope(string var_name)
        {
            foreach (ScopeMember var in Vars)
            {
                if (var.Name.Equals(var_name))
                {
                    return var;
                }
            }
            return null;
        }

        /// <summary>
        /// Revisa si un nombre de variable ya existe en el ambito actual
        /// </summary>
        /// <param name="var_name"></param>
        /// <returns></returns>
        internal bool ExistInCurrentScope(string var_name) {
            return GetVarInCurrentScope(var_name) != null;
        }

        /// <summary>
        /// Auto incrementa o decrementa una variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal void AutoSum(string name, double val)
        {
            ScopeMember member = GetVar(name);

            if (member != null && member.IsVar)
            {
                member.Value = Calc.Add(member.Value, val);
            }
        }

        /// <summary>
        /// Asigna un valor a una variable ya existente
        /// </summary>
        /// <param name="node"></param>
        internal void AsignVar(ParseTreeNode node)
        {
            if (node.ChildNodes.Count == 2)
            {
                string name = node.ChildNodes[0].FindTokenAndGetText();

                ScopeMember member = GetVar(name);

                if (member != null && member.IsVar)
                {
                    member.Value = Solve(node.ChildNodes[1]);
                }
            }
            else
            {
                Logger.AddError(node, "Variable no encontrada. ¿Esta seguro que la inicializó?");
            }
        }

        /// <summary>
        /// Agregar variable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddVar(string type, string name, object value)
        {
            if (!ExistInCurrentScope(name))
            {
                ScopeMember var = new ScopeMember(type, name, value);

                Vars.Add(var);
            }
            else
            {
                Logger.AddWarning(string.Format("Variable [{0} {1} = {2}] declarada anteriormente. No se modifico el valor inicial.", type, name, value));
            }
        }

        /// <summary>
        /// Añade una variable al ambito
        /// </summary>
        /// <param name="container">Nodo de tipo VARIABLE o de tipo PARAMETRO</param>
        public void AddVar(ParseTreeNode container) {

            string type = container.Term.Name;

            int childs = container.ChildNodes.Count;

            if (childs >= 2)
            {
                //Obtener el tipo de variable
                string var_type = container.ChildNodes[0].FindTokenAndGetText();

                // Terminal as: type name, name2, ..., nameN [= exp];
                if (type == "VARIABLE")
                {
                    ParseTreeNode assign_value = childs == 3 ? container.ChildNodes[2] : null;

                    object value = Solve(assign_value);

                    ParseTreeNodeList names = container.ChildNodes[1].ChildNodes;

                    if (names != null)
                    {
                        if (names.Count != 0)
                        {
                            //Obtener cada identificador
                            foreach (ParseTreeNode node in container.ChildNodes[1].ChildNodes)
                            {
                                string name = node.FindTokenAndGetText();
                                AddVar(var_type, name, value);
                            }
                        }
                        else
                        {
                            //Obtener identificador unico
                            string name = container.ChildNodes[1].FindTokenAndGetText();
                            AddVar(type, name, value);
                        }
                    }

                }
            }
        }
        
        /// <summary>
        /// Anade un parametro al ambito actual.
        /// </summary>
        /// <param name="param"></param>
        internal void AddVar(Parameter param)
        {
            AddVar(param.TypeAsString(), param.Name, param.Data);
        }

        /// <summary>
        /// Obtiene el valor de una variable que exista en el ambito
        /// </summary>
        /// <param name="var_name">Nombre de la variable</param>
        /// <returns></returns>
        internal object GetVarValue(String var_name) {
            //Buscar la variable
            foreach (ScopeMember var in Vars) {
                if (var.Name == var_name) {
                    return var.Value; 
                }
            }
            //variable no encontrada
            return null;
        }
           
        /// <summary>
        /// Resuelve una expresion
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal object Solve(ParseTreeNode node)
        {
            object val = GetExpValue(node);

            return val ?? "[NULL]";
        }
        
        /// <summary>
        /// Obtiene la variable, buscándola en todos los ambitos que hay
        /// </summary>
        /// <param name="var_name"></param>
        /// <returns></returns>
        internal ScopeMember GetVar(string var_name)
        {
            var scope = this;

            do
            {
                ScopeMember val = scope.GetVarInCurrentScope(var_name);

                if (val != null) return val;

                scope = scope.Parent;
            }
            while (scope != null);

            return Program.Global.GetVarInCurrentScope(var_name);
        }
    }
}
