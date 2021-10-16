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

enum EOperationSymbol {divSy, mulSy, leftSy, commaSy};

enum conditions{start, identif, operat, constant};

enum KeyWords {Program, Var, Begin, End, Integer, Float, String, If, Else, Then,};

map<string, int> SpecToken = { {"program", 0},
								{"var", 1},
								{"begin", 2},
								{"end", 3},
								{"integer", 4},
								{"float", 5},
								{"string", 6},
								{"if", 7},
								{"else", 8},
								{"then", 9},
								{":=", 10},
								{";", 11},
								{":", 12},
								{"<>", 13},
								{"<=", 14},
								{"=>", 15},
								{".", 16},
								{">", 17},
								{"<", 18},
								{"div", 19},
								{"mod", 20},
								{"*", 21},
								{"+", 22},
								{"/", 23},
								{"-", 24},
								{",", 25},
								{"(", 26},
								{")", 27},
								{"const", 28},
								{"bool", 29},
								{"true", 30},
								{"false", 31},
								{"char", 32},
								{"=", 33}
};

string letters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";
string numbers = "0123456789";


class CToken
{
public:
	TokenType tt;
	string ident = "";
	pair<int, int> XY;


	CToken(TokenType a, string c, pair<int, int> d)
	{
		this->tt = a;
		this->ident = c;
		this->XY = d;
	}
};




void read(string &str, TokenType& token, string& ident, string& buf, int& y)
{
	char leks;
	leks = str[0];
	str.erase(0, 1);
	y++;

	while (leks == '\t')
	{
		leks = str[0];
		str.erase(0, 1);
	}

	bool state = true, flag = true;

	for (int i = 0; i < buf.size(); i++) {

		if (buf != "" && (letters.find(buf[i]) == -1 && numbers.find(buf[i]) == -1)) {
			flag = false;
			break;
		}
	}

	while ((letters.find(leks) != -1 || numbers.find(leks) != -1)
		&& leks != ' ' && (flag == true || buf == ""))
	{
		buf += leks;
		y++;
		leks = str[0];

		state = false;

		if (str.size() > 0)
			str.erase(0, 1);
		else
			break;
	}

	flag == true;

	for (int i = 0; i < buf.size(); i++) {

		if (buf != "" && (letters.find(buf[i]) == -1 && numbers.find(buf[i]) == -1)) {
			flag = false;
			break;
		}
	}

	while ((letters.find(leks) == -1 && numbers.find(leks) == -1) 
		&& leks != ' ' && (flag == false || buf == ""))
	{
		buf += leks;
		y++;
		leks = str[0];

		if (str.size() > 0)
			str.erase(0, 1);
		else
			break;
	}

	if (SpecToken.find(buf) != SpecToken.end())
	{
		token = ttOperation;
		ident = buf;
		if (leks != ' ' && leks!='\0') {
			buf = leks;
		}
		else
		{
			buf = "";
		}
		return;
	}

	if (state == true)
	{
		token = ttConst;
		ident = buf;
		if (leks != ' ' && leks != '\0') {
			buf = leks;
		}
		else
		{
			buf = "";
		}
		return;
	}

	token = ttIdent;
	ident = buf;

	if (leks != ' ' && leks != '\0') {
		buf = leks;
	}
	else
	{
		buf = "";
	}

}

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


CToken parser(string& str, string& buf, string& last_token, int& i, int& j)
{
	TokenType token = ttIdent;
	char leks='\0';
	if (str.size() > 0) {
		i++;
		leks = str[0];
		str.erase(0, 1);
	}

	while (leks == '\t')
	{
		leks = str[0];
		i = +4;
		str.erase(0, 1);
	}

	

	while (numbers.find(leks)!=-1 &&(NUM(buf) || buf == "") || NUM(buf) && leks == '.' || last_token == ":=" && (leks == '+' || leks == '-'))
	{
		i++;
		token = ttConst;
		buf += leks;

		leks = str[0];
		if (str.size() == 0)
			break;

		str.erase(0, 1);

	}

	while ((numbers.find(leks) != -1 || letters.find(leks)!=-1) && (IDENT(buf) || buf == ""))
	{
		i++;
		buf += leks;

		leks = str[0];
		if (str.size() == 0)
			break;

		str.erase(0, 1);
	}

	if (leks == '"' && buf=="" || buf == "\"") {


		token = ttConst;
		do
		{
			buf += leks;

			i++;

			leks = str[0];
			if (str.size() == 0)
				break;

			str.erase(0, 1);
		} while (leks != '"');
		buf += leks;

		leks = str[0];
		if (str.size() != 0)
			str.erase(0, 1);
	}


	if (leks == '\'' && buf == "" || buf == "\'") {
		
		token = ttConst;

		do
		{
			buf += leks;

			leks = str[0];

			i++;

			if (str.size() == 0)
				break;

			str.erase(0, 1);
		} while (leks != '"');
		buf += leks;

		leks = str[0];
		if (str.size() != 0)
			str.erase(0, 1);
	}


	while (numbers.find(leks) == -1 && letters.find(leks) == -1 && (!IDENT(buf) && !NUM(buf) || buf=="") 
		&& leks!='\"' && buf.find('\"')==-1
		&&leks!='\'' && buf.find('\'')==-1
		&& leks!=' ')
	{

		i++;

		token = ttOperation;

		buf += leks;

		leks = str[0];
		if (str.size() == 0)
			break;

		str.erase(0, 1);
	}

	//cout << buf << endl;

	if (SpecToken.find(buf)!= SpecToken.end())
	{
		token = ttOperation;
	}

	string rez = buf;

	if (leks == ' ' || leks == '\0')
		buf = "";
	else
		buf = leks;

	pair<int, int> XoY;
	XoY.first = j;
	XoY.second = i;

	return CToken(token, rez, XoY);


}


void leks_analis(CToken token)
{
	switch (token.tt)
	{	
	case ttIdent: 
	default:
		break;
	}
}

CToken GetNextToken(string& str, string& buf, int& x, int&y)
{
	TokenType tt;
	string ident = "";
	read(str, tt, ident, buf, x);
	pair<int, int> X_Y;
	X_Y.first = x;
	X_Y.second = y;

	return CToken(tt, ident, X_Y);
}

void main()
{
	ifstream r("C:\\Users\\Михаил\\Desktop\\Компилятор\\Kompilertor\\Kompilertor\\file.txt");

	TokenType tt;
	string ident = "", buf = "", lt="";
	int i = 0;
	int j = 1;

	while (!r.eof())
	{
		string str;
		getline(r, str);
		/*i = 1;
		j++;
		while (str != "") {
			CToken cur =GetNextToken(str, buf, i, j);
			cout << cur.ident << " " << cur.tt <<" "<<cur.XY.first<<" "<<cur.XY.second<< endl;
		}*/

		while (str !="" || buf!="")
		{
			CToken A =parser(str, buf, lt,i,j);
			cout << A.ident << " " << A.tt << " " << A.XY.first << " " << A.XY.second << endl;
		}
		j++;
	}

	//cout<<NUM(".");
}