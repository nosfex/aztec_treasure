using UnityEngine;
using System.Collections;

static class StringExt
{
	
	public static string TrimEnd(this string source, string value)
	{
		if ( !source.EndsWith (value ) )
		     return source;
		
		return source.Remove ( source.LastIndexOf ( value ) );
	}	
}
