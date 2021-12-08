using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Компилятор
{
	enum EType { Tint, Tfloat, Tstring, Tbool };
	abstract class CType
	{
		public EType TT;
		public CType(EType TT)
		{
			this.TT = TT;
		}
		public virtual bool isDerivedTo(CType b) { return true; }
	}

	class CIntType : CType
	{
		public CIntType() : base(EType.Tint) { }
		public override bool isDerivedTo(CType a)
		{
			if (a is CIntType || a is CFloatType)
				return true;
			return false;
		}
	}

	class CFloatType : CType 
	{
		public CFloatType() : base(EType.Tfloat) { }

		public override bool isDerivedTo(CType a)
		{
			if (a is CIntType || a is CFloatType)
				return true;
			return false;
		}
	}


	class CBoolType : CType
	{
		public CBoolType() : base(EType.Tbool) { }

		public override bool isDerivedTo(CType a)
		{
			if(a is CBoolType)
				return true;
			return false;
		}
	}


	class CStringType : CType
	{
		public CStringType() : base(EType.Tstring) { }

		public override bool isDerivedTo(CType a)
		{
			if(a is CStringType)
				return true;
			return false;
		}
	}


	class CEXPR
	{
		public CToken curToken;
		public IO_leks leks = new IO_leks() { fl = new StreamReader("C:/Users/Михаил/Desktop/Компилятор/Компилятор/Компилятор/test2.txt") };
		Dictionary<string, CType> Ctable_name = new Dictionary<string, CType>();// таблица имен

		public CType DerivetTo(CType left, CType right)
        {

			if (left is CIntType && right is CIntType)
				return left;

			if (left is CBoolType && right is CBoolType)
				return left;

			if (left is CStringType && right is CStringType)
				return left;

			return new CFloatType();
		}

		public void isDerivetToAssignemt(CType left, CType right)
		{

			if (left is CIntType && right is CIntType)
				return;

			if (left is CBoolType && right is CBoolType)
				return;

			if (left is CStringType && right is CStringType)
				return;
			if(left is CFloatType && (right is CIntType || right is CFloatType))
				return;
			throw new Exception();
		}

		void GetNextToken()
		{
			try
			{
				curToken = leks.CIO();

				if (curToken is C_IdentToken)
					Console.WriteLine((curToken as C_IdentToken).ident);
				else
				{
					if (curToken is C_OperToken)
						Console.WriteLine((curToken as C_OperToken).symb);
					else
						Console.WriteLine(curToken.tt);
				}
			}
			catch
            {
				GetNextToken();
			}
		}

		bool C_Accept(OperSymbol sym)
		{
			if (curToken == null || curToken.tt != TokenType.ttOperation || (curToken as C_OperToken).symb != sym)
			{
				return false;
			}
			return true;
		}

		public void C_Programm()
		{
			GetNextToken();

			if (!C_Accept(OperSymbol.Sprogram))
				leks.C_ERR.Add(new errors(3));

			GetNextToken();

			if (curToken.tt != TokenType.ttIdent)
				leks.C_ERR.Add(new errors(4));

			GetNextToken();

			if (!C_Accept(OperSymbol.Ssemicon))
				leks.C_ERR.Add(new errors(3));

			GetNextToken();

			C_Block();

			if (!C_Accept(OperSymbol.Spoint))
				throw new Exception();
		}


		void C_Block()
		{
			C_ConstArea();
			C_VarArea();
			C_Operator();

		}

		void C_ConstArea()
		{
			if (C_Accept(OperSymbol.Sconst))
			{
				C_ConstDeter();
				if (!C_Accept(OperSymbol.Ssemicon))
					throw new Exception();
				GetNextToken();
				while (true)
				{
					try
					{
						C_ConstDeter();
					}
					catch
					{
						break;
					}
					if (!C_Accept(OperSymbol.Ssemicon))
						throw new Exception();
					GetNextToken();
				}
			}
		}

		void C_ConstDeter()
		{
			if (curToken.tt != TokenType.ttIdent)
			{
				throw new Exception();
			}
			GetNextToken();
			if (!C_Accept(OperSymbol.Sequals))
			{
				throw new Exception();
			}
			GetNextToken();
			C_Const();
		}
		void C_Const()
		{
			if (C_Accept(OperSymbol.Ssubtraction))
			{
				GetNextToken();
				if (curToken.tt == TokenType.ttConst || curToken.tt == TokenType.ttIdent)
				{
					GetNextToken();
				}
				else
				{
					throw new Exception();
				}
			}
			else
			{
				if (curToken.tt == TokenType.ttConst || curToken.tt == TokenType.ttIdent)
				{
					GetNextToken();
				}
				else
				{
					throw new Exception();
				}
			}
		}

		void C_VarArea()
		{
			if (C_Accept(OperSymbol.Svar))
			{
				GetNextToken();
				C_var();
				if (!C_Accept(OperSymbol.Ssemicon))
					throw new Exception();
				GetNextToken();
				while (true)
				{
					try
					{
						C_var();
					}
					catch
					{
						break;
					}
					if (!C_Accept(OperSymbol.Ssemicon))
						throw new Exception();
					GetNextToken();
				}
			}
		}

		void C_var()
		{
			if (curToken.tt != TokenType.ttIdent)
				throw new Exception();

			/*имена переменных*/
			List<string> name = new List<string>();
			name.Add((curToken as C_IdentToken).ident);

			GetNextToken();

			while (C_Accept(OperSymbol.Scomma))
			{
				GetNextToken();
				if (curToken.tt != TokenType.ttIdent)
					throw new Exception();

				/*имена переменных*/
				name.Add((curToken as C_IdentToken).ident);

				GetNextToken();
			}

			if (!C_Accept(OperSymbol.Scolon))
				throw new Exception();

			GetNextToken();

			if (curToken.tt != TokenType.ttIdent)
				throw new Exception();
			/**/

			CType t;
			/*добавление в таблицу имен*/
			switch ((curToken as C_IdentToken).ident)
            {
				case "integer": t = new CIntType(); break;
				case "float": t = new CFloatType(); break;
				case "string": t = new CStringType(); break;
				case "bool": t = new CBoolType(); break;
				default: throw new Exception();
			}

			foreach(string egge in name)
            {
				Ctable_name.Add(egge, t);
            }

			GetNextToken();
		}

		void C_Operator() //<оператор>::=<простой оператор>|<сложный оператор>
		{
			if(curToken is C_OperToken)
            {
				switch((curToken as C_OperToken).symb)
                {
					case OperSymbol.Sbegin: C_CompositeOperator(); return;
					case OperSymbol.Sif:C_ChoiseOperator(); return;
					case OperSymbol.Swhile: C_CycleOpertor();return;
				}
			}

			C_SimpleOperator();
		}

		void C_SimpleOperator() // <простой оператор>::=<оператор присваивания>|<пустой оператор>
		{
			try
			{
				C_AssigmentOperator();
			}
			catch
			{

			}
		}
		/*тут должны возвращать тип выражения*/
		/* тип имени сравниваем с типом выражения*/
		void C_AssigmentOperator() // <оператор присваивания>::=<имя>:=<выражение>
		{
			if (curToken.tt != TokenType.ttIdent)
				throw new Exception();

			CType left = Ctable_name[(curToken as C_IdentToken).ident];

			GetNextToken();

			if (!C_Accept(OperSymbol.Sassigment))
				throw new Exception();

			GetNextToken();

			CType right = C_Excpression();

			isDerivetToAssignemt(left, right);

		}


		/*тут должны возвращать тип выражения*/
		CType C_Excpression() // <выражение>::=<простое выражение>|<простое выражение><операция отношения><простое выражение>
		{
			CType left = C_SimpleExpression();
			if (C_realat())
			{
				GetNextToken();
				CType right = C_SimpleExpression();
				if (!left.isDerivedTo(right))
					throw new Exception();
			}
			return left;

		}
		CType C_SimpleExpression() // <простое выражение>::=<знак><слагаемое>{<аддитивная операция><слагаемое>}
		{
			if (C_Accept(OperSymbol.Ssubtraction))
				GetNextToken();

			CType left = C_Term();

			while (true)
			{
				if (C_add())
				{
					GetNextToken();
					CType right = C_Term();

					if (!left.isDerivedTo(right))
						throw new Exception();
					left = DerivetTo(left, right);
				}
				else
				{
					break;
				}
			}
			return left;
		}

		CType C_Term() // <слагаемое>::=<множитель>{<мультипликативная операция><множитель>}
		{
			CType left = C_Factor();
			while (true)
			{
				if (C_mult())
				{
					GetNextToken();
					CType right = C_Factor();
					if(!left.isDerivedTo(right))
						throw new Exception();
					left = DerivetTo(left, right);
				}
				else
				{
					break;
				}
			}
			return left;
		}

		bool C_mult()
		{
			if ((curToken is C_OperToken)&&((curToken as C_OperToken).symb == OperSymbol.Smult ||
					(curToken as C_OperToken).symb == OperSymbol.Sdivision ||
					(curToken as C_OperToken).symb == OperSymbol.Sdiv ||
					(curToken as C_OperToken).symb == OperSymbol.Smod ||
					(curToken as C_OperToken).symb == OperSymbol.Sand))

			{
				return true;
			}
			return false;
		}


		bool C_add() // <аддитивная операция>::=+|-|or		*
		{
			if (curToken.tt == TokenType.ttOperation)
			{
				if ((curToken as C_OperToken).symb == OperSymbol.Sadd ||
					(curToken as C_OperToken).symb == OperSymbol.Ssubtraction ||
					(curToken as C_OperToken).symb == OperSymbol.Sor)
				{
					return true;
				}
			}
			return false;
		}


		bool C_realat() // <операция отношения>::==|<>|<|<=|>=|>|in		*
		{

			if (curToken.tt == TokenType.ttOperation)
			{
				if ((curToken as C_OperToken).symb == OperSymbol.Sequals ||
					(curToken as C_OperToken).symb == OperSymbol.Snotequals ||
					(curToken as C_OperToken).symb == OperSymbol.Ssmall ||
					(curToken as C_OperToken).symb == OperSymbol.Smore ||
					(curToken as C_OperToken).symb == OperSymbol.Smorequals ||
					(curToken as C_OperToken).symb == OperSymbol.Ssmallequals ||
					(curToken as C_OperToken).symb == OperSymbol.Sin)

				{
					return true;
				}
			}
			return false;
		}

		CType C_Factor() //<множитель>::=<имя>|<константа без знака>|(<выражение>)
		/*если первые 2 фора прошли то если не пройдёт 3 то кинет исключение а если пройдёт то норм*/
		{
			if (curToken.tt == TokenType.ttIdent)
			{
				CType a = Ctable_name[(curToken as C_IdentToken).ident];
				GetNextToken();
				if (a != null)
					return a;
				else
					throw new Exception();
			}

			if (curToken.tt == TokenType.ttConst)
			{
				if (curToken is IntVariant)
				{
					GetNextToken();
					return new CIntType();
				}
				if (curToken is FloatVariant)
				{
					GetNextToken();
					return new CFloatType();
				}
				if(curToken is StringVariant)
                {
					GetNextToken();
					return new CStringType();
                }
				if(curToken is BoolVariant)
                {
					GetNextToken();
					return new CBoolType();
                }
			}
			throw new Exception();
            //return C_Excpression();
        }

		void C_CompositeOperator() // <составной оператор>::= begin <оператор>;{<оператор>;} end
		{
			GetNextToken();

			C_Operator();
            if (!C_Accept(OperSymbol.Ssemicon))
                throw new Exception();

            while (true)
			{
				if (C_Accept(OperSymbol.Ssemicon))
				{
					GetNextToken();
					C_Operator();
				}
				else
				{
					break;
				}
			}
			if (!C_Accept(OperSymbol.Send))
			{
				throw new Exception();
			}
			GetNextToken();
		}

		void C_ChoiseOperator() // <выбирающий оператор>::= if <выражение> then <оператор>|if <выражение> then <оператор> else <оператор>
		{
			GetNextToken();

			CType left = C_Excpression();
			if(!(left is CBoolType))
				throw new Exception();

			if (!C_Accept(OperSymbol.Sthen))
				throw new Exception();

			GetNextToken();

			C_Operator();

			if (C_Accept(OperSymbol.Selse))
			{
				GetNextToken();
				C_Operator();
			}
		}

		void C_CycleOpertor() // <оператор цикла>::= while <выражение> do <оператор>
		{
			GetNextToken();

			CType left = C_Excpression();

			if (!(left is CBoolType))
				throw new Exception();

			if (!C_Accept(OperSymbol.Sdo))
				throw new Exception();
			GetNextToken();

			C_Operator();
		}

	}
}