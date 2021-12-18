using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using System.Text;
using System.Threading.Tasks;

namespace Комп
{
	public class errors
	{
		public int X;
		public int Y;
		public int eror_code;

		public void output()
		{
			Console.WriteLine(error[eror_code] + " code: " + eror_code.ToString());
		}
		public errors(int X, int Y, int eror_code)
		{
			this.X = X;
			this.Y = Y;
			this.eror_code = eror_code;
			output();
		}

		public errors(int eror_code)
		{
			this.eror_code = eror_code;
			output();
		}

		Dictionary<int, string> error = new Dictionary<int, string> {
			{0, "неликвидный символ"},
			{1, "идентификатор начинающийся с числа"},
			{2, "строка без конца"},
			{3, "нет обязательного спец слова"},
			{4, "нет идентификатора"},
			{5, "нет константы"},
			{6, "блок коментария не закрыт" },
			{7,"Несоотвествие типов" }
		};
	}

	public enum TokenType { ttIdent, ttOperation, ttConst }
	public enum OperSymbol
	{
		Sprogram,
		Svar,
		Sbegin,
		Send,
		Sif,
		Sthen,
		Selse,
		Sassigment, // :=
		Sequals, // =
		Snotequals, // <>
		Smorequals, // >=
		Ssmallequals, // <=
		Smore, // >
		Ssmall, // <
		Spoint, //.
		Scomma, //,
		Ssemicon, //;
		Scolon, //:
		Sbracketr, //(
		Sbracketl, //)
		Sqmark, //'
		Sadd, // +
		Ssubtraction, // -
		Smult, // *
		Sdivision, // /
		Smod,
		Sdiv,
		Sconst,
		Strue,
		Sfalse,
		Sin,
		Sor,
		Sand,
		Swhile,
		Sdo,
		Swrite,
		none
	}
	abstract public class CToken
	{
		public TokenType tt;

		public CToken(TokenType _tt)
		{
			tt = _tt;
		}
	}

	public class C_IdentToken : CToken // идентификатор
	{
		public string ident = "";
		public C_IdentToken(string str, TokenType tt) : base(tt)
		{
			ident = str;
		}
	}

	public class C_OperToken : CToken // операция
	{
		public OperSymbol symb;
		public C_OperToken(OperSymbol a, TokenType tt) : base(tt)
		{
			symb = a;
		}
	}
	/**/
	public class CVariant : CToken // константа
	{
		public CVariant() : base(TokenType.ttConst) { }
	}

	public class IntVariant : CVariant // целочисленная константа
	{
		public int value;
		public IntVariant(int val) : base() { value = val; }
		public int GetValue() { return value; }
	}

	public class FloatVariant : CVariant // вещественная константа
	{
		public float value;
		public FloatVariant(float val) : base() { value = val; }
		public float GetValue() { return value; }
	};

	public class BoolVariant : CVariant // булевая константа
	{
		public bool value;
		public BoolVariant(bool val) : base() { value = val; }
		public bool GetValue() { return value; }
	}

	public class StringVariant : CVariant // строковая константа
	{
		public string value;
		public StringVariant(string val) : base() { value = val; }
		public string GetValue() { return value; }
	}

	public class CharVariant : CVariant // строковая константа
	{
		public char value;
		public CharVariant(char val) : base() { value = val; }
		public char GetValue() { return value; }
	};

	public class IO_leks
	{

		int i = 0;
		int j = 0;
		char leks;
		public string buf = "";
		public StreamReader fl;
		public List<errors> C_ERR = new List<errors>();


		public Dictionary<string, OperSymbol> SpecToken = new Dictionary<string, OperSymbol> {
								{"program", OperSymbol.Sprogram},
								{"var", OperSymbol.Svar},
								{"begin", OperSymbol.Sbegin},
								{"end", OperSymbol.Send},
								{"if", OperSymbol.Sif},
								{"then", OperSymbol.Sthen},
								{"else", OperSymbol.Selse},
								{":=", OperSymbol.Sassigment},
								{"<>", OperSymbol.Snotequals},
								{">=", OperSymbol.Smorequals},
								{"<=", OperSymbol.Ssmallequals},
								{"=", OperSymbol.Sequals},
								{">", OperSymbol.Smore},
								{"<", OperSymbol.Ssmall},
								{",", OperSymbol.Scomma},
								{";", OperSymbol.Ssemicon},
								{".", OperSymbol.Spoint},
								{":", OperSymbol.Scolon},
								{"+", OperSymbol.Sadd},
								{"*", OperSymbol.Smult},
								{"/", OperSymbol.Sdivision},
								{"-", OperSymbol.Ssubtraction},
								{"(", OperSymbol.Sbracketr},
								{")", OperSymbol.Sbracketl},
								{"const", OperSymbol.Sconst},
								{"true", OperSymbol.Strue},
								{"false", OperSymbol.Sfalse},
								{"div", OperSymbol.Sdiv},
								{"mod", OperSymbol.Smod},
								{"in", OperSymbol.Sin },
								{"or", OperSymbol.Sor },
								{"and", OperSymbol.Sand },
								{"while", OperSymbol.Swhile },
								{"do", OperSymbol.Sdo },
								{"writeln", OperSymbol.Swrite }
		};

