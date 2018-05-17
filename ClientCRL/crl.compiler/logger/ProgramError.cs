namespace crl.Compiler
{
    /// <summary>
    /// Crea un log del programa
    /// </summary>
    internal class ProgramLog
    {
        internal int Line { get; private set; }
        internal int Col { get; private set; }
        internal string Message { get; private set; }
        internal string Type { get; private set; }

        /// <summary>
        /// Crea un Log
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public ProgramLog(int line, int col, string message, string type)
        {
            Line = line;
            Col = col;
            Message = message;
            Type = type;
        }

        /// <summary>
        /// Devuelve la representacion en string del log
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Format("Tipo {0} Linea: {1} Columna: {2} -> {3}", Type, Line, Col, Message);
    }
}