using ClientCRL.crl.compiler.symtbl;
using Irony.Parsing;
using System;
using System.Collections.Generic;

namespace crl.Compiler.SymbolTable
{
    public class ScopeMember
    {
        internal ParseTreeNode Root { get; set; }
        internal List<Parameter> Parameters { get; set; }
        internal ParseTreeNode Statements { get; set; }
        
        public object Value { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public bool IsGlobal { get; set; }
        public bool IsVar { get; set; }
        public bool IsFunct { get; set; }

        public double Uncertainty { get; set; }
        public string ImagePath { get; set; }

        public bool IsMain
        {
            get => Type == "Vacio" && Name == "Principal" && (Parameters == null || Parameters.Count == 0);
            set => IsMain = value;
        }
        
        private void GenerateParameterList(ParseTreeNode pars)
        {
            Parameters = new List<Parameter>();

            foreach (var par in pars.ChildNodes)
            {
                if (par.ChildNodes.Count == 2)
                {
                    string type = par.ChildNodes[0].FindTokenAndGetText();
                    string name = par.ChildNodes[1].FindTokenAndGetText();

                    Parameters.Add(new Parameter { Name = name, Type = Parameter.GetType(type) });
                }
            }
        }

        /// <summary>
        /// Crear una funcion
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <param name="statements"></param>
        internal ScopeMember(string type, string name, ParseTreeNode root)
        {
            Type = type;
            Name = name;

            if (root.ChildNodes.Count == 4)
            {
                Root = root;
                //Obtener parametros
                GenerateParameterList(root.ChildNodes[2]);
                //Obtener sentencias
                Statements = root.ChildNodes[3];
            }
            
            IsGlobal = true;
            IsFunct = true;
        }

        /// <summary>
        /// Crea una variable
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ScopeMember(string type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;

            IsVar = true;
        }
    
        public override bool Equals(object obj)
        {
            var value = obj as ScopeMember;
            return value != null &&
                   Type == value.Type &&
                   Name == value.Name;
        }
        
        public override int GetHashCode()
        {
            var hashCode = -1890651077;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public override string ToString()
        {
            return "{tipo=\"" + Type + "\"; name=\"'" + Name + "\"}";
        }

        internal object Raise() {

            if (IsVar) return Value;
            
            if ((Statements == null || Statements.ChildNodes.Count == 0))
            {
                if (Type != "Vacio")
                {
                    Logger.AddWarning(string.Format("No se devolvio ningun valor de tipo [{0}] para la funcion [{1}].", Type, Name));
                }

                return null;
            }
            
            Scope local = new Scope(ScopeType.FUNCTION);

            Parameters?.ForEach((param) => { local.AddVar(param); });

            object returned = null;

            foreach (ParseTreeNode statement in Statements.ChildNodes)
            {
                ControlBlock cb = new ControlBlock(statement, local);            

                if (ControlBlock.Signal == Signal.RETURNING)
                {
                    returned = ControlBlock.ReturnedValue ;

                    ControlBlock.ReturnedValue = null;
                    ControlBlock.Signal = Signal.NORMAL;

                    switch (Type)
                    {
                        case "Vacio":
                            if (returned is null) return null;
                            break;

                        case "Double":
                            if (returned is double) return returned;
                            break;

                        case "Bool":
                            if (returned is bool) return returned;
                            break;

                        case "String":
                            if (returned is string) return returned;
                            break;

                        case "Int":
                            if (returned is double) return Math.Ceiling((double)returned);
                            break;
                    }

                    break;
                }
            }            

            return null;
        }
    }

    public class Parameter
    {
        internal string Name { get; set; }
        internal ParameterType Type { get; set; }
        internal object Data { get; set; }

        internal enum ParameterType
        {
            String,
            Numeric,
            Bool,
            Char,
            Empty
        }

        internal string TypeAsString()
        {
            switch (Type)
            {
                case ParameterType.String: return "String";
                case ParameterType.Bool: return "Bool";
                case ParameterType.Char: return "Char";
                case ParameterType.Numeric:
                    if (Data is int) return "Int";
                    else return "Double";
            }
            return "";
        }

        internal static ParameterType GetType(string type)
        {
            switch (type)
            {
                case "String": return ParameterType.String;
                case "Int":
                case "Double": return ParameterType.Numeric;
                case "Bool": return ParameterType.Bool;
                case "Char": return ParameterType.Char;
            }
            return ParameterType.Empty;
        }
    }
}
