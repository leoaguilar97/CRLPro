using Irony.Parsing;

namespace crl.Compiler
{
    [Language("CRL", "1.0", "Gramatica del lenguaje CRL")]
    public class CRLGrammar : Grammar
    {
        /// <summary>
        /// Creates an Irony Grammar
        /// </summary>
        public CRLGrammar() : base(caseSensitive: false)
        {
            //Comentarios
            var singleLineComment = new CommentTerminal("SingleLineComment", "!!", "\r", "\n", "\u2085", "\u2028", "\u2029");
            var delimitedComment = new CommentTerminal("DelimitedComment", "<<", ">>");            

            NonGrammarTerminals.Add(singleLineComment);
            NonGrammarTerminals.Add(delimitedComment);

            MarkPunctuation(";", ",", "(", ")", "{", "}", "=", ":");

            InitializeSyntax();
        }

        private void InitializeSyntax()
        {
            #region Identifiers 
            var id_literal = TerminalFactory.CreateCSharpIdentifier("IDENTIFICADOR");
            var number_literal = TerminalFactory.CreateCSharpNumber("NUMERO");
            var string_literal = TerminalFactory.CreateCSharpString("CADENA");
            var true_literal = ToTerm("TRUE", "LOGICO_TRUE");
            var false_literal = ToTerm("FALSE", "LOGICO_FALSE");
            #endregion

            #region NonTerminal
            NonTerminal program = new NonTerminal("PROGRAMA");

            NonTerminal define_dec = new NonTerminal("DEFINIR");
            NonTerminal define_list = new NonTerminal("DEFINICIONES");

            NonTerminal import_dec = new NonTerminal("IMPORTAR");
            NonTerminal import_list = new NonTerminal("IMPORTACIONES");
            
            NonTerminal member = new NonTerminal("MIEMBRO");
            NonTerminal member_list = new NonTerminal("MIEMBROS");
            
            NonTerminal var_dec = new NonTerminal("VARIABLE");
            NonTerminal var_type = new NonTerminal("TIPO");
            NonTerminal var_dec_list = new NonTerminal("VARIABLES");
            NonTerminal var_dec_list_names = new NonTerminal("LISTA_VARIABLES");
            NonTerminal var_dec_asign = new NonTerminal("VARIABLE_DECLARACION");

            NonTerminal function_dec = new NonTerminal("FUNCION");
            NonTerminal function_type = new NonTerminal("TIPO_FUNCION");
            NonTerminal parameters = new NonTerminal("PARAMETROS");
            NonTerminal parameter = new NonTerminal("PARAMETRO");

            NonTerminal var_asign = new NonTerminal("ASIGNACION_VARIABLE");

            NonTerminal print = new NonTerminal("MOSTRAR");

            NonTerminal statements = new NonTerminal("SENTENCIAS");
            NonTerminal statement = new NonTerminal("SENTENCIA");
            NonTerminal op = new NonTerminal("OPERANDO");
            NonTerminal ar = new NonTerminal("OPERACION");
            NonTerminal operand = new NonTerminal("OPERANDO");
            NonTerminal ao = new NonTerminal("AR_OP");
            NonTerminal expression = new NonTerminal("EXPRESION");
            NonTerminal co = new NonTerminal("COMPARADOR");
            NonTerminal bo = new NonTerminal("OPERADOR_BOOLEANO");
            NonTerminal scope = new NonTerminal("AMBITO");
            NonTerminal if_block = new NonTerminal("SI");
            NonTerminal else_dec = new NonTerminal("SINO");
            #endregion

            #region Gramaticas

            #region Program
            NonTerminal program_member = new NonTerminal("MIEMBRO_PROGRAMA");

            program.Rule = MakeStarRule(program, program_member);

            program_member.Rule = import_dec + ";" | define_dec + ";" | var_dec + ToTerm(";") | function_dec ;


            #endregion

            #region Import
            import_dec.Rule = "Importar" + string_literal;
            #endregion

            #region Definir
            define_dec.Rule = "Definir" + string_literal
                | "Definir" + number_literal;
            #endregion
            
            #region Funciones

            function_dec.Rule = ToTerm("Vacio") + id_literal + ToTerm("(") + parameters + ToTerm(")") + scope
                | var_type + id_literal + ToTerm("(") + parameters + ToTerm(")") + scope;
            
            function_type.Rule = var_type | ToTerm("Vacio");

            parameters.Rule = MakeStarRule(parameters, ToTerm(","), parameter);

            parameter.Rule = var_type + id_literal;

            NonTerminal func_call = new NonTerminal("LLAMADA_FUNCION");
            NonTerminal func_call_params = new NonTerminal("PARAMETROS_LLAMADA");

            func_call.Rule = id_literal + ToTerm("(") + func_call_params + ToTerm(")");            
            func_call_params.Rule = MakeStarRule(func_call_params, ToTerm(","), expression);

            NonTerminal func_return = new NonTerminal("RETORNO")
            {
                Rule = "Retorno" + expression
                | "Retorno"
            };

            #endregion

            #region Variables
            var_dec.Rule = var_type + var_dec_list_names
                | var_type + var_dec_list_names + ToTerm("=") + expression;

            var_type.Rule = ToTerm("Int") | ToTerm("Char") | ToTerm("Bool") | ToTerm("String") | ToTerm("Double");

            var_dec_list_names.Rule = MakePlusRule(var_dec_list_names, ToTerm(","), id_literal);

            var_asign.Rule = id_literal + ToTerm("=") + expression;
            #endregion
            
            #region Expresion
            op.Rule = string_literal
                | number_literal
                | id_literal
                | true_literal
                | false_literal
                | func_call;

            ar.Rule = ar + ToTerm("+") + ar
                | ar + ToTerm("-") + ar
                | ar + ToTerm("*") + ar
                | ar + ToTerm("/") + ar
                | ar + ToTerm("^") + ar
                | ar + ToTerm("~") + ar
                | ar + ToTerm("%") + ar
                | ToTerm("(") + ar + ToTerm(")")
                | ToTerm("-") + ar
                | op;

            ao.Rule = ToTerm("+")
                | ToTerm("-")
                | ToTerm("/")
                | ToTerm("*");

            expression.Rule = expression + bo + expression
                | ToTerm("!") + operand
                | operand ;

            operand.Rule = ar
                | ar + co + ar;

            bo.Rule = ToTerm("&&") | ToTerm("||") | ToTerm("|&");

            co.Rule = ToTerm(">") | ToTerm(">=") | ToTerm("<") | ToTerm("<=") | ToTerm("==") | ToTerm("!=");

            #endregion

            #region print
            print.Rule = ToTerm("Mostrar") + ToTerm("(") + func_call_params + ToTerm(")");
            #endregion

            #region If Else
            if_block.Rule = ToTerm("Si") + "(" + expression + ")" + scope
                | ToTerm("Si") + "(" + expression + ")" + scope + else_dec ; 
            
            else_dec.Rule = ToTerm("Sino") + scope ;
            #endregion

            #region While
            NonTerminal while_block = new NonTerminal("MIENTRAS")
            {
                Rule = "Mientras" + ToTerm("(") + expression + ToTerm(")") + scope
            };
            #endregion

            #region Repetir
            NonTerminal repeat_block = new NonTerminal("REPETIR")
            {
                Rule = ToTerm("Hasta") + ToTerm("(") + expression + ToTerm(")") + scope
            };
            #endregion
            
            #region Para
            NonTerminal for_var_asign = new NonTerminal("VARIABLE")
            {
                Rule = ToTerm("Double") + id_literal + "=" + expression
            };

            NonTerminal for_var_action = new NonTerminal("ACCION_FOR")
            {
                Rule = ToTerm("++") | ToTerm("--")
            };

            NonTerminal for_block = new NonTerminal("PARA")
            {
                Rule = ToTerm("Para") + ToTerm("(") + for_var_asign + ";" + expression + ";" + for_var_action + ToTerm(")") + scope
            };
            #endregion

            #region Break
            NonTerminal break_stmt = new NonTerminal("DETENER")
            {
                Rule = ToTerm("Detener")
            };
            #endregion

            #region Continue
            NonTerminal continue_stmt = new NonTerminal("CONTINUAR")
            {
                Rule = ToTerm("Continuar")
            };
            #endregion

            #region Selecciona
            NonTerminal switch_block = new NonTerminal("SELECCIONA");
            NonTerminal switch_body = new NonTerminal("CASOS");

            NonTerminal case_block = new NonTerminal("CASO");
            NonTerminal def_block = new NonTerminal("DEFECTO");
            NonTerminal case_list = new NonTerminal("LISTA_CASOS");

            switch_block.Rule = "Selecciona" + ToTerm("(") + expression + ToTerm(")") + switch_body;

            case_block.Rule = expression + ToTerm(":") + statements;

            case_list.Rule = MakePlusRule(case_list, case_block);

            switch_body.Rule = ToTerm("{") + case_list + def_block + ToTerm("}")
                   | ToTerm("{") + case_list + ToTerm("}");

            def_block.Rule = "Defecto" + ToTerm(":") + statements;
            #endregion

            #region Statements   
            scope.Rule = ToTerm("{") + statements + ToTerm("}");
            
            statements.Rule = MakeStarRule(statements, statement);

            statement.Rule = var_dec + ";"
                | var_asign + ";"
                | if_block
                | switch_block
                | while_block
                | repeat_block
                | for_block
                | func_call + ";"
                | func_return + ";"
                | print + ";" 
                | continue_stmt + ";"
                | break_stmt + ";";

            statement.ErrorRule = SyntaxError + ";" | SyntaxError + "}";
            #endregion
            #endregion

            #region Preferencias
            MarkReservedWords("Importar", "Definir", "Int", "String", "Bool", "Char", "Double", "Vacio", "Si", "Sino", "true", "false", "Retorno", "Continuar", "Detener", "Mientras", "Para");
        
            MarkTransient(scope, member, member_list, statement, var_type, for_var_action);

            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");

            RegisterOperators(1, Associativity.Left, ToTerm("+"), ToTerm("-"));
            RegisterOperators(2, Associativity.Left, ToTerm("/"), ToTerm("*"), ToTerm("%"));
            RegisterOperators(3, Associativity.Left, ToTerm("^"));
            //comparissons
            RegisterOperators(4, Associativity.Left, ToTerm("<"), ToTerm(">"), ToTerm("<="), ToTerm(">="), ToTerm("~"));
            //booleans
            RegisterOperators(5, Associativity.Left, ToTerm("||"));
            RegisterOperators(6, Associativity.Left, ToTerm("|&"));
            RegisterOperators(7, Associativity.Left, ToTerm("&&"));
            RegisterOperators(8, Associativity.Right, ToTerm("!"));
            RegisterOperators(9, Associativity.Left, ToTerm("("), ToTerm(")"));

            Root = program;
            #endregion
        }
    }  
}