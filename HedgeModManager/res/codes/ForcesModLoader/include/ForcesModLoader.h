#pragma once
#include <cstdio>
#include <fstream>
#include "MemAccess.h"

// From MemAccess
// JMP (5 BYTES) (Relative 32-bit address)
static inline BOOL WriteJump(void *writeaddress, void *funcaddress)
{
    uint8_t data[5];
    data[0] = 0xE9; // JMP DWORD (relative)
    *(int32_t*)(data + 1) = (uint32_t)((uint64_t)funcaddress - ((uint64_t)writeaddress + 5));
    return WriteData(writeaddress, data);
}

// From MemAccess
// Call (5 BYTES) (Relative 32-bit address)
static inline BOOL WriteCall_32(void *writeaddress, void *funcaddress)
{
    uint8_t data[5];
    data[0] = 0xE8;
    *(int32_t*)(data + 1) = (uint32_t)funcaddress - ((uint32_t)writeaddress + 5);
    return WriteData(writeaddress, data);
}

// From MemAccess
// Call (16 BYTES) (Absolute 64-bit address)
static inline BOOL WriteCall(void *writeaddress, void *funcaddress)
{
    uint8_t data[16];
    data[0] = 0xFF;
    data[1] = 0x15;
    data[2] = 0x02;
    data[3] = 0x00;
    data[4] = 0x00;
    data[5] = 0x00;
    data[6] = 0xEB;
    data[7] = 0x08;
    *(int64_t*)(data + 8) = (int64_t)funcaddress;
    return WriteData(writeaddress, data);
}

static bool FileExists(const char *fileName)
{
    std::ifstream infile(fileName);
    bool result = infile.good();
    infile.close();
    return result;
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

// CPK Types
typedef bool CriBool;
typedef int* CriFsFileHn;
typedef char CriChar8;
typedef signed long long CriSint64;
typedef unsigned int CriUint32;
typedef CriUint32 CriFsBindId;
typedef void* *CriFsBinderHn;
typedef void* CriFsLoaderHn;

typedef struct CriFsBinderFileInfoTag
{
    CriFsFileHn filehn;
    CriChar8 *path;
    CriSint64 offset;
    CriSint64 read_size;
    CriSint64 extract_size;
    CriFsBindId binderid;
    CriUint32 reserved[1];
} CriFsBinderFileInfo;

typedef enum
{
    CRIFSLOADER_STATUS_STOP,
    CRIFSLOADER_STATUS_LOADING,
    CRIFSLOADER_STATUS_COMPLETE,
    CRIFSLOADER_STATUS_ERROR,
    CRIFSLOADER_STATUS_ENUM_BE_SINT32 = 0x7FFFFFFF
} CriFsLoaderStatus;


// CPK Functions
FastcallFunctionPointer(__int64, criFsLoader_GetStatus, (CriFsLoaderHn loader, CriFsLoaderStatus *status), 0x1400ACBF4);
FastcallFunctionPointer(__int64, criFsLoader_Stop, (CriFsLoaderHn loader), 0x1400ACF84);
FastcallFunctionPointer(__int64, criFsLoader_Load, (CriFsLoaderHn loader, CriFsBinderHn binder, const CriChar8 *path, CriSint64 offset, CriSint64 load_size, void *buffer, CriSint64 buffer_size), 0x1400ACCFC);
FastcallFunctionPointer(__int64, criFsBinder_Find, (CriFsBinderHn bndrhn, const CriChar8 *filepath, CriFsBinderFileInfo *finfo, CriBool *exist), 0x1400AE8E4);
