using crl.Compiler;
using System;

namespace ClientCRL.crl.compiler.calc
{
    /// <summary>
    /// Calculadora de operaciones de CRL
    /// </summary>
    class Calculator
    {
        private string GetAsString(object value)
        {
            //Si el valor es un boolean
            if ((value is bool))
            {
                return ((bool)value) ? "1" : "0";
            }

            return value.ToString();
        }

        private double GetAsDouble(object value)
        {
            //Convertor booleano a decimal
            if ((value is bool)) return ((bool)value) ? 1 : 0;

            //Tratar de convertir un char a entero usando el codigo ASCII
            if (value is char) return ((int)value);

            //Tratar de convertir el string a un double
            if (value is string)
            {
                if (double.TryParse(value.ToString(), out double result))
                {
                    return result;
                }         
            }

            //Tratar de convertir desde un numero
            if (value is int || value is double)
            {
                return (double) value;
            }

            Logger.AddWarning(string.Format("El valor [{0}] no es un decimal correcto. Se asignó CERO como valor por defecto.", value));
            return 0;           
        }

        private bool GetAsBool(object value)
        {
            if (value is string)
            {
                String val = value.ToString().ToLower();

                if (val.Equals("true")) return true;
                if (val.Equals("false")) return false;                
            }

            if (value is int || value is double) {
                return (double)value == 0 ? false : true; 
            }

            Logger.AddWarning(string.Format("El valor [{0}] no es un booleano correcto. Se asignó FALSE como valor por defecto.", value));
            return false;
        }
                
        #region Operator +
        /// <summary>
        /// Suma dos numeros, si hay al menos un string, concatena los operandos.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public object Add(object o1, object o2)
        {
            if (o1 is string || o2 is string)
            {
                return GetAsString(o1) + GetAsString(o2);
            }

            return GetAsDouble(o1) + GetAsDouble(o2);
        }
        #endregion

        #region Operador -
        /// <summary>
        /// Sustrae dos numeros
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public double Sub(object o1, object o2)
        {
            return GetAsDouble(o1) - GetAsDouble(o2);
        }

        /// <summary>
        /// Devuelve el negativo de un numero
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public double Sub(object o1)
        {
            return -GetAsDouble(o1);
        }
        #endregion

        #region Operador *
        /// <summary>
        /// Multiplica dos numeros
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public double Mul(object o1, object o2)
        {
            return GetAsDouble(o1) * GetAsDouble(o2);
        }
        #endregion

        #region Operador /
        /// <summary>
        /// Divide dos numeros
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public double Div(object o1, object o2)
        {
            return GetAsDouble(o1) / GetAsDouble(o2);
        }
        #endregion

        #region Operador %
        /// <summary>
        /// Realiza el modulo de dos numeros
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public double Mod(object o1, object o2)
        {
            return GetAsDouble(o1) % GetAsDouble(o2);
        }
        #endregion

        #region Operador ^
        /// <summary>
        /// Realiza la potencia del primer operando al segundo.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public double Pot(object o1, object o2)
        {
            return System.Math.Pow(GetAsDouble(o1), GetAsDouble(o2));
        }
        #endregion

        #region Operador ~
        /// <summary>
        /// Compara si dos valores son parecidos por cierta incerteza, si los valores son strings compara si son iguales en contenido.
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Compare(object o1, object o2)
        {
            if (o1 is double && o2 is double)
            {
                double abs = Math.Abs((double)o1 - (double)o2);

                return abs <= Program.Uncertainty;
            }

            if (o1 is string && o2 is string)
            {
                o1 = o1.ToString().ToLower().Trim();
                o2 = o2.ToString().ToLower().Trim();

                return o1.Equals(o2);
            }

            Logger.AddWarning(string.Format("No se puede utilizar el operador de semejanza para los valores [{0}] ~ [{1}].", o1, o2));

            return false;
        }
        #endregion

        #region Operador > 
        /// <summary>
        /// Compara si el primer valor es mayor que el segundo
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Greater(object o1, object o2)
        {
            if (o1 is string && o2 is string)
            {
                return string.Compare(GetAsString(o1), GetAsString(o2)) == -1;
            }

            return GetAsDouble(o1) > GetAsDouble(o2);
        }
        #endregion

        #region Operador <
        /// <summary>
        /// Compara si el primer valor es menor que el segundo
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Less(object o1, object o2)
        {
            if (o1 is string && o2 is string)
            {
                return string.Compare(GetAsString(o1), GetAsString(o2)) == 1;
            }

            return GetAsDouble(o1) < GetAsDouble(o2);
        }
        #endregion
        
        #region Operador >=
        /// <summary>
        /// Compara si el primer valor es mayor que el segundo
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool GreaterEql(object o1, object o2)
        {
            if (o1 is string && o2 is string)
            {
                return string.Compare(GetAsString(o1), GetAsString(o2)) == -1;
            }

            return GetAsDouble(o1) >= GetAsDouble(o2);
        }

        #endregion

        #region Operador <=
        /// <summary>
        /// Compara si el primer valor es menor o igual que el segundo
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool LessEql(object o1, object o2)
        {
            if (o1 is string && o2 is string)
            {
                return string.Compare(GetAsString(o1), GetAsString(o2)) != -1;
            }

            return GetAsDouble(o1) <= GetAsDouble(o2);
        }

        #endregion
        
        #region Operador ==
        /// <summary>
        /// Compara si dos valores son iguales
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Equal(object o1, object o2)
        {
            bool e = string.Compare(GetAsString(o1), GetAsString(o2)) == 0;
            return e;
        }
        #endregion

        #region Operador !=
        /// <summary>
        /// Compara si dos valores son diferentes
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Different(object o1, object o2)
        {
            return string.Compare(GetAsString(o1), GetAsString(o2)) != 0;
        }
        #endregion
        
        #region Operador &&
        /// <summary>
        /// Devuelve el resultado de operar AND a dos booleanos
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool And(object o1, object o2)
        {
            return GetAsBool(o1) && GetAsBool(o2);
        }
        #endregion

        #region Operador ||
        /// <summary>
        /// Devuelve el resultado e operar OR a dos booleanos
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Or(object o1, object o2)
        {
            return GetAsBool(o1) || GetAsBool(o2);
        }
        #endregion
        
        #region Operador |&
        /// <summary>
        /// Devuelve el resultado de operar XOR a dos booleanos
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Xor(object o1, object o2)
        {
            bool bo1 = GetAsBool(o1);
            bool bo2 = GetAsBool(o2);

            return (bo1 || bo2) && (!bo1 || !bo2);
        }
        #endregion

        #region Operador !
        /// <summary>
        /// Devuelve el negado de un booleano
        /// </summary>
        /// <param name="o1"></param>
        /// <returns></returns>
        public bool Not(object o1)
        {
            return !GetAsBool(o1);
        }
        #endregion
    }
}