		public int GetConstInt(string buf)
		{
			int i = 0;
			int const_value = 0;
			while (i < buf.Length)
			{
				const_value *= 10;
				const_value += (buf[i] - '0');
				i++;
			}
			return const_value;
		}

		public float GetConstFloat(string buf)
		{
			int i = 0;
			float const_value = 0;
			while (i < buf.Length && buf[i] != '.')
			{
				const_value *= 10;
				const_value += (buf[i] - '0');
				i++;
			}
			i++;
			float dr_val = 0;
			while (i < buf.Length)
			{
				dr_val += (buf[i] - '0');
				dr_val /= 10;
				i++;
			}
			const_value += dr_val;
			return const_value;
		}

		public bool GetConstBool(string buf)
		{
			if (buf == "true")
				return true;

			return false;

		}

		public string GetConstString(string buf)
		{
			string const_value = "";

			for (int i = 0; i < buf.Length; i++)
			{
				if (buf[i] != '\"')
				{
					const_value += buf[i];
				}
			}

			return const_value;
		}

		public char GetConstChar(string buf)
		{
			char const_value = '\0';

			for (int i = 0; i < 3; i++)
			{
				if (buf[i] != '\'')
				{
					const_value = buf[i];
				}
			}

			return const_value;
		}

		public bool num(char leks)
		{
			return leks >= '0' && '9' >= leks;
		}

		public bool word(char leks)
		{
			return leks >= 'a' && 'z' >= leks || leks >= 'A' && 'Z' >= leks;
		}

		public bool number(string buf)
		{
			int count_point = 0;
			for (int i = 0; i < buf.Length; i++)
			{
				if (buf[i] == '.')
					count_point++;
				if (word(buf[i]) && i > 0)
					return false;
			}
			if (count_point > 1)
			{
				return false;
			}
			return true;
		}



		public CToken CIO()
		{
			buf = "";

			if (leks == '{') /*нетрализация коментариев*/
			{
				while (leks != '}')
				{
					leks = (char)fl.Read();
					i++;
					if (leks == '\uffff')
					{
						break;
					}
				}
				leks = (char)fl.Read();
			}

			while (leks == '\t' || leks == '\r' || leks == ' ' || leks == '\0' || leks == '\n')
			{
				if (leks == '\t')
					i += 4;
				if (leks == '\n')
				{
					i = 0;
					j += 1;
				}
				if (leks == ' ')
					i++;
				leks = (char)fl.Read();
			}


			if (leks == '\uffff')
			{
				C_ERR.Add(new errors(6));
				throw new Exception("Error");
			}

			/*число*/
			while (num(leks) || (word(leks) || leks == '.') && buf != "")
			{
				buf += leks;
				leks = (char)fl.Read();
			}


			if (buf != "")
			{
				if (number(buf))
				{
					//leks = '\0';
					if (buf.Contains('.'))
					{
						return new FloatVariant(GetConstFloat(buf));
					}
					else
					{
						return new IntVariant(GetConstInt(buf));
					}
				}
				else
				{
					C_ERR.Add(new errors(1));
					Console.WriteLine(buf);
					throw new Exception("Error");
				}
			}

			/*строка*/
			if (leks == '\"')
			{
				do
				{
					i++;
					buf += leks;
					leks = (char)fl.Read();
					if (leks == '\uffff')
						break;

				} while (leks != '\"');

				if (leks == '\uffff')
				{
					C_ERR.Add(new errors(2)); ;
					throw new Exception("Error");
				}
				buf += leks;

				leks = '\0';
				return new StringVariant(GetConstString(buf));
			}

			/*	:=	или	:	*/
			if (leks == ':')
			{
				buf += leks;
				leks = (char)fl.Read();
				if (leks == '=')
				{
					buf += leks;
					leks = '\0';
				}

				return new C_OperToken(SpecToken[buf], TokenType.ttOperation);
			}
			/*	>=	или	>	*/
			if (leks == '>')
			{
				buf += leks;
				leks = (char)fl.Read();
				if (leks == '=')
				{
					buf += leks;
					leks = '\0';
				}
				return new C_OperToken(SpecToken[buf], TokenType.ttOperation);
			}

			/*	<=	или	<	или	 <>	*/
			if (leks == '<')
			{
				buf += leks;
				leks = (char)fl.Read();
				if (leks == '=' || leks == '>')
				{
					buf += leks;
					leks = '\0';
				}
				return new C_OperToken(SpecToken[buf], TokenType.ttOperation);
			}

			if (SpecToken.ContainsKey(leks.ToString()))
			{
				buf += leks;
				leks = '\0';
				return new C_OperToken(SpecToken[buf], TokenType.ttOperation);
			}

			while (word(leks) || num(leks) || leks == '_') // читаем слово
			{
				buf += leks;
				leks = (char)fl.Read();
			}

			if (SpecToken.ContainsKey(buf) && buf != "")
			{
				//leks = '\0';
				return new C_OperToken(SpecToken[buf], TokenType.ttOperation);
			}

			if (buf == "bool" || buf == "false")
			{
				return new BoolVariant(GetConstBool(buf));
			}


			if (buf != "")
			{
				return new C_IdentToken(buf, TokenType.ttIdent);
			}

			Console.WriteLine(leks);
			C_ERR.Add(new errors(0));
			leks = '\0';
			throw new Exception("Error");

		}

	}
}
