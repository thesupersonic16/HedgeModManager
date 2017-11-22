#pragma once
#include <cstdio>
#include "MemAccess.h"

// From MemAccess
static inline BOOL WriteJump(void *writeaddress, void *funcaddress)
{
    uint8_t data[5];
    data[0] = 0xE9; // JMP DWORD (relative)
    *(int32_t*)(data + 1) = (uint32_t)((uint64_t)funcaddress - ((uint64_t)writeaddress + 5));
    return WriteData(writeaddress, data);
}

// From MemAccess
static inline BOOL WriteCall(void *writeaddress, void *funcaddress)
{
    uint8_t data[16];
    data[0] = 0xFF; // CALL QWORD
    data[1] = 0x15; // CALL QWORD
    data[2] = 0x02; // CALL QWORD
    data[3] = 0x00; // CALL QWORD
    data[4] = 0x00; // CALL QWORD
    data[5] = 0x00; // CALL QWORD
    data[6] = 0xEB; // CALL QWORD
    data[7] = 0x08; // CALL QWORD
    *(int64_t*)(data + 8) = (int64_t)funcaddress;
    return WriteData(writeaddress, data);
}

static const int ModLoaderVer = 1;
static const int GameVer = 0;

struct PatchInfo
{
    void *address;
    const void *data;
    int datasize;
};

struct PatchList
{
    const PatchInfo *Patches;
    int Count;
};

struct PointerInfo
{
    void *address;
    void *data;
};

struct PointerList
{
    const PointerInfo *Pointers;
    int Count;
};

typedef void(__cdecl *ModInitFunc)(const char *path);

typedef void(__cdecl *ModEvent)();

struct ModInfo
{
    int LoaderVersion;
    int GameVersion;
};

typedef uint32_t _DWORD;
typedef uint16_t _WORD;
typedef uint8_t _BYTE;
