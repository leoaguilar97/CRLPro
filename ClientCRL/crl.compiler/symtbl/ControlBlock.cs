using Irony.Parsing;
using ClientCRL.crl.compiler.symtbl;
using System;

namespace crl.Compiler.SymbolTable
{
    /// <summary>
    /// Representa una sentencia de control [SI, SINO, HASTA, MIENTRAS, PARA, SELECCIONA...]
    /// </summary>
    public class ControlBlock
    {        
        public ParseTreeNode Statements { get; set; }
        Scope Scope { get; set; }
        private static ScopeType currentScopeType = ScopeType.NORMAL;

        internal static Signal Signal = Signal.NORMAL;
        public static object ReturnedValue { get; internal set; }

        /// <summary>
        /// Crea una nueva sentencia de control y la corre
        /// </summary>
        /// <param name="program"></param>
        /// <param name="block"></param>
        /// <param name="parent"></param>
        internal ControlBlock(ParseTreeNode block, Scope scope) {
            Statements = block;
            Scope = scope;

            Run(Statements);
        }
        
        #region IF
        /// <summary>
        /// Realiza un IF
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="node"></param>
        private void PerformIf(bool expression, ParseTreeNode node)
        {
            if (expression)
            {
                DoStatements(node);
            }
        }

        /// <summary>
        /// Realiza un IF con ELSE
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="if_node"></param>
        /// <param name="else_node"></param>
        private void PerformIf(bool expression, ParseTreeNode if_node, ParseTreeNode else_node)
        {
            if (expression)
            {
                DoStatements(if_node);
            }
            else if (else_node.ChildNodes.Count == 2)
            {
                DoStatements(else_node.ChildNodes[1]);
            }            
        }
        #endregion

        #region While
        /// <summary>
        /// Realiza un WHILE
        /// </summary>
        /// <param name="node_enter"></param>
        /// <param name="statements"></param>
        private void PerformWhile(ParseTreeNode node_enter, ParseTreeNode statements)
        {
            while (true)
            {
                object exp = Scope.Solve(node_enter);

                if (exp == null || !(exp is bool) || !(bool)exp) { return; }

                DoStatements(statements);

                if (Signal == Signal.BREAK)
                {
                    break;
                }

                Signal = Signal.NORMAL;
            }
            Signal = Signal.NORMAL;
        }
        #endregion

        #region Repeat
        private void PerformRepeat(ParseTreeNode node_enter, ParseTreeNode statements)
        {
            while (true)
            {
                object exp = Scope.Solve(node_enter);

                if (exp == null || !(exp is bool) || (bool)exp) { return; }

                DoStatements(statements);

                if (Signal == Signal.BREAK)
                {
                    break;
                }
                Signal = Signal.NORMAL;
            }
            Signal = Signal.NORMAL;
        }
        #endregion

        #region For
        private void PerformFor(string for_var_name, ParseTreeNode enter, ParseTreeNode action, ParseTreeNode statements)
        {
            int sum = action.FindTokenAndGetText() == "++" ? 1 : -1;

            while (true)
            {
                object exp = Scope.Solve(enter);

                if (exp == null || !(exp is bool) || !(bool)exp) { return; }

                DoStatements(statements);

                if (Signal == Signal.BREAK)
                {
                    break;
                }

                Scope.AutoSum(for_var_name, sum);

                Signal = Signal.NORMAL;

            }
            Signal = Signal.NORMAL;
        }
        #endregion

        #region Select
        private void Switch(string match, ParseTreeNode cases)
        {
            if (cases.ChildNodes.Count >= 1)
            {
                currentScopeType = ScopeType.SWITCH;

                //Obtener lista de casos <VALOR> : <EXPRESIONES>
                var case_list = cases.ChildNodes[0];
                //Obtener default
                var def = cases.ChildNodes.Count == 2 ? cases.ChildNodes[1] : null;

                bool forceCase = false;

                foreach (var case_block in case_list.ChildNodes)
                {

                    if (case_block.ChildNodes.Count == 2)
                    {
                        if (Signal == Signal.BREAK)
                        {
                            break;
                        }

                        var exp = Scope.Solve(case_block.ChildNodes[0]);

                        if (exp is string)
                        {
                            //Realizar case
                            if (forceCase || exp.Equals(match))
                            {
                                DoStatements(case_block.ChildNodes[1]);
                                forceCase = true;
                            }
                        }
                        else
                        {
                            Logger.AddError(case_block.ChildNodes[0], "El valor a comparar debe ser de tipo String. Se omitio el caso.");
                        }
                    }
                }

                if (Signal != Signal.BREAK && def != null && def.ChildNodes.Count == 2)
                {
                    DoStatements(def.ChildNodes[1]);
                }

                if (Signal == Signal.BREAK) Signal = Signal.NORMAL;
            }
        }
        
