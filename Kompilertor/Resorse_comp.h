#pragma once
#include<string>
#include<map>

std::string letters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";
std::string numbers = "0123456789";
enum TokenType { ttIdent, ttOperation, ttConst };

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


enum KeyWords { Program, Var, Begin, End, Integer, Float, String, If, Else, Then, };

std::map<std::string, int> SpecToken = { {"program", Sprogram},
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
	StringVariant(std::string val) { value = val; }
	std::string GetValue() { return value; }
	std::string GetClassVariant() override { return "string"; }
};

struct error {
	int code_error;
	std::pair<int, int> XoY;	
};

std::map<int, std::string> errors = {
	{0, "Запрешённый символ"},
	{1, "Превышен лимит константы"},
	{2, "Неправильный идентификатор"},
	{3, "Неправильный формат вещественного числа"}};