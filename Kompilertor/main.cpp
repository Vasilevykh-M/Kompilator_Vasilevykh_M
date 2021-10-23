#include<iostream>
#include<vector>
#include<list>
#include<string>
#include<variant>
#include<fstream>
#include<algorithm>
#include<map>


using namespace std;

enum TokenType{ttIdent, ttOperation, ttConst};

enum OperSymbol
{
	Sprogram,
	Svar,
	Sbegin,
	Send,
	Sif,
	Sthen,
	Selse,
	Sint,
	Sbool,
	Sreal,
	Sstring,
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
	none
};



enum conditions{start, identif, operat, constant};

enum KeyWords {Program, Var, Begin, End, Integer, Float, String, If, Else, Then,};

map<string, int> SpecToken = { {"program", Sprogram},
								{"var", Svar},
								{"begin", Sbegin},
								{"end", Send},
								{"if", Sif},
								{"then", Sthen},
								{"else", Selse},
								{"int", Sint},
								{"real", Sreal},
								{"bool", Sbool},
								{"string", Sstring},
								{":=", Sassigment},
								{"<>", Snotequals},
								{">=", Smorequals},
								{"<=", Ssmallequals},
								{"=", Sequals},
								{">", Smore},
								{"<", Ssmall},
								{",", Scomma},
								{":", Ssemicon},
								{".", Spoint},
								{";", Scolon},
								{"+", Sadd},
								{"*", Smult},
								{"/", Sdivision},
								{"-", Ssubtraction},
								{"(", Sbracketr},
								{")", Sbracketl},
								{"const", Sconst},
								{"true", Strue},
								{"false", Sfalse},
								{"div", Sdiv},
								{"mod", Smod}
};

string letters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";
string numbers = "0123456789";

struct error {
	int code_error;
	pair<int, int> XoY;
};

list<error> eror;

map<int, string> errors = { 
	{0, "Запрешённый символ"},
	{1, "Превышен лимит константы"}};

class Variant
{
public:
	virtual std::string GetClassVariant() = 0;
};

class IntVariant : public Variant
{
private:
	int value;
public:
	IntVariant(int val) { value = val; }
	int GetValue() { return value; }
	std::string GetClassVariant() override { return "int"; }
};

class FloatVariant : public Variant
{
private:
	float value;
public:
	FloatVariant(float val) { value = val; }
	float GetValue() { return value; }
	std::string GetClassVariant() override { return "float"; }
};

class BoolVariant : public Variant
{
private:
	bool value;
public:
	BoolVariant(bool val) { value = val; }
	bool GetValue() { return value; }
	std::string GetClassVariant() override { return " bool"; }
};

class StringVariant : public Variant
{
private:
	std::string value;
public:
	StringVariant(std::string val) {value = val; }
	std::string GetValue() { return value; }
	std::string GetClassVariant() override { return "string"; }
};


class CToken
{
public:
	TokenType tt;
	OperSymbol oper;
	Variant* const_val;
	std::string ident="";


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
	string GetTypeToken()
	{
		switch (tt)
		{
		case ttOperation: return "Operation";
		case ttConst: return "Const";
		case ttIdent: return "Identifier";
		}
	}

};


bool NUM(string buf)
{
	bool num = false;
	bool tik = false;
	for (int i = 0; i < buf.size(); i++)
	{

		if (numbers.find(buf[i]) != -1) {
			num = true;
		}
		else {
			if (buf[i] != '.') {
				return false;
			}
		}


	}
	return num;
}


bool IDENT(string buf)
{
	for (int i = 0; i < buf.size(); i++)
	{

		if (numbers.find(buf[i]) == -1 && letters.find(buf[i]) == -1)
			return false;
	}
	return true;
}


bool num(char leks)
{
	return leks >= '0' && '9' >= leks;
}

bool word(char leks)
{
	return leks >= 'a' && 'z' >= leks || leks >= 'A' && 'Z' >= leks;
}

Variant* GetConstInt(string buf)
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

Variant* GetConstFloat(string buf)
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

Variant* GetConstBool(string buf)
{
	bool const_value;

	if (buf == "true")
		const_value = true;

	if (buf == "false")
		const_value = false;

	BoolVariant* a = new BoolVariant(const_value);
	return a;

}

Variant* GetConstString(string buf)
{
	string const_value = "";

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

bool READ(ifstream& fl, char& leks)
{
	fl.get(leks);
	if (!fl)
		return false;
	return true;		
}

string ToString(char leks)
{
	string str = "";
	return str+leks;
}


void GetError(int i, int j, string buf, int code)
{
	error A;
	A.code_error = code;
	pair<int, int> B;
	B.first = i;
	B.second = j;
	A.XoY = B;
	eror.push_back(A);
	cout << "'" << buf << "' " << "Code error : " << A.code_error << " line: " << A.XoY.second << " position: " << A.XoY.first << endl;
}


void GetError(int i, int j, char buf, int code)
{
	error A;
	A.code_error = code;
	pair<int, int> B;
	B.first = i;
	B.second = j;
	A.XoY = B;
	eror.push_back(A);
	cout << "'" << buf << "' " << "Code error : " << A.code_error << " line: " << A.XoY.second << " position: " << A.XoY.first << endl;
}

CToken* IO(string buf, string& last_token, char& leks, ifstream& fl, int &i, int&j)
{
	TokenType token = ttIdent;
	Again:
	if (leks == '\n')
	{
		j++;
	}

	while (leks == '\t' || leks == ' ' || leks == '\n' || leks == '\0') // сббрасываем табы ентеры пустые символы
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


	while (num(leks) || buf!="" && leks =='.') // читаем число
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
		do{

			i++;
			buf += leks;

			if (!READ(fl, leks)) {
				leks = '\0';
				break;
			}
		} while (leks != '\'');

		buf += leks;

		leks = '\0';

		if (buf.size() > 2 || buf =="''")
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


void main()
{
	ifstream r("C:\\Users\\Михаил\\Desktop\\Компилятор\\Kompilertor\\Kompilertor\\file.txt");

	TokenType tt;
	string ident = "", buf = "", lt="";
	int i = 0;
	int j = 1;
	char leks='\0';
	CToken* A;

	do
	{
		A = IO(buf, lt, leks, r, i,j);
		if(A!=NULL)
			cout << A->ident << " " << A->tt << endl;
	} while (A != NULL);

	for (auto egge : eror)
	{
		cout << egge.code_error << " X: " << egge.XoY.first << "; Y: " << egge.XoY.second << endl;
	}
}