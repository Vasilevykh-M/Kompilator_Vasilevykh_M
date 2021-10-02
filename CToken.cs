using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace IO
{

    enum TokenType { ttIdentifier, ttOperation, ttConst, ttKeyWord }

    enum EOpartoinsSymbols { divSy, mulSy, leftSy, commaSy }

    enum KeyWords { beginSy, varSy, endSy }

    enum Status {zero, identifier, operation, Const, keyword, delimiter} // задел на лексический анализатор



    class CToken
    {
        public TokenType tt;

        public String ident;

        public Tuple<int, int> XY; // кординаты токена

        public static string buf ="";


        public CToken CIO(StreamReader file)
        {
            string A = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_"; // набор символов
            string B = "0123456789"; // набор цифр
            string C = "+-/*()"; // арифметические операции
            List<string> D = new List<string> { ";", ":", "=", ",", ".", ":=", "{", "}" }; // спец операции

            List<string> Keyword = new List<string> { "begin", "var", "end", "program", "if", "else", "then", "for", "while" }; // спец слова
            List<string> ArimfWord = new List<string> { "div", "mod" }; // арифметические слова

            char leks; // считываемый символ
            string rez = ""; // буфер 2.0

            leks = (char)file.Read();

            while(leks =='\n' || leks == '\r' || leks =='\t') // выбрасываем символы перехода табы
                leks = (char)file.Read();

            if (leks == '\uffff') // проверка на конец файла
                if (buf == "" || buf == "\uffff")
                    return null;
                else
                {
                    rez = buf;
                    buf = "";
                    return new CToken { ident = rez, tt = TokenType.ttOperation }; // последний символ
                }

            while (!C.Contains(leks) && !D.Contains(leks+"") && leks != ' ' && // получение набора символов 1 и 2 группы
                (!D.Contains(buf) && !C.Contains(buf) && buf!="") || (buf==""))
            {
                buf += leks;
                if (Keyword.Contains(buf) || ArimfWord.Contains(buf))
                {
                    leks = (char)file.Read();
                    break;
                }
                leks = (char)file.Read();
            }

            if (Keyword.Contains(buf))
            {
                while (leks == '\n' || leks == '\r' || leks =='\t')
                    leks = (char)file.Read();

                rez = buf;
                if (leks == ' ')
                {
                    buf = "";
                }
                else
                {
                    buf = "" + leks;
                }
                return new CToken { ident = rez, tt = TokenType.ttKeyWord };
            }

            if (ArimfWord.Contains(buf))
            {
                rez = buf;
                if (leks == ' ')
                {
                    buf = "";
                }
                else
                {
                    buf = "" + leks;
                }
                return new CToken { ident = rez, tt = TokenType.ttOperation };
            }

            while (D.Contains(leks+"") &&
                D.Contains(buf) && leks != ' ') // получениее набора символов 4 группы
            {
                buf += leks;
                leks = (char)file.Read();
            }


            rez = buf;

            if(leks == ' ')
            {
                buf = "";
            }
            else
            {
                buf = "" + leks;
            }

            if (C.Contains(rez))
            {
                return new CToken { ident = rez, tt = TokenType.ttOperation };
            }

            if(D.Contains(rez))
            {
                return new CToken { ident = rez, tt = TokenType.ttOperation };
            }

            for (int i=0;i<rez.Length;i++)
            {
                if(A.Contains(rez[i]))
                {
                    return new CToken { ident = rez, tt = TokenType.ttIdentifier };
                }
            }

            return new CToken { ident = rez, tt = TokenType.ttConst };
        }
    }
}
