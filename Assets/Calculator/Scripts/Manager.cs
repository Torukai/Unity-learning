using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Operators;
using Utility;

public class Manager : MonoBehaviour
{
	public VerticalLayoutGroup buttonGroup;
	public HorizontalLayoutGroup bottomRow;
	public RectTransform canvasRect;
	Button[] bottomButtons;

	public Text digitLabel;
	public Text operatorLabel;
	bool errorDisplayed;
	bool displayValid;
	bool specialAction;
	double currentVal;
	double storedVal;
	double result;
	char storedOperator;

	bool canvasChanged;



	static List<object> ConvertToRPN(string s)
	{
		//Stack<char> stack = new Stack<char>();
		List<object> rpn = new List<object>();
		Stack<IOperator> operatorStack = new Stack<IOperator>();

		for (int i = 0; i < s.Length; i++)
		{
			if (char.IsDigit(s[i]) ||
			   (i == 0 && s[i] == '-' && char.IsDigit(s[i + 1])) ||
			   (i >= 1 && s[i] == '-' && s[i - 1] == '(') && char.IsDigit(s[i + 1]))
			{
				rpn.Add (s.ReadNumber(ref i));

			}
			else if (s[i] == '(')
			{
				operatorStack.Push(OperatorFactory.Create<LeftBracket>());
			}
			else if (s[i] == ')')
			{
				while (operatorStack.Peek().GetType() != typeof(LeftBracket))
				{
					rpn.Add(operatorStack.Pop());
				}
				operatorStack.Pop();
			}
			else if (char.IsLetter(s[i]))
			{
				bool negative = false;
				if ((i == 1 && s[i - 1] == '-') || (i > 1 && s[i - 1] == '-' && s[i - 2] == '(')) negative = true;

				var token = OperatorParser.ToFunction(s.ReadFunction(ref i));
				if (token is IConstant)
				{
					var constant = (IConstant)token;
					rpn.Add(negative ? constant.Value * -1 : constant.Value);
				}
				else
				{
					var func = (IFunction)token;
					if (negative) func.isNegative = true;
					operatorStack.Push(func);
				}
			}
			else if ((!char.IsWhiteSpace(s[i]) && s[i] != '.') &&
			!(i == 0 && s[i] == '-') &&
			!(i >= 1 && s[i] == '-' && s[i - 1] == '('))
			{
				var token = s.ToOperator(i);
				while (operatorStack.Count != 0 &&
					  operatorStack.Peek().GetType() != typeof(LeftBracket) &&
					  token.ComparePriority(operatorStack.Peek()))
				{
					rpn.Add(operatorStack.Pop());
				}
				operatorStack.Push(token);
			}
		}
		while (operatorStack.Count != 0)
		{
			rpn.Add(operatorStack.Pop());
		}
		return rpn;

		//string str = input.Replace(" ", string.Empty);
		//StringBuilder formula = new StringBuilder();
		//for (int i = 0; i < str.Length; i++)
		//{
		//	char x = str[i];
		//	if (x == '(')
		//		stack.Push(x);
		//	else if (x == ')')
		//	{
		//		while (stack.Count > 0 && stack.Peek() != '(')
		//			formula.Append(stack.Pop());
		//		stack.Pop();
		//	}
		//	else if (IsOperand(x))
		//	{
		//		formula.Append(x);
		//	}
		//	else if (IsOperator(x))
		//	{
		//		while (stack.Count > 0 && stack.Peek() != '(' && Prior(x) <= Prior(stack.Peek()))
		//			formula.Append(stack.Pop());
		//		stack.Push(x);
		//	}
		//	else
		//	{
		//		char y = stack.Pop();
		//		if (y != '(')
		//			formula.Append(y);
		//	}
		//}
		//while (stack.Count > 0)
		//{
		//	formula.Append(stack.Pop());
		//}
		//return formula.ToString();
	}

	static double Calculate(string s)
	{

		List<object> rpn = ConvertToRPN(s);
		Stack<double> stack = new Stack<double>();

		foreach (var c in rpn)
		{
			if (c is double)
			{
				stack.Push((double)c);
			}
			if (c is IExecutableOperator)
			{
				var Oper = (IExecutableOperator)c;
				var a = stack.Pop();
				var b = stack.Pop();
				stack.Push(Oper.CalculateOperator(a, b));

			}
			if (c is IOneArgFunction)
			{
				var function = (IOneArgFunction)c;
				var a = stack.Pop();
				stack.Push(function.isNegative ? function.CalculateFunction(a) * -1 : function.CalculateFunction(a));
			}
		}


		return stack.Pop();
	}

