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
						GetNextToken();
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
		return CONST_AREA() && VAR_AREA();
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
				}
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
					while (VAR())
					{
						GetNextToken();
						if (curToken->ident != ";")
						{
							return false;
						}
					}
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

	bool OPERATOR_AREA()
	{
		return OPERATOR();
	}

	bool OPERATOR()
	{
		if (SIMPLE_OPERATOR() || DIF_OPERATOR())
		{
			return true;
		}
		return false;
	}

	bool SIMPLE_OPERATOR()
	{
		if (ASSIGMENT_OPERATOR() || curToken == NULL)
		{
			return true;
		}
		return false;
	}

	bool ASSIGMENT_OPERATOR()
	{
		if (curToken->tt == ttIdent)
		{
			GetNextToken();
			if (curToken->ident == ":=")
			{
				GetNextToken();
				if (EXPRESSION())
				{
					return true;
				}
			}
		}
		return false;
	}

	bool EXPRESSION()
	{
		if (SIMPLE_EXPESSION())
		{
			GetNextToken();
			if (RELAT_EXPRESSION())
			{
				GetNextToken();
				if (SIMPLE_EXPESSION())
					return true;
				return false;
			}
			return true;
		}
		return false;
	}

	bool SIMPLE_EXPESSION()
	{
		if (curToken->ident == "-")
		{
			GetNextToken();
		}

		if (TERM())
		{
			GetNextToken();
			while (ADD_EXPRESSION())
			{
				GetNextToken();
				if (!TERM())
					return false;
				GetNextToken();
			}
		}
		return true;
	}

	bool RELAT_EXPRESSION()
	{
		if (curToken->ident == "=" || curToken->ident == "<>" || curToken->ident == "<" || curToken->ident == ">" || curToken->ident == "<=" ||
			curToken->ident == ">=" || curToken->ident == "in") // переделаю в switch
		{
			return true;
		}
		return false;
	}

	bool ADD_EXPRESSION()
	{
		if (curToken->ident == "+" || curToken->ident == "-" || curToken->ident == "or")
		{
			return true;
		}
		return false;
	}

	bool MULT_EXPRESSION()
	{
		if (curToken->ident == "*" || curToken->ident == "/" || curToken->ident == "div" || curToken->ident == "mod" || curToken->ident == "and")
		{
			return true;
		}
		return false;
	}

	bool TERM()
	{
		if (FACTOR())
		{
			GetNextToken();
			while (MULT_EXPRESSION())
			{
				GetNextToken();
				if (!FACTOR())
					return false;
				GetNextToken();
			}
		}
	}

	bool FACTOR()
	{
		if (curToken->tt == ttIdent)
		{
			return true;
		}

		if (NUM_NOT_SIGN())
		{
			return true;
		}

		if (EXPRESSION())
		{
			return true;
		}
	}

	bool DIF_OPERATOR()
	{
		if (COMPOSITE_OPERATOR() || CHOICE_OPERATOR() || CYCLE_OPERATOR())
		{
			return true;
		}
		return false;
	}

	bool COMPOSITE_OPERATOR()
	{
		if (curToken->ident == "begin")
		{
			GetNextToken();
			if (!OPERATOR())
				return false;
			GetNextToken();
			while (curToken->ident == ";")
			{
				GetNextToken();
				if (!OPERATOR())
					return false;
				GetNextToken();
			}

			if(curToken->ident == "end")
				return true;
		}
		return false;
	}

	bool CHOICE_OPERATOR()
	{
		if (curToken->ident == "if")
		{
			GetNextToken();
			if (EXPRESSION())
			{
				GetNextToken();
				if (curToken->ident == "then")
				{
					GetNextToken();
					if (OPERATOR())
					{
						GetNextToken();
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

	bool CYCLE_OPERATOR()
	{
		if (curToken->ident == "while")
		{
			GetNextToken();
			if (EXPRESSION())
			{
				GetNextToken();
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