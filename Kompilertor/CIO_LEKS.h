#pragma once
#include<string>
#include"Resorse_comp.h"

//std::string letters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";
//std::string numbers = "0123456789";
//enum TokenType { ttIdent, ttOperation, ttConst };
class CToken
{
public:
	TokenType tt;
	OperSymbol oper;
	Variant* const_val = NULL;
	std::string ident = "";


	CToken(TokenType _tt, OperSymbol _op, std::string val) // операция
	{
		tt = _tt;
		oper = _op;
		const_val = nullptr;
		ident = val;
	}

	CToken(TokenType _tt, Variant* _constVal, std::string val)
	{
		tt = _tt;
		oper = none;
		const_val = _constVal;
		ident = val;
	}

	CToken(TokenType _tt, std::string val) //идентификатор
	{
		tt = _tt;
		oper = none;
		const_val = nullptr;
		ident = val;
	}

	~CToken() {
		delete const_val;
	}
	std::string GetTypeToken()
	{
		switch (tt)
		{
		case ttOperation: return "Operation";
		case ttConst: return "Const";
		case ttIdent: return "Identifier";
		}
	}

};



Variant* GetConstInt(std::string buf)
{
	int i = 0;
	int const_value = 0;
	while (buf[i] != '.' && i < buf.size())
	{
		const_value *= 10;
		const_value += (buf[i] - '0');
		i++;
	}
	IntVariant* a = new IntVariant(const_value);
	return a;
}

Variant* GetConstFloat(std::string buf)
{
	int i = 0;
	float const_value = 0;
	while (buf[i] != '.' && i < buf.size())
	{
		const_value *= 10;
		const_value += (buf[i] - '0');
		i++;
	}
	i++;
	int dr_val = 0;
	while (i < buf.size())
	{
		dr_val += (buf[i] - '0');
		dr_val /= 10;
		i++;
	}
	FloatVariant* a = new FloatVariant(const_value);

	return a;
}

Variant* GetConstBool(std::string buf)
{
	bool const_value;

	if (buf == "true")
		const_value = true;

	if (buf == "false")
		const_value = false;

	BoolVariant* a = new BoolVariant(const_value);
	return a;

}

Variant* GetConstString(std::string buf)
{
	std::string const_value = "";

	for (int i = 0; i < buf.size(); i++)
	{
		if (buf[i] != '\'')
		{
			const_value += buf[i];
		}
	}

	StringVariant* a = new StringVariant(const_value);

	return a;
}

bool READ(std::ifstream& fl, char& leks)
{
	fl.get(leks);
	if (!fl)
		return false;
	return true;
}

std::string ToString(char leks)
{
	std::string str = "";
	return str + leks;
}

bool num(char leks)
{
	return leks >= '0' && '9' >= leks;
}

bool word(char leks)
{
	return leks >= 'a' && 'z' >= leks || leks >= 'A' && 'Z' >= leks;
}


bool NUM(std::string buf)
{
	bool tik = false;
	for (int i = 0; i < buf.size();i++)
	{
		if(!num(buf[i]))
		{
			if (buf[i] == '.')
			{
				if (!tik)
					tik=!tik;
				return false;
			}
			else
			{
				return false;
			}
		}
	}
	return true;
}



void GetError(int i, int j, std::string buf, int code)
{
	error A;
	A.code_error = code;
	std::pair<int, int> B;
	B.first = i;
	B.second = j;
	A.XoY = B;
	//eror.push_back(A);
	std::cout << "'" << buf << "' " << "Code error : " << A.code_error << " line: " << A.XoY.second << " position: " << A.XoY.first << std::endl;
}


void GetError(int i, int j, char buf, int code)
{
	error A;
	A.code_error = code;
	std::pair<int, int> B;
	B.first = i;
	B.second = j;
	A.XoY = B;
	//eror.push_back(A);
	std::cout << "'" << buf << "' " << "Code error : " << A.code_error << " line: " << A.XoY.second << " position: " << A.XoY.first << std::endl;
}

