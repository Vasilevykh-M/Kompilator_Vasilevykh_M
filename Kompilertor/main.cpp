#include<iostream>
#include<fstream>
#include<string>
#include"CIO_LEKS.h"
#include"Resorse_comp.h"
#include"CEXPR.h"

using namespace std;



void main()
{
	ifstream r("C:\\Users\\Михаил\\Desktop\\Компилятор\\Kompilertor\\Kompilertor\\file.txt");

	TokenType tt;
	string ident = "", buf = "", lt="";
	int i = 0;
	int j = 1;
	char leks='\0';
	CToken* A;

	/*do
	{
		A = IO(buf, lt, leks, r, i,j);
		if(A!=NULL)
			cout << A->ident << " " << A->tt << endl;
	} while (A != NULL);*/

	EXPR *a = new EXPR();
	cout<<a->PROGRAM();

	/*for (auto egge : eror)
	{
		cout << egge.code_error << " X: " << egge.XoY.first << "; Y: " << egge.XoY.second << endl;
	}*/
}