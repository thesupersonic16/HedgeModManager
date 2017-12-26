/**
 * Mania Mod Loader.
 * Memory access inline functions.
 */

#ifndef MODLOADER_MEMACCESS_H
#define MODLOADER_MEMACCESS_H

#include <stdint.h>

// Utility Functions
#ifdef __cplusplus
// C++ version.

/**
* Get the number of elements in an array.
* @return Number of elements in the array.
*/
template <typename Tret = size_t, typename T, size_t N>
static inline Tret LengthOfArray(const T(&)[N])
{
	return (Tret)N;
}

/**
* Get the size of an array.
* @return Size of the array, in bytes.
*/
template <typename Tret = size_t, typename T, size_t N>
static inline Tret SizeOfArray(const T(&)[N])
{
	return (Tret)(N * sizeof(T));
}

// Macros for functions that need both an array
// and the array length or size.
#define arrayptrandlengthT(data,T) data, LengthOfArray<T>(data)
#define arraylengthandptrT(data,T) LengthOfArray<T>(data), data
#define arrayptrandsizeT(data,T) data, SizeOfArray<T>(data)
#define arraysizeandptrT(data,T) SizeOfArray<T>(data), data
#else

// C version.

/**
 * Number of elements in an array.
 *
 * Includes a static check for pointers to make sure
 * a dynamically-allocated array wasn't specified.
 * Reference: http://stackoverflow.com/questions/8018843/macro-definition-array-size
 */
#define LengthOfArray(x) \
	((int)(((sizeof(x) / sizeof(x[0]))) / \
		(size_t)(!(sizeof(x) % sizeof(x[0])))))

#define SizeOfArray(x) sizeof(x)

#endif

// Macros for functions that need both an array
// and the array length or size.
#define arrayptrandlength(data) data, LengthOfArray(data)
#define arraylengthandptr(data) LengthOfArray(data), data
#define arrayptrandsize(data) data, SizeOfArray(data)
#define arraysizeandptr(data) SizeOfArray(data), data

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <windows.h>

static const HANDLE curproc = GetCurrentProcess();

static inline BOOL WriteData(void *writeaddress, const void *data, SIZE_T datasize, SIZE_T *byteswritten)
{
	return WriteProcessMemory(curproc, writeaddress, data, datasize, byteswritten);
}

static inline BOOL WriteData(void *writeaddress, const void *data, SIZE_T datasize)
{
	return WriteData(writeaddress, data, datasize, nullptr);
}

template<typename T>
static inline BOOL WriteData(T const *writeaddress, const T data, SIZE_T *byteswritten)
{
	return WriteData((void*)writeaddress, (void*)&data, (SIZE_T)sizeof(data), byteswritten);
}

template<typename T>
static inline BOOL WriteData(T const *writeaddress, const T data)
{
	return WriteData(writeaddress, data, nullptr);
}

template<typename T>
static inline BOOL WriteData(T *writeaddress, const T &data, SIZE_T *byteswritten)
{
	return WriteData(writeaddress, &data, sizeof(data), byteswritten);
}

template<typename T>
static inline BOOL WriteData(T *writeaddress, const T &data)
{
	return WriteData(writeaddress, data, nullptr);
}

template <typename T, size_t N>
static inline BOOL WriteData(void *writeaddress, const T(&data)[N], SIZE_T *byteswritten)
{
	return WriteData(writeaddress, data, SizeOfArray(data), byteswritten);
}

template <typename T, size_t N>
static inline BOOL WriteData(void *writeaddress, const T(&data)[N])
{
	return WriteData(writeaddress, data, nullptr);
}

/**
 * Write a repeated byte to an arbitrary address.
 * @param address	[in] Address.
 * @param data		[in] Byte to write.
 * @param byteswritten	[out, opt] Number of bytes written.
 * @return Nonzero on success; 0 on error (check GetLastError()).
 */
template <int count>
static inline BOOL WriteData(void *address, const char data, SIZE_T *byteswritten)
{
	char buf[count];
	memset(buf, data, count);
	int result = WriteData(address, buf, count, byteswritten);
	return result;
}

/**
 * Write a repeated byte to an arbitrary address.
 * @param address	[in] Address.
 * @param data		[in] Byte to write.
 * @return Nonzero on success; 0 on error (check GetLastError()).
 */
template <int count>
static inline BOOL WriteData(void *address, char data)
{
	return WriteData<count>(address, data, nullptr);
}
#if (defined(__i386__) || defined(_M_IX86)) && \
	!(defined(__x86_64__) || defined(_M_X64))
/**
 * Write a JMP instruction to an arbitrary address.
 * @param writeaddress Address to insert the JMP instruction.
 * @param funcaddress Address to JMP to.
 * @return Nonzero on success; 0 on error (check GetLastError()).
 */
static inline BOOL WriteJump(void *writeaddress, void *funcaddress)
{
	uint8_t data[5];
	data[0] = 0xE9; // JMP DWORD (relative)
	*(int32_t*)(data + 1) = (uint32_t)funcaddress - ((uint32_t)writeaddress + 5);
	return WriteData(writeaddress, data);
}

/**
 * Write a CALL instruction to an arbitrary address.
 * @param writeaddress Address to insert the CALL instruction.
 * @param funcaddress Address to CALL.
 * @return Nonzero on success; 0 on error (check GetLastError()).
 */
static inline BOOL WriteCall(void *writeaddress, void *funcaddress)
{
	uint8_t data[5];
	data[0] = 0xE8; // CALL DWORD (relative)
	*(int32_t*)(data + 1) = (uint32_t)funcaddress - ((uint32_t)writeaddress + 5);
	return WriteData(writeaddress, data);
}

#endif

// Data pointer and array declarations.
#define DataPointer(type, name, address) \
	static type &name = *(type *)address
#define DataArray(type, name, address, length) \
	static type *const name = (type *)address; static const int name##_Length = length

// Function pointer declarations.
#define FunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) \
	static RETURN_TYPE (__cdecl *const NAME)ARGS = (RETURN_TYPE (__cdecl *)ARGS)ADDRESS
#define StdcallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) \
	static RETURN_TYPE (__stdcall *const NAME)ARGS = (RETURN_TYPE (__stdcall *)ARGS)ADDRESS
#define FastcallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) \
	static RETURN_TYPE (__fastcall *const NAME)ARGS = (RETURN_TYPE (__fastcall *)ARGS)ADDRESS
#define ThiscallFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) \
	static RETURN_TYPE (__thiscall *const NAME)ARGS = (RETURN_TYPE (__thiscall *)ARGS)ADDRESS
#define VoidFunc(NAME, ADDRESS) FunctionPointer(void,NAME,(void),ADDRESS)

// Non-static FunctionPointer.
// If declaring a FunctionPointer within a function, use this one instead.
// Otherwise, the program will crash on Windows XP.
#define NonStaticFunctionPointer(RETURN_TYPE, NAME, ARGS, ADDRESS) \
	RETURN_TYPE (__cdecl *const NAME)ARGS = (RETURN_TYPE (__cdecl *)ARGS)ADDRESS

#define patchdecl(address,data) { (void*)address, arrayptrandsize(data) }
#define ptrdecl(address,data) { (void*)address, (void*)data }

#endif /* MODLOADER_MEMACCESS_H */
