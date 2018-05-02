#pragma once
#include <cstdio>
#include <string>
#include <vector>
#include "MemAccess.h"

static const int ModLoaderVer = 2;
static const int GameVer = 0;


using std::string;
using std::vector;

struct ModConfigEntry
{
    string name;
    void* value;
};
struct ModConfig
{
    vector<ModConfigEntry> Entries;
};

struct Mod
{
    string* Name;
    string* Path;
    ModConfig* Config;
    vector<string>* IncludePaths;
};

struct ModEvent
{
    vector<Mod*>* ModList;
    Mod* CurrentMod;
};

typedef void(__cdecl *ModInitEvent)(ModEvent* modEvent);
typedef void(__cdecl *ModCallEvent)();

struct ModInfo
{
	int LoaderVersion;
	int GameVersion;
};

static char* StageID = (char*)0x01E774D4;