#pragma once
#include"CIO_LEKS.h"
#include"Resorse_comp.h"
#include<fstream>
#include<string>
#include<exception>

int i = 0, j = 0;
std::ifstream r("C:\\Users\\Михаил\\Desktop\\Компилятор\\Kompilertor\\Kompilertor\\file.txt");
std::string buf = "", lt = "";
char leks = '\0';

class EXPR
{
public:
	CToken* curToken;

	void GetNextToken()
	{
		curToken = IO(buf, lt, leks, r, i, j);
	}
	bool PROGRAM()
	{
		GetNextToken();
		if (curToken->ident == "program")
		{
			GetNextToken();
			if (curToken->tt == ttIdent)//пофиксить что бы ебануть имя
			{
				GetNextToken();
				if (curToken->ident == ";")
				{
					GetNextToken();
					if (BLOC()) {
						//GetNextToken();
						if (curToken->ident == ".")
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	bool BLOC()
	{
		bool a = CONST_AREA();
		bool b = VAR_AREA();
		bool c = OPERATOR_AREA();
		return a && b && c;
	}


	bool CONST_AREA()
	{
		if (curToken->ident == "const")
		{
			if (CONST_DETER())
			{
				GetNextToken();
				if (curToken->ident == ";")
				{
					GetNextToken();
					while (CONST_DETER())
					{
						GetNextToken();
						if (curToken->ident != ";")
						{
							return false;
						}
						GetNextToken();
					}
					GetNextToken();
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	bool CONST_DETER()
	{
		if (curToken->tt == ttIdent)
		{
			GetNextToken();
			if (curToken->ident == "=")
			{
				GetNextToken();
				if (CONST())
				{
					return true;
				}
			}
		}
		return false;
	}




	bool CONST()
	{
		if (NUM_NOT_SIGN()) // число
		{
			return true;
		}

		if (curToken->tt == ttConst) // строка
		{
			return true;
		}
		
		if (curToken->ident == "-")
		{
			GetNextToken();
			if (NUM_NOT_SIGN()) // число
			{
				return true;
			}

			if (curToken->tt == ttIdent) // имя константы
			{
				return true;
			}
		}

		if (curToken->tt == ttIdent) // имя константы
		{
			return true;
		}

		return false;
	}
	bool NUM_NOT_SIGN()
	{
		if (curToken->tt == ttConst) // число
		{
			return true;
		}
	}

	bool VAR_AREA()
	{
		if (curToken->ident == "var")
		{
			GetNextToken();
			if (VAR())
			{
				GetNextToken();
				if (curToken->ident == ";")
				{
					GetNextToken();
					while (VAR())
					{
						GetNextToken();
						if (curToken->ident != ";")
						{
							return false;
						}
					}
					GetNextToken();
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
	}
	return true;
}

	bool VAR()
	{
		if (curToken->tt == ttIdent)
		{
			GetNextToken();
			if (curToken->ident == ",")
			{
				GetNextToken();
				while(curToken->tt == ttIdent)
				{
					GetNextToken();
					if (curToken->ident != ",")
						break;
					GetNextToken();
				}
			}

			if (curToken->ident == ":")
			{
				GetNextToken();
				if (curToken->tt == ttIdent)
				{
					return true;
				}
			}
		}
		return false;
	}

	bool OPERATOR_AREA() // раздел операторов		*
	{
		return OPERATOR();//		*
	}

	bool OPERATOR() // <оператор>::=<простой оператор>|<сложный оператор>
	{
		if (DIF_OPERATOR() || SIMPLE_OPERATOR()) // простой оператор | сложный оператор		*		*
		{
			return true;
		}
		return false;
	}

	bool SIMPLE_OPERATOR() // < простой оператор > :: = <оператор присваивания> | <пустой оператор>
	{
		if (ASSIGMENT_OPERATOR())//		*
		{
			return true;
		}
		return true; //?
	}

	bool ASSIGMENT_OPERATOR() // < оператор присваивания > :: = <имя> := < выражение >		*
	{
		if (curToken->tt == ttIdent) // по идее мы можем цепануть и тип
		{
			GetNextToken();
			if (curToken->ident == ":=") // :=
			{
				GetNextToken();
				if (EXPRESSION()) // выражение		*
				{
					return true; // всё тип топ это присванивание
				}
			}
		}
		return false; //если не прошли во внутрь значит точно не это
	}

	bool EXPRESSION() // <выражение>::=<простое выражение>|<простое выражение><операция отношения><простое выражение>		*
	{
		if (SIMPLE_EXPESSION()) // простое выражение		*
		{
			if (RELAT_EXPRESSION()) // операция отношения		*
			{
				GetNextToken();
				if (SIMPLE_EXPESSION())// простое выражение			*
				{
					return true;
				}
				return false; // если не попали в операцию
			}
			return true; // если пришли сюда то первая ветка
		}
		return false;
	}

	bool SIMPLE_EXPESSION() // <простое выражение>::=<знак><слагаемое>{<аддитивная операция><слагаемое>}	*
	{
		if (curToken->ident == "-")
		{
			GetNextToken();
		}

		if (TERM())//	*
		{
			//GetNextToken();
			while (ADD_EXPRESSION())//		*
			{
				GetNextToken();
				if (!TERM())//		*
					return false;
				//GetNextToken();
			}
		}
		return true;
	}

	bool RELAT_EXPRESSION() // <операция отношения>::==|<>|<|<=|>=|>|in		*
	{
		if (curToken->ident == "=" || curToken->ident == "<>" || curToken->ident == "<" || curToken->ident == ">" || curToken->ident == "<=" ||
			curToken->ident == ">=" || curToken->ident == "in") // переделаю в switch
		{
			return true;
		}
		return false;
	}

	bool ADD_EXPRESSION() // <аддитивная операция>::=+|-|or		*
	{
		if (curToken->ident == "+" || curToken->ident == "-" || curToken->ident == "or")
		{
			return true;
		}
		return false;
	}

	bool MULT_EXPRESSION() // <мультипликативная операция>::=*|/|div|mod|and	*
	{
		if (curToken->ident == "*" || curToken->ident == "/" || curToken->ident == "div" || curToken->ident == "mod" || curToken->ident == "and")
		{
			return true;
		}
		return false;
	}

	bool TERM() // <слагаемое>::=<множитель>{<мультипликативная операция><множитель>}	 *
	{
		if (FACTOR())
		{
			//GetNextToken();
			while (MULT_EXPRESSION())
			{
				GetNextToken();
				if (!FACTOR())
					return false;
				GetNextToken();
			}
			return true;
		}
		return false;
	}

	bool FACTOR() // <множитель>::=<имя>|<константа без знака>|(<выражение>)	*
	{
		if (curToken->tt == ttIdent)
		{
			GetNextToken();
			return true;
		}

		if (NUM_NOT_SIGN())
		{
			GetNextToken();
			return true;
		}

		if (EXPRESSION())
		{
			return true;
		}
		GetNextToken();
		return false;
	}

	bool DIF_OPERATOR() // <сложный оператор>::=<составной оператор>|<выбирающий оператор>|<оператор цикла>		*
	{
		// такой странный вид реализован что бы лишний раз не проверять условие
		if (COMPOSITE_OPERATOR()) //		*
			return true;
		if (CHOICE_OPERATOR())//		*
			return true;
		if (CYCLE_OPERATOR())//		*
			return true;
		return false;
	}

	bool COMPOSITE_OPERATOR() // <составной оператор>::= begin <оператор>{;<оператор>} end 		*
	{
		if (curToken->ident == "begin")
		{
			GetNextToken();
			if (!OPERATOR())//		*
				return false;
			while (curToken->ident == ";")
			{
				GetNextToken();
				if (!OPERATOR()) //		*
					break;
			}

			if (curToken->ident == "end") {
				GetNextToken();
				return true;
			}
			else
			{
				GetNextToken();
			}
		}
		return false;
	}

	bool CHOICE_OPERATOR() // <выбирающий оператор>::= if <выражение> then <оператор>|if <выражение> then <оператор> else <оператор>	*
	{
		if (curToken->ident == "if")
		{
			GetNextToken();
			if (EXPRESSION()) //	*
			{
				if (curToken->ident == "then")
				{
					GetNextToken();
					if (OPERATOR())//		*
					{
						if (curToken->ident == "else")
						{
							GetNextToken();
							if(OPERATOR())
								return true;
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	bool CYCLE_OPERATOR() // <оператор цикла>::= while <выражение> do <оператор>
	{
		if (curToken->ident == "while")
		{
			GetNextToken();
			if (EXPRESSION())
			{
				if (curToken->ident == "do")
				{
					GetNextToken();
					if (OPERATOR())
					{
						return true;
					}
				}
			}
		}
		return false;
	}

};