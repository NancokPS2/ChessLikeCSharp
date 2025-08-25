using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public struct UISceneParameters
{

	public bool NeedsBackground = true;

	public bool CanBeBackedOut = true;

	public UISceneParameters()
	{
		NeedsBackground = true;
		CanBeBackedOut = true;
	}

}
