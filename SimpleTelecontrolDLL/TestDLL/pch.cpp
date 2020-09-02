// pch.cpp: 對應到先行編譯標頭的來源檔案

#pragma comment(lib, "ws2_32.lib")
#include <stdlib.h>
#include <winsock2.h>
#include <windows.h>
#define MasterPort 999  //定??听端口999

#include "stdio.h"
#include "pch.h"


// 使用先行編譯的標頭時，需要來源檔案才能使編譯成功。