CToken* IO(std::string buf, std::string& last_token, char& leks, std::ifstream& fl, int& i, int& j)
{
	TokenType token = ttIdent;
Again:
	if (leks == '\n')
	{
		j++;
	}

	while (leks == '\t' || leks == ' ' || leks == '\n' || leks == '\0') // сббрасываем табы энтеры пустые символы
	{
		if (leks == '\t')
			i += 4;
		else
			i++;

		if (leks == '\n')
			j++;

		if (!READ(fl, leks))
			return NULL;
	}


	while (num(leks) || word(leks) && buf!="" || buf != "" && leks == '.') // читаем число
	{
		token = ttConst;
		buf += leks;
		if (!READ(fl, leks))
		{
			leks = '\0';
			break;
		}

		if (leks == '\t')
			i += 4;
		else
			i++;

		if (leks == '\n')
			j++;
	}


	if (buf!="" && !NUM(buf))
	{
		bool flag = false;
		if (num(buf[0]))
		{
			for (int x = 0; x < buf.size(); x++)
			{
				if (word(buf[x]))
				{
					GetError(i, j, buf, 2);
					buf = "";
					goto Again;
				}
				if (buf[x] == '.')
				{
					if (flag == true) {
						GetError(i, j, buf, 3);
						buf = "";
						goto Again;
					}
					flag = !flag;
				}
			}
		}
	}


	if (buf != "" && NUM(buf)) // сбрасываем токен числа
	{
		if (buf.find('\.') != -1)
		{
			return new CToken(ttConst, GetConstInt(buf), buf);
		}

		return new CToken(ttConst, GetConstFloat(buf), buf);
	}

	if (leks == '\"') { // строковая константа

		token = ttConst;

		do
		{
			i++;
			buf += leks;

			if (!READ(fl, leks)) {
				leks = '\0';
				break;
			}
		} while (leks != '\"');

		if (leks == '\0')
		{
			GetError(i, j, buf, 1);
			return NULL;

		}
		buf += leks;

		leks = '\0';

		return new CToken(token, GetConstString(buf), buf);
	}


	if (leks == '\'') {  // символьная константа
		token == ttConst;
		do {

			i++;
			buf += leks;

			if (!READ(fl, leks)) {
				leks = '\0';
				break;
			}
		} while (leks != '\'');

		buf += leks;

		leks = '\0';

		if (buf.size() > 2 || buf == "''")
		{
			GetError(i, j, buf, 1);

			if (!READ(fl, leks))
			{
				return NULL;
			}

			buf = "";

			goto Again;
		}

		return new CToken(token, GetConstString(buf), buf);

	}


	while (word(leks) || num(leks) || leks == '_') // читаем слово
	{
		token = ttIdent;
		buf += leks;
		if (!READ(fl, leks)) {
			leks = '\0';
			break;
		}

		if (leks == '\t')
			i += 4;
		else
			i++;

		if (leks == '\n')
			j++;

	}

	if (buf == "bool" || buf == "false")
	{
		token = ttConst;
		return new CToken(ttConst, GetConstBool(buf), buf);
	}

	if (SpecToken.find(buf) != SpecToken.end())
	{
		token = ttOperation;
		return new CToken(token, buf);
	}
	if (buf != "") {
		return new CToken(ttIdent, buf);
	}


	while (SpecToken.find(ToString(leks)) != SpecToken.end() && leks != '\t' && leks != '\0' && leks != '\n' && leks != '\'')
	{
		token = ttOperation;
		buf += leks;
		if (!READ(fl, leks)) {
			leks = '\0';
			break;
		}

		if (leks == '\t')
			i += 4;
		else
			i++;

		if (leks == '\n')
			j++;

	}

	if (buf != "")
	{
		return new CToken(token, buf);
	}

	GetError(i, j, leks, 0);

	if (!READ(fl, leks))
	{
		return NULL;
	}

	goto Again;

}