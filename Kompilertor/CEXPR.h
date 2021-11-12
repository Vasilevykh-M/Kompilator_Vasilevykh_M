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
private:
	CToken* curToken;

	void GetNextToken()
	{
		curToken = IO(buf, lt, leks, r, i, j);
	}

	void Name()
	{
		if (curToken->tt != ttIdent)
		{
			throw std::exception("crtital errror!!!");
		}
	}


	void area_label()
	{
		if (curToken->ident == "label")
		{
			label();
			GetNextToken();
			while (curToken->ident != ";")
			{
				label();
				GetNextToken();
			}
		}
	}

	void label()
	{
		if (curToken->tt != ttConst)
		{
			throw std::exception("crtital errror!!!");
		}
	}

	void const_area()
	{
		if (curToken->ident == "const")
		{
			def_const();
			GetNextToken();
			while (curToken->ident != ";")
			{
				label();
				GetNextToken();
			}
		}
	}

	void def_const()
	{
		name();
	}
	void name()
	{
		if (curToken->tt != ttIdent)
		{
			throw std::exception("crtital errror!!!");
		}
		GetNextToken();
	}

	void Const()
	{

	}
	void num_nS()
	{
		if(cur)
	}
	void num()
	{
		if(curToken->tt != ttConst)
			throw std::exception("crtital errror!!!");
		GetNextToken();
	}

	void type_area()
	{

	}

	void var_area()
	{

	}

	void operators()
	{

	}

	void blok()
	{

	}

	void programm()
	{
		if (curToken->ident != "program")
		{
			throw std::exception("crtital errror!!!");
		}
		GetNextToken();
		Name();
	}

};