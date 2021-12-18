using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Комп
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
			if (a is CBoolType)
				return true;
			return false;
		}
	}


	class CStringType : CType
	{
		public CStringType() : base(EType.Tstring) { }

		public override bool isDerivedTo(CType a)
		{
			if (a is CStringType)
				return true;
			return false;
		}
	}


	class CEXPR
	{
		public CToken curToken;
		public IO_leks leks = new IO_leks() { fl = new StreamReader(@"C:\Users\Михаил\Desktop\Компилятор\Kompilertor\Kompilator_Vasilevykh_M\Компилятор\test2.txt") };
		Dictionary<string, CType> Ctable_name = new Dictionary<string, CType>();// таблица имен
		Dictionary<string, object> OBJ_name = new Dictionary<string, object>(); 
		public ILGenerator ilGenerator;
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
			if (left is CFloatType && (right is CIntType || right is CFloatType))
				return;

			leks.C_ERR.Add(new errors(7));
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
				if (leks.C_ERR[leks.C_ERR.Count - 1].eror_code != 6 && leks.C_ERR[leks.C_ERR.Count - 1].eror_code != 2)
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
				leks.C_ERR.Add(new errors(3));
		}


		void C_Block() // <блок>::=<раздел констант><раздел переменных><оператор>
		{
			try
			{
				C_ConstArea();
			}
			catch
			{
				GetNextToken();
			}


			try
			{
				C_VarArea();
			}
			catch
			{
				GetNextToken();
			}


			try
			{
				C_Operator();
			}
			catch
			{
				GetNextToken();
			}

		}

		void C_ConstArea() // <раздел констант>::=<пусто>|const <определение константы>;{<определение константы>;}
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

		void C_ConstDeter() // <определение константы>::=<имя>=<константа>
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
		void C_Const()  // <константа>::=<число без знака>|<знак><число без знака>|
						//<имя константы>|<знак><имя константы>|<строка>
						//<число без знака>::=<целое без знака>|<вещественное без знака>
						//<целое без знака>::=<цифра>{<цифра>}
						//<вещественное без знака>::=<целое без знака>.<цифра>{<цифра>}|< целое без знака>.<цифра>{<цифра>}E<порядок> |< целое без знака>E<порядок>
						//<порядок>::=<целое без знака>|<знак><целое без знака>
						//<знак>::=+|-
						//<имя константы>::=<имя>
						//<строка>::='<символ>{<символ>}'

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

		void C_VarArea() //<раздел переменных>::= var <описание однотипных переменных>;{<описание однотипных переменных>;}|<пусто>
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

		void C_var() // <описание однотипных переменных>::=<имя>{,<имя>}:<тип>
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

			foreach (string egge in name)
			{
				switch ((curToken as C_IdentToken).ident)
				{
					case "integer": var x = ilGenerator.DeclareLocal(typeof(int)); OBJ_name.Add(egge, x); break;
					case "float": var x1 = ilGenerator.DeclareLocal(typeof(double)); OBJ_name.Add(egge, x1); break;
					case "string": var x2 = ilGenerator.DeclareLocal(typeof(string)); OBJ_name.Add(egge, x2); break;
					case "bool": var x3 = ilGenerator.DeclareLocal(typeof(bool)); OBJ_name.Add(egge, x3); break;
					default: throw new Exception();
				}
				Ctable_name.Add(egge, t);
			}

			GetNextToken();
		}


		void C_Operator() //<оператор>::=<простой оператор>|<сложный оператор>
		{
			if (curToken is C_OperToken)
			{
				switch ((curToken as C_OperToken).symb)
				{
					case OperSymbol.Sbegin: C_CompositeOperator(); return;
					case OperSymbol.Sif: C_ChoiseOperator(); return;
					case OperSymbol.Swhile: C_CycleOpertor(); return;
					case OperSymbol.Swrite: C_Write(); return;
				}
			}

			C_SimpleOperator();
		}

		void C_Write() // вывод ::= <writeln>'('<выражение>')'> ;
        {
			GetNextToken();
			if (!C_Accept(OperSymbol.Sbracketr))
				throw new Exception();
			GetNextToken();

			var a = C_Excpression();

			MethodInfo writeStr = typeof(Console).GetMethod("WriteLine", new Type[] {typeof(string) });

			MethodInfo writeInt = typeof(Console).GetMethod("WriteLine", new Type[] {typeof(int) });

			MethodInfo writeFlt = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(float)});

			MethodInfo writeBoll = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(bool)});

			if (a is CIntType)
				ilGenerator.Emit(OpCodes.Call, writeInt);
			if(a is CFloatType)
				ilGenerator.Emit(OpCodes.Call, writeFlt);
			if (a is CStringType)
				ilGenerator.Emit(OpCodes.Call, writeStr);
			if (a is CBoolType)
				ilGenerator.Emit(OpCodes.Call, writeBoll);

			if (!C_Accept(OperSymbol.Sbracketl))
				throw new Exception();

			GetNextToken();

            //if (!C_Accept(OperSymbol.Ssemicon))
            //    throw new Exception();

            //GetNextToken();

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
			string a = (curToken as C_IdentToken).ident;

			GetNextToken();

			if (!C_Accept(OperSymbol.Sassigment))
				throw new Exception();

			GetNextToken();

			CType right = C_Excpression();

			ilGenerator.Emit(OpCodes.Stloc, (OBJ_name[a] as LocalBuilder));

			isDerivetToAssignemt(left, right);

		}


		/*тут должны возвращать тип выражения*/
		CType C_Excpression() // <выражение>::=<простое выражение>|<простое выражение><операция отношения><простое выражение>
		{
			CType left = C_SimpleExpression();
			if (C_realat())
			{
				CToken curToken1 = curToken;
				GetNextToken();
				CType right = C_SimpleExpression();
				if (!left.isDerivedTo(right))
				{
					leks.C_ERR.Add(new errors(7));
					throw new Exception();
				}

				if (curToken1.tt == TokenType.ttOperation)
				{
					switch((curToken1 as C_OperToken).symb)
                    {
						case OperSymbol.Sequals: ilGenerator.Emit(OpCodes.Beq); break;
						case OperSymbol.Snotequals: ilGenerator.Emit(OpCodes.Ceq); ilGenerator.Emit(OpCodes.Ldc_I4_0); ilGenerator.Emit(OpCodes.Ceq); break;
						case OperSymbol.Ssmall: ilGenerator.Emit(OpCodes.Clt); break;
						case OperSymbol.Smore: ilGenerator.Emit(OpCodes.Cgt); break;
						case OperSymbol.Smorequals: ilGenerator.Emit(OpCodes.Clt); ilGenerator.Emit(OpCodes.Ldc_I4_0);ilGenerator.Emit(OpCodes.Ceq); break;
						case OperSymbol.Ssmallequals: ilGenerator.Emit(OpCodes.Cgt); ilGenerator.Emit(OpCodes.Ldc_I4_0); ilGenerator.Emit(OpCodes.Ceq); break;

					}
				}

				return new CBoolType();
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
					CToken curToken1 = curToken;

					GetNextToken();
					CType right = C_Term();

					if (!left.isDerivedTo(right))
					{
						leks.C_ERR.Add(new errors(7));
						throw new Exception();
					}
					left = DerivetTo(left, right);
					if ((curToken1 as C_OperToken).symb == OperSymbol.Sadd)
					{
						ilGenerator.Emit(OpCodes.Add);
					}

					if ((curToken1 as C_OperToken).symb == OperSymbol.Ssubtraction)
					{
						ilGenerator.Emit(OpCodes.Sub);
					}

					if ((curToken1 as C_OperToken).symb == OperSymbol.Sor)
					{
						ilGenerator.Emit(OpCodes.Or);
					}

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
					CToken curToken1 = curToken;

					GetNextToken();
					CType right = C_Factor();
					if (!left.isDerivedTo(right))
					{
						leks.C_ERR.Add(new errors(7));
						throw new Exception();
					}
					left = DerivetTo(left, right);

					if ((curToken1 as C_OperToken).symb == OperSymbol.Smult)
					{
						ilGenerator.Emit(OpCodes.Mul);
					}

					if ((curToken1 as C_OperToken).symb == OperSymbol.Sdivision)
					{
						ilGenerator.Emit(OpCodes.Div);
					}

					if ((curToken1 as C_OperToken).symb == OperSymbol.Sand)
					{
						ilGenerator.Emit(OpCodes.And);
					}
				}
				else
				{
					break;
				}
			}
			return left;
		}

		bool C_mult() // mod нет
		{
			if(curToken.tt == TokenType.ttOperation)
            {
				if ((curToken as C_OperToken).symb == OperSymbol.Smult)
				{
					return true;
				}

				if ((curToken as C_OperToken).symb == OperSymbol.Sdivision)
				{
					return true;
				}

				if ((curToken as C_OperToken).symb == OperSymbol.Sand)
				{
					return true;
				}
			}
			return false;
		}


		bool C_add() // <аддитивная операция>::=+|-|or		*
		{
			if (curToken.tt == TokenType.ttOperation)
			{
				if((curToken as C_OperToken).symb == OperSymbol.Sadd)
                {
					return true;
				}

				if ((curToken as C_OperToken).symb == OperSymbol.Ssubtraction)
				{
					return true;
				}

				if ((curToken as C_OperToken).symb == OperSymbol.Sor)
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
				ilGenerator.Emit(OpCodes.Ldloc, (OBJ_name[(curToken as C_IdentToken).ident] as LocalBuilder).LocalIndex);
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
					ilGenerator.Emit(OpCodes.Ldc_I4, (int)(curToken as IntVariant).value);
					GetNextToken();
					return new CIntType();
				}
				if (curToken is FloatVariant)
				{
					ilGenerator.Emit(OpCodes.Ldc_R4, (float)(curToken as FloatVariant).value);
					GetNextToken();
					return new CFloatType();
				}
				if (curToken is StringVariant)
				{
					ilGenerator.Emit(OpCodes.Ldstr, (curToken as StringVariant).value.ToString());
					GetNextToken();
					return new CStringType();
				}
				if (curToken is BoolVariant)
				{
					ilGenerator.Emit(OpCodes.Ldc_I4, ((curToken as BoolVariant).value) == true? 1 : 0) ;
					GetNextToken();
					return new CBoolType();
				}
			}

			return C_Excpression();
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
			Label falseLabel = ilGenerator.DefineLabel();
			Label trueLabel = ilGenerator.DefineLabel();

			CType left = C_Excpression();
			if (!(left is CBoolType))
				throw new Exception();

			if (!C_Accept(OperSymbol.Sthen))
				throw new Exception();


			ilGenerator.Emit(OpCodes.Brfalse_S, falseLabel);

			GetNextToken();

			C_Operator();

			ilGenerator.Emit(OpCodes.Br_S, trueLabel);

			ilGenerator.MarkLabel(falseLabel);
			if (C_Accept(OperSymbol.Selse))
			{
				GetNextToken();
				//ilGenerator.MarkLabel(falseLabel);
				C_Operator();
			}

			ilGenerator.MarkLabel(trueLabel);
		}

		void C_CycleOpertor() // <оператор цикла>::= while <выражение> do <оператор>
		{
			GetNextToken();
			Label stopLabel = ilGenerator.DefineLabel();
			Label continueLabel = ilGenerator.DefineLabel();

			ilGenerator.MarkLabel(stopLabel);

			CType left = C_Excpression();


			if (!(left is CBoolType))
				throw new Exception();

			ilGenerator.Emit(OpCodes.Brfalse_S, continueLabel);

			if (!C_Accept(OperSymbol.Sdo))
				throw new Exception();
			GetNextToken();

			C_Operator();

			ilGenerator.Emit(OpCodes.Br_S, stopLabel);
			ilGenerator.MarkLabel(continueLabel);

		}

	}
}
