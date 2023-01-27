// Compile this file using ilasm /DLL HMMCodes.Unsafe.il

.assembly HMMCodes
{
	
}

.class public HMMCodes.Unsafe
{
	.method public static void* AsPointer<T>(!!T&) cil managed
	{
		.maxstack 8
		ldarg.0
		conv.u
		ret
	}
	
	.method public static !!T& AsRef<T>(void*) cil managed
	{
		.maxstack 8
		ldarg.0
		conv.u
		ret
	}

	.method public static void CopyBlock(void*, void*, uint32) cil managed
	{
		.maxstack 8
		ldarg.0
		ldarg.1
		ldarg.2
		cpblk
		ret
	}

	.method public static void CopyBlockUnaligned(void*, void*, uint32) cil managed
	{
		.maxstack 8
		ldarg.0
		ldarg.1
		ldarg.2
		unaligned. 1
		cpblk
		ret
	}

	.method public static uint32 SizeOf<T>() cil managed
	{
		.maxstack 8
		sizeof !!T
		ret
	}
}