	static bool IsOperator(char c)
	{
		return (c == '-' || c == '+' || c == '*' || c == '/');
	}
	static bool IsOperand(char c)
	{
		return (c >= '0' && c <= '9' || c == '.');
	}
	static int Prior(char c)
	{
		switch (c)
		{
			case '=':
				return 1;
			case '+':
				return 2;
			case '-':
				return 2;
			case '*':
				return 3;
			case '/':
				return 3;
			case '^':
				return 4;
			default:
				return 5;
		}
	}



	private void Awake()
	{
		bottomButtons = bottomRow.GetComponentsInChildren<Button>();
	}

	// Start is called before the first frame update
	void Start()
    {
		bottomRow.childControlWidth = false;
		canvasChanged = true;
		buttonTapped('c');
	}

    // Update is called once per frame
    void Update()
    {
		if (canvasChanged)
		{
			canvasChanged = false;
			adjustButtons();
		}
	}

	private void OnRectTransformDimensionsChange()
	{
		canvasChanged = true;
	}

	void adjustButtons()
	{
		if (bottomButtons == null || bottomButtons.Length == 0)
			return;
		float buttonSize = canvasRect.sizeDelta.x / 5;
		float bWidth = buttonSize - bottomRow.spacing;
		for (int i = 0; i < bottomButtons.Length; i++)
		{
			bottomButtons[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
																	 bWidth);
		}
		bottomButtons[3].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
																 bWidth * 2 + bottomRow.spacing);
	}

	void clearCalc()
	{
		digitLabel.text = "0";
		operatorLabel.text = "";
		specialAction = displayValid = errorDisplayed = false;
		currentVal = result = storedVal = 0;
		storedOperator = ' ';
	}
	void updateDigitLabel()
	{
		if (!errorDisplayed)
			digitLabel.text = currentVal.ToString();
		displayValid = false;
	}

	void calcResult(char activeOp)
	{
		switch (activeOp)
		{
			case '=':
				result = currentVal;
				break;
			case '+':
				result = storedVal + currentVal;
				break;
			case '-':
				result = storedVal - currentVal;
				break;
			case 'x':
				result = storedVal * currentVal;
				break;
			case '÷':
				if (currentVal != 0)
				{
					result = storedVal / currentVal;
				}
				else
				{
					errorDisplayed = true;
					digitLabel.text = "ERROR";
				}
				break;
			default:
				Debug.Log("unknown: " + activeOp);
				break;
		}
		currentVal = result;
		updateDigitLabel();
	}

	public void buttonTapped(char caption)
	{
		if (errorDisplayed)
			clearCalc();

		if ((caption >= '0' && caption <= '9') || caption == '.' || caption == '+' || caption == '-' || caption == '*' || caption == '÷' || caption == '(' || caption == ')')
		{
			if (digitLabel.text.Length < 15 || !displayValid)
			{
				if (!displayValid)
					digitLabel.text = (caption == '.' ? "0" : "");
				else if (digitLabel.text == "0" && caption != '.')
					digitLabel.text = "";
				digitLabel.text += caption;
				displayValid = true;
			}
		}
		else if (caption == '←')
		{
			digitLabel.text = digitLabel.text.Remove(digitLabel.text.Length-1);
			//updateDigitLabel();
		}
		else if (caption == 'C')
		{
			clearCalc();
		}
		else if (caption == '±')
		{
			currentVal = -double.Parse(digitLabel.text);
			updateDigitLabel();
			specialAction = true;
		}
		else if (caption == '%')
		{
			currentVal = double.Parse(digitLabel.text) / 100.0d;
			updateDigitLabel();
			specialAction = true;
		}
		else if (displayValid || storedOperator == '=' || specialAction)
		{
			System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");
			//currentVal = double.Parse(digitLabel.text);
			currentVal = Calculate (digitLabel.text);
			displayValid = false;
			//if (storedOperator != ' ')
			//{
			//	calcResult(storedOperator);
			//	storedOperator = ' ';
			//}
			operatorLabel.text = caption.ToString();
			storedOperator = caption;
			storedVal = currentVal;
			updateDigitLabel();
			specialAction = false;
		}
	}
}
