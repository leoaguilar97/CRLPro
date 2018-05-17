using Irony;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace crl.Compiler
{
    public class Logger
    {
        internal static LogMessageList Logs { get; set; }

        internal static List<String> Images { get; set; }

        internal static void AddLog(string message, ParseTreeNode log_node) => Logs?.Add(new LogMessage(ErrorLevel.Info, log_node.FindToken().Location, message, null));

        internal static void AddWarning(string message) => Logs?.Add(new LogMessage(ErrorLevel.Warning, new SourceLocation(), message, null));

        internal static void AddError(ParseTreeNode node, String message)
        {
            Token token = node.FindToken();

            int line = 0;
            int col = 0;

            if (token != null)
            {
                line = token.Location.Line;
                col = token.Location.Column;
            }

            message = "[" + node.FindTokenAndGetText() + "] " + message;

            Logs?.Add(new LogMessage(ErrorLevel.Error, new SourceLocation(0, line, col), message, null));
        }

        internal static string GetLogsAsText(LogMessageList messages)
        {
            StringBuilder sb = new StringBuilder();

            List<ProgramLog> errors = new List<ProgramLog>();

            foreach (LogMessage log in messages)
            {
                int line = log.Location.Line + 1;
                int col = log.Location.Column + 1;

                string message = log.Message;
                string level = log.Level.ToString();

                string type = log.Level == ErrorLevel.Info ? "LOG" : log.Level == ErrorLevel.Warning ? "ADVERTENCIA" : "ERROR";

                if (message.Contains("Syntax error"))
                {
                    level = "Sintactico";
                }

                ProgramLog error = new ProgramLog(line, col, message, type);

                sb.Append(error + Environment.NewLine);                
            }

            return sb.ToString();
        }

        internal static string GetLogsAsHtml() {
            string result = "";

            if (Logs.Count == 0) {
                result = "<h3>No hay datos por mostrar.</h3>";
            }
            else
            {
                foreach (LogMessage log in Logs)
                {
                    result += "log.GetAsHtml()" + "<hr />";
                }
            }
            return "<div>%result</div>".Replace("%result", result);
        }

        internal static void AddImg(string base64Img)
        {
            Images?.Add(base64Img);
        }

        /*
        #region HTML_TABLE
        private string htmlErrorTable = "<style type=\"text/css\"> .tg {border-collapse:collapse;border-spacing:0;margin:0px auto;} .tg td{font-family:Arial, sans-serif;font-size:14px;padding:6px 20px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:black;} .tg th{font-family:Arial, sans-serif;font-size:14px;font-weight:normal;padding:6px 20px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:black;} .tg .tg-7uzy{vertical-align:top} .tg .tg-n385{font-weight:bold;background-color:#ffce93;color:#ffffff;text-align:center;vertical-align:top} .tg .tg-4jb6{background-color:#ffffff;color:#333333;text-align:center;vertical-align:top} .tg .tg-itju{background-color:#fd6864;color:#ffffff;vertical-align:top} .tg .tg-25wf{font-weight:bold;font-family:\"Lucida Console\", Monaco, monospace !important;;background-color:#ffccc9;color:#cb0000;text-align:center;vertical-align:top} .tg .tg-yw4l{vertical-align:top} .tg .tg-z9dd{background-color:#ffffff;color:#333333;text-align:center;vertical-align:top} th.tg-sort-header::-moz-selection { background:transparent; }th.tg-sort-header::selection { background:transparent; }th.tg-sort-header { cursor:pointer; }table th.tg-sort-header:after { content:''; float:right; margin-top:7px; border-width:0 4px 4px; border-style:solid; border-color:#404040 transparent; visibility:hidden; }table th.tg-sort-header:hover:after { visibility:visible; }table th.tg-sort-desc:after,table th.tg-sort-asc:after,table th.tg-sort-asc:hover:after { visibility:visible; opacity:0.4; }table th.tg-sort-desc:after { border-bottom:none; border-width:4px 4px 0; }@media screen and (max-width: 767px) {.tg {width: auto !important;}.tg col {width: auto !important;}.tg-wrap {overflow-x: auto;-webkit-overflow-scrolling: touch;margin: auto 0px;}}</style> <div class=\"tg-wrap\"><table id=\"tg-ATCzm\" class=\"tg\"> <tr> <th class=\"tg-itju\">#</th> <th class=\"tg-25wf\">TIPO</th> <th class=\"tg-25wf\">LINEA</th> <th class=\"tg-25wf\">COLUMNA</th> <th class=\"tg-25wf\">MENSAJE</th> </tr> %rows </table></div>";

        #endregion
        */
    }
}