        private void Switch(double match, ParseTreeNode cases)
        {
            if (cases.ChildNodes.Count >= 1)
            {
                currentScopeType = ScopeType.SWITCH;

                //Obtener lista de casos <VALOR> : <EXPRESIONES>
                var case_list = cases.ChildNodes[0];
                //Obtener default
                var def = cases.ChildNodes.Count == 2 ? cases.ChildNodes[1] : null;

                bool forceCase = false;

                foreach (var case_block in case_list.ChildNodes)
                {

                    if (case_block.ChildNodes.Count == 2)
                    {
                        if (Signal == Signal.BREAK)
                        {
                            break;
                        }

                        var exp = Scope.Solve(case_block.ChildNodes[0]);

                        if (exp is double)
                        {
                            //Realizar case
                            if (forceCase || exp.Equals(match))
                            {
                                DoStatements(case_block.ChildNodes[1]);
                                forceCase = true;
                            }
                        }
                        else
                        {
                            Logger.AddError(case_block.ChildNodes[0], "El valor a comparar debe ser de tipo String. Se omitio el caso.");
                        }
                    }
                }

                if (Signal != Signal.BREAK && def != null && def.ChildNodes.Count == 2)
                {
                    DoStatements(def.ChildNodes[1]);
                }

                if (Signal == Signal.BREAK) Signal = Signal.NORMAL;
            }
        }
            
        #endregion

        private void Run(ParseTreeNode node) {
            
            if (Signal != Signal.NORMAL) return;
            
            switch (node.Term.Name)
            {
                case "VARIABLE": Scope.AddVar(node); break;

                case "ASIGNACION_VARIABLE": Scope.AsignVar(node); break;

                case "LLAMADA_FUNCION":
                case "MOSTRAR": Scope.Raise(node, Scope); break;
                
                //control blocks
                case "SI":
                    {
                        int childs = node.ChildNodes.Count;

                        if (childs < 3) { return; }

                        object exp = Scope.Solve(node.ChildNodes[1]);

                        if (exp == null || !(exp is bool)) { return; }

                        bool enter = (bool)exp;

                        switch (childs)
                        {
                            //IF [exp] [statements]
                            case 3:
                                PerformIf(enter, node.ChildNodes[2]);
                                return;
                            //IF [exp] [statements] [else_statements]
                            case 4:
                                PerformIf(enter, node.ChildNodes[2], node.ChildNodes[3]);
                                return;
                        }
                    }
                    return;

                case "MIENTRAS":
                    if (node.ChildNodes.Count == 3)
                    {
                        currentScopeType = ScopeType.LOOP;
                        PerformWhile(node.ChildNodes[1], node.ChildNodes[2]);
                        Signal = Signal.NORMAL;
                    }
                    break;

                case "REPETIR":
                    if (node.ChildNodes.Count == 3)
                    {
                        currentScopeType = ScopeType.LOOP;
                        PerformRepeat(node.ChildNodes[1], node.ChildNodes[2]);
                    }
                    break;

                case "PARA":
                    if (node.ChildNodes.Count == 5)
                    {
                        currentScopeType = ScopeType.LOOP;

                        Scope.AddVar(node.ChildNodes[1]);
                        string name = node.ChildNodes[1].ChildNodes[1].FindTokenAndGetText();

                        PerformFor(name, node.ChildNodes[2], node.ChildNodes[3], node.ChildNodes[4]);

                    }
                    break;

                case "SELECCIONA":
                    {
                        if (node.ChildNodes.Count == 3)
                        {
                            var exp_value = Scope.Solve(node.ChildNodes[1]);

                            ParseTreeNode cases = node.ChildNodes[2];

                            if (exp_value is string)
                            {
                                Switch(exp_value.ToString(), cases);
                            }
                            if (exp_value is double)
                            {
                                Switch((double)exp_value, cases);
                            }
                        }
                    }
                    break;

                case "RETORNO":
                    {
                        ReturnedValue = node.ChildNodes.Count == 1 ? null : Scope.Solve(node.ChildNodes[1]);
                        Signal = Signal.RETURNING;
                    }
                    break;

                case "CONTINUAR":
                    if (Scope.Type == ScopeType.LOOP)
                    {
                        Signal = Signal.CONTINUE;
                    }
                    else
                    {
                        Logger.AddWarning("No se puede utilizar la sentencia CONTINUAR en este ambito. Se omitió de la compilación.");
                    }
                    break;

                case "DETENER":
                    if (Scope.Type == ScopeType.LOOP || Scope.Type == ScopeType.SWITCH)
                    {
                        Signal = Signal.BREAK;
                    }
                    else
                    {
                        Logger.AddWarning("No se puede utilizar la sentencia DETENER en este ambito. Se omitió de la compilación.");
                    }
                    break;
            }
        }


        /// <summary>
        /// Inicia un recorrido desde un nodo establecido
        /// </summary>
        /// <param name="statements"></param>
        private void DoStatements(ParseTreeNode statements)
        {
            if (statements == null && statements.Term != null) return;

            if (statements.Term.Name == "SENTENCIAS")
            {
                Scope scope = new Scope(currentScopeType, Scope);
                foreach (ParseTreeNode statement in statements.ChildNodes)
                {
                    ControlBlock cb = new ControlBlock(statement, scope);
                }
            }
        }
    }
}